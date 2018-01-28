namespace MB.MatEdit
{
    public enum PackagePart { x = 0, y = 1, z = 2, w = 3, r = 0, g = 1, b = 2, a = 3 };
    public enum GroupStyle { Main = 0, MainTwo = 1, Sub = 2, Rounded = 3 };
    public enum TextureFieldType { Small = 16, Medium = 32, Large = 64 };

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
}