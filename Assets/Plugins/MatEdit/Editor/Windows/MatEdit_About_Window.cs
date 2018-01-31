using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

namespace MB.MatEdit
{
    public class MatEdit_About_Window : EditorWindow
    {
        private string asset;
        private string version;
        private string author;

        [MenuItem("Window/MatEdit/About")]
        static void Init()
        {
            MatEdit_About_Window window = EditorWindow.CreateInstance<MatEdit_About_Window>();
            window.ShowUtility();
        }

        private void OnEnable()
        {
            minSize = new Vector2(320f, 160f);
            maxSize = new Vector2(320f, 160f);
            titleContent = new GUIContent("MatEdit | About");

            GatherVersionInfo();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(new GUIContent(asset + " V" + version));
            EditorGUILayout.LabelField(new GUIContent("By " + author));
        }

        private void GatherVersionInfo()
        {
            string[] checked_VersionFiles = Directory.GetFiles(Application.dataPath, "*" + Basics.VERSION_FILE, SearchOption.AllDirectories);
            if (checked_VersionFiles.Length == 1)
            {
                XmlDocument xmlCurrentVersion = new XmlDocument();
                xmlCurrentVersion.Load(checked_VersionFiles[0]);

                asset = LoadSingleInfo(xmlCurrentVersion, "asset");
                version = LoadSingleInfo(xmlCurrentVersion, "version");
                author = LoadSingleInfo(xmlCurrentVersion, "author");
            }
        }

        private string LoadSingleInfo(XmlDocument document, string tag)
        {
            XmlNodeList nodes = document.GetElementsByTagName(tag);

            foreach (XmlNode xn in nodes)
            {
                return xn.InnerText;
            }
            return "";
        }
    }

}