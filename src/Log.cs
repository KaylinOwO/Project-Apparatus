using UnityEngine;
using System.IO;

namespace ProjectApparatus
{
    public class Log : MonoBehaviour
    {
        private static string logText;
        private static Vector2 scrollPos;
        private static int logNumber = 0;

        internal enum logType
        { 
            info,
            warning,
            error,
            message,
            none
        }

        public static Log Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Log();
                }
                return instance;
            }
        }

        public void ConsoleWindow(int windowID)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Clear Log"))
                Clear();

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(300f));
            GUILayout.Label(logText, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        internal static Color GetColorFromType(logType type)
        {
            switch (type)
            {
                case logType.info: return Color.grey;
                case logType.warning: return Color.yellow;
                case logType.error: return Color.red;
                case logType.message: return Color.blue;
                case logType.none: return Color.white;
                default: return Color.white;
            }
        }

        public static void LogColor(string message, Color color)
        {
            logText += $"{logNumber}. <color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>\n";
            logNumber++;
        }

        private static void LogCommon(string message, logType type)
        {
            logText += $"{logNumber}. <color=#{ColorUtility.ToHtmlStringRGB(GetColorFromType(type))}>{message}</color>\n";
            logNumber++;           
        }

        public static void Info(object message)
        {
            LogCommon(message.ToString(), logType.info);
        }

        public static void Warning(object message)
        {
            LogCommon(message.ToString(), logType.warning);
        }

        public static void Error(object message)
        {
            LogCommon(message.ToString(), logType.error);
        }

        public static void Message(object message)
        {
            LogCommon(message.ToString(), logType.message);
        }

        public static void Clear()
        {
            logNumber = 0;
            logText = "";
        }

        private static Log instance;
    }
}
