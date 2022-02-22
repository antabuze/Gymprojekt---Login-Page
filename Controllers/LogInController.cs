using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication; 
using Microsoft.AspNetCore.Authentication.Cookies; 
using System.Security.Claims; 
using SysAdminLogin.Models;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace ReactGym_Employee.Controllers
{
    public class LogInController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(Admin userInfo, string returnUrl = null)
        {
            userInfo.RoleId = "3";
            bool userOk = await VerifyAdminLogin(userInfo); 

            if (userOk == true)
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Email, userInfo.Email));

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));

                HttpContext.Session.SetString("SessionKeyRole", "role3");

                if (returnUrl != null) 
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ViewBag.ErrorMessage = "Fel inloggningsuppgifter"; 

            return View();
        }

        public async Task<bool> VerifyAdminLogin(Admin userinfo)
        {
            HttpClient WebApiClient = new HttpClient();

            WebApiClient.BaseAddress = new Uri("http://informatik3.ei.hv.se/login/api/");

            var response = await WebApiClient.PostAsJsonAsync<Admin>("Admins/VerifyAdminLogin/", userinfo);

            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (jsonResponse.Contains("true"))
            {
                return true;
            }

            return false;
        }

        public async Task<IActionResult> SignOut()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "LogIn");
        }
    }
}