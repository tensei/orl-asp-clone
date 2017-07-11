using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrLog.Utils
{
    public class Cache
    {
        public Dictionary<string, DateTime> _times;
        public Dictionary<string, string> _logs;

        public Cache()
        {
            _times = new Dictionary<string, DateTime>();
            _logs = new Dictionary<string, string>();
        }

        public string Get(string key)
        {
            if (!_times.ContainsKey(key))
            {
                return null;
            }
            var age = DateTime.UtcNow - _times[key];
            if (age >= TimeSpan.FromMinutes(5))
            {
                RemoveKeys(new List<string>{key});
                return null;
            }
            if (!_logs.ContainsKey(key))
            {
                return null;
            }
            return _logs[key];
        }

        public void Add(string log, string key)
        {
            _times[key] = DateTime.UtcNow;
            _logs[key] = log;
            var toRemove = new List<string>();
            foreach (var dateTime in _times)
            {
                var isOld = DateTime.UtcNow - dateTime.Value >= TimeSpan.FromMinutes(5);
                if (isOld)
                {
                    toRemove.Add(dateTime.Key);
                }
            }
            RemoveKeys(toRemove);
        }

        private void RemoveKeys(List<string> keys)
        {
            try
            {
                keys.ForEach(r => _times.Remove(r));
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                keys.ForEach(r => _logs.Remove(r));
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
