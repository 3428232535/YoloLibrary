namespace YoloLibrary.WebAPI.Models;

public class DetectResult
{
    public IEnumerable<YoloDto> Results { get; set; }
    public string ResultImage { get; set; }
}