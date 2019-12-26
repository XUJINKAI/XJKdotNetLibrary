using System;

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
}
