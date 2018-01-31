using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MB.MatEdit
{
    /// <summary>
    /// The class which saves the data for the material which cannot be saved in the material
    /// </summary>
    internal sealed class MatGUI_DATA : ScriptableObject
    {
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

    }

    /// <summary>
    /// A Dictionary to save a string and a bool in cooperation
    /// </summary>
    [System.Serializable]
    internal sealed class ToggleDictionary : SerializableDictionary<string, bool>
    {
    }
    
    /// <summary>
    /// A Dictionary to save a string and an AnimationCurve in cooperation
    /// </summary>
    [System.Serializable]
    internal sealed class CurveDictionary : SerializableDictionary<string, AnimationCurve>
    {
    }

    /// <summary>
    /// A Dictionary to save a string and an Gradient in cooperation
    /// </summary>
    [System.Serializable]
    internal sealed class GradientDictionary : SerializableDictionary<string, Gradient>
    {
    }

    /// <summary>
    /// A Dictionary to save a string and an Gradient in cooperation
    /// </summary>
    [System.Serializable]
    internal sealed class Texture2DDictionary : SerializableDictionary<string, Texture2D>
    {
    }
}