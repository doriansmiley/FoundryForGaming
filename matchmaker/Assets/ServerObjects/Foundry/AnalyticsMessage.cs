using GPF.ServerObjects;
using System;

namespace ServerObjects
{
    public abstract class AnalyticsMessage : ServerObjectMessage, ITimeStampReceiver
    {
        public string DeviceId =
#if UNITY_2017_1_OR_NEWER
            UnityEngine.SystemInfo.deviceUniqueIdentifier
#else
            ""
#endif
            ;

        public string Platform =
#if UNITY_2017_1_OR_NEWER
            UnityEngine.SystemInfo.operatingSystem
#else
            ""
#endif
            ;

        public DateTime timestamp { get; set; }
    }
}
