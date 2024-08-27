using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using QRC.Models;

namespace QRC.Services;
public class AuthService {
  private readonly Db db;
  private readonly IConfiguration config;
  public AuthService(Db _db, IConfiguration _config) {
    db = _db;
    config = _config;
  }

  public UserCreatedResponse NewUser(UserDto userDto) {
    var response = new UserCreatedResponse();
    var user = db.Users.FirstOrDefault(item => item.Email == userDto.Email);
    if(user != null) {
      response.Success = false;
      response.Message = "User with provided email already exists";
      return response;
    }

    var newUser = new User {
      Email = userDto.Email,
      CreatedAt = DateTime.UtcNow,
      Role = userDto.Role,
      PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userDto.Password, HashType.SHA512, workFactor: 12),
    };

    db.Users.Add(newUser);
    db.SaveChanges();
    response.Success = true;
    response.Message = "User created successfully";
    return response;
  }

  public LoginResponse Login(UserLogin userLogin) {
    var response = new LoginResponse();
    var user = db.Users.FirstOrDefault(item => item.Email == userLogin.Email);
    if(user == null) {
      response.Success = false;
      response.Message = "User not found";
      return response;
    }

    if(!BCrypt.Net.BCrypt.EnhancedVerify(userLogin.Password, user.PasswordHash, HashType.SHA512)) {
      response.Success = false;
      response.Message = "Invalid username or password";
      return response;
    }
    TokenResponse token = CreateToken(user);
    response.Success = true;
    response.Message = "Login successful";
    response.Data = new LoginSuccess {
      Email = user.Email,
      Role = user.Role,
      Token = token.Token,
      Exp = token.ExpirationDate
    };
    return response;
  }

  private TokenResponse CreateToken(User user){
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
    var claims = new List<Claim> {
      new("UserId", user.UserId.ToString()),
      new(ClaimTypes.Email, user.Email),
      new(ClaimTypes.Role, user.Role.ToString())
    };
    DateTime exp = DateTime.Now.AddMinutes(15);
    var token = new JwtSecurityToken(
      config["Jwt:Issuer"],
      config["Jwt:Audience"],
      claims: claims,
      expires: exp,
      signingCredentials: credentials
    );
    TokenResponse tokenResponse = new() {
      Token = new JwtSecurityTokenHandler().WriteToken(token),
      ExpirationDate = exp,
    };
    return tokenResponse;
  }

}

