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
    }
}
