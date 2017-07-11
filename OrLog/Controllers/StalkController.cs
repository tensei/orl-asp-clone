using System;
using System.Collections.Generic;
using System.Diagnostics;
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

                var isMonthCached = _cache.Get(monthsurl);
                if (isMonthCached != null)
                {
                    var months = JsonConvert.DeserializeObject<List<string>>(isMonthCached);
                    ViewBag.months = months;
                }
                else
                {
                    var monthsjson = await _client.GetStringAsync(monthsurl);
                    var months = JsonConvert.DeserializeObject<List<string>>(monthsjson);
                    ViewBag.months = months;
                    _cache.Add(monthsjson, monthsurl);
                }

                var latesturl = $"https://overrustlelogs.net/{channel} chatlog/current/{nick}.txt";
                var isLatestCached = _cache.Get(latesturl);
                if (isLatestCached != null)
                {
                    ViewBag.latest = isLatestCached;
                }
                else
                {
                    var log =  await _client.GetStringAsync(latesturl);
                    ViewBag.latest = log;
                    _cache.Add(log, latesturl);
                }
                Log();
                return View();
            }
            catch
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
            Console.WriteLine($"{HttpContext.Request?.Host} - {HttpContext.Request?.Path} - {HttpContext.Request.QueryString.Value} - {ip} in {elapsed}");
        }
    }
}