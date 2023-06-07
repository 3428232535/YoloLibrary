# YoloLibrary

A c# library for [YoloV5](https://github.com/ultralytics/yolov5) object detection, using [OpencvSharp4](https://github.com/shimat/opencvsharp) 
and [ONNXRuntime](https://github.com/microsoft/onnxruntime) for inference.

## Library Usage

```csharp
// Load Model
var detector = new YoloDetector();
detector.InitDetectSession("yolov5s.onnx");
// or
var detector = new("yolov5s.onnx");

// Load Image
var image = detector.FromPath(imagePath);// From path
var image = detector.FromBase64(base64Image);// From Base64
var image = detector.FromStream(stream);// From Stream

// Image to Tensor
detector.ImagePreprocess(image, out Tensor<float> tensor);

// Detect
var results = detector.Detect(tensor);
```

## API Usage

```csharp
// Request from API
var client = new HttpClient();
var request = await client.PostAsJsonAsync("http://your-host/Defect", new {base64Image = base64Image});
var content = await request.Content.ReadAsStringAsync();
var json = JsonNode.Parse(content);
// parse JsonResult
var resultImage = json["resultImage"].Get<string>();
var results = json["results"].Deserialize<List<YoloResult>>();
```

| Method| Url | Body                   |
| ---   | --- |------------------------|
| POST  | /Defect | { "base64Image": string} |

