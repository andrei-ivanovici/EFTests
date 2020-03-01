using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ef.Main.Data
{
    public partial class EfTestsContext : DbContext
    {
        private readonly RlsSecurityContext _securityContext;

        public EfTestsContext()
        {
        }

        public EfTestsContext(DbContextOptions<EfTestsContext> options, RlsSecurityContext securityContext)
            : base(options)
        {
            _securityContext = securityContext;
        }

        public virtual DbSet<AuthorBook> AuthorBooks { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<History> History { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            if (!optionsBuilder.IsConfigured)
            {
                var sqlConnection = PrepareConnection();
                optionsBuilder.UseSqlServer(sqlConnection);
            }
        }

        private SqlConnection PrepareConnection()
        {
            var connection = new SqlConnection("server=.;database=EfTests; Trusted_Connection= True");
            connection.StateChange += (s, ev) =>
            {
                if (ev.CurrentState == ConnectionState.Open && !string.IsNullOrEmpty(_securityContext.Owner))
                {
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = @"exec sp_set_session_context @key=N'owner', @value=@owner";
                    cmd.Parameters.AddWithValue("@owner", _securityContext.Owner);
                    cmd.ExecuteNonQuery();
                }
            };
            return connection;
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

                entity.HasOne(p => p.Profile)
                    .WithOne(p => p.Author)
                    .HasForeignKey<Author>(a => a.ProfileId);
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