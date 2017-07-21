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
using OrLog.Utils;

namespace OrLog.Controllers
{
    [Route("/")]
    public class StalkController : Controller
    {
        private readonly HttpUtility _client;
        private readonly Cache _cache;
        private readonly Stopwatch _stopwatch;
        public StalkController(HttpUtility client, Cache cache)
        {
            _client = client;
            _cache = cache;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        // GET: Stalk
        public async Task<ActionResult> Index([FromQuery]string channel, [FromQuery]string nick)
        {
            ViewBag.channel = channel;
            ViewBag.nick = nick;
            if (string.IsNullOrWhiteSpace(channel) || string.IsNullOrWhiteSpace(nick))
            {
                Log();
                return View();
            }
            try
            {
                var monthsurl = $"https://overrustlelogs.net/api/v1/{channel}/months.json";
                ViewBag.url = monthsurl;

                var latesturl = $"https://overrustlelogs.net/{channel} chatlog/current/{nick}.txt";
                var log =  await _client.GetAsync(latesturl);
                if (!log.IsSuccessStatusCode)
                {
                    Log();
                    return View();
                }
                var logcontent = await log.Content.ReadAsStringAsync();
                if (!logcontent.StartsWith("["))
                {
                    Log();
                    return View();
                }
                var d = logcontent.Substring(1, 23).Replace("UTC", "-0000");
                
                var date = DateTime.Parse(d);
                ViewBag.latest = logcontent;
                var monthsjson = await _client.GetAsync(monthsurl);
                var monthcontent = await monthsjson.Content.ReadAsStringAsync();
                if (!monthsjson.IsSuccessStatusCode)
                {
                    Log();
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
                Log();
                return View();
            }
            catch(Exception)
            {
                Log();
                return View();
            }
        }

        private void Log()
        {
            _stopwatch.Stop();
            var elapsed = $"{_stopwatch.ElapsedMilliseconds} ms" ;
            var ip = HttpContext.Request.Headers?["CF-Connecting-IP"] ??
                     HttpContext.Connection?.RemoteIpAddress.ToString();
            Console.WriteLine($"{HttpContext.Request.Method} {HttpContext.Request?.Host} - {HttpContext.Request?.Path}{HttpContext.Request.QueryString.Value} - {ip} in {elapsed}");
        }
    }
}