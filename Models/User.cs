using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QRC.Models;

[Index(nameof(Email), IsUnique = true)]

public class User {
  [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public Guid UserId { get; set; }
  [EmailAddress]
  public string Email { get; set; } = string.Empty;
  public string PasswordHash { get; set; } = string.Empty;
  public Roles Role { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? ModifiedAt { get; set; }
  public DateTime? LastLogin { get; set; }
}

public class UserDto {
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
  public Roles Role { get; set; }
}

public class UserCreatedResponse {
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
}

public class UserLogin {
  public string Email { get; set; } = string.Empty;
  public string Password { get; set; } = string.Empty;
}


public class LoginResponse {
  public bool Success { get; set; }
  public string Message { get; set; } = string.Empty;
  public LoginSuccess? Data { get; set; }
}


public class LoginSuccess {
  public string Email { get; set;} = string.Empty;
  public Roles Role { get; set; }
  public string Token { get; set; } = string.Empty;
  public DateTime Exp { get; set; }
}

public class TokenResponse {
  public string Token { get; set; } = string.Empty;
  public DateTime ExpirationDate { get; set; }
}

public enum Roles {
  Admin,
  User
}
