namespace Shin_Megami_Tensei_View.ConsoleLib;

public class TeamSelectionView
{
    private readonly View _view;

    public TeamSelectionView(View view)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public string SelectTeamFile(List<string> teamFiles)
    {
        ShowTeamFileOptions(teamFiles);
        
        var input = _view.ReadLine();

        if (!TryParseSelection(input, out int selection))
        {
            return null;
        }

        if (!IsValidSelection(selection, teamFiles.Count))
        {
            return null;
        }

        return teamFiles[selection];
    }

    private void ShowTeamFileOptions(List<string> teamFiles)
    {
        _view.WriteLine("Elige un archivo para cargar los equipos");

        for (int i = 0; i < teamFiles.Count; i++)
        {
            var fileName = Path.GetFileName(teamFiles[i]);
            _view.WriteLine($"{i}: {fileName}");
        }
    }

    private bool TryParseSelection(string input, out int selection)
    {
        return int.TryParse(input, out selection);
    }

    private bool IsValidSelection(int selection, int maxCount)
    {
        return selection >= 0 && selection < maxCount;
    }
}