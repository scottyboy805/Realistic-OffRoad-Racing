using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using RGSK.Extensions;

namespace RGSK
{
    public static class Logger
    {
        const string k_tag = "RGSK";
        static Color k_tagColor = Color.red;

        static void DoLog(Action<string, Object> LogFunction, Object obj, object[] message)
        {
            if (!RGSKCore.Instance.GeneralSettings.enableLogs)
                return;

            LogFunction($"[{k_tag.AddColorTags(k_tagColor)}][{obj.name}] {String.Join("-", message)}", obj);
        }

        static void DoLog(Action<string> LogFunction, object[] message)
        {
            if (!RGSKCore.Instance.GeneralSettings.enableLogs)
                return;

            LogFunction($"[{k_tag.AddColorTags(k_tagColor)}] {String.Join("-", message)}");
        }

        public static void Log(this Object obj, params object[] message) => DoLog(Debug.Log, obj, message);
        public static void LogWarning(this Object obj, params object[] message) => DoLog(Debug.LogWarning, obj, message);
        public static void LogError(this Object obj, params object[] message) => DoLog(Debug.LogError, obj, message);
        public static void Log(params object[] message) => DoLog(Debug.Log, message);
        public static void LogWarning(params object[] message) => DoLog(Debug.LogWarning, message);
        public static void LogError(params object[] message) => DoLog(Debug.LogError, message);
    }
}