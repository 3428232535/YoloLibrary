using YoloLibrary.WebAPI.Models;

namespace YoloLibrary.WebAPI.Services;

public interface IYoloService
{
    public YoloDetector Detector { get; }
    public Task<DetectResult> DetectAsync(string base64Image);
}

class YoloService : IYoloService
{
    private static readonly string ModelPath = Path.Combine(Environment.CurrentDirectory, "Assets", "CARAFE.onnx");
    public YoloDetector Detector { get; } = new(ModelPath, true);
    public Task<DetectResult> DetectAsync(string base64Image)
    {
        using var img = Detector.FromBase64(base64Image);
        Detector.ImagePreprocess(img, out var tensor);
        var results = Detector.Detect(tensor).ToList();
        var resultImage = Detector.DrawBase64Result(img, results);
        return Task.FromResult(new DetectResult
        {
            Results = results.Select(YoloDto.FromYoloResult),
            ResultImage = resultImage
        });
    }
}