using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        // GET: Stalk
        [HttpGet]
        public async Task<ActionResult> Index([FromQuery]string channel, [FromQuery]string nick)
        {
            ViewBag.channel = channel;
            ViewBag.nick = nick;
            var monthsurl = $"https://overrustlelogs.net/api/v1/{channel}/months.json";
            ViewBag.url = monthsurl;
            var latesturl = $"https://overrustlelogs.net/{channel} chatlog/current/{nick}.txt";

            if (string.IsNullOrWhiteSpace(channel) || string.IsNullOrWhiteSpace(nick))
            {
                return View();
            }
            try
            {
                var logcontent = await GetString(latesturl);
                if (logcontent == null || !logcontent.StartsWith("["))
                {
                    Console.WriteLine($"Couldn't get {nick} for channel {channel}");
                    ViewBag.Error = $"Couldn't get {nick} for channel {channel}";
                    return View();
                }
                var d = logcontent.Substring(1, 23).Replace("UTC", "-0000");
                
                var date = DateTime.Parse(d);
                ViewBag.latest = logcontent;

                var monthcontent = await GetString(monthsurl);
                if (monthcontent == null)
                {
                    ViewBag.Error = $"Failed getting months for {channel}";
                    return View();
                }
                var months = JsonConvert.DeserializeObject<List<string>>(monthcontent);

                ViewBag.latestmonth = date.ToString("MMMM yyyy");
                var newmonths = new List<string>();
                months.ForEach(m =>
                {
                    var mo = DateTime.Parse(m);

                    if (mo < date && mo.ToString("Y") != date.ToString("Y"))
                    {
                        newmonths.Add(m);
                    }
                });
                ViewBag.months = newmonths;
                return View();
            }
            catch(Exception)
            {
                HttpContext.Response.StatusCode = 404;
                return View();
            }
        }

        private async Task<string> GetString(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var log = await client.GetAsync(url);
                    if (log.IsSuccessStatusCode)
                    {
                        return await log.Content.ReadAsStringAsync();
                    }
                    HttpContext.Response.StatusCode = 404;
                    return null;
                }
            }
            catch (Exception)
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
        }
    }
}