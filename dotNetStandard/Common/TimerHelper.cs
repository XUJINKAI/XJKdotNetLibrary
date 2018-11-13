using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace XJK
{
    public class TickCountDownEventArgs : EventArgs
    {
        public DateTime StartTime { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public int TotalMilliseconds { get; set; }
        public int Rounds { get; set; }
    }

    public delegate void TickCountDownEventHandler(object sender, TickCountDownEventArgs e);

    public static class TimerHelper
    {
        public static DispatcherTimer NewCountDown(int Milliseconds, EventHandler Callback)
        {
            var timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, Milliseconds),
            };
            timer.Tick += (sender, e) =>
            {
                timer.Stop();
                Callback(sender, e);
            };
            timer.Start();
            return timer;
        }

        public static DispatcherTimer NewTickCountDown(int TickMilliseconds, int TotalMilliseconds, TickCountDownEventHandler TickCallback, TickCountDownEventHandler StopCallback)
        {
            int rounds = 0;
            TimeSpan span;
            DateTime startTime = DateTime.Now;
            var timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, TickMilliseconds),
            };
            timer.Tick += (sender, _) =>
            {
                rounds++;
                span = DateTime.Now - startTime;
                var e = new TickCountDownEventArgs()
                {
                    StartTime = startTime,
                    TimeSpan = span,
                    TotalMilliseconds = TotalMilliseconds,
                    Rounds = rounds,
                };
                if (span.TotalMilliseconds >= TotalMilliseconds)
                {
                    timer.Stop();
                    StopCallback(sender, e);
                }
                else
                {
                    TickCallback(sender, e);
                }
            };
            timer.Start();
            return timer;
        }

        public static DispatcherTimer NewTickTock(int TickMilliseconds, EventHandler Callback)
        {
            var timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, TickMilliseconds),
            };
            timer.Tick += (sender, _) =>
            {
                Callback(sender, new EventArgs());
            };
            timer.Start();
            return timer;
        }
    }
}
