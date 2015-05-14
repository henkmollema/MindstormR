using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace MindstormR.Client.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ControlClick(int id, string command)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://test.henkmollema.nl");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(string.Format("robot/{0}/{1}", id, command)).Result;
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
    }
}