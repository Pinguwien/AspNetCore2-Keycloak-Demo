using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Confidential_App.Models;
using Microsoft.AspNetCore.Authentication;

namespace Confidential_App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult UserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var preferredUsername = identity.FindFirst("preferred_username").Value;
            var email = identity.FindFirst(ClaimTypes.Email).Value;
            var displayName = identity.FindFirst("name").Value;

            return View(new UserInfoModel
                {UserId = userId, Email = email, Username = preferredUsername, DisplayName = displayName});
        }

        public async Task<IActionResult> BearerResult()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            var url1 = "https://localhost:6001/api/values/5";
            var call1 = await CallThirdPartyServiceWithBearerAsync(accessToken, url1);
            var url2 = "https://localhost:6001/api/values";
            var call2 = await CallThirdPartyServiceWithBearerAsync(accessToken, url2);

            return View(new BearerResultModel{MessageOne = call1, MessageTwo = call2});        
        }
       
        private async Task<string> CallThirdPartyServiceWithBearerAsync(string accessToken, string url)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                //hack to get around self-signed cert errors in dev
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    AuthenticationHeaderValue authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", accessToken);
                    httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                    try {
                        var content = await httpClient.GetStringAsync(url);
                        return content;
                    } catch (Exception e) {
                        return "tilt: " + e;
                    }
                }
            }
            
        }
        [Authorize(Roles = "admin")]
        public IActionResult AdminInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
//                string accessToken = await HttpContext.GetTokenAsync("access_token");
//                string idToken = await HttpContext.GetTokenAsync("id_token");
            var userId = identity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var preferredUsername = identity.FindFirst("preferred_username").Value;
            var email = identity.FindFirst(ClaimTypes.Email).Value;
            var displayName = identity.FindFirst("name").Value;

            return View(new UserInfoModel
                {UserId = userId, Email = email, Username = preferredUsername, DisplayName = displayName});
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}