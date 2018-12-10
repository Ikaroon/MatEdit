// System
using System.IO;

// Unity
using UnityEngine;

namespace MB.MatEditDistribute
{
    /// <summary>
    /// The General Operator of the MatEdit Plugin
    /// </summary>
    internal static class MatEdit
    {

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-------------------------------------< CONST DATA >------------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region CONST DATA

        /// <summary>
        /// The root of the MatEdit resources in the Resource folder
        /// </summary>
        public const string ROOT_PATH = "MatEdit";

        /// <summary>
        /// The name of the class in MatEdit which is responsible for drawing the GUI elements
        /// </summary>
        public const string GUI_DRAWER = "MatGUI";

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< HELPER METHODS >----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        
        #region Helper Methods

        /// <summary>
        /// The Project path - Not the Asset path
        /// </summary>
        /// <returns>The project path</returns>
        internal static string ProjectPath()
        {
            return Directory.GetParent(Application.dataPath).FullName;
        }

        /// <summary>
        /// Converts a system absolute path to the Asset folder as root
        /// </summary>
        /// <param name="path">The absolute path to convert</param>
        /// <returns>The path with start in the Asset folder</returns>
        internal static string AbsoluteToAssetPath(string path)
        {
            path = Directory.GetParent(path).FullName;
            path = path.Replace(MatEdit.ProjectPath(), "");
            path = path.Replace(@"\", "/");
            path = path.Substring(1, path.Length - 1);
            return path;
        }

        #endregion

    }
}