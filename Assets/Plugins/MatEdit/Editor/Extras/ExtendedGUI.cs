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
        public static void DrawLine(Vector2 a, Vector2 b, float contrastValue = 0.9f)
        {
            // Start with skin: free color
            Color lineColor = new Color(1f - contrastValue, 1f - contrastValue, 1f - contrastValue);

            // If pro skin is enabled change Color acordingly
            if (EditorGUIUtility.isProSkin)
            {
                lineColor = new Color(contrastValue, contrastValue, contrastValue);
            }

            // Draw the line
            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = lineColor;
            Handles.DrawLine(a, b);
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        public static Object[] DropAreaGUI(Rect rect, bool multiObject, System.Type validType)
        {
            Event evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!rect.Contains(evt.mousePosition))
                        return null;

                    if ((!multiObject ? DragAndDrop.objectReferences.Length == 1 : true) && AreOfType(validType, DragAndDrop.objectReferences))
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();

                            return DragAndDrop.objectReferences;
                        }
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }

                    break;
            }
            return null;
        }

        private static bool AreOfType(System.Type type, Object[] objects)
        {
            for (int o = 0; o < objects.Length; o++)
            {
                if (objects[o].GetType() == type)
                {
                    return true;
                }
            }
            return false;
        }
    }
}