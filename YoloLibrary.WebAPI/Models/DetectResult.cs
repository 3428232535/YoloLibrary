namespace YoloLibrary.WebAPI.Models;

public class DetectResult
{
    public IEnumerable<YoloDto> YoloDtos { get; set; }
    public string ResultImage { get; set; }
}