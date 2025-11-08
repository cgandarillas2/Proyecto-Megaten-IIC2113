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
        var allLines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var nonEmptyLines = new List<string>();

        for (int i = 0; i < allLines.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(allLines[i]))
            {
                nonEmptyLines.Add(allLines[i]);
            }
        }

        return nonEmptyLines;
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

        var result = new List<string>();
        for (int i = startIndex + 1; i < endIndex; i++)
        {
            result.Add(allLines[i]);
        }

        return result;
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
        var samuraiLines = new List<string>();
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Contains(SamuraiMarker))
            {
                samuraiLines.Add(lines[i]);
            }
        }

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
        var skillParts = skillsText.Split(',');
        var skills = new List<string>();

        for (int i = 0; i < skillParts.Length; i++)
        {
            string trimmed = skillParts[i].Trim();
            if (!string.IsNullOrWhiteSpace(trimmed))
            {
                skills.Add(trimmed);
            }
        }

        return skills;
    }
    
    private static List<string> ExtractMonsterNames(List<string> lines)
    {
        var monsters = new List<string>();
        for (int i = 0; i < lines.Count; i++)
        {
            if (!lines[i].Contains(SamuraiMarker))
            {
                monsters.Add(lines[i].Trim());
            }
        }
        return monsters;
    }
}