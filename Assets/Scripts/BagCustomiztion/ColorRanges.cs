using UnityEngine;

public static class ColorRanges
{
    public static readonly ColorRange Red = new ColorRange
    {
        hueRange = new Vector2(0.95f, 1f),
        satRange = new Vector2(0.5f, 1f),
        valRange = new Vector2(0.3f, 1f)
    };

    public static readonly ColorRange Orange = new ColorRange
    {
        hueRange = new Vector2(0.05f, 0.15f),
        satRange = new Vector2(0.5f, 1f),
        valRange = new Vector2(0.3f, 1f)
    };

    public static readonly ColorRange Yellow = new ColorRange
    {
        hueRange = new Vector2(0.15f, 0.20f),
        satRange = new Vector2(0.5f, 1f),
        valRange = new Vector2(0.3f, 1f)
    };

    public static readonly ColorRange Green = new ColorRange
    {
        hueRange = new Vector2(0.20f, 0.40f),
        satRange = new Vector2(0.3f, 1f),
        valRange = new Vector2(0.3f, 1f)
    };

    public static readonly ColorRange Cyan = new ColorRange
    {
        hueRange = new Vector2(0.40f, 0.55f),
        satRange = new Vector2(0.3f, 1f),
        valRange = new Vector2(0.3f, 1f)
    };

    public static readonly ColorRange Blue = new ColorRange
    {
        hueRange = new Vector2(0.55f, 0.70f),
        satRange = new Vector2(0.3f, 1f),
        valRange = new Vector2(0.3f, 1f)
    };

    public static readonly ColorRange Purple = new ColorRange
    {
        hueRange = new Vector2(0.70f, 0.80f),
        satRange = new Vector2(0.3f, 1f),
        valRange = new Vector2(0.3f, 1f)
    };

    public static readonly ColorRange Pink = new ColorRange
    {
        hueRange = new Vector2(0.80f, 0.95f),
        satRange = new Vector2(0.3f, 1f),
        valRange = new Vector2(0.3f, 1f)
    };

    public static readonly ColorRange Black = new ColorRange
    {
        hueRange = new Vector2(0f, 1f),
        satRange = new Vector2(0f, 0.15f),
        valRange = new Vector2(0f, 0.25f)
    };

    public static readonly ColorRange White = new ColorRange
    {
        hueRange = new Vector2(0f, 1f),
        satRange = new Vector2(0f, 0.15f),
        valRange = new Vector2(0.75f, 1f)
    };

    public static readonly ColorRange Grey = new ColorRange
    {
        hueRange = new Vector2(0f, 1f),
        satRange = new Vector2(0f, 0.15f),
        valRange = new Vector2(0.25f, 0.75f)
    };
}