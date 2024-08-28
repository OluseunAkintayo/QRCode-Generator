using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace QRC.Models.QRCodeModel;

[Index(nameof(UrlId), IsUnique = true)]

public class QrCodeModel {
  [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string ImageUrl { get; set; } = string.Empty;
  public string SiteUrl { get; set; } = string.Empty;
  public string UrlId { get; set; } = string.Empty;
  public int VisitCount { get; set; } = 0;
  public bool IsActive{ get; set; }
  public Guid CreatedBy { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime ModifiedAt { get; set; }
}

public class QrCodeDto {
  public string Title { get; set; } = string.Empty;
  public string SiteUrl { get; set; } = string.Empty;
}


public class QrCodeListResponse {
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
  public List<QrCodeModel>? Data { get; set; }
}

public class QrCodeErrorResponse {
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
  public IActionResult? Error { get; set; }
}

public class QrCodeResponse {
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
  public QrCodeModel? Data { get; set; }
}

public class QRScanResponse {
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;
}

