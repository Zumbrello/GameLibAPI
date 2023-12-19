using System;
using System.Collections.Generic;
using GameLib.Models;
using Microsoft.EntityFrameworkCore;

namespace GameLib.Context;

public partial class GtcymkznContext : DbContext
{
    public GtcymkznContext()
    {
    }

    public GtcymkznContext(DbContextOptions<GtcymkznContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<GameGenre> GameGenres { get; set; }

    public virtual DbSet<Genere> Generes { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userrole> Userroles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=snuffleupagus.db.elephantsql.com;Database=gtcymkzn;Username=gtcymkzn;password=1FgXRFfmDumZegpQ-cnmPs-VpcMfRm1L");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("btree_gin")
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("citext")
            .HasPostgresExtension("cube")
            .HasPostgresExtension("dblink")
            .HasPostgresExtension("dict_int")
            .HasPostgresExtension("dict_xsyn")
            .HasPostgresExtension("earthdistance")
            .HasPostgresExtension("fuzzystrmatch")
            .HasPostgresExtension("hstore")
            .HasPostgresExtension("intarray")
            .HasPostgresExtension("ltree")
            .HasPostgresExtension("pg_stat_statements")
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("pgrowlocks")
            .HasPostgresExtension("pgstattuple")
            .HasPostgresExtension("tablefunc")
            .HasPostgresExtension("unaccent")
            .HasPostgresExtension("uuid-ossp")
            .HasPostgresExtension("xml2");

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.IdDeveloper).HasName("developer_pkey");

            entity.ToTable("developer");

            entity.Property(e => e.IdDeveloper)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_developer");
            entity.Property(e => e.Developer1).HasColumnName("developer");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => new { e.IdUser, e.IdGame }).HasName("favorite_pkey");

            entity.ToTable("favorite");

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.Note)
                .HasMaxLength(50)
                .HasColumnName("note");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.IdGame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_id_game_fkey");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_id_user_fkey");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(e => e.IdGame).HasName("games_pkey");

            entity.ToTable("games");

            entity.Property(e => e.IdGame)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_game");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.GameName).HasColumnName("game_name");
            entity.Property(e => e.IdDeveloper).HasColumnName("id_developer");
            entity.Property(e => e.IdPublisher).HasColumnName("id_publisher");
            entity.Property(e => e.MainImage)
                .HasMaxLength(255)
                .HasColumnName("main_image");
            entity.Property(e => e.ReleaseDate)
                .HasMaxLength(100)
                .HasColumnName("release_date");
            entity.Property(e => e.SystemRequestMin).HasColumnName("system_request_min");
            entity.Property(e => e.SystemRequestRec).HasColumnName("system_request_rec");

            entity.HasOne(d => d.IdDeveloperNavigation).WithMany(p => p.Games)
                .HasForeignKey(d => d.IdDeveloper)
                .HasConstraintName("games_id_developer_fkey");

            entity.HasOne(d => d.IdPublisherNavigation).WithMany(p => p.Games)
                .HasForeignKey(d => d.IdPublisher)
                .HasConstraintName("games_id_publisher_fkey");
        });

        modelBuilder.Entity<GameGenre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_genres_pk");

            entity.ToTable("game_genres");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IdGame).HasColumnName("id_game");
            entity.Property(e => e.IdGenre).HasColumnName("id_genre");

            entity.HasOne(d => d.IdGameNavigation).WithMany(p => p.GameGenres)
                .HasForeignKey(d => d.IdGame)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("game_genres_id_game_fkey");

            entity.HasOne(d => d.IdGenreNavigation).WithMany(p => p.GameGenres)
                .HasForeignKey(d => d.IdGenre)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("game_genres_id_genre_fkey");
        });

        modelBuilder.Entity<Genere>(entity =>
        {
            entity.HasKey(e => e.IdGenre).HasName("genere_pkey");

            entity.ToTable("genere");

            entity.Property(e => e.IdGenre)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_genre");
            entity.Property(e => e.Gener)
                .HasMaxLength(250)
                .HasColumnName("gener");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.IdPublisher).HasName("publisher_pkey");

            entity.ToTable("publisher");

            entity.Property(e => e.IdPublisher)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_publisher");
            entity.Property(e => e.Publisher1).HasColumnName("publisher");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.IdUser)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id_user");
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .HasColumnName("email");
            entity.Property(e => e.Login)
                .HasMaxLength(250)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(250)
                .HasColumnName("password_");
            entity.Property(e => e.UserRole).HasColumnName("user_role");

            entity.HasOne(d => d.UserRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.UserRole)
                .HasConstraintName("users_user_role_fkey");
        });

        modelBuilder.Entity<Userrole>(entity =>
        {
            entity.HasKey(e => e.IdRole).HasName("userrole_pkey");

            entity.ToTable("userrole");

            entity.Property(e => e.IdRole)
                .ValueGeneratedNever()
                .HasColumnName("id_role");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .HasColumnName("role_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
