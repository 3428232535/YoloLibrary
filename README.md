# YoloLibrary

A c# library for [YoloV5](https://github.com/ultralytics/yolov5) object detection, using [OpencvSharp4](https://github.com/shimat/opencvsharp) 
and [ONNXRuntime](https://github.com/microsoft/onnxruntime) for inference.

## Performance
| Step            | Time      |
|-----------------|-----------|
| Preprocess      | 10ms      |
| Inference(CPU)  | 200-300ms |
| Inference(Cuda) | 30-60ms   |
| NMS             | 40ms      |

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

## Deploy
```bash
docker pull nvidia/cuda:11.6.0-cudnn8-runtime-ubuntu20.04
docker run -it --gpus all -p 5001:5001 --name yolo-api nvidia/cuda:11.6.0-cudnn8-runtime-ubuntu20.04
```
note **--gpus all** is required for cuda support
### Requirements
1. install **dotnet8-runtime** and export to **PATH**
```bash
docker cp aspnetcore-runtime-8.0.0-preview.4.23260.4-linux-x64.tar.gz yourContainer:/root
mkdir -p /root/.dotnet
tar -xzf aspnetcore-runtime-8.0.0-preview.4.23260.4-linux-x64.tar.gz -C /root/.dotnet
export PATH=$PATH:/root/.dotnet
```
2. check **libOpenCvSharpExtern.so** and dependency with
```bash
ldd libOpenCvSharpExtern.so | grep "not found"
```
3. install missing dependency(examples)
```bash
sudo apt update && sudo apt install -y \
    apt-utils \
    libgdiplus \
    libc6-dev \
    libgtk2.0-dev \
    libtbb-dev \
    libatlas-base-dev \
    libvorbis-dev \
    libxvidcore-dev \
    libopencore-amrnb-dev \
    libopencore-amrwb-dev \
    libavresample-dev \
    x264 \
    v4l-utils \
    libwebp-dev \
    tesseract-ocr \
    libtesseract-dev \
    libleptonica-dev \
    libtiff-dev \
    libavcodec-dev \
    libavformat-dev \
    libswscale-dev \
    libdc1394-22-dev \
    libxine2-dev \
    libv4l-dev \
    tesseract-ocr
```