using Shin_Megami_Tensei_View;
using Shin_Megami_Tensei;
using Shin_Megami_Tensei_GUI;
using Shin_Megami_Tensei_View.Gui;
using Shin_Megami_Tensei_Model.Repositories;
using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_Model.Combat;

/*
 * Este código permite replicar un test case O ejecutar con interfaz gráfica.
 *
 * Para ejecutar con GUI: Cambia USE_GUI = true
 * Para ejecutar tests de consola: Cambia USE_GUI = false
 *
 * MODO TEST (USE_GUI = false):
 * Primero pregunta por el grupo de test case a replicar.
 * Luego pregunta por el test case específico que se quiere replicar.
 *
 * Por ejemplo, si tu programa está fallando el test case:
 *      "data/E1-BasicCombat-Tests/006.txt"
 * ... puedes ver qué está ocurriendo mediante correr este programa y decir que quieres
 * replicar del grupo "E1-BasicCombat-Tests" el test case 6.
 *
 * Al presionar enter, se ingresa el input del test case en forma automática. Si el
 * color es azúl significa que el output de tu programa es el esperado. Si es rojo
 * significa que el output de tu programa es distinto al esperado (i.e., el test falló).
 */

const bool USE_GUI = false; // Cambia a true para usar interfaz gráfica

if (USE_GUI)
{
    RunGUIMode();
}
else
{
    RunConsoleView();
}

void RunGUIMode()
{
    // Inicializar repositorios necesarios para construir equipos
    var fileSystem = new FileSystemWrapper();
    var jsonSerializer = new NewtonsoftJsonSerializer();
    var damageCalculator = new DamageCalculator();

    // Cargar skills
    var skillRepository = new JsonSkillRepository(fileSystem, jsonSerializer, damageCalculator);
    skillRepository.LoadData("Data/skills.json");

    // Cargar units (samurai y monsters)
    var unitRepository = new JsonUnitRepository(fileSystem, jsonSerializer, skillRepository);
    unitRepository.LoadData("Data/samurai.json", "Data/monsters.json");

    // Crear TeamBuilder y GUI View
    var teamBuilder = new TeamBuilder(unitRepository);
    var guiView = new ShinMegamiTenseiGuiView(teamBuilder);

    // Iniciar GUI
    guiView.Start(() => {
        try
        {
            // Seleccionar equipos desde la GUI
            var (player1, player2) = guiView.SelectTeams("data");

            // Por ahora solo mostramos que se crearon los equipos
            Console.WriteLine($"Equipo 1 creado: {player1.PlayerName}");
            Console.WriteLine($"  Samurai: {player1.ActiveBoard.GetSamurai().Name}");
            Console.WriteLine($"  Monsters en reserva: {player1.Reserve.Count()}");

            Console.WriteLine($"\nEquipo 2 creado: {player2.PlayerName}");
            Console.WriteLine($"  Samurai: {player2.ActiveBoard.GetSamurai().Name}");
            Console.WriteLine($"  Monsters en reserva: {player2.Reserve.Count()}");

            guiView.ShowEndGameMessage("Equipos creados exitosamente!\n(El juego completo está en construcción)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            guiView.ShowEndGameMessage($"Error al crear equipos: {ex.Message}");
        }
    });
}

void RunConsoleView()
{
    
    string testFolder = SelectTestFolder();
    string test = SelectTest(testFolder);
    string teamsFolder = testFolder.Replace("-Tests","");
    AnnounceTestCase(test);

    var view = View.BuildManualTestingView(test);
    var game = new Game(view, teamsFolder);
    game.Play();
}

void Main() {}
string SelectTestFolder()
{
    Console.WriteLine("¿Qué grupo de test quieres usar?");
    string[] dirs = GetAvailableTestsInOrder();
    ShowArrayOfOptions(dirs);
    return AskUserToSelectAnOption(dirs);
}

string[] GetAvailableTestsInOrder()
{
    string[] dirs = Directory.GetDirectories("data", "*-Tests", SearchOption.TopDirectoryOnly);
    Array.Sort(dirs);
    return dirs;
}

void ShowArrayOfOptions(string[] options)
{
    for(int i = 0; i < options.Length; i++)
        Console.WriteLine($"{i}- {options[i]}");
}

string AskUserToSelectAnOption(string[] options)
{
    int minValue = 0;
    int maxValue = options.Length - 1;
    int selectedOption = AskUserToSelectNumber(minValue, maxValue);
    return options[selectedOption];
}

int AskUserToSelectNumber(int minValue, int maxValue)
{
    Console.WriteLine($"(Ingresa un número entre {minValue} y {maxValue})");
    int value;
    bool wasParsePossible;
    do
    {
        string? userInput = Console.ReadLine();
        wasParsePossible = int.TryParse(userInput, out value);
    } while (!wasParsePossible || IsValueOutsideTheValidRange(minValue, value, maxValue));

    return value;
}

bool IsValueOutsideTheValidRange(int minValue, int value, int maxValue)
    => value < minValue || value > maxValue;

string SelectTest(string testFolder)
{
    Console.WriteLine("¿Qué test quieres ejecutar?");
    string[] tests = Directory.GetFiles(testFolder, "*.txt" );
    Array.Sort(tests);
    return AskUserToSelectAnOption(tests);
}

void AnnounceTestCase(string test)
{
    Console.WriteLine($"----------------------------------------");
    Console.WriteLine($"Replicando test: {test}");
    Console.WriteLine($"----------------------------------------\n");
}