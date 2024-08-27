using Microsoft.AspNetCore.Mvc;
using QRC.Services;
using QRC.Models;

namespace QRC.Controllers;

[ApiController]
[Route("auth")]
public class UserController : ControllerBase {
	private readonly AuthService authService;
  public UserController(AuthService _authService) {
    authService = _authService;
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

}

