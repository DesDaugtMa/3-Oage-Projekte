using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel.DataAnnotations;

namespace Chat.Database
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string EMailAddress { get; set; }

        [MaxLength(16)]
        public string Username { get; set; }

        public string Password { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        public void GenerateHashedPassword(string passwordToHash)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var identityUser = new IdentityUser(EMailAddress);
            Password = hasher.HashPassword(identityUser, passwordToHash);
        }

        public bool CheckPassword(string passwordToCheck)
        {
            var hasher = new PasswordHasher<IdentityUser>();
            var identityUser = new IdentityUser(EMailAddress);
            return PasswordVerificationResult.Failed != hasher.VerifyHashedPassword(identityUser, Password, passwordToCheck);
        }
    }
}
