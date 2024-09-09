using Microsoft.EntityFrameworkCore;
using QRC.Models;
using QRC.Models.QRCodeModel;

namespace QRC.Services {
  public class DbService : DbContext {
    public DbService(DbContextOptions options) : base(options) { }
    public DbSet<QrCodeModel> QrCodes { get; set; }
    public DbSet<User> Users { get; set; }
  }
}
