using SixLabors.ImageSharp;
using System;

public static class ColorParser
{
    public static bool TryParseRgbColor(string input, out SixLabors.ImageSharp.PixelFormats.Rgba32 color)
    {
        color = default(SixLabors.ImageSharp.PixelFormats.Rgba32);

        string[] components = input.Split(',');

        if (components.Length != 3)
            return false;

        if (!int.TryParse(components[0], out int red) ||
            !int.TryParse(components[1], out int green) ||
            !int.TryParse(components[2], out int blue))
        {
            return false;
        }

        color = new SixLabors.ImageSharp.PixelFormats.Rgba32(red, green, blue);
        return true;
    }
}