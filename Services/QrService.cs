using QRCoder;
using NanoidDotNet;
using System.Security.Claims;
using QRC.Models.QRCodeModel;

namespace QRC.Services;

public class QrService {
  private readonly DbService db;
  private readonly IHttpContextAccessor httpContextAccessor;
  private readonly IWebHostEnvironment webHostEnvironment;
  public QrService(DbService _db, IHttpContextAccessor _httpContextAccessor, IWebHostEnvironment _iwebHostEnvironment){
    db = _db;
    httpContextAccessor = _httpContextAccessor;
    webHostEnvironment = _iwebHostEnvironment;
  }

  public QrCodeResponse NewQrCode(QrCodeDto qrCodeDto) {
    QrCodeResponse response = new();

    var user = GetCurrentUser();
    if(user == null) {
      response.Success = false;
      response.Message = "User cannot be null";
      return response;
    }

    var request = httpContextAccessor.HttpContext?.Request;
    string id = Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 10);

    string dirPath = Path.Combine(webHostEnvironment.WebRootPath, "img/");
    if(!Path.Exists(dirPath)) Directory.CreateDirectory(dirPath);
    string partialFilePath = "img/" + Guid.NewGuid().ToString() + ".png";
    string filePath = Path.Combine(webHostEnvironment.WebRootPath, partialFilePath);
    string url = $"https://michaelson-qr-code-generator.vercel.app/scan?url-id={id}";
    // var imgFormat = Base64QRCode.ImageType.Png;
    QRCodeGenerator qRCodeGenerator = new();
    QRCodeData qrCodeData = qRCodeGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
    Base64QRCode base64QRCode = new(qrCodeData);
    
    string qrCodeImage = base64QRCode.GetGraphic(20);
    byte[] imgBytes = Convert.FromBase64String(qrCodeImage);
    File.WriteAllBytes(filePath, imgBytes);
    
    // string imgUrl = $"data:image/{imgFormat.ToString().ToLower()};base64,{qrCodeImage}";
    string fileUri = $"{request?.Scheme}://{request?.Host}/{partialFilePath}";
    QrCodeModel qrCode = new() {
      Title = qrCodeDto.Title,
      VisitCount = 0,
      CreatedAt = DateTime.UtcNow,
      IsActive = true,
      SiteUrl = qrCodeDto.SiteUrl,
      UrlId = id,
      CreatedBy = Guid.Parse(user),
      ImageUrl = fileUri
    };

    db.QrCodes.Add(qrCode);
    db.SaveChanges();
    
    response.Success = true;
    response.Data = qrCode;
    response.Message = "Item created successfully";
    return response;
  }

  public QrCodeListResponse GetQrCodes() {
    QrCodeListResponse response = new();
    var codes = db.QrCodes.OrderByDescending(item => item.CreatedAt).ToList();
    if(codes == null) {
      response.Success = false;
      response.Message = "Error retrieving items";
      return response;
    }

    response.Success = true;
    response.Data = codes;
    response.Message = "Item created successfully";
    return response;
  }

  public QRScanResponse ScanQrCode(string urlId) {
    QRScanResponse response = new();
    var code = db.QrCodes.FirstOrDefault(item => item.UrlId == urlId);
    if(code == null) {
      response.Success = false;
      response.Message = "Item not found";
      return response;
    }

    if(!code.IsActive) {
      response.Success = false;
      response.Message = "This QR Code has been deactivated";
      return response;
    }

    code.VisitCount = code.VisitCount + 1;
    db.SaveChanges();

    response.Success = true;
    response.Message = "Success!";
    response.Url = code.SiteUrl;
    return response;
  }


  public QrCodeResponse DeleteQrCode(Guid Id) {
    QrCodeResponse response = new();
    var code = db.QrCodes.Find(Id);
    if(code == null) {
      response.Success = false;
      response.Message = "Error retrieving item";
      return response;
    }

    db.QrCodes.Remove(code);
    db.SaveChanges();

    response.Success = true;
    response.Message = "Item deleted successfully";
    return response;
  }

  public string? GetCurrentUser() {
    var user = httpContextAccessor?.HttpContext?.User;
    if (user?.Identity is not ClaimsIdentity identity) {
      return null;
    };
    var claims = identity.Claims;
    string? UserId = claims.FirstOrDefault(u => u.Type == "UserId")?.Value;
    return UserId;
  }
}
