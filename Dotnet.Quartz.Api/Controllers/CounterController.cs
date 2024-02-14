using Dotnet.Quartz.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Quartz.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CounterController : ControllerBase
{
    [HttpGet]
    public IActionResult GetCounter()
    {
        return Ok(IncrementCounter.GetCounter());
    }
}