using System;
using Microsoft.Extensions.Configuration;

namespace Bearer_App.JwtConfig
{
    public class JwtTokenDefinitions
    {
        public static void LoadFromConfig(IConfiguration configuration)
        {
            var config = configuration.GetSection("JwtConfiguration");
            Audience = config.GetValue<string>("Audience");
            Issuer = config.GetValue<string>("Issuer");
            TokenExpirationTime = TimeSpan.FromMinutes(config.GetValue<int>("TokenExpirationTime"));
        }

        public static TimeSpan TokenExpirationTime { get; set; } = TimeSpan.FromHours(60);

        public static string Issuer { get; set; } = "";
        
        public static string Audience { get; set; } = "";
        
    }
}