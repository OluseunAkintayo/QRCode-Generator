using Microsoft.EntityFrameworkCore;
using QRC.Models;
using QRC.Models.QRCodeModel;

namespace QRC.Services {
  public class Db : DbContext {
    public Db(DbContextOptions options) : base(options) { }
    public DbSet<QrCodeModel> QrCodes { get; set; }
    public DbSet<User> Users { get; set; }
  }
}
