using Microsoft.Extensions.Configuration;
using System;

namespace HPVTesting.Business.Helpers
{
    public class AppSettings
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ApiUrl => _configuration.GetSection("Api")["ApiUrl"];
        public string WebUrl => _configuration.GetSection("Web")["WebUrl"];
    }

    public class MailSettings
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string MailFrom => _configuration.GetSection("MailSettings")["From"];
        public string MailHost => _configuration.GetSection("MailSettings")["Host"];
        public string MailPort => _configuration.GetSection("MailSettings")["Port"];
        public string MailPassword => _configuration.GetSection("MailSettings")["Password"];
        public bool EnableMail => Convert.ToBoolean(_configuration.GetSection("MailSettings")["EnableMail"]);
        public string FromName => _configuration.GetSection("MailSettings")["FromName"];
    }

    public class Jwt
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Key => _configuration.GetSection("Jwt")["Key"];
        public string Issuer => _configuration.GetSection("Jwt")["Issuer"];
    }

    public class GoogleOAuth
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ClientId => _configuration.GetSection("GoogleOAuth")["ClientId"];

        public string ClientSecret => _configuration.GetSection("GoogleOAuth")["ClientSecret"];
    }
}