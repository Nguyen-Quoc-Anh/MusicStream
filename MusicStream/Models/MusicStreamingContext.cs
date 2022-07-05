using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace MusicStream.Models
{
    public partial class MusicStreamingContext : DbContext
    {
        public MusicStreamingContext()
        {
        }

        public MusicStreamingContext(DbContextOptions<MusicStreamingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<AlbumGenre> AlbumGenres { get; set; }
        public virtual DbSet<ArtistAlbum> ArtistAlbums { get; set; }
        public virtual DbSet<ArtistTrack> ArtistTracks { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<GenreOfTrack> GenreOfTracks { get; set; }
        public virtual DbSet<LikeTrack> LikeTracks { get; set; }
        public virtual DbSet<PlayListTrack> PlayListTracks { get; set; }
        public virtual DbSet<Playlist> Playlists { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Track> Tracks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("MyConStr"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("accountID");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Fullname)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("fullname");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("password");

                entity.Property(e => e.RoleId).HasColumnName("roleID");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Account__roleID__25869641");
            });

            modelBuilder.Entity<Album>(entity =>
            {
                entity.ToTable("Album");

                entity.Property(e => e.AlbumId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("albumID");

                entity.Property(e => e.AlbumName)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("albumName");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image");

                entity.Property(e => e.ReleaseDate)
                    .HasColumnType("date")
                    .HasColumnName("releaseDate")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.View)
                    .HasColumnName("view")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<AlbumGenre>(entity =>
            {
                entity.HasKey(e => new { e.GenreId, e.AlbumId })
                    .HasName("PK__AlbumGen__DB0F854C35709862");

                entity.ToTable("AlbumGenre");

                entity.Property(e => e.GenreId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("genreID");

                entity.Property(e => e.AlbumId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("albumID");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.AlbumGenres)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AlbumGenr__album__3B75D760");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.AlbumGenres)
                    .HasForeignKey(d => d.GenreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AlbumGenr__genre__3A81B327");
            });

            modelBuilder.Entity<ArtistAlbum>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.AlbumId })
                    .HasName("PK__Artist_A__153CD6D01ED4A575");

                entity.ToTable("Artist_Album");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("accountID");

                entity.Property(e => e.AlbumId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("albumID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ArtistAlbums)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Artist_Al__accou__2C3393D0");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.ArtistAlbums)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Artist_Al__album__2D27B809");
            });

            modelBuilder.Entity<ArtistTrack>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.TrackId })
                    .HasName("PK__ArtistTr__C73C7AA52B5E12FF");

                entity.ToTable("ArtistTrack");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("accountID");

                entity.Property(e => e.TrackId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("trackID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ArtistTracks)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ArtistTra__accou__34C8D9D1");

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.ArtistTracks)
                    .HasForeignKey(d => d.TrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ArtistTra__track__35BCFE0A");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ClassId)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("classID")
                    .IsFixedLength(true);

                entity.Property(e => e.ClassName)
                    .HasMaxLength(15)
                    .IsUnicode(false)
                    .HasColumnName("className")
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comment");

                entity.Property(e => e.CommentId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("commentID");

                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("accountID");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(300)
                    .HasColumnName("content");

                entity.Property(e => e.ParentId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("parentID");

                entity.Property(e => e.TrackId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("trackID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Comment__account__4D94879B");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK__Comment__parentI__4F7CD00D");

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.TrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Comment__trackID__4E88ABD4");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genre");

                entity.Property(e => e.GenreId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("genreID");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<GenreOfTrack>(entity =>
            {
                entity.HasKey(e => new { e.GenreId, e.TrackId })
                    .HasName("PK__GenreOfT__090F2939EFED0A2B");

                entity.ToTable("GenreOfTrack");

                entity.Property(e => e.GenreId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("genreID");

                entity.Property(e => e.TrackId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("trackID");

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.GenreOfTracks)
                    .HasForeignKey(d => d.GenreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GenreOfTr__genre__3E52440B");

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.GenreOfTracks)
                    .HasForeignKey(d => d.TrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__GenreOfTr__track__3F466844");
            });

            modelBuilder.Entity<LikeTrack>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.TrackId })
                    .HasName("PK__LikeTrac__C73C7AA5DA4DD92B");

                entity.ToTable("LikeTrack");

                entity.Property(e => e.AccountId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("accountID");

                entity.Property(e => e.TrackId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("trackID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.LikeTracks)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LikeTrack__accou__4AB81AF0");

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.LikeTracks)
                    .HasForeignKey(d => d.TrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__LikeTrack__track__49C3F6B7");
            });

            modelBuilder.Entity<PlayListTrack>(entity =>
            {
                entity.HasKey(e => new { e.PlaylistId, e.TrackId })
                    .HasName("PK__PlayList__E0714E9D7371A5D0");

                entity.ToTable("PlayListTrack");

                entity.Property(e => e.PlaylistId)
                    .HasMaxLength(50)
                    .HasColumnName("playlistID");

                entity.Property(e => e.TrackId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("trackID");

                entity.HasOne(d => d.Playlist)
                    .WithMany(p => p.PlayListTracks)
                    .HasForeignKey(d => d.PlaylistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PlayListT__playl__45F365D3");

                entity.HasOne(d => d.Track)
                    .WithMany(p => p.PlayListTracks)
                    .HasForeignKey(d => d.TrackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PlayListT__track__46E78A0C");
            });

            modelBuilder.Entity<Playlist>(entity =>
            {
                entity.ToTable("Playlist");

                entity.Property(e => e.PlaylistId)
                    .HasMaxLength(50)
                    .HasColumnName("playlistID");

                entity.Property(e => e.AccountId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("accountID");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image");

                entity.Property(e => e.IsPrivate)
                    .HasColumnName("isPrivate")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Playlists)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Playlist__accoun__4222D4EF");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.Property(e => e.RoleId).HasColumnName("roleID");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Track>(entity =>
            {
                entity.ToTable("Track");

                entity.Property(e => e.TrackId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("trackID");

                entity.Property(e => e.AlbumId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("albumID");

                entity.Property(e => e.Duration)
                    .HasColumnName("duration")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image");

                entity.Property(e => e.Mp3)
                    .IsRequired()
                    .HasColumnName("mp3");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.View)
                    .HasColumnName("view")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Tracks)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Track__albumID__30F848ED");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
