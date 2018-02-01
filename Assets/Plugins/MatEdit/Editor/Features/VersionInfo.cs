// System
using System.IO;
using System.Xml;

// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    /// <summary>
    /// A class which collects all the Version data and stores it
    /// </summary>
    internal class VersionInfo
    {

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //------------------------------------< VERSION DATA >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        
        #region Content

        private Texture2D logo;

        private string asset;
        private string version;

        private string description;
        private string instruction;

        private string year;
        private string author;

        private string url;
        private string doc;

        public InstallType installType;

        #endregion

        #region Getter

        public Texture2D pluginLogo
        {
            get
            {
                return logo;
            }
        }

        public Vector2 pluginLogoSize
        {
            get
            {
                return new Vector2(logo.width, logo.height);
            }
        }

        public string docURL
        {
            get
            {
                return doc;
            }
        }

        public string websiteURL
        {
            get
            {
                return url;
            }
        }

        public string extraContent
        {
            get
            {
                return description + "\n\n" + instruction;
            }
        }

        public float versionNumber
        {
            get
            {
                return float.Parse(version);
            }
        }

        public string fullVersion
        {
            get
            {
                return asset + " Version " + version;
            }
        }

        public string copyright
        {
            get
            {
                return '\u00A9' + " " + year + " " + author;
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< VERSION SYSTEM >----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Main

        /// <summary>
        /// Creates a new Version Info pack which searches for the data
        /// </summary>
        public VersionInfo()
        {
            GatherVersionInfo();
        }

        #endregion

        #region Version Info Methods

        /// <summary>
        /// Gathers the data from the version xml
        /// </summary>
        public void GatherVersionInfo()
        {
            // Gets the version file
            string versionFile = Content.Get(Content.VERSION_FILE);

            // Opens the version file
            XmlDocument xmlCurrentVersion = new XmlDocument();
            xmlCurrentVersion.Load(versionFile);

            // Read the content from it
            asset = LoadSingleInfo(xmlCurrentVersion, "asset");
            description = LoadSingleInfo(xmlCurrentVersion, "description");
            version = LoadSingleInfo(xmlCurrentVersion, "version");
            author = LoadSingleInfo(xmlCurrentVersion, "author");
            year = LoadSingleInfo(xmlCurrentVersion, "year");
            url = LoadSingleInfo(xmlCurrentVersion, "url");
            doc = LoadSingleInfo(xmlCurrentVersion, "doc");
            instruction = LoadSingleInfo(xmlCurrentVersion, "instruction");

            // Find the image which is linked
            string img = LoadSingleInfo(xmlCurrentVersion, "logo");
            img = Path.Combine(Directory.GetParent(versionFile).FullName, img);
            logo = AssetDatabase.LoadAssetAtPath<Texture2D>(img.Replace(Path.GetFullPath(Application.dataPath), "Assets"));
            
            CheckInstallMode();
        }

        /// <summary>
        /// Checks if the stored install mode is still valid
        /// </summary>
        public void CheckInstallMode()
        {
            // Collect the version information
            string installedVersion = Installer.GetInstalledVersionNumber();
            float installedVersionNumber = float.Parse(installedVersion);
            float currentVersionNumber = versionNumber;

            // Process the version information
            if (installedVersionNumber == -1f)
            {
                // No version is installed
                installType = InstallType.Install;
            }
            else if (installedVersionNumber < currentVersionNumber)
            {
                // The installed version is older
                installType = InstallType.Upgrade;
            }
            else if (installedVersionNumber > currentVersionNumber)
            {
                // The installed version is newer
                installType = InstallType.Downgrade;
            }
            else
            {
                // The installed version is up-to-date
                installType = InstallType.Uninstall;
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< HELPER METHODS >----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Helper Methods

        /// <summary>
        /// Loads a single tag content from a XmlDocument
        /// </summary>
        /// <param name="document">The document which should be read</param>
        /// <param name="tag">The tag which should be searched for</param>
        /// <returns>The content of the Xml tag</returns>
        private string LoadSingleInfo(XmlDocument document, string tag)
        {
            XmlNodeList nodes = document.GetElementsByTagName(tag);

            foreach (XmlNode xn in nodes)
            {
                return xn.InnerText;
            }
            return "";
        }

        #endregion

    }
}