using System.Collections.Generic;
using System.Linq;

public enum BagTheme { Girly, Goth, FairyLight, Y2K, Sportsy }
public enum TrinketType { Metal, Sticker, Cloth, Keychain } // TrinketType currently not used


[System.Serializable]
public class OrderRequest
{
    public string customerName;

    public BagTheme? loveTheme;
    public BagTheme? hateTheme;

    public ColorRange? loveColorRange;
    public ColorRange? hateColorRange;

    // Fluff are display only, no effect
    public string[] loveFluff;  // "death", "chocolate"
    public string[] hateFluff;  // "mornings", "pop music"


    private string JoinWithAnd(List<string> items)
    {
        if (items.Count == 0) return "";
        if (items.Count == 1) return items[0];
        return string.Join(", ", items.Take(items.Count - 1)) + " and " + items.Last();
    }

    public string GenerateOrderText()
    {
        var loves = new List<string>();
        var hates = new List<string>();

        if (loveTheme.HasValue) loves.Add(loveTheme.Value.ToString());
        if (loveColorRange.HasValue) loves.Add(ColorRangeToColorName(loveColorRange.Value).ToString());
        if (loveFluff != null) loves.AddRange(loveFluff);

        if (hateTheme.HasValue) hates.Add(hateTheme.Value.ToString());
        if (hateColorRange.HasValue) hates.Add(ColorRangeToColorName(hateColorRange.Value).ToString());
        if (hateFluff != null) hates.AddRange(hateFluff);

        string loveStr = loves.Count > 0 ? $"loves {JoinWithAnd(loves)}" : "";
        string hateStr = hates.Count > 0 ? $"hates {JoinWithAnd(hates)}" : "";

        return $"Customer: {customerName}.\n {loveStr}.\n {hateStr}.".Trim();
    }

    private ColorName ColorRangeToColorName(ColorRange range)
    {
        float midSat = (range.satRange.x + range.satRange.y) / 2f;
        float midVal = (range.valRange.x + range.valRange.y) / 2f;
        float midHue = (range.hueRange.x + range.hueRange.y) / 2f;

        if (midSat < 0.15f)
        {
            if (midVal < 0.25f) return ColorName.Black;
            if (midVal > 0.75f) return ColorName.White;
            return ColorName.Gray;
        }

        return midHue switch
        {
            < 0.05f => ColorName.Red,
            < 0.15f => ColorName.Orange,
            < 0.20f => ColorName.Yellow,
            < 0.40f => ColorName.Green,
            < 0.55f => ColorName.Cyan,
            < 0.70f => ColorName.Blue,
            < 0.80f => ColorName.Purple,
            < 0.95f => ColorName.Pink,
            _ => ColorName.Red,
        };
    }
}