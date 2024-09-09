using Microsoft.AspNetCore.Mvc;
using QRC.Services;
using QRC.Models;

namespace QRC.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase {
	private readonly AuthService authService;
  private readonly ILogger<AuthController> logger;
  public AuthController(AuthService _authService, ILogger<AuthController> _logger) {
    authService = _authService;
    logger = _logger;
  }

  [HttpPost("user/signup", Name = "Signup")]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public IActionResult Signup([FromBody] UserDto userDto) {
    var res = authService.NewUser(userDto);
    if(!res.Success) return BadRequest(res);
    return Ok(res);
  }


  [HttpPost("user/login", Name = "Login")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public IActionResult Login([FromBody] UserLogin user) {
    var res = authService.Login(user);
    if(!res.Success) return BadRequest(res);
    return Ok(res);
  }

  // [HttpDelete("user/delete", Name = "Delete")]
	// [ProducesResponseType(StatusCodes.Status200OK)]
	// [ProducesResponseType(StatusCodes.Status400BadRequest)]
	// [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  // public IActionResult Delete(Guid userId) {
  //   var res = authService.Login(user);
  //   if(!res.Success) return BadRequest(res);
  //   return Ok(res);
  // }
}

