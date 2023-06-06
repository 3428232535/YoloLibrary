namespace YoloLibrary.WebAPI.Models;

public record YoloDto(string ClassId, float Confidence, int X, int Y, int Width, int Height)
{
    public static YoloDto FromYoloResult(YoloResult result)
    {
        return new YoloDto(result.ClassId, result.Confidence, result.Box.X, result.Box.Y, result.Box.Width, result.Box.Height);
    }
}