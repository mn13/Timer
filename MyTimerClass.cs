using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTimer
{
    class MyTimerClass
    {
        Action<string> _print;        
        Action _stop;
        System.Timers.Timer MyTimer;
        DateTime time;
        Double secs;
        public MyTimerClass(Action<string> Prnt = null, Action Stop = null)
        {
            MyTimer = new System.Timers.Timer(1000);
            MyTimer.Enabled = false;
            MyTimer.Elapsed += OnTimedEvent;
            _stop = Stop;
            _print = Prnt;
        }
        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            secs--;
            if (secs >= 0)
            {
                time = time.AddSeconds(-1);
                _print(string.Format("{0:m:ss}", time));
            }
            else
            {
                MyTimer.Stop();
                _stop();
            }
        }
        public void Start(string minutes, string seconds)
        {
            time = new DateTime();
            time = time.AddMinutes(Double.Parse(minutes));
            time = time.AddSeconds(Double.Parse(seconds));
            secs = time.Second + time.Minute*60;
            MyTimer.Start();
        }
        public void Pause ()
        {
            MyTimer.Enabled = !MyTimer.Enabled;
        }
    }
}
