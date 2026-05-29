using Microsoft.EntityFrameworkCore;

namespace BTVN5.Models
{
    public class BookStoreContext : DbContext
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Sach> Sachs { get; set; }
        public DbSet<ChuDe> ChuDes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sach>().ToTable("Sach");
            modelBuilder.Entity<ChuDe>().ToTable("ChuDe");

            modelBuilder.Entity<Sach>()
                .HasOne(s => s.ChuDe)
                .WithMany(cd => cd.Sachs)
                .HasForeignKey(s => s.MaChuDe);
        }
    }
}