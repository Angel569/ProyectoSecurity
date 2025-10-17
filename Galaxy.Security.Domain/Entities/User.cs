using Galaxy.Security.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy.Security.Domain.Entities
{
    public class User
    {
        //Keys
        public Guid UserId { get; private set; }
        public string Id { get; private set; }

        //Principal Properties
        public string FullName { get; private set; }
        public string UserName { get; private set; }
        public string NormalizedUserName { get; private set; }
        public string Email { get; private set; }
        public string NormalizedEmail { get; private set; }

        //Email Status
        public bool EmailConfirmed { get; private set; }

        //Security Properties
        public string Password { get; private set; }

        //Contact Properties
        public string? PhoneNumber { get; private set; }
        public bool PhoneNumerConfirmed { get; private set; }

        //Lockout
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; private set; }
        public int AccessFailedCount { get; private set; }

        public User(Guid userId, string fullName, string userName, string email, string password)
        {
            UserId = userId == Guid.Empty ? Guid.NewGuid() : userId;
            Id = Guid.NewGuid().ToString();
            FullName = fullName;
            UserName = userName;
            NormalizedUserName = userName.ToUpperInvariant();
            Email = email;
            NormalizedEmail = email.ToUpperInvariant();
            Password = password;
            EmailConfirmed = false;
            LockoutEnabled = true;
            AccessFailedCount = 0;
        }

        public static User Create(Guid userId, string fullName, string userName, string email, string password)
        {
            if (string.IsNullOrEmpty(fullName)) throw new DomainException("Full name is required");
            if (string.IsNullOrEmpty(userName)) throw new DomainException("username is required");
            if (string.IsNullOrEmpty(email)) throw new DomainException("email is required");
            if (string.IsNullOrEmpty(password)) throw new DomainException("password is required");

            return new User(userId, fullName, userName, email, password);
        }

        ///Metodos de dominio
        public void ConfirmEmail()
        {
            if (EmailConfirmed)
                throw new Exception("Email is confirmed");

            EmailConfirmed = true;
        }

        public void ChangeFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required");

            FullName = fullName;
        }

        public void ChangeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            Email = email;
            NormalizedEmail = email.ToUpperInvariant();
            EmailConfirmed = false;
        }
    }
}
