// System
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    /// <summary>
    /// A Class which defines several context methods for MatEdit
    /// </summary>
    internal static class ContextCommands
    {

        #region Simple Editor Context
        
        /// <summary>
        /// Starts the Creation of a custom ShaderGUI for each Shader selected
        /// </summary>
        [MenuItem("Assets/Create/MatEdit/Create ShaderGUI")]
        private static void CreateEditor()
        {
            // Filter Objects for Shaders
            Object[] selection = Selection.objects;
            List<Shader> shader = new List<Shader>();
            for (int o = 0; o < selection.Length; o++)
            {
                if (selection[0].GetType() == typeof(Shader))
                {
                    shader.Add((Shader)selection[0]);
                }
            }

            // Create Shader Editor
            for (int s = 0; s < shader.Count; s++)
            {
                CreateShaderEditor(shader[s]);
            }
        }

        /// <summary>
        /// Checks if at least one selected Object is a Shader
        /// </summary>
        /// <returns>True if atleast one Shader is selected</returns>
        [MenuItem("Assets/Create/MatEdit/Create ShaderGUI", validate = true)]
        private static bool CreateEditorValidate()
        {
            // Check if Shader is Selected
            Object[] selection = Selection.objects;
            for (int o = 0; o < selection.Length; o++)
            {
                if (selection[0].GetType() == typeof(Shader))
                {
                    return true;
                }
            }

            // No Shader is selected
            return false;
        }

        #endregion

        #region Simple Editor Creation

        /// <summary>
        /// Generates a new ShaderGUI for a specified Shader
        /// </summary>
        /// <param name="shader">The Shader which should get a new ShaderGUI</param>
        private static void CreateShaderEditor(Shader shader)
        {
            // Generate the absolute Path for the Shader
            string path = Path.GetFullPath(AssetDatabase.GetAssetPath(shader));
            path = Path.Combine(MatEdit.ProjectPath(), path);

            // Get the filename of the shader -> shader.name only returns the internal shader name
            string shaderName = Path.GetFileNameWithoutExtension(path);

            // Get the folder which locates the Shader
            string rootPath = Directory.GetParent(path).FullName;

            // Generate the path used for the Editor script
            string editorRootPath = Path.Combine(rootPath, "Editor");
            string editorPath = Path.Combine(editorRootPath, shaderName + "_GUI.cs");

            // Abandon task if a ShaderGUI is already located
            if (File.Exists(editorPath))
            {
                return;
            }

            // Abandon if a ShaderGUI is already paired but the user wants to override it
            string shaderContent = FileOperator.ReadStringFromFile(path);
            if (Regex.IsMatch(shaderContent, "CustomEditor.*?" + '"' + ".*?" + '"'))
            {
                bool overwrite = EditorUtility.DisplayDialog("ShaderGUI Creation", "The shader:\n"
                    + shaderName + "\n" +
                    "allready contains a definition for a ShaderGUI.\n" +
                    "Do you want to overwrite it?", "Yes", "No");

                if (!overwrite)
                {
                    return;
                }
                shaderContent = Regex.Replace(shaderContent, "CustomEditor.*?" + '"' + ".*?" + '"', "CustomEditor " + '"' + shaderName + "_GUI" + '"');
            }
            else
            {
                // Make the last } the last for good
                int lastIndex = shaderContent.LastIndexOf("}");
                shaderContent = shaderContent.Substring(0, lastIndex + 1);

                shaderContent = Regex.Replace(shaderContent, "}$", "    CustomEditor " + '"' + shaderName + "_GUI" + '"' + "\n" +
                    "}");
            }

            // Write new Shader content to file
            FileOperator.WriteStringToFile(shaderContent, path);

            // Folder

            // If the Editor folder is not created yet - create one
            if (!Directory.Exists(editorRootPath))
            {
                Directory.CreateDirectory(editorRootPath);
            }

            // Template

            // Get the Template for a SimpleShaderGUI
            string template = Templates.Get(Templates.SIMPLE_SHADER_GUI);

            // Replace the placeholder with the actual class name
            template = template.Replace(Templates.SHADER_GUI_CLASS_NAME, Path.GetFileNameWithoutExtension(editorPath));

            // Write the template to the script file
            FileOperator.WriteStringToFile(template, editorPath);

            // Reimport

            // Generate the Asset folder located path and reimport the ShaderGUI script
            string assetPath = MatEdit.AbsoluteToAssetPath(editorPath) + "/" + Path.GetFileName(editorPath);
            AssetDatabase.ImportAsset(assetPath);
        }

        #endregion

        #region Distribute Context

        /// <summary>
        /// Starts the Creation of a custom ShaderGUI for each Shader selected
        /// </summary>
        [MenuItem("Assets/Create/MatEdit/Create Distribution")]
        private static void DistributeShader()
        {
            // Filter Objects for Shaders
            Object[] selection = Selection.objects;
            List<Shader> shader = new List<Shader>();
            for (int o = 0; o < selection.Length; o++)
            {
                if (selection[0].GetType() == typeof(Shader))
                {
                    shader.Add((Shader)selection[0]);
                }
            }

            // Create Shader Editor
            for (int s = 0; s < shader.Count; s++)
            {
                string fileName = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(shader[s]));

                string shaderPath = Path.Combine(MatEdit.ProjectPath(), AssetDatabase.GetAssetPath(shader[s]));
                string path = EditorUtility.SaveFolderPanel("Distribute " + shader[s].name + " To", Application.dataPath, fileName + "Distribution");
                Distribute.ToPath(shaderPath, path);
            }
        }

        /// <summary>
        /// Checks if at least one selected Object is a Shader
        /// </summary>
        /// <returns>True if atleast one Shader is selected</returns>
        [MenuItem("Assets/Create/MatEdit/Create Distribution", validate = true)]
        private static bool DistributeShaderValidate()
        {
            // Check if Shader is Selected
            Object[] selection = Selection.objects;
            for (int o = 0; o < selection.Length; o++)
            {
                if (selection[0].GetType() == typeof(Shader))
                {
                    return true;
                }
            }

            // No Shader is selected
            return false;
        }

        #endregion
    }
}