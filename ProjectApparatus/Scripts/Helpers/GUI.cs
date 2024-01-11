using UnityEngine;

namespace Hax
{
    public static partial class Helper
    {
        public static void DrawLabel(Vector2 position, string label, Color colour)
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;

            Vector2 size = labelStyle.CalcSize(new GUIContent(label));
            Vector2 newPosition = position - (size * 0.5f);

            labelStyle.normal.textColor = Color.black;
            GUI.Label(new Rect(newPosition.x, newPosition.y, size.x, size.y), label, labelStyle);

            labelStyle.normal.textColor = colour;
            GUI.Label(new Rect(newPosition.x + 1, newPosition.y + 1, size.x, size.y), label, labelStyle);
        }

        public static void DrawLabel(Vector2 position, string label)
        {
            DrawLabel(position, label, Color.white);
        }

        public static void DrawOutlineBox(Vector2 centrePosition, Size size, float lineWidth, Color colour)
        {
            float halfWidth = 0.5f * size.Width;
            float halfHeight = 0.5f * size.Height;

            float leftX = centrePosition.x - halfWidth;
            float rightX = centrePosition.x + halfWidth;
            float topY = centrePosition.y - halfHeight;
            float bottomY = centrePosition.y + halfHeight;

            Vector2 topLeft = new Vector2(leftX, topY);
            DrawBox(topLeft, new Size(size.Width, lineWidth), colour);

            Vector2 rightBorderTopLeft = new Vector2(rightX - lineWidth, topY);
            DrawBox(rightBorderTopLeft, new Size(lineWidth, size.Height), colour);

            Vector2 bottomBorderTopLeft = new Vector2(leftX, bottomY - lineWidth);
            DrawBox(bottomBorderTopLeft, new Size(size.Width, lineWidth), colour);

            DrawBox(topLeft, new Size(lineWidth, size.Height), colour);
        }

        public static void DrawOutlineBox(Vector2 centrePosition, Size size, float lineWidth)
        {
            DrawOutlineBox(centrePosition, size, lineWidth, Color.white);
        }

        public static void DrawBox(Vector2 position, Size size, Color colour)
        {
            Color previousColour = GUI.color;
            GUI.color = colour;

            Rect rect = new Rect(position.x, position.y, size.Width, size.Height);
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.StretchToFill);

            GUI.color = previousColour;
        }

        public static void DrawBox(Vector2 position, Size size)
        {
            DrawBox(position, size, Color.white);
        }
    }
}

namespace Hax
{
    public class Size
    {
        public float Width { get; private set; }
        public float Height { get; private set; }

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }
    }
}
