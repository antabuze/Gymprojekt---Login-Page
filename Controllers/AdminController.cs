using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using SysAdminLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SysAdminLogin.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        Logger log = LogManager.GetCurrentClassLogger();

        // Hämtar en lista med alla users från API och returnerar en view där de visas upp
        public async Task<IActionResult> Index()
        {
            List<Admin> admins = new List<Admin>();

            var response = await GlobalVariables.WebApiClient.GetAsync("Admins");
            
            log.Info("Retrieving list from API...");
            if (response.IsSuccessStatusCode) 
            { 
                log.Info("List successfully retrieved."); 
            }
            else 
            {
                log.Error(response.StatusCode + " List could not be retrieved."); 
            }
           
            string jsonResponse = await response.Content.ReadAsStringAsync();
            admins = JsonConvert.DeserializeObject<List<Admin>>(jsonResponse);
            var sortedList = admins.OrderBy(a => a.Email);
            return View(sortedList);

        }

        public ActionResult Create(int id = 0)
        {
            // Om ID är 0 skapar man en ny user
            if (id == 0)
            {
                return View(new Admin());
            }
            // Om ID inte är 0 redigerar man en user som redan finns baserat på det ID som passas in.
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("Admins/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<Admin>().Result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Admin admin)
        {
            log.Info("Sending user to API...");
            // Om ID är 0 skickar man en ny admin till API
            if (admin.Id == 0)
            {  
                var postTask = GlobalVariables.WebApiClient.PostAsJsonAsync<Admin>("Admins/PostAdmin", admin);
                postTask.Wait();

                var result = postTask.Result;
                log.Info("User added to the API.");

            }
            // Om ID inte är 0 uppdaterar man en redan befintlig admin hos API med hjälp av PUT
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PutAsJsonAsync("Admins/" + admin.Id, admin).Result;
                if (!response.IsSuccessStatusCode)
                {
                    log.Error(response.StatusCode + " User was not retrieved by the API.");
                }
                else
                {
                    log.Info("User added to the API.");
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            log.Info("Sending delete request to API...");
            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("Admins/" + id.ToString()).Result;
            if (!response.IsSuccessStatusCode)
            {
                log.Error(response.StatusCode + "Request failed");
            }
            else
            {
                log.Info("Successfully deleted the user.");
            }
            return RedirectToAction("Index");
        }

        public ActionResult VerifyAdminLogin()
        {
            return View(new Admin());
        }

        [HttpPost]
        public async Task<IActionResult> VerifyAdminLogin(Admin admin)
        {
            log.Info("Sending login request to API...");
            var response = await GlobalVariables.WebApiClient.PostAsJsonAsync<Admin>("Admins/VerifyAdminLogin/", admin);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                log.Info("Request successfully retrieved by the API.");
            }
            else
            {
                log.Error(response.StatusCode + "Request failed");
            }
            if (jsonResponse.Contains("true"))
            {
                
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("Create");
        }
    }
}
