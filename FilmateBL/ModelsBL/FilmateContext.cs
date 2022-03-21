using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmateBL.Models
{
    partial class FilmateContext
    {
        public Account GetAccountByID(int id) => this.Accounts.FirstOrDefault(a => a.AccountId == id);

        // receives an object of type Account and adds it to the DB. Returns the Account object.
        public Account Register(Account a)
        {
            try
            {
                this.Accounts.Add(a);
                this.SaveChanges();

                return a;
            }
            catch
            {
                return null;
            }
        }

        // receives the needed info to create an account, creates the object and registers it. Returns the Account object.
        public Account Register(string name, string email, string username, string password, int age, string salt)
        {
            Account a = new Account()
            {
                AccountName = name,
                Email = email,
                Username = username,
                Pass = password,
                Age = age,
                Salt = salt,
            };

            return Register(a);
        }

        // returns salt of given email
        public string GetSaltByEmail(string email)
        {
            Account a = this.Accounts.FirstOrDefault(a => a.Email == email);
            if (a != null)
                return a.Salt;
            return null;
        }

        // returns hash of given email
        public string GetHashByEmail(string email)
        {
            Account a = this.Accounts.FirstOrDefault(a => a.Email == email);
            if (a != null)
                return a.Pass;
            return null;
        }

        public void DeleteAuthToken(string authToken)
        {
            UserAuthToken userAuthToken = this.UserAuthTokens.FirstOrDefault(a => a.AuthToken == authToken);
            if (userAuthToken != null)
                this.UserAuthTokens.Remove(userAuthToken);
        }

        // returns the accounts if correct credentials, else returns null
        public Account Login(string email, string password) => this.Accounts.FirstOrDefault(a => a.Email == email && a.Pass == password);

        // log in using token
        public Account LoginToken(string token)
        {
            UserAuthToken u = this.UserAuthTokens.FirstOrDefault(a => a.AuthToken == token);
            if (u != null && u.CreationDate.AddMonths(1).CompareTo(DateTime.Now) > 0) // check that the auth token is less than 1 month old
                return GetAccountByID(u.AccountId);
            return null;
        }

        // returns true if username exists otherwise returns false
        public bool UsernameExists(string username) => this.Accounts.Any(a => a.Username == username);

        // returns true if email exists otherwise returns false
        public bool EmailExists(string email) => this.Accounts.Any(a => a.Email == email);

        // returns true of token exists in the db, otherwise false
        public bool TokenExists(string token) => this.UserAuthTokens.Any(a => a.AuthToken == token);

        // adds token to db and returns true if it succeeded
        public bool AddToken(string token, int id)
        {
            try
            {
                this.UserAuthTokens.Add(new UserAuthToken()
                {
                    AccountId = id,
                    AuthToken = token
                });
                this.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddLikedMovie(int accountID, int movieId)
        {
            Account account = this.Accounts.FirstOrDefault(a => a.AccountId == accountID);
            if (account != null)
            {
                account.LikedMovies.Add(new LikedMovie()
                {
                    AccountId = accountID,
                    MovieId = movieId
                });
                this.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveLikedMovie(int accountID, int movieId)
        {
            Account account = this.Accounts.FirstOrDefault(a => a.AccountId == accountID);
            if (account != null)
            {
                LikedMovie likedMovie = account.LikedMovies.FirstOrDefault(m => m.MovieId == movieId);

                if (likedMovie != null)
                {
                    account.LikedMovies.Remove(likedMovie);
                    this.SaveChanges();
                    return true;
                }

                return false;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateAccountPfp(string path, int id)
        {
            Account account = this.Accounts.FirstOrDefault(a => a.AccountId == id);
            if (account != null)
            {
                account.ProfilePicture = path;
                this.SaveChanges();
                return true;
            }

            return false;
        }

        public bool UpdateGroupPfp(string path, int id)
        {
            Chat group = this.Chats.FirstOrDefault(c => c.ChatId == id);
            if (group != null)
            {
                group.Icon = path;
                this.SaveChanges();
                return true;
            }

            return false;
        }

        public bool AddSuggestion(int accountID, int ogMovieID, int suggMovieID)
        {
            try
            {
                if (!this.Suggestions.Any(s => s.SuggestionMovieId == suggMovieID && s.OriginalMovieId == ogMovieID))
                {
                    this.Suggestions.Add(new Suggestion()
                    {
                        AccountId = accountID,
                        OriginalMovieId = ogMovieID,
                        SuggestionMovieId = suggMovieID
                    });
                    this.SaveChanges();

                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public List<Suggestion> GetSuggestions(int movieID) => this.Suggestions.Where(s => s.OriginalMovieId == movieID).OrderByDescending(s => s.Upvotes - s.Downvotes).ToList();

        public bool AddVote(int accountID, int suggestionID, bool voteType)
        {
            try
            {
                Suggestion suggestion = this.Suggestions.FirstOrDefault(s => s.SuggestionId == suggestionID);
                if (suggestion != null)
                {
                    RemoveVote(accountID, suggestionID);
                    suggestion.AccountVotesHistories.Add(new AccountVotesHistory()
                    {
                        SuggestionId = suggestionID,
                        AccountId = accountID,
                        VoteType = voteType
                    });
                    if (voteType) // voteType == true means that it is an upvote
                        suggestion.Upvotes += 1;
                    else
                        suggestion.Downvotes += 1;

                    this.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public bool RemoveVote(int accountID, int suggestionID)
        {
            try
            {
                AccountVotesHistory accountVotesHistory = this.AccountVotesHistories.FirstOrDefault(a => a.AccountId == accountID && a.SuggestionId == suggestionID);
                if (accountVotesHistory != null)
                {
                    Suggestion suggestion = this.Suggestions.FirstOrDefault(s => s.SuggestionId == suggestionID);
                    if (accountVotesHistory.VoteType)  // voteType == true means that it is an upvote
                        suggestion.Upvotes -= 1;
                    else
                        suggestion.Downvotes -= 1;

                    this.AccountVotesHistories.Remove(accountVotesHistory);
                    this.SaveChanges();

                    return true;
                }
                else
                    return false;
            }
            catch { return false; }
        }

        public Review AddReview(Review review)
        {
            try
            {
                this.Reviews.Add(review);
                this.SaveChanges();
                return review;
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteReview(int accountID, int reviewID)
        {
            try
            {
                Review review = this.Reviews.FirstOrDefault(r => r.ReviewId == reviewID);
                if (review != null)
                {
                    this.Reviews.Remove(review);
                    this.SaveChanges();

                    return true;
                }
                else
                    return false;
            }
            catch { return false; }
        }

        public List<Review> GetReviews(int movieID) => this.Reviews.Where(r => r.MovieId == movieID).ToList(); // maybe add upvotes and downvotes

        public bool CreateChatMember(int accountId, int chatId, bool isAdmin = false)
        {
            try
            {
                this.ChatMembers.Add(new ChatMember()
                {
                    AccountId = accountId,
                    ChatId = chatId,
                    IsAdmin = isAdmin
                });
                this.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Chat CreateGroup(Chat c, int accountId)
        {
            try
            {
                c.InviteCode = GenerateInviteCode(c.ChatId);

                this.Chats.Add(c);
                this.SaveChanges();

                CreateChatMember(accountId, c.ChatId, true);

                return c;
            }
            catch
            {
                return null;
            }
        }

        private string GenerateInviteCode(int chatId)
        {
            string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            while (true)
            {
                Random random = new Random();
                string code = "";
                for (int i = 0; i < 8; i++)
                    code += allowedChars[random.Next(0, allowedChars.Length)];

                if (!this.Chats.Any(c => c.InviteCode == code))
                    return code;
            }
        }

        public List<Chat> GetGroups(int accountID)
        {
            try
            {
                List<ChatMember> userChats = this.ChatMembers.Where(c => c.AccountId == accountID).ToList();
                List<Chat> chats = new List<Chat>();
                foreach (ChatMember chatMember in userChats)
                    chats.Add(chatMember.Chat);

                return chats;
            }
            catch
            {
                return null;
            }
        }

        public Chat GetGroup(int chatId)
        {
            try
            {
                return this.Chats.Where(c => c.ChatId == chatId).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public List<int> GetChatLikedMovies(int chatId)
        {
            try
            {
                Chat chat = this.Chats.Where(c => c.ChatId == c.ChatId).FirstOrDefault();
                ICollection<ChatMember> chatMembers = chat.ChatMembers;

                List<int> likedMovies = new List<int>();
                foreach (ChatMember cm in chatMembers)
                {
                    likedMovies.AddRange(cm.Account.LikedMovies.Select(l => l.MovieId).ToList());
                }

                return likedMovies;
            }
            catch
            {
                return null;
            }
        }

        public Msg AddMsg(Msg m)
        {
            try
            {
                this.Msgs.Add(m);
                this.SaveChanges();
                return m;
            }
            catch
            {
                return null;
            }
        }
    }
}
