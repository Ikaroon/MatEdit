using UnityEditor;
using UnityEngine;
using System.IO;

/// <summary>
/// A Tool to create an Shader GUI easily
/// </summary>
namespace MB.MatEdit
{
    //---------------------------------------------------------------------------------------\\

    //---------------------------------------------------------------------------------------\\
    //------------------------------------< PUBLIC DATA >------------------------------------\\ 
    //---------------------------------------------------------------------------------------\\

    //---------------------------------------------------------------------------------------\\

    /// <summary>
    /// Defines which component of a Vector should be used
    /// </summary>
    public enum PackagePart
    {
        /// <summary>
        /// The first component
        /// </summary>
        x = 0, r = 0,
        /// <summary>
        /// The second component
        /// </summary>
        y = 1, g = 1,
        /// <summary>
        /// The third component
        /// </summary>
        z = 2, b = 2,
        /// <summary>
        /// The fourth component
        /// </summary>
        w = 3, a = 3
    };

    /// <summary>
    /// Defines which style a group uses
    /// </summary>
    public enum GroupStyle
    {
        /// <summary>
        /// The main group in style of a Button
        /// </summary>
        Main = 0,
        /// <summary>
        /// The second main group in style of group box
        /// </summary>
        MainTwo = 1,
        /// <summary>
        /// The sub group in style of a help box
        /// </summary>
        Sub = 2,
        /// <summary>
        /// The rounded group in style of a flat rounded button
        /// </summary>
        Rounded = 3
    };

    /// <summary>
    /// Defines the size for a texture field
    /// </summary>
    public enum TextureFieldType
    {
        /// <summary>
        /// A small one line field
        /// </summary>
        Small = 16,
        /// <summary>
        /// A small one line field with a thumbnail
        /// </summary>
        Medium = 32,
        /// <summary>
        /// A big texture field with a big thumbnail
        /// </summary>
        Large = 64
    };

    /// <summary>
    /// Defines which mode should be used for a toggle
    /// </summary>
    public enum ToggleMode
    {
        /// <summary>
        /// Modifies an int property
        /// </summary>
        Int,
        /// <summary>
        /// Enables/Disables a shader keyword
        /// </summary>
        Feature,
        /// <summary>
        /// Enables/Disables a shaderpass
        /// Requires custom rendering pipelines!
        /// </summary>
        Pass
    };

    //---------------------------------------------------------------------------------------\\

    //---------------------------------------------------------------------------------------\\
    //-----------------------------------< INTERNAL DATA >-----------------------------------\\ 
    //---------------------------------------------------------------------------------------\\

    //---------------------------------------------------------------------------------------\\

    /// <summary>
    /// The type of a field (used for copying and pasting)
    /// </summary>
    internal enum FieldType
    {
        /// <summary>
        /// An int field
        /// </summary>
        Int,
        /// <summary>
        /// A float field
        /// </summary>
        Float,
        /// <summary>
        /// A vector field
        /// </summary>
        Vector,
        /// <summary>
        /// A float as vector field
        /// </summary>
        FloatVector,
        /// <summary>
        /// A color field
        /// </summary>
        Color,
        /// <summary>
        /// A texture field
        /// </summary>
        Texture,
        /// <summary>
        /// A keyword field (toggle)
        /// </summary>
        Keyword,
        /// <summary>
        /// A pass field (toggle)
        /// </summary>
        Pass,
        /// <summary>
        /// An animation curve field
        /// </summary>
        Curve,
        /// <summary>
        /// A gradient field
        /// </summary>
        Gradient
    }
}