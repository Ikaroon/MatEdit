using UnityEngine;

namespace MB.MatEdit
{
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