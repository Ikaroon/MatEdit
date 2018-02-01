// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    /// <summary>
    /// The class which saves the data for the material which cannot be saved in the material
    /// </summary>
    internal sealed class MaterialData : ScriptableObject
    {
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< CONTENT DATA >------------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Content

        /// <summary>
        /// The toggles which are saved outside of the material (fold groups)
        /// </summary>
        [SerializeField]
        public ToggleDictionary toggles = new ToggleDictionary();

        /// <summary>
        /// The curve properties which are saved outside of the material (curve fields)
        /// </summary>
        [SerializeField]
        public CurveDictionary curves = new CurveDictionary();

        /// <summary>
        /// The gradient properties which are saved outside of the material (gradient fields)
        /// </summary>
        [SerializeField]
        public GradientDictionary gradients = new GradientDictionary();

        /// <summary>
        /// The generated textures which are saved outside of the material (curve + gradient fields)
        /// </summary>
        [SerializeField]
        public Texture2DDictionary unsavedTextures = new Texture2DDictionary();

        /// <summary>
        /// The generated textures which are saved outside of the material (curve + gradient fields)
        /// </summary>
        [SerializeField]
        public Texture2DDictionary savedTextures = new Texture2DDictionary();

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< DATA METHODS >------------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        
        #region Getter

        /// <summary>
        /// Gets the MaterialData of the specified Material
        /// </summary>
        /// <param name="material">The material which should expose its MaterialData</param>
        /// <returns>The MaterialData of material</returns>
        public static MaterialData Of(Material material)
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
            MaterialData data = null;
            int o = 0;
            while (o < obj.Length && data == null)
            {
                if (obj[o].GetType() == typeof(MaterialData))
                {
                    data = (MaterialData)obj[o];
                }
                o++;
            }

            // If the data is not created -> create one and place it in the Asset Database as subasset of the material
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<MaterialData>();
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
        public static bool GetToggleOf(Material material, string property)
        {
            // Get the MatGUI_DATA
            MaterialData data = MaterialData.Of(material);
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
        public static void SetToggleOf(Material material, string property, bool value)
        {
            // Get the MatGUI_DATA
            MaterialData data = MaterialData.Of(material);
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