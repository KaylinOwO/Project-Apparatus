using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Globalization;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using UnityEngine.UIElements;

namespace ProjectApparatus
{
    public static class PAUtils
    {
        static public BindingFlags protectedFlags = (BindingFlags.NonPublic | BindingFlags.Instance);

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int key);
        [DllImport("User32.dll")]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public static void ShowMessageBox(string message)
        {
            MessageBox(IntPtr.Zero, message, "Project Apparatus", 0);
        }

        public static void SetValue(object instance, string variableName, object value, BindingFlags bindingFlags)
        {
            Type type = instance.GetType();
            FieldInfo fieldInfo = type.GetField(variableName, bindingFlags);
            fieldInfo?.SetValue(instance, value);
        }
        public static object GetValue(object instance, string variableName, BindingFlags bindingFlags)
        {
            Type type = instance.GetType();
            FieldInfo fieldInfo = type.GetField(variableName, bindingFlags);
            if (fieldInfo != null)
                return fieldInfo.GetValue(instance);

            return default(object);
        }

        public static object CallMethod(object instance, string methodName, BindingFlags bindingFlags, params object[] parameters)
        {
            Type type = instance.GetType();
            MethodInfo methodInfo = type.GetMethod(methodName, bindingFlags);

            if (methodInfo != null)
            {
                object result = methodInfo.Invoke(instance, parameters);
                return result;
            }

            return null;
        }

        public static string ConvertFirstLetterToUpperCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(input);
        }

        public static string TruncateString(string inputStr, int charLimit)
        {
            if (inputStr.Length <= charLimit)
            {
                return inputStr;
            }
            else
            {
                return inputStr.Substring(0, charLimit - 3) + "...";
            }
        }

        public static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screen)
        {
            screen = camera.WorldToViewportPoint(world);
            screen.x *= (float)UnityEngine.Screen.width;
            screen.y *= (float)UnityEngine.Screen.height;
            screen.y = (float)UnityEngine.Screen.height - screen.y;
            return screen.z > 0f;
        }

        public static float GetDistance(Vector3 pos1, Vector3 pos2)
        {
            return (float)Math.Round((double)Vector3.Distance(pos1, pos2));
        }

        public static void SendChatMessage(string str, int playerid = -1)
        {
            string finalString = str;
            if (HUDManager.Instance.lastChatMessage == finalString) // Bypass chat spam prevention
                finalString += "\r";
            HUDManager.Instance.AddTextToChatOnServer(finalString, playerid);
        }
    }

    public static class UI
    {
        public enum Tabs
        {
            Start = 0,
            Self,
            Misc,
            ESP,
            Players,
            Graphics,
            Upgrades,
            Settings
        }

        public static Tabs nTab = 0;
        public static string strTooltip = null;

        public static void Reset()
        {
            strTooltip = null;
        }

        public static void TabContents(string strTabName, UI.Tabs tabToDisplay, Action tabContent)
        {
            if (nTab == tabToDisplay)
            {
                if (strTabName != null)
                    Header(strTabName);
                tabContent.Invoke();
            }
        }

        public static bool CenteredButton(string strName)
        {
            bool bReturn;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            bReturn = GUILayout.Button(strName, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            return bReturn;
        }

        public static void Tab<T>(string strTabName, ref T iTab, T iTabEle, bool bCenter = false)
        {
            if (bCenter ? CenteredButton(strTabName) : GUILayout.Button(strTabName))
            {
                iTab = iTabEle;
                Settings.Instance.windowRect.height = 400f;
            }
        }

        public static void Header(string str)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(str, GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        public static void ColorPicker(string str, ref Color col)
        {
            GUILayout.Label(str + " (R: " + Mathf.RoundToInt(col.r * 255f)
                + ", G: " + Mathf.RoundToInt(col.g * 255f)
                + ", B: " + Mathf.RoundToInt(col.b * 255f) + ")",
                Array.Empty<GUILayoutOption>());

            GUILayout.BeginHorizontal();
            col.r = GUILayout.HorizontalSlider(col.r, 0, 1f, GUILayout.Width(80));
            col.g = GUILayout.HorizontalSlider(col.g, 0, 1f, GUILayout.Width(80));
            col.b = GUILayout.HorizontalSlider(col.b, 0, 1f, GUILayout.Width(80));
            GUILayout.EndHorizontal();
        }

        public static bool Checkbox(ref bool var, string option, string tooltip = "")
        {
            bool previousValue = var;

            var = GUILayout.Toggle(var, option, Array.Empty<GUILayoutOption>());

            Rect lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition))
                strTooltip = tooltip;

            return previousValue != var;
        }

        public static void Button(string option, string tooltip, Action action)
        {
            if (GUILayout.Button(option))
                action.Invoke();

            Rect lastRect = GUILayoutUtility.GetLastRect();
            if (lastRect.Contains(Event.current.mousePosition))
                strTooltip = tooltip;
        }

        public static void RenderTooltip()
        {
            if (!Settings.Instance.settingsData.b_Tooltips || string.IsNullOrEmpty(strTooltip))
                return;

            GUIStyle tooltipStyle = GUI.skin.label;
            GUIContent tooltipContent = new GUIContent(strTooltip);
            float tooltipWidth = tooltipStyle.CalcSize(tooltipContent).x + 10f;
            float tooltipHeight = tooltipStyle.CalcHeight(tooltipContent, tooltipWidth - 10f) + 10f;

            Vector2 mousePos = Event.current.mousePosition;
            Color theme = Settings.Instance.settingsData.c_Theme;
            GUI.color = new Color(theme.r, theme.g, theme.b, 0.8f);

            Rect tooltipRect = new Rect(mousePos.x + 20f, mousePos.y + 20f, tooltipWidth, tooltipHeight);
            GUI.Box(tooltipRect, GUIContent.none);

            GUI.color = Color.white;
            GUI.Label(new Rect(tooltipRect.x + 5f, tooltipRect.y + 5f, tooltipWidth - 10f, tooltipHeight - 10f), strTooltip);
        }

        public static void Keybind(ref int Key)
        {
            string strKey = "Unbound";
            if (Key > 0)
                strKey = keyNames.ContainsKey(Key) ? keyNames[Key] : "Unknown";

            GUILayout.Button(strKey);
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Event guiEvent = Event.current;

            if (lastRect.Contains(guiEvent.mousePosition)) 
            {
                for (int i = 0; i < 256; i++)
                {
                    if (i == (int)Keys.LButton
                        || i == (int)Keys.Insert) continue;
                    if (i > 6 && Event.current.type != EventType.KeyDown) continue;

                    if ((PAUtils.GetAsyncKeyState(i) & 1) != 0)
                    {
                        Key = (i == (int)Keys.Escape) ? 0 : i;
                        break;
                    }
                }
            }
        }

        public static Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        public static Texture2D MakeGradientTexture(int width, int height, Color startColor, Color endColor, bool isHorizontal = true)
        {
            Color[] pixels = new Color[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float t = isHorizontal ? (float)x / (width - 1) : (float)y / (height - 1);
                    pixels[y * width + x] = Color.Lerp(startColor, endColor, t);
                }
            }

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        private static readonly Dictionary<int, string> keyNames = new Dictionary<int, string> // Ghetto balls
        {
            { 0x01, "Mouse1" },
            { 0x02, "Mouse2" },
            { 0x03, "Control-break processing" },
            { 0x04, "Mouse3" },
            { 0x05, "Mouse4" },
            { 0x06, "Mouse5" },
            { 0x08, "Backspace" },
            { 0x09, "Tab" },
            { 0x0C, "Clear" },
            { 0x0D, "Enter" },
            { 0x10, "Shift" },
            { 0x11, "Ctrl" },
            { 0x12, "Alt" },
            { 0x13, "Pause" },
            { 0x14, "Caps Lock" },
            { 0x1B, "Esc" },
            { 0x20, "Spacebar" },
            { 0x21, "Page Up" },
            { 0x22, "Page Down" },
            { 0x23, "End" },
            { 0x24, "Home" },
            { 0x25, "Left arrow" },
            { 0x26, "Up arrow" },
            { 0x27, "Right arrow" },
            { 0x28, "Down arrow" },
            { 0x2D, "Insert" },
            { 0x2E, "Delete" },
            { 0x30, "0" },
            { 0x31, "1" },
            { 0x32, "2" },
            { 0x33, "3" },
            { 0x34, "4" },
            { 0x35, "5" },
            { 0x36, "6" },
            { 0x37, "7" },
            { 0x38, "8" },
            { 0x39, "9" },
            { 0x41, "A" },
            { 0x42, "B" },
            { 0x43, "C" },
            { 0x44, "D" },
            { 0x45, "E" },
            { 0x46, "F" },
            { 0x47, "G" },
            { 0x48, "H" },
            { 0x49, "I" },
            { 0x4A, "J" },
            { 0x4B, "K" },
            { 0x4C, "L" },
            { 0x4D, "M" },
            { 0x4E, "N" },
            { 0x4F, "O" },
            { 0x50, "P" },
            { 0x51, "Q" },
            { 0x52, "R" },
            { 0x53, "S" },
            { 0x54, "T" },
            { 0x55, "U" },
            { 0x56, "V" },
            { 0x57, "W" },
            { 0x58, "X" },
            { 0x59, "Y" },
            { 0x5A, "Z" },
            { 0x5B, "Left Windows" },
            { 0x5C, "Right Windows" },
            { 0x5D, "Applications" },
            { 0x5F, "Sleep" },
            { 0x60, "Numeric keypad 0" },
            { 0x61, "Numeric keypad 1" },
            { 0x62, "Numeric keypad 2" },
            { 0x63, "Numeric keypad 3" },
            { 0x64, "Numeric keypad 4" },
            { 0x65, "Numeric keypad 5" },
            { 0x66, "Numeric keypad 6" },
            { 0x67, "Numeric keypad 7" },
            { 0x68, "Numeric keypad 8" },
            { 0x69, "Numeric keypad 9" },
            { 0x6A, "Multiply" },
            { 0x6B, "Add" },
            { 0x6C, "Separator" },
            { 0x6D, "Subtract" },
            { 0x6E, "Decimal" },
            { 0x6F, "Divide" },
            { 0x70, "F1" },
            { 0x71, "F2" },
            { 0x72, "F3" },
            { 0x73, "F4" },
            { 0x74, "F5" },
            { 0x75, "F6" },
            { 0x76, "F7" },
            { 0x77, "F8" },
            { 0x78, "F9" },
            { 0x79, "F10" },
            { 0x7A, "F11" },
            { 0x7B, "F12" },
            { 0x7C, "F13" },
            { 0x7D, "F14" },
            { 0x7E, "F15" },
            { 0x7F, "F16" },
            { 0x80, "F17" },
            { 0x81, "F18" },
            { 0x82, "F19" },
            { 0x83, "F20" },
            { 0x84, "F21" },
            { 0x85, "F22" },
            { 0x86, "F23" },
            { 0x87, "F24" },
            { 0x90, "Num Lock" },
            { 0x91, "Scroll Lock" },
            { 0xA0, "Left Shift" },
            { 0xA1, "Right Shift" },
            { 0xA2, "Left Ctrl" },
            { 0xA3, "Right Ctrl" },
            { 0xA4, "Left Alt" },
            { 0xA5, "Right Alt" },
            { 0xBA, "Semicolon" },
            { 0xBB, "Plus" },
            { 0xBC, "Comma" },
            { 0xBD, "Minus" },
            { 0xBE, "Period" },
            { 0xBF, "Forward slash" },
            { 0xC0, "Tilde" },
            { 0xDB, "Left bracket" },
            { 0xDC, "Backslash" },
            { 0xDD, "Right bracket" },
            { 0xDE, "Apostrophe" }
        };
    }

    public static class Render
    {
        private class RingArray
        {
            public Vector2[] Positions { get; private set; }

            public RingArray(int numSegments)
            {
                Positions = new Vector2[numSegments];
                var stepSize = 360f / numSegments;
                for (int i = 0; i < numSegments; i++)
                {
                    var rad = Mathf.Deg2Rad * stepSize * i;
                    Positions[i] = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
                }
            }
        }

        public static Color Color
        {
            get { return GUI.color; }
            set { GUI.color = value; }
        }

        public static void Line(Vector2 from, Vector2 to, float thickness, Color color)
        {
            Color = color;
            Line(from, to, thickness);
        }
        public static void Line(Vector2 from, Vector2 to, float thickness)
        {
            var delta = (to - from).normalized;
            var angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            GUIUtility.RotateAroundPivot(angle, from);
            Box(from, Vector2.right * (from - to).magnitude, thickness, false);
            GUIUtility.RotateAroundPivot(-angle, from);
        }

        public static void Box(Vector2 position, Vector2 size, float thickness, Color color, bool centered = true)
        {
            Color = color;
            Box(position, size, thickness, centered);
        }
        public static void Box(Vector2 position, Vector2 size, float thickness, bool centered = true)
        {
            var upperLeft = centered ? position - size / 2f : position;
            GUI.DrawTexture(new Rect(position.x, position.y, size.x, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(position.x, position.y, thickness, size.y), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(position.x + size.x, position.y, thickness, size.y), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(position.x, position.y + size.y, size.x + thickness, thickness), Texture2D.whiteTexture);
        }

        public static void Cross(Vector2 position, Vector2 size, float thickness, Color color)
        {
            Color = color;
            Cross(position, size, thickness);
        }
        public static void Cross(Vector2 position, Vector2 size, float thickness)
        {
            GUI.DrawTexture(new Rect(position.x - size.x / 2f, position.y, size.x, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(position.x, position.y - size.y / 2f, thickness, size.y), Texture2D.whiteTexture);
        }

        public static void Dot(Vector2 position, Color color)
        {
            Color = color;
            Dot(position);
        }
        public static void Dot(Vector2 position)
        {
            Box(position - Vector2.one, Vector2.one * 2f, 1f);
        }

        public static void String(GUIStyle Style, float X, float Y, float W, float H, string str, Color col, bool centerx = false, bool centery = false)
        {
            GUIContent content = new GUIContent(str);

            Vector2 size = Style.CalcSize(content);
            float fX = centerx ? (X - size.x / 2f) : X,
                fY = centery ? (Y - size.y / 2f) : Y;

            Style.normal.textColor = Color.black;
            GUI.Label(new Rect(fX, fY, size.x, H), str, Style);

            Style.normal.textColor = col;
            GUI.Label(new Rect(fX + 1f, fY + 1f, size.x, H), str, Style);
        }

        public static void Circle(Vector2 center, float radius, float thickness, Color color)
        {
            Color = color;
            Vector2 previousPoint = center + new Vector2(radius, 0);

            for (int i = 1; i <= 360; i++)
            {
                float angle = i * Mathf.Deg2Rad;
                Vector2 nextPoint = center + new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
                Line(previousPoint, nextPoint, thickness);
                previousPoint = nextPoint;
            }
        }

        public static void FilledCircle(Vector2 center, float radius, Color color)
        {
            Color = color;
            float sqrRadius = radius * radius;

            for (float y = -radius; y <= radius; y++)
                for (float x = -radius; x <= radius; x++)
                    if (x * x + y * y <= sqrRadius)
                        Line(center + new Vector2(x, y), center + new Vector2(x + 1, y), 1f);
        }
    }
}