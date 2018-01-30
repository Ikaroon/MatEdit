using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    internal static class MatGUI_DATA_Editor
    {
        #region Helper Methods

        private static MatGUI_DATA GetMatData(Material material)
        {
            // Get the path of the 
            string path = AssetDatabase.GetAssetPath(material);
            if (path == "")
            {
                // Stops if the material is currently not in the Asset Database
                return null;
            }

            // Loads all assets at this location and checks if it is the MatGUI_DATA
            Object[] obj = AssetDatabase.LoadAllAssetsAtPath(path);
            MatGUI_DATA data = null;
            int o = 0;
            while (o < obj.Length && data == null)
            {
                if (obj[o].GetType() == typeof(MatGUI_DATA))
                {
                    data = (MatGUI_DATA)obj[o];
                }
                o++;
            }

            // If the data is not created -> create one and place it in the Asset Database as subasset of the material
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MatGUI_DATA>();
                data.hideFlags = HideFlags.HideInHierarchy;
                data.name = "MatGUI_DATA";
                AssetDatabase.AddObjectToAsset(data, material);
                AssetDatabase.ImportAsset(path);
            }

            return data;
        }

        #endregion

        #region Toggle Methods

        /// <summary>
        /// Returns the toggle value of a specific key in the material data
        /// </summary>
        /// <param name="property">The key used for this toggle</param>
        /// <param name="material">The material which stores the data</param>
        /// <returns>The toggle value concerning this key</returns>
        public static bool GetMaterialSubToggle(string property, Material material)
        {
            // Get the MatGUI_DATA
            MatGUI_DATA data = GetMatData(material);
            if (data == null)
            {
                return false;
            }

            // Check if the key is given in the dictionary
            if (data.toggles.ContainsKey(property))
            {
                // If so return the value
                return data.toggles[property];
            }
            else
            {
                // Otherwise create a new entry in the dictionary with the defaut value true
                data.toggles.Add(property, true);
                EditorUtility.SetDirty(data);
                return data.toggles[property];
            }
        }

        /// <summary>
        /// Sets a toggle value of a specific key in the material data
        /// </summary>
        /// <param name="property">The key used for this toggle</param>
        /// <param name="value">The value which should be writen under the given key</param>
        /// <param name="material">The material which stores the data</param>
        public static void SetMaterialSubToggle(string property, bool value, Material material)
        {
            // Get the MatGUI_DATA
            MatGUI_DATA data = GetMatData(material);
            if (data == null)
            {
                return;
            }

            // Check if the key is given in the dictionary
            if (data.toggles.ContainsKey(property))
            {
                // If so set the value
                data.toggles[property] = value;
                EditorUtility.SetDirty(data);
            }
            else
            {
                // Otherwise create a new entry in the dictionary with the given value
                data.toggles.Add(property, value);
                EditorUtility.SetDirty(data);
            }
        }

        #endregion
    }
}