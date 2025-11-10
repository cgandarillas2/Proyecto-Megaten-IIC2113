using Shin_Megami_Tensei_Model.Combat;
using Shin_Megami_Tensei_Model.Game;
using Shin_Megami_Tensei_Model.Repositories;
using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_GUI;

namespace Shin_Megami_Tensei.GUI;

/// <summary>
/// Handles the game flow when using the GUI interface
/// </summary>
public class GUIGameController
{
    private const string DataFolder = "Data";
    private const string SamuraiFile = "samurai.json";
    private const string MonstersFile = "monsters.json";
    private const string SkillsFile = "skills.json";

    private readonly SMTGUI _gui;
    private JsonUnitRepository _unitRepository;
    private TeamBuilder _teamBuilder;

    public GUIGameController()
    {
        _gui = new SMTGUI();
        InitializeRepositories();
    }

    public void Run()
    {
        _gui.Start(RunGameLoop);
    }

    private void RunGameLoop()
    {
        try
        {
            // 1. Get team information from GUI
            var team1Info = _gui.GetTeamInfo(1);
            var team2Info = _gui.GetTeamInfo(2);

            // Optional: Show team info for debugging
            ShowTeamInfo(team1Info, "Jugador 1");
            ShowTeamInfo(team2Info, "Jugador 2");

            // 2. Convert ITeamInfo to Team using repositories
            var team1 = _teamBuilder.BuildTeam(team1Info, "Jugador 1");
            var team2 = _teamBuilder.BuildTeam(team2Info, "Jugador 2");

            // 3. Create game state
            var gameState = new GameState(team1, team2);

            // 4. Execute game loop
            ExecuteGameLoop(gameState);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en el juego: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void ExecuteGameLoop(GameState gameState)
    {
        // Initial update to show the starting state
        UpdateGUIState(gameState);

        // Simple game loop - wait for user interactions
        // For now, just demonstrate that the game state is displayed
        // A full implementation would handle combat actions here

        bool gameRunning = true;
        while (gameRunning && !gameState.IsGameOver())
        {
            var clickedElement = _gui.GetClickedElement();

            // Handle different types of clicks
            if (clickedElement.Type == ClickedElementType.Button)
            {
                HandleButtonClick(clickedElement, gameState);
            }
            else if (clickedElement.Type == ClickedElementType.UnitInBoard)
            {
                HandleBoardUnitClick(clickedElement, gameState);
            }
            else if (clickedElement.Type == ClickedElementType.UnitInReserve)
            {
                HandleReserveUnitClick(clickedElement, gameState);
            }

            // Update GUI after any action
            UpdateGUIState(gameState);

            // For this basic implementation, check if game is over
            if (gameState.IsGameOver())
            {
                var winner = gameState.GetWinner();
                _gui.ShowEndGameMessage($"{winner.PlayerName} ha ganado");
                gameRunning = false;
            }
        }
    }

    private void HandleButtonClick(IClickedElement clickedElement, GameState gameState)
    {
        Console.WriteLine($"Button clicked: {clickedElement.Text}");

        // Here you would implement the logic for each button
        // For now, just log it
        switch (clickedElement.Text)
        {
            case "Atacar":
                // TODO: Implement attack logic
                break;
            case "Usar Habilidad":
                // TODO: Implement skill usage logic
                break;
            case "Invocar":
                // TODO: Implement summon logic
                break;
            case "Pasar Turno":
                // TODO: Implement pass turn logic
                gameState.AdvanceActionQueue();
                break;
        }
    }

    private void HandleBoardUnitClick(IClickedElement clickedElement, GameState gameState)
    {
        Console.WriteLine($"Board unit clicked: {clickedElement.Text} (Player {clickedElement.PlayerId})");
        // TODO: Handle board unit selection (e.g., for targeting)
    }

    private void HandleReserveUnitClick(IClickedElement clickedElement, GameState gameState)
    {
        Console.WriteLine($"Reserve unit clicked: {clickedElement.Text}");
        // TODO: Handle reserve unit selection (e.g., for summoning)
    }

    private void UpdateGUIState(GameState gameState)
    {
        var guiState = GUIAdapter.ConvertGameStateToGUIState(gameState);
        _gui.Update(guiState);
    }

    private void ShowTeamInfo(ITeamInfo team, string playerName)
    {
        Console.WriteLine($"\n=== {playerName} ===");
        Console.WriteLine($"Samurai: {team.SamuraiName}");
        Console.WriteLine("Skills:");
        foreach (var skill in team.SkillNames)
            Console.WriteLine($"  - {skill}");
        Console.WriteLine("Demons:");
        foreach (var demon in team.DemonNames)
            Console.WriteLine($"  - {demon}");
    }

    private void InitializeRepositories()
    {
        var fileSystem = new FileSystemWrapper();
        var jsonSerializer = new NewtonsoftJsonSerializer();
        var damageCalculator = new DamageCalculator();

        var skillRepository = new JsonSkillRepository(fileSystem, jsonSerializer, damageCalculator);
        var skillsPath = BuildDataPath(SkillsFile);
        skillRepository.LoadData(skillsPath);

        _unitRepository = new JsonUnitRepository(fileSystem, jsonSerializer, skillRepository);
        var samuraiPath = BuildDataPath(SamuraiFile);
        var monstersPath = BuildDataPath(MonstersFile);
        _unitRepository.LoadData(samuraiPath, monstersPath);

        _teamBuilder = new TeamBuilder(_unitRepository);
    }

    private string BuildDataPath(string fileName)
    {
        return $"{DataFolder}/{fileName}";
    }
}
