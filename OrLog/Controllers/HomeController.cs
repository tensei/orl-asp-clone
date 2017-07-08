using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace OrLog.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var channelsjson = await _httpClient.GetStringAsync("https://overrustlelogs.net/api/v1/channels.json");
                var channel = JsonConvert.DeserializeObject<List<string>>(channelsjson);
                ViewData["channels"] = channel;
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View("Error");
            }
        }
        [HttpGet("{channel}")]
        public async Task<IActionResult> Months(string channel)
        {
            try
            {
                var monthsjson = await _httpClient.GetStringAsync($"https://overrustlelogs.net/api/v1/{channel}/months.json");
                var months = JsonConvert.DeserializeObject<List<string>>(monthsjson);
                ViewData["channel"] = channel;
                ViewData["months"] = months;
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View("Error");
            }
        }
        [HttpGet("{channel}/{month}")]
        public async Task<IActionResult> Days(string channel, string month)
        {
            try
            {
                var daysjson = await _httpClient.GetStringAsync($"https://overrustlelogs.net/api/v1/{channel}/{month}/days.json");
                var days = JsonConvert.DeserializeObject<List<string>>(daysjson);
                ViewData["channel"] = channel;
                ViewData["month"] = month;
                ViewData["days"] = days;
                return View();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View("Error");
            }
        }
    }
}
