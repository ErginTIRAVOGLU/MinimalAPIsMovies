using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalAPIsMovies.Entities;

namespace MinimalAPIsMovies.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<GenreMovie> GenresMovies { get; set; }
        public DbSet<ActorMovie> ActorsMovies { get; set; }
        public DbSet<Error> Errors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genre>().Property(g => g.Name).HasMaxLength(150);

            modelBuilder.Entity<Actor>().Property(a => a.Name).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(a => a.Picture).IsUnicode(false);

            modelBuilder.Entity<Movie>().Property(m => m.Title).HasMaxLength(250);
            modelBuilder.Entity<Movie>().Property(m => m.Poster).IsUnicode();

            modelBuilder.Entity<GenreMovie>().HasKey(gm => new { gm.MovieId, gm.GenreId });

            modelBuilder.Entity<ActorMovie>().HasKey(am => new { am.MovieId, am.ActorId });



            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RolesClaims");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UsersClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UsersLogins");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UsersRoles");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UsersTokens");

        }
    }
}
