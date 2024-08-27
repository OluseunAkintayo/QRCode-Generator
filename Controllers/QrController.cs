using Microsoft.AspNetCore.Mvc;
using QRC.Services;
using QRC.Models.QRCodeModel;
using Microsoft.AspNetCore.Authorization;

namespace QRC.Controllers;

[ApiController]
[Route("qrcode")]
public class QrController : ControllerBase {
	private readonly QrService qrService;
  public QrController(QrService _qrService) {
    qrService = _qrService;
  }

  [HttpPost("new", Name = "NewQrCode"), Authorize]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult NewQrCode([FromBody] QrCodeDto qrCodeDto) {
    var res = qrService.NewQrCode(qrCodeDto);
    if(!res.Success) return BadRequest(res);
    return Ok(res);
  }


  [HttpGet("get-current-user", Name = "GetCurrentUser"), Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult GetCurrentUser() {
    var res = qrService.GetCurrentUser();
    return Ok(res);
  }
  
  [HttpGet("list", Name = "GetQrCodes"), Authorize]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult GetQrCodes() {
    var res = qrService.GetQrCodes();
    if (!res.Success) return BadRequest(res);
    return Ok(res);
  }

  [HttpDelete(Name = "DeleteQrCode"), Authorize(Roles = "Admin")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  public IActionResult DeleteQrCode(Guid Id) {
    var res = qrService.DeleteQrCode(Id);
    if (!res.Success) return BadRequest(res);
    return Ok(res);
  }
}
