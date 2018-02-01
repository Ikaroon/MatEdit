using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

namespace MB.MatEdit
{
    public class AboutWindow : EditorWindow
    {
        private string asset;
        private string description;
        private string version;
        private string author;
        private string year;
        private string url;
        private string instruction;
        private Texture2D logo;

        private enum InstallType { Install, Uninstall, Upgrade, Downgrade };
        private InstallType installType;

        [MenuItem("Window/MatEdit/About")]
        static void Init()
        {
            AboutWindow window = EditorWindow.CreateInstance<AboutWindow>();
            window.ShowUtility();
        }

        private void OnEnable()
        {
            minSize = new Vector2(308f, 400f);
            maxSize = new Vector2(308f, 400f);
            titleContent = new GUIContent("MatEdit | About");

            GatherVersionInfo();
        }

        #region Style Data

        private static GUIStyle versionLabel;
        private static GUIStyle authorLabel;

        private static GUIStyle contentLabel;

        private static GUIStyle uninstallButton;
        private static GUIStyle documentationButton;
        private static GUIStyle websiteButton;

        #endregion

        #region Style Methods

        private static void InitStyles()
        {
            if (versionLabel != null)
            {
                return;
            }

            versionLabel = new GUIStyle(EditorStyles.label);
            versionLabel.alignment = TextAnchor.MiddleRight;

            authorLabel = new GUIStyle(EditorStyles.label);
            authorLabel.alignment = TextAnchor.MiddleCenter;

            contentLabel = new GUIStyle(EditorStyles.miniLabel);
            contentLabel.wordWrap = true;

            uninstallButton = new GUIStyle(EditorStyles.miniButtonLeft);
            uninstallButton.fixedHeight = 16f;
            uninstallButton.fixedWidth = 80f;

            documentationButton = new GUIStyle(EditorStyles.miniButtonMid);
            documentationButton.fixedHeight = 16f;
            documentationButton.fixedWidth = 140f;

            websiteButton = new GUIStyle(EditorStyles.miniButtonRight);
            websiteButton.fixedHeight = 16f;
            websiteButton.fixedWidth = 80f;
        }

        [InitializeOnLoadMethod()]
        private static void ResetStyles()
        {
            versionLabel = null;
            authorLabel = null;

            contentLabel = null;

            uninstallButton = null;
            documentationButton = null;
            websiteButton = null;
        }

        #endregion

        private void OnGUI()
        {

            InitStyles();

            Vector2 winCenter = new Vector2(position.width / 2f, position.height / 2f);


            Vector2 imgSize = new Vector2(logo.width, logo.height);

            GUI.DrawTexture(new Rect(winCenter.x - imgSize.x / 2f, 16f, imgSize.x, imgSize.y), logo, ScaleMode.StretchToFill, true);
            EditorGUI.LabelField(new Rect(winCenter.x - imgSize.x / 2f, 8f + imgSize.y, imgSize.x, 16f), new GUIContent(asset + " Version " + version), versionLabel);

            DrawLine(new Vector2(4f, imgSize.y + 40f), new Vector2(position.width - 4f, imgSize.y + 40f));

            GUILayout.BeginArea(new Rect(16f, imgSize.y + 48f, position.width - 32f, position.height - 56f));

            EditorGUILayout.LabelField(new GUIContent(description), contentLabel);
            EditorGUILayout.LabelField(new GUIContent(instruction), contentLabel);

            GUILayout.EndArea();

            DrawLine(new Vector2(4f, position.height - 48f), new Vector2(position.width - 4f, position.height - 48f));

            EditorGUI.LabelField(new Rect(4f, position.height - 40f, position.width - 8f, 16f), new GUIContent('\u00A9' + " " + year + " " + author), authorLabel);
            
            GUILayout.BeginArea(new Rect(4f, position.height - 20f, (position.width - 8f), 17f));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent(installType.ToString() + " CG"), uninstallButton))
            {
                PerformInstall();
            }
            if (GUILayout.Button(new GUIContent("Documentation"), documentationButton))
            {
                Application.OpenURL(url);
            }
            if (GUILayout.Button(new GUIContent("Website"), websiteButton))
            {
                Application.OpenURL(url);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }

        private static void DrawLine(Vector2 a, Vector2 b)
        {
            Color lineColor = new Color(0.1f, 0.1f, 0.1f);

            if (EditorGUIUtility.isProSkin)
            {
                lineColor = new Color(0.9f, 0.9f, 0.9f);
            }

            Handles.BeginGUI();
            Color oldColor = Handles.color;
            Handles.color = lineColor;
            Handles.DrawLine(a, b);
            Handles.color = oldColor;
            Handles.EndGUI();
        }

        private void GatherVersionInfo()
        {
            string versionFile = Content.Get(Content.VERSION_FILE);

            XmlDocument xmlCurrentVersion = new XmlDocument();
            xmlCurrentVersion.Load(versionFile);

            asset = LoadSingleInfo(xmlCurrentVersion, "asset");
            description = LoadSingleInfo(xmlCurrentVersion, "description");
            version = LoadSingleInfo(xmlCurrentVersion, "version");
            author = LoadSingleInfo(xmlCurrentVersion, "author");
            year = LoadSingleInfo(xmlCurrentVersion, "year");
            url = LoadSingleInfo(xmlCurrentVersion, "url");
            instruction = LoadSingleInfo(xmlCurrentVersion, "instruction");

            string img = LoadSingleInfo(xmlCurrentVersion, "logo");
            img = Path.Combine(Directory.GetParent(versionFile).FullName, img);
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>(img.Replace(Path.GetFullPath(Application.dataPath), "Assets"));

            CheckInstallMode(version);
        }

        private void CheckInstallMode(string version)
        {
            string installedVersion = Installer.GetInstalledVersionNumber();
            float installedVersionNumber = float.Parse(installedVersion);
            float currentVersionNumber = float.Parse(version);

            if (installedVersionNumber == -1f)
            {
                installType = InstallType.Install;
            }
            else if (installedVersionNumber < currentVersionNumber)
            {
                installType = InstallType.Upgrade;
            }
            else if (installedVersionNumber > currentVersionNumber)
            {
                installType = InstallType.Downgrade;
            }
            else
            {
                installType = InstallType.Uninstall;
            }
        }

        private void PerformInstall()
        {
            switch (installType)
            {
                case InstallType.Install:
                    Installer.AskInstall();
                    break;
                case InstallType.Upgrade:
                    Installer.Uninstall();
                    Installer.Install();
                    break;
                case InstallType.Uninstall:
                    Installer.AskUninstall();
                    break;
                case InstallType.Downgrade:
                    Installer.Uninstall();
                    Installer.Install();
                    break;
            }

            CheckInstallMode(version);
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