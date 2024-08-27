using QRC.Models.QRCodes;
using QRCoder;
using NanoidDotNet;

namespace QRC.Services;

public class QrService {
  private readonly Db db;
  private readonly IHttpContextAccessor httpContextAccessor;
  private readonly IWebHostEnvironment webHostEnvironment;
  public QrService(Db _db, IHttpContextAccessor _httpContextAccessor, IWebHostEnvironment _iwebHostEnvironment){
    db = _db;
    httpContextAccessor = _httpContextAccessor;
    webHostEnvironment = _iwebHostEnvironment;
  }

  public QrCodeResponse NewQrCode(QrCodeDto qrCodeDto) {
    var request = httpContextAccessor.HttpContext.Request;
    string id = Nanoid.Generate(Nanoid.Alphabets.LowercaseLettersAndDigits, 10);
    QrCodeModel qrCode = new() {
      Title = qrCodeDto.Title,
      VisitCount = 0,
      CreatedAt = DateTime.UtcNow,
      IsActive = true,
      SiteUrl = qrCodeDto.SiteUrl,
      UrlId = id
    };

    string dirPath = Path.Combine(webHostEnvironment.WebRootPath, "img/");
    if(!Path.Exists(dirPath)) Directory.CreateDirectory(dirPath);
    string partialFilePath = "img/" + Guid.NewGuid().ToString() + ".png";
    string filePath = Path.Combine(webHostEnvironment.WebRootPath, partialFilePath);
    // var imgFormat = Base64QRCode.ImageType.Png;
    QRCodeGenerator qRCodeGenerator = new();
    QRCodeData qrCodeData = qRCodeGenerator.CreateQrCode(qrCodeDto.SiteUrl, QRCodeGenerator.ECCLevel.Q);
    Base64QRCode base64QRCode = new(qrCodeData);
    
    string qrCodeImage = base64QRCode.GetGraphic(20);
    byte[] imgBytes = Convert.FromBase64String(qrCodeImage);
    File.WriteAllBytes(filePath, imgBytes);
    
    // string imgUrl = $"data:image/{imgFormat.ToString().ToLower()};base64,{qrCodeImage}";
    string fileUri = $"{request.Scheme}://{request.Host}/{partialFilePath}";
    qrCode.ImageUrl = fileUri;

    // db.QrCodes.Add(qrCode);
    // db.SaveChanges();

    QrCodeResponse response = new() {
      Success = true,
      Data = qrCode,
      Message = "Item created successfully"
    };
    return response;
  }

  public QrCodeListResponse GetQrCodes() {
    var codes = db.QrCodes.ToList();
    if(codes == null) {
      QrCodeListResponse error = new() {
        Success = false,
        Message = "Error retrieving items"
      };
      return error;
    }

    QrCodeListResponse response = new() {
      Success = true,
      Data = codes,
      Message = "Item created successfully"
    };
    return response;
  }

  public QrCodeResponse DeleteQrCode(Guid Id) {
    var code = db.QrCodes.Find(Id);
    if(code == null) {
      QrCodeResponse error = new() {
        Success = false,
        Message = "Error retrieving item"
      };
      return error;
    }

    db.QrCodes.Remove(code);
    db.SaveChanges();

    QrCodeResponse response = new() {
      Success = true,
      Message = "Item deleted successfully"
    };
    return response;
  }
}
