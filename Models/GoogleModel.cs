using System;
using System.ComponentModel.DataAnnotations;

namespace appacd.Models
{
    public class GoogleTokenRequest
    {
        public string IdToken { get; set; }
    }

    public class GoogleTokenPayload
    {
        public string Sub { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
        public string Aud { get; set; }
    }

    public class GoogleNewUser
    {
        public string UserIdGoogle { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
    }

    public class StatusRequest
    {
        public string userId { get; set; }
        public string Pesan { get; set; }
    }
}