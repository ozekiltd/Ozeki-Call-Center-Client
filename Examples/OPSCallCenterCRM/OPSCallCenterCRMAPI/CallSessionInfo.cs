using System;

namespace OPSCallCenterCRMAPI
{
    public class CallSessionInfo : EventArgs
    {
        public string Callee { get; set; }
        public string Caller { get; set; }
        public TimeSpan RingDuration { get; set; }
        public string SessionID { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan StateDuration { get; set; }
        public TimeSpan TalkDuration { get; set; }
    }
}
