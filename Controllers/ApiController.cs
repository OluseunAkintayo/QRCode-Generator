using Microsoft.AspNetCore.Mvc;

namespace QRC.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase {
  [HttpGet("", Name = "Api")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public IActionResult Test() {
    var res = new {
      Success = true,
      Message = "Test successful"
    };
    return Ok(res);
  }
}
