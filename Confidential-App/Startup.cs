using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Confidential_App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;                                                                               
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options => { options.AccessDeniedPath = "/account/denied"; })
                .AddAutomaticTokenManagement()              
                .AddOpenIdConnect("Keycloak", options =>
                {
                    options.Authority = Configuration["Authentication:oidc:Authority"];
                    options.ClientId = Configuration["Authentication:oidc:ClientId"];
                    options.ClientSecret = Configuration["Authentication:oidc:ClientSecret"];
                    
                    // Configure the scope
                    options.ResponseType = "code";
                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    
                    options.CallbackPath = new PathString("/oauth/callback");
                    options.SignedOutCallbackPath = new PathString("/oauth/logout");
                    
                    options.RequireHttpsMetadata = false;
                    options.SaveTokens = true;
                    options.RemoteSignOutPath = "/keycloak/k_logout";
                    options.SignedOutRedirectUri = "/";
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = context =>
                        {
                            var resourceAccess = JObject.Parse(context.Principal.FindFirst("resource_access").Value);
                            var clientResource = resourceAccess[context.Options.ClientId];
                            var clientRoles = clientResource["roles"];
                            var currentIdentity = (ClaimsIdentity) context.Principal.Identity;

                            foreach (var clientRole in clientRoles)
                            {
                                currentIdentity.AddClaim(new Claim(ClaimTypes.Role, clientRole.ToString()));
                            }

                            return Task.CompletedTask;
                        },

                        OnRedirectToIdentityProvider = context =>
                        {
                            context.ProtocolMessage.SetParameter("audience",
                                Configuration["Authentication:oidc:ClientId"]);

                            return Task.FromResult(0);
                        }
                    };
                });
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}