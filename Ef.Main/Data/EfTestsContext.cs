using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace Ef.Main.Data
{
    public partial class EfTestsContext : DbContext
    {
        public static readonly ILoggerFactory logger
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public EfTestsContext()
        {
        }

        public EfTestsContext(DbContextOptions<EfTestsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(logger);
            optionsBuilder.EnableSensitiveDataLogging();
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http: //go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("server=.;database=EfTests; Trusted_Connection= True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorBook>(entity =>
            {
                entity.HasKey(e => new {e.AuthorId, e.BookId})
                    .HasName("AuthorBooks_pk")
                    .IsClustered(false);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.AuthorBooks)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AuthorBooks_Author_Id_fk");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.AuthorBooks)
                    .HasForeignKey(d => d.BookId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AuthorBooks_Books_Id_fk");
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("Author_pk")
                    .IsClustered(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.HasOne<Profile>(p => p.Profile)
                    .WithOne(p => p.Author);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("Books_pk")
                    .IsClustered(false);

                entity.Property(e => e.Isbn)
                    .HasColumnName("ISBN")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}