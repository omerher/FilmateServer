using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmateBL.Models;

namespace FilmateAPI.DataTransferObjects
{
    public class AccountDTO
    {
        public AccountDTO(Account a)
        {
            this.AccountId = a.AccountId;
            this.AccountName = a.AccountName;
            this.Email = a.Email;
            this.Username = a.Username;
            this.Pass = a.Pass;
            this.Age = a.Age;
            this.ProfilePicture = a.ProfilePicture;
            this.IsAdmin = a.IsAdmin;
            this.Salt = a.Salt;
            this.SignUpDate = a.SignUpDate;
        }

        public AccountDTO()
        {
            AccountVotesHistories = new List<AccountVotesHistory>();
            ChatMembers = new List<ChatMember>();
            LikedMovies = new List<LikedMovie>();
            Msgs = new List<Msg>();
            Reviews = new List<Review>();
            Suggestions = new List<Suggestion>();
            UserAuthTokens = new List<UserAuthToken>();
        }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Pass { get; set; }
        public int Age { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsAdmin { get; set; }
        public string Salt { get; set; }
        public DateTime SignUpDate { get; set; }

        public virtual List<AccountVotesHistory> AccountVotesHistories { get; set; }
        public virtual List<ChatMember> ChatMembers { get; set; }
        public virtual List<LikedMovie> LikedMovies { get; set; }
        public virtual List<Msg> Msgs { get; set; }
        public virtual List<Review> Reviews { get; set; }
        public virtual List<Suggestion> Suggestions { get; set; }
        public virtual List<UserAuthToken> UserAuthTokens { get; set; }
    }
}
