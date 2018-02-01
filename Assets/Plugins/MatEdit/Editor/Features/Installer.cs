// System
using System.IO;
using System.Xml;

// Unity
using UnityEngine;
using UnityEditor;

namespace MB.MatEdit
{
    /// <summary>
    /// A class which enables the MatEdit system to install Files in the Project folder
    /// </summary>
    internal static class Installer
    {

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //------------------------------------< STARTUP DATA >-----------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region StartUp Methods
        
        /// <summary>
        /// This method is called when the plugin is imported
        /// </summary>
        [InitializeOnLoadMethod()]
        private static void OnPluginInstallation()
        {
            // Check if the start up question is allready done
            if (!MatEditData.Data.welcomeMessage)
            {
                AskUserForInstall();

                // Disable the start up question for future updates
                MatEditData.Data.welcomeMessage = true;
                EditorUtility.SetDirty(MatEditData.Data);
            }
        }

        #endregion
        
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //-----------------------------------< INSTALL METHODS >---------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Installation Methods
        
        /// <summary>
        /// This method asks the user before it will install the extra features
        /// </summary>
        public static void AskUserForInstall()
        {
            // Ask the user if they want to install it
            bool install = EditorUtility.DisplayDialog("MatEdit | Installation",
                "You're about to install MatEdit's optional CG support.\n" +
                "This will enable you to use #include " + '"' + "MatEditCG.cginc" + '"' + " everywhere in the Project.\n" +
                "\n" +
                "Do you want to proceed?", "Yes", "No");

            // If no then skip installation
            if (!install)
            {
                return;
            }

            Install();
        }

        /// <summary>
        /// This method installs all neacessary data for MatEdit CG support
        /// </summary>
        public static void Install()
        {
            // Generate all paths

            string installPath = Path.Combine(MatEdit.ProjectPath(), MatEdit.ROOT_PATH);

            string versionFile = Path.Combine(installPath, Content.VERSION_FILE);

            string cgFile = Path.Combine(MatEdit.ProjectPath(), Content.MATGUI_CG);

            string cgCurveFile = Path.Combine(installPath, Content.MATGUI_CG_CURVE);
            string cgGradientFile = Path.Combine(installPath, Content.MATGUI_CG_GRADIENT);

            // Check if the install location already exists - if not create it
            if (!Directory.Exists(installPath))
            {
                Directory.CreateDirectory(installPath);
            }

            // Install the seperate files
            InstallFile(versionFile, Content.VERSION_FILE);
            InstallFile(cgFile, Content.CG_PATH + Content.MATGUI_CG);
            InstallFile(cgCurveFile, Content.CG_PATH + Content.MATGUI_CG_CURVE);
            InstallFile(cgGradientFile, Content.CG_PATH + Content.MATGUI_CG_GRADIENT);
        }

        /// <summary>
        /// Copies the of the content set with the Name name into the path
        /// </summary>
        /// <param name="path">The path where the file should be copied to</param>
        /// <param name="name">The name of the file in the content set</param>
        private static void InstallFile(string path, string name)
        {
            string file = Content.Get(name);
            File.Copy(file, path, true);
        }

        #endregion

        #region Uninstallation Methods

        /// <summary>
        /// This method asks the user before it will uninstall the extra features
        /// </summary>
        public static void AskUserForUninstall()
        {
            // Ask the user if they want to uninstall it
            bool uninstall = EditorUtility.DisplayDialog("MatEdit | Uninstallation",
                "You're about to uninstall MatEdit's optional CG support.\n" +
                "You wont delete the Files which are located in your Asset folder!\n" +
                "\n" +
                "Do you want to proceed?", "Yes", "No");

            // If no then skip uninstallation
            if (!uninstall)
            {
                return;
            }

            Uninstall();
        }

        /// <summary>
        /// This method uninstalls all neacessary data for MatEdit CG support
        /// </summary>
        public static void Uninstall()
        {
            // Generate all paths

            string installPath = Path.Combine(MatEdit.ProjectPath(), MatEdit.ROOT_PATH);

            string cgFile = Path.Combine(MatEdit.ProjectPath(), Content.MATGUI_CG);

            // Check if the install location already exists - if so delete it
            if (Directory.Exists(installPath))
            {
                Directory.Delete(installPath, true);
            }

            // Uninstall remaining files
            if (File.Exists(cgFile))
            {
                File.Delete(cgFile);
            }
        }

        #endregion

        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\
        //------------------------------------< HELPER METHODS >---------------------------------\\ 
        //---------------------------------------------------------------------------------------\\

        //---------------------------------------------------------------------------------------\\

        #region Helper Methods
        
        /// <summary>
        /// Checks for the installed Version number
        /// </summary>
        /// <returns>The Version number as string</returns>
        public static string GetInstalledVersionNumber()
        {
            // Generate all paths

            string installPath = Path.Combine(MatEdit.ProjectPath(), MatEdit.ROOT_PATH);

            string versionFile = Path.Combine(installPath, Content.VERSION_FILE);

            // Check if a version file is installed
            if (File.Exists(versionFile))
            {
                // If so open the xml and search for the version tag
                XmlDocument xmlInstalledVersion = new XmlDocument();
                xmlInstalledVersion.Load(versionFile);

                XmlNodeList installedVersionNodes = xmlInstalledVersion.GetElementsByTagName("version");

                foreach (XmlNode xn in installedVersionNodes)
                {
                    // return its value
                    return xn.InnerText;
                }
            }

            // return -1 if no file is installed
            return "-1";
        }

        /// <summary>
        /// Checks automatically how to perform the install
        /// </summary>
        /// <param name="installType">The type of install registered</param>
        public static void PerformInstall(InstallType installType)
        {
            switch (installType)
            {
                case InstallType.Install:
                    Installer.AskUserForInstall();
                    break;
                case InstallType.Upgrade:
                    Installer.Uninstall();
                    Installer.Install();
                    break;
                case InstallType.Uninstall:
                    Installer.AskUserForUninstall();
                    break;
                case InstallType.Downgrade:
                    Installer.Uninstall();
                    Installer.Install();
                    break;
            }
        }

        #endregion

    }
}