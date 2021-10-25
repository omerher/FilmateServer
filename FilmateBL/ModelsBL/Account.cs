using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmateBL.Models
{
    partial class Account
    {
        public Account(Account a)
        {
            AccountVotesHistories = new List<AccountVotesHistory>();
            ChatMembers = new List<ChatMember>();
            LikedMovies = new List<LikedMovie>();
            Msgs = new List<Msg>();
            Reviews = new List<Review>();
            Suggestions = new List<Suggestion>();
            UserAuthTokens = new List<UserAuthToken>();

            this.AccountId = a.AccountId;
            this.AccountName = a.AccountName;
            this.Email = a.Email;
            this.Username = a.Username;
            this.Pass = a.Pass;
            this.Age = a.Age;
            this.ProfilePicture = a.ProfilePicture;
            this.IsAdmin = a.IsAdmin;
            this.Salt = a.Salt;
        }
    }
}
