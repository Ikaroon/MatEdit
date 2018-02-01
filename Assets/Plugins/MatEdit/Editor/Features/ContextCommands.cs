using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.IO;
using System.Text;

namespace MB.MatEdit
{
    public class ContextCommands
    {
        [MenuItem("Assets/MatEdit/Create ShaderGUI")]
        static void CreateEditor()
        {
            Object[] selection = Selection.objects;
            List<Shader> shader = new List<Shader>();
            for (int o = 0; o < selection.Length; o++)
            {
                if (selection[0].GetType() == typeof(Shader))
                {
                    shader.Add((Shader)selection[0]);
                }
            }

            for (int s = 0; s < shader.Count; s++)
            {
                CreateShaderEditor(shader[s]);
            }
        }

        [MenuItem("Assets/MatEdit/Create ShaderGUI", validate = true)]
        static bool CreateEditorValidate()
        {
            Object[] selection = Selection.objects;
            for (int o = 0; o < selection.Length; o++)
            {
                if (selection[0].GetType() == typeof(Shader))
                {
                    return true;
                }
            }
            return true;
        }

        private static void CreateShaderEditor(Shader shader)
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;

            string path = Path.GetFullPath(AssetDatabase.GetAssetPath(shader));
            path = Path.Combine(projectPath, path);

            string shaderName = Path.GetFileNameWithoutExtension(path);

            string rootPath = Directory.GetParent(path).FullName;
            string editorRootPath = Path.Combine(rootPath, "Editor");
            string editorPath = Path.Combine(editorRootPath, shaderName + "_GUI.cs");

            Debug.Log(editorPath);

            if (File.Exists(editorPath))
            {
                return;
            }

            if (!Directory.Exists(editorRootPath))
            {
                Directory.CreateDirectory(editorRootPath);
            }
            
            using (FileStream fs = File.Create(editorPath))
            {
                string dataasstring = Templates.Get("ShaderGUI");
                dataasstring = dataasstring.Replace("#CLASS_NAME#", Path.GetFileNameWithoutExtension(editorPath));
                byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                fs.Write(info, 0, info.Length);
            }

            string assetPath = editorPath.Replace(projectPath, "").Replace(@"\", "/");
            assetPath = assetPath.Substring(1, assetPath.Length - 1);
            AssetDatabase.ImportAsset(assetPath);
        }
    }
}