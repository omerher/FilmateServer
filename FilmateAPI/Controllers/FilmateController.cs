using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmateBL.Models;
using FilmateAPI.DataTransferObjects;
using System.Net.Http;

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


        [Route("register")]
        [HttpGet]
        public Account Register([FromQuery] string email, [FromQuery] string username, [FromQuery] string password, [FromQuery] int age, [FromQuery] string salt)
        {
            Account current = HttpContext.Session.GetObject<Account>("account");
            // Check if user isn't logged in!
            if (current == null)
            {
                Account acc = null;
                try
                {
                    bool exists = context.UsernameExists(username) || context.EmailExists(email);
                    if (!exists)
                    {
                        string name = username;
                        acc = context.Register(name, email, username, password, age, salt);
                        return acc;
                    }
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
                }
                catch
                {
                    Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
                }

                if (acc != null)
                {
                    HttpContext.Session.SetObject("player", acc);
                    Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                    return acc;
                }
                else
                    return null;
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                return null;
            }
        }


        //[Route("register")]
        //[HttpPost]
        //public Account Register([FromBody] Account newAccount)
        //{
        //    if (newAccount == null)
        //    {
        //        Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
        //        return null;
        //    }

        //    Account current = HttpContext.Session.GetObject<Account>("account");
        //    // Check if user isn't logged in!
        //    if (current == null)
        //    {
        //        Account acc = null;
        //        try
        //        {
        //            bool exists = context.UsernameExists(newAccount.Username) || context.EmailExists(newAccount.Email);
        //            if (!exists)
        //            {
        //                acc = context.Register(newAccount);
        //                return new Account(acc);
        //            }
        //            Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
        //        }
        //        catch
        //        {
        //            Response.StatusCode = (int)System.Net.HttpStatusCode.Conflict;
        //        }

        //        if (acc != null)
        //        {
        //            HttpContext.Session.SetObject("player", acc);
        //            Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

        //            return acc;
        //        }
        //        else
        //            return null;
        //    }
        //    else
        //    {
        //        Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
        //        return null;
        //    }
        //}

        [Route("get-salt")]
        [HttpGet]
        public string GetSalt([FromQuery] string email)
        {
            try
            {
                string? salt = context.GetSaltByEmail(email);
                if (salt != null)
                    return System.Web.HttpUtility.UrlEncode(salt);
                else
                    return "Error";
            }
            catch
            {
                return "Error";
            }
        }

        [Route("get-hash")]
        [HttpGet]
        public string GetHash([FromQuery] string email)
        {
            try
            {
                string? hash = context.GetHashByEmail(email);
                if (hash != null)
                    return System.Web.HttpUtility.UrlEncode(hash);
                else
                    return "Error";
            }
            catch
            {
                return "Error";
            }
        }

        [Route("login")]
        [HttpGet]
        public Account Login([FromQuery] string email, [FromQuery] string password)
        {
            Account account = null;
            try
            {
                account = context.Login(email, password);
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            } 

            // Check username and password
            if (account != null)
            {
                HttpContext.Session.SetObject("player", account);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                return account;
            }
            else
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                return null;
            }
        }

        [Route("login")]
        [HttpPost]
        public AccountDTO Login([FromQuery] string token)
        {
            Account account = null;
            try
            {
                account = context.Login(token);
            }
            catch
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }

            // Check username and password
            if (account != null)
            {
                AccountDTO aDTO = new AccountDTO(account);

                HttpContext.Session.SetObject("player", aDTO);
                Response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                return aDTO;
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
                AccountDTO current = HttpContext.Session.GetObject<AccountDTO>("account");
                if (current != null)
                {
                    bool isUnique = false;
                    string token = "";
                    while (!isUnique)
                    {
                        token = GeneralProcessing.GenerateAlphanumerical(16);
                        isUnique = context.TokenExists(token);
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
    }
}
