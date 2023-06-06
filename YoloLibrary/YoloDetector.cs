using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OpenCvSharp;
using OpenCvSharp.Dnn;


namespace YoloLibrary;

public class YoloDetector
{
    private const int ModelSize = 640;
    private static readonly string[] TargetIds = new[] {"a1", "a2", "a3", "c1", "c2", "c3"};
    private static readonly Scalar[] Colors = {
        Scalar.Red, Scalar.Green, Scalar.Blue,
        Scalar.Yellow, Scalar.Cyan, Scalar.Magenta
    };
    public bool IsInitialized => Session is not null;
    private InferenceSession? Session { get; set; }
    public YoloDetector() { }
    public YoloDetector(string modelPath) => InitDetectSession(modelPath);
    public void InitDetectSession(string modelPath) => Session = new(modelPath);
    public void ImagePreprocess(Mat image, out Tensor<float> tensor)
    {
        int maxSize = Math.Max(image.Width, image.Height);
        float ratio = maxSize * 1.0f / ModelSize;
        using Mat resized = Mat.Zeros(maxSize, maxSize, MatType.CV_8UC3);
        image.CopyTo(resized[new Rect(0, 0, image.Width, image.Height)]);
        using Mat blob = CvDnn.BlobFromImage(resized, 1 / 255f, new(ModelSize, ModelSize), new(0, 0, 0), true, false);
        tensor = new DenseTensor<float>(blob.AsSpan<float>().ToArray().AsMemory(),new[] {1, 3, 640, 640});
    }
    private IEnumerable<float[]> InferenceResult(in Tensor<float> tensor)
    {
        if (IsInitialized is false) throw new InvalidOperationException("Session is not initialized.");
        var container = new[] {NamedOnnxValue.CreateFromTensor("images", tensor)};
        var results = Session!.Run(container);
        return results.First().AsEnumerable<float>().Chunk(TargetIds.Length + 5);
    }
    public IEnumerable<YoloResult> Detect(in Tensor<float> tensor)
    {
        var scores = InferenceResult(tensor);
        List<int> ids = new(); List<float> confidences = new(); List<Rect2d> boxes = new();
        foreach (var score in scores)
        {
            float confidence = score[4];
            if (confidence < .5f) continue;
            int id = MaxIndex(score.AsSpan()[5..], out float max);
            if (max < .45f) continue;
            ids.Add(id);
            confidences.Add(max*confidence);
            boxes.Add(ToRect(score.AsSpan()[..4], 2240 / 640f));
        }
        CvDnn.NMSBoxes(boxes, confidences, .45f, .5f, out int[] indices);
        return indices.Select(i => new YoloResult(TargetIds[ids[i]], confidences[i], boxes[i].ToRect()));
    }
    private static int MaxIndex(Span<float> span, out float max)
    {
        max = span[0];
        int index = 0;
        for (int i = 1; i < span.Length; i++)
        {
            if (span[i] > max)
            {
                max = span[i];
                index = i;
            }
        }
        return index;
    }
    private static Rect2d ToRect(Span<float> array, float ratio)
    {
        float left = (array[0] - 0.5f * array[2]) * ratio, 
            right = (array[1] - 0.5f * array[3]) * ratio,
            width = array[2] * ratio, height = array[3] * ratio;
        return new(left, right, width, height);
    }
    public string DrawBase64Result(Mat image, IEnumerable<YoloResult> results)
    {
        foreach (var result in results)
        {
            Cv2.Rectangle(image, result.Box, Colors[Array.IndexOf(TargetIds, result.ClassId)], 2);
            Cv2.PutText(image, $"{result.ClassId} {result.Confidence:F2}", result.Box.TopLeft, HersheyFonts.HersheySimplex, 1, Colors[Array.IndexOf(TargetIds, result.ClassId)]);
        }
        Cv2.ImEncode(".png", image, out var buf);
        return Convert.ToBase64String(buf);
    }
    public Mat FromPath(string imagePath) => Cv2.ImRead(imagePath);
    public Mat FromBase64(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        return Cv2.ImDecode(bytes, ImreadModes.Color);
    }
    public Mat FromFileStream(Stream stream)
    {
        byte[] bytes = new byte[stream.Length];
        _ = stream.Read(bytes, 0, bytes.Length);
        return Cv2.ImDecode(bytes, ImreadModes.Color);
    }
}