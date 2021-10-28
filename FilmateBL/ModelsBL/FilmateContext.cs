using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmateBL.Models
{
    partial class FilmateContext
    {
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

        // returns the accounts if correct credentials, else returns null
        public Account Login(string email, string password) => this.Accounts.FirstOrDefault(a => a.Email == email && a.Pass == password);

        // log in using token
        public Account Login(string token)
        {
            UserAuthToken u = this.UserAuthTokens.FirstOrDefault(a => a.AuthToken == token);
            if (u != null)
                return u.Account;
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
    }
}
