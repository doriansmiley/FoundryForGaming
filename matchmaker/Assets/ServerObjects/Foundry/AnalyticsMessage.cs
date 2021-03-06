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

        public string Element = GetElement();

        public string Label = GetLabel();

        public string Interaction = GetInteraction();

        public DateTime timestamp { get; set; }

        static string GetElement()
        {
#if UNITY_2017_1_OR_NEWER
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                return UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
            }
            else
            {
                return "automation";
            }
#else
            return "";
#endif
        }

        static string GetLabel()
        {
#if UNITY_2017_1_OR_NEWER
            var label =
                UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject.GetComponentInChildren<UnityEngine.UI.Text>()?.text;
            if (!string.IsNullOrEmpty(label))
            {
                return label;
            }
            else
            {
                return "automation";
            }
#else
            return "";
#endif
        }

        static string GetInteraction()
        {
#if UNITY_2017_1_OR_NEWER
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                return UnityEngine.EventSystems.EventSystem.current.currentInputModule.input.touchCount > 0 ? "touch" : "click";
            }
            else
            {
                return "automation";
            }
#else
            return "";
#endif
        }
    }
}
