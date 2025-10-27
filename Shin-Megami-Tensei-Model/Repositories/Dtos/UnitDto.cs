namespace Shin_Megami_Tensei_Model.Repositories.Dtos;

public class UnitDto
{
    public string Name { get; set; }
    public StatsDto Stats { get; set; }
    public Dictionary<string, string> Affinity { get; set; }
    public List<string> Skills { get; set; }
}

