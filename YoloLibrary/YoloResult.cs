using OpenCvSharp;

namespace YoloLibrary;

public record YoloResult(string ClassId, float Confidence, Rect Box);