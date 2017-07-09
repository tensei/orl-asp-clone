using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OrLog.Controllers
{
    [Route("/")]
    public class StalkController : Controller
    {
        private readonly HttpClient _client;

        public StalkController(HttpClient client)
        {
            _client = client;
        }

        // GET: Stalk
        public async Task<ActionResult> Index([FromQuery]string channel, [FromQuery]string nick)
        {
            if (string.IsNullOrWhiteSpace(channel) || string.IsNullOrWhiteSpace(nick))
            {
                return View();
            }
            try
            {
                ViewBag.channel = channel;
                ViewBag.nick = nick;
                ViewBag.url = $"https://overrustlelogs.net/api/v1/{ViewBag.channel}/months.json";
                var monthsjson = await _client.GetStringAsync($"https://overrustlelogs.net/api/v1/{ViewBag.channel}/months.json");
                var months = JsonConvert.DeserializeObject<List<string>>(monthsjson);
                ViewBag.months = months;
                return View();
            }
            catch
            {
                return View();
            }
        }
        
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Index()
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here
        //        ViewBag.channel = channel;
        //        ViewBag.nick = nick;
        //        ViewBag.url = $"https://overrustlelogs.net/api/v1/{ViewBag.channel}/months.json";
        //        var monthsjson = await _client.GetStringAsync($"https://overrustlelogs.net/api/v1/{ViewBag.channel}/months.json");
        //        var months = JsonConvert.DeserializeObject<List<string>>(monthsjson);
        //        ViewBag.months = months;
        //        return View();
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        
    }
}