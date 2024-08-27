using Microsoft.EntityFrameworkCore;
using QRC.Models.QRCodes;
using QRC.Models;

namespace QRC.Services {
  public class Db : DbContext {
    public Db(DbContextOptions options) : base(options) { }

    public DbSet<QrCodeModel> QrCodes { get; set; }
    public DbSet<User> Users { get; set; }
  }
}
