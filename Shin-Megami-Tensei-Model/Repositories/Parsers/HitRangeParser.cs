using Shin_Megami_Tensei_Model.Skills;

namespace Shin_Megami_Tensei_Model.Repositories.Parsers;

public class HitRangeParser
{
    public HitRange ParseHits(string hits)
    {
        if (string.IsNullOrWhiteSpace(hits) || hits == "-")
        {
            return HitRange.Fixed(1);
        }

        if (hits.Contains("-"))
        {
            var parts = hits.Split('-');
            if (parts.Length != 2)
            {
                throw new ArgumentException($"Invalid hit range format: {hits}");
            }

            if (!int.TryParse(parts[0].Trim(), out int min) ||
                !int.TryParse(parts[1].Trim(), out int max))
            {
                throw new ArgumentException($"Invalid hit range numbers: {hits}");
            }

            return HitRange.Variable(min, max);
        }

        if (int.TryParse(hits.Trim(), out int fixedHits))
        {
            return HitRange.Fixed(fixedHits);
        }

        throw new ArgumentException($"Invalid hit range: {hits}");
    }
}