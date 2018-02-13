// System
using System.IO;
using System.Text.RegularExpressions;

// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    internal static class Distribute
    {
        private const string DISTRIBUTION_PATH = "Distribute/";
        
        public const string MATEDIT_DISTRIBUTE = "MatEditDistribute";

        public static void ToPath(string shader, string path)
        {
            string distribute = Content.Get(DISTRIBUTION_PATH + MATEDIT_DISTRIBUTE);
            distribute = Directory.GetParent(distribute).FullName;
            distribute = Path.Combine(distribute, MATEDIT_DISTRIBUTE);

            string shaderContent = FileOperator.ReadStringFromFile(shader);
            Match shaderEditor = Regex.Match(shaderContent, "CustomEditor.*?" + '"' + ".*?" + '"');
            shaderEditor = Regex.Match(shaderEditor.Value, '"' + ".*?" + '"');
            string shaderEditorName = shaderEditor.Value.Replace('"' + "", "").Replace(" ", "");
            Debug.Log(shaderEditorName);

            string shaderEditorPath = GetMonoScriptOfType(shaderEditorName);
            Debug.Log(shaderEditorPath);
            if (shaderEditorPath == "")
            {
                return;
            }
            shaderEditorPath = Path.Combine(MatEdit.ProjectPath(), shaderEditorPath);
            
            Debug.Log(shaderEditorPath);

            CopyDirectory(distribute, path);

            string newShaderPath = Path.Combine(path, Path.GetFileName(shader));
            shaderContent = Regex.Replace(shaderContent, "CustomEditor.*?" + '"' + ".*?" + '"', "CustomEditor " + '"' + "MED_" + shaderEditorName + '"');
            FileOperator.WriteStringToFile(shaderContent, newShaderPath);
            string scriptContent = FileOperator.ReadStringFromFile(shaderEditorPath);
            scriptContent = scriptContent.Replace(shaderEditorName, "MED_" + shaderEditorName);
            FileOperator.WriteStringToFile(scriptContent, Path.Combine(Path.Combine(path, "Editor"), "MED_" + Path.GetFileName(shaderEditorPath)));

            AssetDatabase.Refresh();

            Debug.Log(distribute);
        }

        private static string GetMonoScriptOfType(string type)
        {
            string[] assets = AssetDatabase.FindAssets("t:MonoScript");
            for (int s = 0; s < assets.Length; s++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[s]);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script != null && script.GetClass() != null)
                {
                    if (script.GetClass().Name == type)
                    {
                        return AssetDatabase.GetAssetPath(script);
                    }
                }
            }
            return "";
        }

        private static void CopyDirectory(string input, string output)
        {
            foreach (string dirPath in Directory.GetDirectories(input, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(input, output));
            
            foreach (string newPath in Directory.GetFiles(input, "*.*",
                SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(input, output), true);
        }
    }
}