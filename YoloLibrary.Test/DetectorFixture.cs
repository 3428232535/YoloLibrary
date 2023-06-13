using OpenCvSharp;

namespace YoloLibrary.Test;

public class DetectorFixture
{
    public YoloDetector Detector { get; } = new(ModelPath);
    private static readonly string ModelPath = Path.Combine(@"D:\Code\csharp\YoloLibrary\YoloLibrary.Test", "Assets", "CARAFE.onnx");
    public Mat TestImage { get; } = new(TestImagePath);
    public const string TestImagePath = @"D:\Code\csharp\YoloLibrary\YoloLibrary.Test\Assets\test.png";

}