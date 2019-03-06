using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace Bearer_App.JwtConfig
{
    public static class JwtConfiguration{
        public static void ConfigureJwtAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });
        }

        public static void ConfigureJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Audience = JwtTokenDefinitions.Audience;
                options.Authority = JwtTokenDefinitions.Issuer;
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters(){
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = JwtTokenDefinitions.Issuer,
                    ValidateLifetime = true
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    },
                    OnTokenValidated = t => {
                        var resourceAccess = JObject.Parse(t.Principal.FindFirst("resource_access").Value);
                        var clientResource = resourceAccess[t.Principal.FindFirstValue("aud")];
                        var clientRoles = clientResource["roles"];

                        foreach (var clientRole in clientRoles)
                        {
                            var roleClaim = new Claim(ClaimTypes.Role, clientRole.ToString());
                            ((ClaimsIdentity) t.Principal.Identity).AddClaim(roleClaim);
                        }

                        return Task.CompletedTask;
                    }
                };
            });
}
    }
}