using GPF.ServerObjects;
using System;

namespace ServerObjects
{
    public abstract class AnalyticsMessage : ServerObjectMessage, ITimeStampReceiver
    {
        public string DeviceId = GetDeviceId();

        public string Platform = GetPlatform();

        public string Element = GetElement();

        public string Label = GetLabel();

        public string Interaction = GetInteraction();

        public string Language = GetDeviceLanguage();

        public string DeviceName = GetDeviceName();

        public string Resolution = GetDeviceResolution();

        public DateTime timestamp { get; set; }

        static string GetDeviceId()
        {
#if UNITY_2017_1_OR_NEWER
            return UnityEngine.SystemInfo.deviceUniqueIdentifier;
#else
            return "";
#endif
        }

        static string GetPlatform()
        {
#if UNITY_2017_1_OR_NEWER
            return UnityEngine.SystemInfo.operatingSystem;
#else
            return "";
#endif
        }

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

        static string GetDeviceLanguage()
        {
#if UNITY_2017_1_OR_NEWER
            return UnityEngine.Application.systemLanguage.ToString();
#else
            return "";
#endif
        }

        static string GetDeviceName()
        {
#if UNITY_2017_1_OR_NEWER
            return UnityEngine.SystemInfo.deviceModel;
#else
            return "";
#endif
        }

        static string GetDeviceResolution()
        {
#if UNITY_2017_1_OR_NEWER
            return UnityEngine.Screen.currentResolution.ToString();
#else
            return "";
#endif
        }
    }
}
