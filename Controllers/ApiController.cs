using Microsoft.AspNetCore.Mvc;

namespace QRC.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase {
  private readonly IConfiguration configuration;
  public ApiController(IConfiguration config) {
    configuration = config;
  }

  [HttpGet("", Name = "Api")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public IActionResult Signup() {
    var setting = configuration.GetConnectionString("Nexus");
    var res = new {
      Success = true,
      Message = "Test successful",
      Setting = setting
    };
    return Ok(res);
  }
}
