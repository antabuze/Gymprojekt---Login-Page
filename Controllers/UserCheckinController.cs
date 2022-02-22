using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SysAdminLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using NLog;
using Microsoft.AspNetCore.Authorization;

namespace SysAdminLogin.Controllers
{
    [Authorize]
    public class UserCheckinController : Controller
    {
        Logger log = LogManager.GetCurrentClassLogger();
        // Hämtar en lista med alla users från API och returnerar en view där de visas upp
        public async Task<IActionResult> Index()
        {
            List<UserCheckin> userCheckins = new List<UserCheckin>();
            var response = await GlobalVariables.WebApiClient.GetAsync("UserCheckins");
            
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
            userCheckins = JsonConvert.DeserializeObject<List<UserCheckin>>(jsonResponse);
            return View(userCheckins);
        }

        public ActionResult CheckIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckIn(int employeeId)
        {    
            var response = await GlobalVariables.WebApiClient.PostAsJsonAsync<int>("UserCheckins/CheckIn/", employeeId);
            log.Info("Check-in was sent to the API...");
            if (!response.IsSuccessStatusCode)
            {
                log.Error("Statuscode: '" + response.StatusCode + "' User:'" + employeeId + "' could not be checked in.");

            }
            return RedirectToAction("Index");
        }

        public ActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(int employeeId)
        {
            var response = await GlobalVariables.WebApiClient.PutAsJsonAsync<int>("UserCheckins/CheckOut/", employeeId);
            log.Info("Check-out sent to the API...");
            if (!response.IsSuccessStatusCode)
            {
                log.Error("Statuscode: '" + response.StatusCode + "' User:'" + employeeId + "' could not be checked out.");

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyAdminLogin(Admin admin)
        {
            var response = await GlobalVariables.WebApiClient.PostAsJsonAsync<Admin>("Admins/VerifyAdminLogin/", admin);
            log.Info("Login request was sent to the API...");
            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                log.Error("Admin: " + admin.Email.ToString() + " failed to log in.");
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("Create");
        }

        public ActionResult Create(int id = 0)
        {
            // Om ID är 0 skapar man en ny user
            if (id == 0)
            {
                return View(new UserCheckin());
            }
            // Om ID inte är 0 redigerar man en user som redan finns baserat på det ID som passas in.
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.GetAsync("UserCheckins/" + id.ToString()).Result;
                return View(response.Content.ReadAsAsync<UserCheckin>().Result);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCheckin userCheckin)
        {
            // Om ID är 0 skickar man en ny user till API
            if (userCheckin.Id == 0)
            {
                var postTask = GlobalVariables.WebApiClient.PostAsJsonAsync<UserCheckin>("UserCheckins/", userCheckin);
                postTask.Wait();

                var result = postTask.Result;
            }
            // Om ID inte är 0 uppdaterar man en redan befintlig user hos API med hjälp av PUT
            else
            {
                HttpResponseMessage response = GlobalVariables.WebApiClient.PutAsJsonAsync("UserCheckins/" + userCheckin.Id, userCheckin).Result;
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = GlobalVariables.WebApiClient.DeleteAsync("UserCheckins/" + id.ToString()).Result;
            return RedirectToAction("Index");
        }
     
        public ActionResult UserCheckinsInterval()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserCheckinsInterval(UserCheckin userCheckin)
        {


            string link = "UserCheckins/GetUserCheckinsInterval?userId=" + userCheckin.EmployeeId + "&startDate=" + userCheckin.StartTime + "&endDate=" + userCheckin.EndTime;
            var response = await GlobalVariables.WebApiClient.GetAsync(link);

            string apiResponse = await response.Content.ReadAsStringAsync();
            List<UserCheckin> userCheckins = JsonConvert.DeserializeObject<List<UserCheckin>>(apiResponse);

            return View("UserCheckinsIntervalResult", userCheckins);
        }
    }
}
