using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QTTest2
{
    public class Timing : IDisposable
    {
        private static Dictionary<string, List<decimal>> s_timingDatabase;
        
        static Timing()
        {
            s_timingDatabase = new Dictionary<string, List<decimal>>();
        }

        public static void ClearTimings()
        {
            s_timingDatabase.Clear();
        }

        public static IEnumerable<decimal> GetTimings(string name)
        {
            lock (s_timingDatabase)
            {
                if (!s_timingDatabase.ContainsKey(name)) return null;
                return s_timingDatabase[name].AsEnumerable();
            }
        }

        public static IEnumerable<string> GetTimers()
        {
            return s_timingDatabase.Keys.AsEnumerable();
        }

        private string _timerName;
        private HighResolutionTimer _timer;

        public Timing(string TimerName)
        {
            _timerName = TimerName;
            _timer = new HighResolutionTimer();
            _timer.Start();
        }

        public void Dispose()
        {
            _timer.Stop();
            lock (s_timingDatabase)
            {
                if (!s_timingDatabase.ContainsKey(_timerName))
                    s_timingDatabase.Add(_timerName, new List<decimal>() { _timer.MilliSeconds });
                else
                    s_timingDatabase[_timerName].Add(_timer.MilliSeconds);
            }
        }
    }
}
