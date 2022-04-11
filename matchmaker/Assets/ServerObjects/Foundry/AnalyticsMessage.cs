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

        public string Button =
#if UNITY_2017_1_OR_NEWER
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name
#else
            ""
#endif
            ;

        public string Label =
#if UNITY_2017_1_OR_NEWER
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponentInChildren<UnityEngine.UI.Text>().text
#else
            ""
#endif
            ;

        public string Interaction =
#if UNITY_2017_1_OR_NEWER
            UnityEngine.EventSystems.EventSystem.current.currentInputModule.input.touchCount > 0 ? "touch" : "click"
#else
            ""
#endif
            ;

        public DateTime timestamp { get; set; }
    }
}
