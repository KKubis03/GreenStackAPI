namespace GreenStackAPI.Controllers
{
    using GreenStackAPI.Models;
    using GreenStackAPI.Services;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class MixController : ControllerBase
    {
        private readonly MixService _mixService;

        public MixController(MixService mixService)
        {
                _mixService = mixService;
        }

        [HttpGet("three-days-averages")]
        public async Task<IActionResult> GetThreeDaysAverages()
        {
            var data = await _mixService.GetThreeDaysAveragesAsync();
            return Ok(data);
        }

        [HttpGet("optimal-charging-window")]
        public async Task<ActionResult<OptimalChargingWindow>> GetOptimalChargingWindow([FromQuery] int windowHours)
        {
            if (windowHours < 1 || windowHours > 6)
                return BadRequest("Window must be between 1 and 6 hours.");

            var result = await _mixService.GetOptimalChargingWindowAsync(windowHours);
            if (result == null)
                return NotFound("No suitable window found.");

            return Ok(result);
        }
    }
}
