using Xunit.Abstractions;

namespace YoloLibrary.Test;

public class TestDetector : IClassFixture<DetectorFixture>
{
    private static readonly string TestImagePath = Path.Combine(@"D:\Code\csharp\YoloLibrary\YoloLibrary.Test", "Assets", "test.png");
    
    private readonly ITestOutputHelper _output;
    private readonly DetectorFixture _fixture;
    public TestDetector(ITestOutputHelper output, DetectorFixture fixture)
    {
        _output = output;
        _fixture = fixture;
    }
    
    [Fact]
    public void TestDetect()
    {
        var detector = _fixture.Detector;
        using var img = _fixture.Detector.FromPath(TestImagePath); 
        detector.ImagePreprocess(img, out var tensor);
        var results = detector.Detect(tensor);
        Assert.NotEmpty(results);
    }
    [Fact]
    public void TestFromFileStream()
    {
        using var img = _fixture.Detector.FromPath(TestImagePath); 
        _fixture.Detector.ImagePreprocess(img, out var expected);
        using Stream stream = File.OpenRead(TestImagePath);
        var matFromStream = _fixture.Detector.FromFileStream(stream);
        _fixture.Detector.ImagePreprocess(matFromStream, out var actual);
        Assert.Equal(expected, actual);
    }
}