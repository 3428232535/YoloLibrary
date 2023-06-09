using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YoloLibrary.WebAPI.Services;

namespace YoloLibrary.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DefectController : ControllerBase
{
    private readonly ILogger<DefectController> _logger;
    private readonly IYoloService _yoloService;
    public DefectController(ILogger<DefectController> logger, IYoloService yoloService)
    {
        _logger = logger;
        _yoloService = yoloService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Base64Input image)
    {
        if (string.IsNullOrEmpty(image.Base64Image)) return BadRequest("Image is null or empty.");
        try
        {
            var start = Stopwatch.GetTimestamp();
            var result = await _yoloService.DetectAsync(image.Base64Image);
            var elapse = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
            _logger.LogInformation("Detection took {Elapse} ms", elapse);
            return Ok(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while detecting defects");
            return BadRequest("Error while detecting defects, Maybe the image is corrupted.");
        }
    }
    
    public record Base64Input(string Base64Image);
}