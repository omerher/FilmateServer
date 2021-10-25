using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace FilmateBL.Models
{
    [Table("Account")]
    [Index(nameof(Email), Name = "I_Account_Email", IsUnique = true)]
    [Index(nameof(Username), Name = "I_Account_Username", IsUnique = true)]
    [Index(nameof(Salt), Name = "UQ__Account__A152BCCE67209FA8", IsUnique = true)]
    public partial class Account
    {
        public Account()
        {
            AccountVotesHistories = new HashSet<AccountVotesHistory>();
            ChatMembers = new HashSet<ChatMember>();
            LikedMovies = new HashSet<LikedMovie>();
            Msgs = new HashSet<Msg>();
            Reviews = new HashSet<Review>();
            Suggestions = new HashSet<Suggestion>();
            UserAuthTokens = new HashSet<UserAuthToken>();
        }

        [Key]
        [Column("AccountID")]
        public int AccountId { get; set; }
        [Required]
        [StringLength(255)]
        public string AccountName { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [Required]
        [StringLength(255)]
        public string Username { get; set; }
        [Required]
        [StringLength(1000)]
        public string Pass { get; set; }
        public int Age { get; set; }
        [Required]
        [StringLength(255)]
        public string ProfilePicture { get; set; }
        public bool IsAdmin { get; set; }
        [Required]
        [StringLength(255)]
        public string Salt { get; set; }

        [InverseProperty(nameof(AccountVotesHistory.Account))]
        public virtual ICollection<AccountVotesHistory> AccountVotesHistories { get; set; }
        [InverseProperty(nameof(ChatMember.Account))]
        public virtual ICollection<ChatMember> ChatMembers { get; set; }
        [InverseProperty(nameof(LikedMovie.Account))]
        public virtual ICollection<LikedMovie> LikedMovies { get; set; }
        [InverseProperty(nameof(Msg.Account))]
        public virtual ICollection<Msg> Msgs { get; set; }
        [InverseProperty(nameof(Review.Account))]
        public virtual ICollection<Review> Reviews { get; set; }
        [InverseProperty(nameof(Suggestion.Account))]
        public virtual ICollection<Suggestion> Suggestions { get; set; }
        [InverseProperty(nameof(UserAuthToken.Account))]
        public virtual ICollection<UserAuthToken> UserAuthTokens { get; set; }
    }
}
