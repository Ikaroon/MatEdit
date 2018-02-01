// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    /// <summary>
    /// Gives some extra GUI methods for the editor
    /// </summary>
    internal static class ExtendedGUI
    {
        /// <summary>
        /// Draws a line from a to b in skin contrast color
        /// </summary>
        /// <param name="a">Start Point</param>
        /// <param name="b">End Point</param>
        public static void DrawLine(Vector2 a, Vector2 b)
        {
            // Start with skin: free color
            Color lineColor = new Color(0.1f, 0.1f, 0.1f);

            // If pro skin is enabled change Color acordingly
            if (EditorGUIUtility.isProSkin)
            {
                lineColor = new Color(0.9f, 0.9f, 0.9f);
            }

            // Draw the line
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = lineColor;
            Handles.DrawLine(a, b);
            Handles.color = oldColor;
            Handles.EndGUI();
        }
    }
}