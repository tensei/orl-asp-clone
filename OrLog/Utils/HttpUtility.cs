using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OrLog.Utils
{
    public class HttpUtility
    {
        private readonly HttpClient _client;

        public HttpUtility(HttpClient client)
        {
            _client = client;
            _client.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<string> GetStringAsync(string url)
        {
            try
            {
                return await _client.GetStringAsync(url);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
