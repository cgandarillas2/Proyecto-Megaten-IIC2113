using Shin_Megami_Tensei_Model.Utils;
using Shin_Megami_Tensei_Model.Validators;
namespace Shin_Megami_Tensei_Model.Repositories;

public class TeamFileParser
{
    private const string SamuraiMarker = "[Samurai]";
    private const string Player1Marker = "Player 1 Team";
    private const string Player2Marker = "Player 2 Team";
    
    private readonly IFileSystem _fileSystem;

    public TeamFileParser(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
    }

    public (TeamData player1, TeamData player2) ParseTeamFile(string filePath)
    {
        var lines = ReadNonEmptyLines(filePath);
        var player1Lines = ExtractPlayerLines(lines, Player1Marker, Player2Marker);
        var player2Lines = ExtractPlayerLines(lines, Player2Marker, null);

        var player1Data = ParseTeamLines(player1Lines);
        var player2Data = ParseTeamLines(player2Lines);

        return (player1Data, player2Data);
    }
    
    private List<string> ReadNonEmptyLines(string filePath)
    {
        var content = _fileSystem.ReadAllText(filePath);
        return content
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
    }

    private static List<string> ExtractPlayerLines(
        List<string> allLines,
        string startMarker,
        string endMarker)
    {
        var startIndex = FindLineIndex(allLines, startMarker);
        var endIndex = endMarker != null
            ? FindLineIndex(allLines, endMarker)
            : allLines.Count;

        return allLines
            .Skip(startIndex + 1)
            .Take(endIndex - startIndex - 1)
            .ToList();
    }

    private static int FindLineIndex(List<string> lines, string marker)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains(marker))
            {
                return i;
            }
        }
        throw new ArgumentException($"Marker not found: {marker}");
    }
    
    private static TeamData ParseTeamLines(List<string> lines)
    {
        if (lines.Count == 0)
        {
            return new TeamData(null, new List<string>(), new List<string>());
        }

        var samuraiLine = FindSamuraiLine(lines);
        var (samuraiName, skills) = ParseSamuraiLine(samuraiLine);
        var monsters = ExtractMonsterNames(lines);

        return new TeamData(samuraiName, skills, monsters);
    }

    private static string FindSamuraiLine(List<string> lines)
    {
        var samuraiLines = lines.Where(line => line.Contains(SamuraiMarker)).ToList();

        if (samuraiLines.Count == 0)
        {
            throw new ArgumentException("Team must have exactly one Samurai");
        }

        if (samuraiLines.Count > 1)
        {
            throw new ArgumentException("Team must have exactly one Samurai");
        }

        return samuraiLines[0];
    }
    
    private static (string name, List<string> skills) ParseSamuraiLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return (null, new List<string>());
        }

        var withoutMarker = line.Replace(SamuraiMarker, "").Trim();
        var name = ExtractName(withoutMarker);
        var skills = ExtractSkills(withoutMarker);

        return (name, skills);
    }

    private static string ExtractName(string line)
    {
        var parenthesisIndex = line.IndexOf('(');
        if (parenthesisIndex >= 0)
        {
            return line.Substring(0, parenthesisIndex).Trim();
        }
        return line.Trim();
    }
    
    private static List<string> ExtractSkills(string line)
    {
        var startIndex = line.IndexOf('(');
        var endIndex = line.IndexOf(')');

        if (startIndex < 0 || endIndex < 0)
        {
            return new List<string>();
        }

        var skillsText = line.Substring(startIndex + 1, endIndex - startIndex - 1);
        return skillsText
            .Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }
    
    private static List<string> ExtractMonsterNames(List<string> lines)
    {
        return lines
            .Where(line => !line.Contains(SamuraiMarker))
            .Select(line => line.Trim())
            .ToList();
    }
}