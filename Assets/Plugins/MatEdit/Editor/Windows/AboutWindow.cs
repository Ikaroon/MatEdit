// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    /// <summary>
    /// The About Window for MatEdit
    /// </summary>
    public class AboutWindow : EditorWindow
    {

        #region Content

        /// <summary>
        /// The Version info which is created with this window
        /// </summary>
        private VersionInfo info;

        #endregion

        //-----------------------------------------------------------------------------------------

        #region Style Data

        // Major labels
        private static GUIStyle versionLabel;
        private static GUIStyle authorLabel;

        // Minor label
        private static GUIStyle contentLabel;

        // Tool buttons
        private static GUIStyle uninstallButton;
        private static GUIStyle documentationButton;
        private static GUIStyle websiteButton;

        #endregion

        #region Style Methods

        /// <summary>
        /// Initializes the styles if they are not set
        /// </summary>
        private static void InitStyles()
        {
            // If one style is already set then abord
            if (versionLabel != null)
            {
                return;
            }

            // Major labels
            versionLabel = new GUIStyle(EditorStyles.label);
            versionLabel.alignment = TextAnchor.MiddleRight;

            authorLabel = new GUIStyle(EditorStyles.label);
            authorLabel.alignment = TextAnchor.MiddleCenter;

            // Minor label
            contentLabel = new GUIStyle(EditorStyles.miniLabel);
            contentLabel.wordWrap = true;

            // Tool buttons
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

        //-----------------------------------------------------------------------------------------

        #region Window Methods
        
        /// <summary>
        /// Initializes the window - user interaction
        /// </summary>
        [MenuItem("Window/MatEdit/About")]
        static void Init()
        {
            AboutWindow window = EditorWindow.CreateInstance<AboutWindow>();
            window.ShowUtility();
        }

        /// <summary>
        /// Sets some major window values
        /// </summary>
        private void OnEnable()
        {
            minSize = new Vector2(308f, 420f);
            maxSize = new Vector2(308f, 420f);
            titleContent = new GUIContent("MatEdit | About");

            info = new VersionInfo();
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-------------------------------------< ABOUT GUI >-------------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Window GUI
        
        /// <summary>
        /// Draws the About GUI
        /// </summary>
        private void OnGUI()
        {
            //TODO: Better layout

            InitStyles();

            Vector2 winCenter = new Vector2(position.width / 2f, position.height / 2f);
            Vector2 imgSize = info.pluginLogoSize;

            GUI.DrawTexture(new Rect(winCenter.x - imgSize.x / 2f, 16f, imgSize.x, imgSize.y), info.pluginLogo, ScaleMode.StretchToFill, true);
            EditorGUI.LabelField(new Rect(winCenter.x - imgSize.x / 2f, 8f + imgSize.y, imgSize.x, 16f), new GUIContent(info.fullVersion), versionLabel);

            ExtendedGUI.DrawLine(new Vector2(4f, imgSize.y + 40f), new Vector2(position.width - 4f, imgSize.y + 40f));

            GUILayout.BeginArea(new Rect(16f, imgSize.y + 48f, position.width - 32f, position.height - 56f));

            EditorGUILayout.LabelField(new GUIContent(info.extraContent), contentLabel);

            GUILayout.EndArea();

            ExtendedGUI.DrawLine(new Vector2(4f, position.height - 48f), new Vector2(position.width - 4f, position.height - 48f));

            EditorGUI.LabelField(new Rect(4f, position.height - 40f, position.width - 8f, 16f), new GUIContent(info.copyright), authorLabel);
            
            GUILayout.BeginArea(new Rect(4f, position.height - 20f, (position.width - 8f), 17f));
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent(info.installType.ToString() + " CG"), uninstallButton))
            {
                Installer.PerformInstall(info.installType);
                info.CheckInstallMode();
            }
            if (GUILayout.Button(new GUIContent("Documentation"), documentationButton))
            {
                Application.OpenURL(info.docURL);
            }
            if (GUILayout.Button(new GUIContent("Website"), websiteButton))
            {
                Application.OpenURL(info.websiteURL);
            }

            GUILayout.EndHorizontal();
            GUILayout.EndArea();

        }

        #endregion

    }

}