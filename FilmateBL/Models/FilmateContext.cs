using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace FilmateBL.Models
{
    public partial class FilmateContext : DbContext
    {
        public FilmateContext()
        {
        }

        public FilmateContext(DbContextOptions<FilmateContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AccountVotesHistory> AccountVotesHistories { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ChatMember> ChatMembers { get; set; }
        public virtual DbSet<ChatSuggestion> ChatSuggestions { get; set; }
        public virtual DbSet<LikedMovie> LikedMovies { get; set; }
        public virtual DbSet<Msg> Msgs { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Suggestion> Suggestions { get; set; }
        public virtual DbSet<UserAuthToken> UserAuthTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server = localhost\\SQLEXPRESS; Database=FilmateDB; Trusted_Connection=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.ProfilePicture).HasDefaultValueSql("('/imgs/default_pfp.jpg')");
            });

            modelBuilder.Entity<AccountVotesHistory>(entity =>
            {
                entity.HasKey(e => e.VoteId)
                    .HasName("PK_AccountVotesHistory_VoteID");

                entity.Property(e => e.VotedDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.AccountVotesHistories)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountVoteHistory_AccountID");

                entity.HasOne(d => d.Suggestion)
                    .WithMany(p => p.AccountVotesHistories)
                    .HasForeignKey(d => d.SuggestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AccountVotesHistory_SuggestionID");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<ChatMember>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.ChatId })
                    .HasName("PK_ChatMembers_AccountID_ChatID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.ChatMembers)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatMembers_AccountID");

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.ChatMembers)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatMembers_ChatID");
            });

            modelBuilder.Entity<ChatSuggestion>(entity =>
            {
                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Chat)
                    .WithMany()
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatSuggestions_ChatID");
            });

            modelBuilder.Entity<LikedMovie>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.MovieId })
                    .HasName("PK_LikedMovies_AccountID_MovieID");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.LikedMovies)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LikedMovies_AccountID");
            });

            modelBuilder.Entity<Msg>(entity =>
            {
                entity.Property(e => e.SentDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Msgs)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Msg_AccountID");

                entity.HasOne(d => d.Chat)
                    .WithMany(p => p.Msgs)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Msg_ChatID");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.PostDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Review_AccountID");
            });

            modelBuilder.Entity<Suggestion>(entity =>
            {
                entity.Property(e => e.PostDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Suggestions)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Suggestion_AccountID");
            });

            modelBuilder.Entity<UserAuthToken>(entity =>
            {
                entity.HasKey(e => e.AuthToken)
                    .HasName("PK_UserAuthToken_AuthToken");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.UserAuthTokens)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAuthToken_AccountID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
