using System;

namespace ITA.Common.Host
{
    [Serializable]
    public enum EComponentStatus
    {
        Error = 0,
        Running = 1,
        Stopped = 2,
        Paused = 3,
        Starting = 4
    }
}
