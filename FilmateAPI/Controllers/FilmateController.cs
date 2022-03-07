using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmateBL.Models;
using FilmateAPI.DataTransferObjects;
using System.Net.Http;
using FilmateAPI.Services;
using Newtonsoft.Json;
using System.Text;
using System.IO;

namespace FilmateAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class FilmateController : ControllerBase
    {
        #region Add connection to the db context using dependency injection
        FilmateContext context;
        public FilmateController(FilmateContext context)
        {
            this.context = context;
        }
        #endregion

        
        [Route("signup")]
        [HttpPost]
        public Account SignUp([FromBody] Account sentAccount)
        {
            Account current = HttpContext.Session.GetObject<Account>("account");
            // Check if user isn't logged in!
            if (current == null)
            {
                HashSalt hashSalt = HashSalt.GenerateSaltedHash(sentAccount.Pass);
                string hash = hashSalt.Hash;
                string salt = hashSalt.Salt;

                Account acc = new Account()
                {
                    Email = sentAccount.Email,
                    Pass = hash,
                    Salt = salt,
                    Username = sentAccount.Username,
                    AccountName = sentAccount.AccountName,
                    Age = sentAccount.Age
                };

                try
                {
                    bool exists = context.UsernameExists(acc.Username) || context.EmailExists(acc.Email);
                    if (!exists)
                    {
                        Account a = context.Register(acc);
                        if (a != null)
                        {
                            HttpContext.Session.SetObject("account", a);

                            Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                            return a;
                        }
                        else
                        {
                            Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
                            return null;
                        }
                        
                    }
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
                }
                catch
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                }
            }
            else
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;

            return null;
        }

        [Route("login")]
        [HttpPost]
        public string Login([FromBody] (string, string) credentials) // credentials is a tuple where item1 is the email and item2 is the password
        {
            Account account = null;
            string email = credentials.Item1;
            string password = credentials.Item2;

            string hash = context.GetHashByEmail(email);
            string salt = context.GetSaltByEmail(email);

            bool validPassword = false;
            if (hash != null && salt != null)
                validPassword = HashSalt.VerifyPassword(password, hash, salt);

            if (validPassword)
            {
                try
                {
                    account = context.Login(email, hash);
                }
                catch
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                }

                // Check username and password
                if (account != null)
                {
                    HttpContext.Session.SetObject("account", account);
                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                    JsonSerializerSettings options = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.All
                    };

                    string json = JsonConvert.SerializeObject(account, options);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    return json;
                }
                else
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    return null;
                }
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return null;
            }
        }

        [Route("login-token")]
        [HttpGet]
        public string LoginToken([FromQuery] string token)
        {
            Account account = null;
            try
            {
                account = context.LoginToken(token);
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }

            // Check username and password
            if (account != null)
            {
                HttpContext.Session.SetObject("account", account);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                JsonSerializerSettings options = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All
                };

                string json = JsonConvert.SerializeObject(account, options);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                return json;
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }

        [Route("generate-token")]
        [HttpGet]
        public string GenerateToken()
        {
            try
            {
                Account current = HttpContext.Session.GetObject<Account>("account");
                if (current != null)
                {
                    bool isUnique = false;
                    string token = "";
                    while (!isUnique)
                    {
                        token = GeneralProcessing.GenerateAlphanumerical(16);
                        isUnique = !context.TokenExists(token);
                    }

                    bool worked = context.AddToken(token, current.AccountId);
                    if (worked)
                    {
                        Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                        return token;
                    }
                    else
                    {
                        Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                        return null;
                    }

                }
                else
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return null;
                }
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        [Route("username-exists")]
        [HttpGet]
        public bool? UsernameExists([FromQuery] string username)
        {
            try
            {
                bool exists = context.UsernameExists(username);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return exists;
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        [Route("email-exists")]
        [HttpGet]
        public bool? EmailExists([FromQuery] string email)
        {
            try
            {
                bool exists = context.EmailExists(email);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return exists;
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        [Route("add-liked-movie")]
        [HttpGet]
        public bool AddLikedMovie([FromQuery] int movieID)
        {
            Account current = HttpContext.Session.GetObject<Account>("account");
            if (current != null)
            {
                try
                {
                    bool added = context.AddLikedMovie(current.AccountId, movieID);
                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return added;
                }
                catch
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return false;
                }
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return false;
            }
        }

        [Route("remove-liked-movie")]
        [HttpGet]
        public bool RemoveLikedMovie([FromQuery] int movieID)
        {
            Account current = HttpContext.Session.GetObject<Account>("account");
            if (current != null)
            {
                try
                {
                    bool added = context.RemoveLikedMovie(current.AccountId, movieID);
                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return added;
                }
                catch
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return false;
                }
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return false;
            }
        }

        [Route("upload-image")]
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            Account account = HttpContext.Session.GetObject<Account>("account");

            if (account != null)
            {
                if (file == null)
                {
                    return BadRequest();
                }

                try
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imgs", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    context.UpdateAccountPfp(file.FileName, account.AccountId);

                    return Ok(new { length = file.Length, name = file.FileName });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest();
                }
            }
            return Forbid();
        }

        [Route("update-profile")]
        [HttpGet]
        public IActionResult UpdateProfile([FromQuery] string email, [FromQuery] string username, [FromQuery] string name, [FromQuery] int age)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                Account account = context.Accounts.FirstOrDefault(a => a.AccountId == loggedInAccount.AccountId);
                if (account == null) return Forbid();

                if (email != null) account.Email = email;
                if (username != null) account.Username = username;
                if (name != null) account.AccountName = name;
                if (age != -1) account.Age = age;
                context.SaveChanges();

                return Ok();
            }

            return Forbid();
        }

        [Route("add-suggestion")]
        [HttpGet]
        public IActionResult AddSuggestion([FromQuery] int ogMovieID, [FromQuery] int suggMovieID)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                if (context.AddSuggestion(loggedInAccount.AccountId, ogMovieID, suggMovieID))
                    return Ok();

                return BadRequest();
            }

            return Forbid();
        }

        [Route("get-suggestions")]
        [HttpGet]
        public string GetSuggestions([FromQuery] int movieID)
        {
            try
            {
                List<Suggestion> suggestions = context.GetSuggestions(movieID);

                JsonSerializerSettings options = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All
                };

                string json = JsonConvert.SerializeObject(suggestions, options);

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return json;
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        [Route("add-vote")]
        [HttpGet]
        public IActionResult AddVote([FromQuery] int suggestionID, [FromQuery]  bool voteType)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                if (context.AddVote(loggedInAccount.AccountId, suggestionID, voteType))
                    return Ok();

                return BadRequest();
            }

            return Forbid();
        }

        [Route("remove-vote")]
        [HttpGet]
        public IActionResult RemoveVote([FromQuery] int suggestionID)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                if (context.RemoveVote(loggedInAccount.AccountId, suggestionID))
                    return Ok();

                return BadRequest();
            }

            return Forbid();
        }

        [Route("add-review")]
        [HttpPost]
        public Review AddReview([FromBody] Review review)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                Review serverReview = context.AddReview(review);
                if (serverReview != null)
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return serverReview;
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
                return null;
            }

            Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
            return null;
        }

        [Route("delete-review")]
        [HttpGet]
        public IActionResult RemoveReview([FromQuery] int reviewID)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                if (context.DeleteReview(loggedInAccount.AccountId, reviewID))
                    return Ok();

                return BadRequest();
            }

            return Forbid();
        }
        
        [Route("get-reviews")]
        [HttpGet]
        public string GetReviews([FromQuery] int movieID)
        {
            try
            {
                List<Review> reviews = context.GetReviews(movieID);

                JsonSerializerSettings options = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.All
                };

                string json = JsonConvert.SerializeObject(reviews, options);

                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                return json;
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }
        }

        [Route("create-group")]
        [HttpPost]
        public string CreateGroup([FromBody] Chat chat)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                Chat returnedChat = context.CreateGroup(chat, loggedInAccount.AccountId);
                if (returnedChat != null)
                {
                    JsonSerializerSettings options = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.All
                    };

                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    string json = JsonConvert.SerializeObject(returnedChat, options);
                    return json;
                }

                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                return null;
            }

            Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            return null;
        }

        [Route("get-groups")]
        [HttpGet]
        public string GetGroups()
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                try
                {
                    List<Chat> chats = context.GetGroups(loggedInAccount.AccountId);

                    JsonSerializerSettings options = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.All
                    };

                    string json = JsonConvert.SerializeObject(chats, options);

                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    return json;
                }
                catch
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return null;
                }
            }

            Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            return null;
        }

        [Route("get-group")]
        [HttpGet]
        public string GetGroup([FromQuery] int chatId)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null)
            {
                try
                {
                    Chat chat = context.GetGroup(chatId);

                    if (chat != null && loggedInAccount.ChatMembers.Any(c => c.ChatId == chat.ChatId))
                    {
                        JsonSerializerSettings options = new JsonSerializerSettings
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.All
                        };

                        string json = JsonConvert.SerializeObject(chat, options);

                        Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                        return json;
                    }

                    Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return null;
                }
                catch
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return null;
                }
            }

            Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            return null;
        }

        [Route("get-chat-liked")]
        [HttpGet]
        public string GetChatLikedMovies([FromQuery] int chatId)
        {
            Account loggedInAccount = HttpContext.Session.GetObject<Account>("account");

            if (loggedInAccount != null && loggedInAccount.ChatMembers.Any(c => c.ChatId == chatId))
            {
                try
                {
                    List<int> likedMovies = context.GetChatLikedMovies(chatId);

                    if (likedMovies != null)
                    {
                        JsonSerializerSettings options = new JsonSerializerSettings
                        {
                            PreserveReferencesHandling = PreserveReferencesHandling.All
                        };

                        string json = JsonConvert.SerializeObject(likedMovies, options);

                        Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                        return json;
                    }

                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return null;
                }
                catch
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                    return null;
                }
            }

            Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
            return null;
        }
    }
}
