using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

namespace MB.MatEdit
{
    public class Installer
    {

        [InitializeOnLoadMethod()]
        public static void AskForInstall()
        {
            if (!MatEditData.Data.welcomeMessage)
            {
                if (EditorUtility.DisplayDialog("Install MatEdit", "Do you want to install MatEdit's improved CG support?\n" +
                    "This will enable you to use #include " + '"' + "MatEditCG.cginc" + '"' + " everywhere in the Project.", "Yes", "No"))
                {
                    Install();
                }
                MatEditData.Data.welcomeMessage = true;
                EditorUtility.SetDirty(MatEditData.Data);
            }
        }

        public static void AskInstall()
        {
            bool install = EditorUtility.DisplayDialog("MatEdit | Installation",
                "You're about to install MatEdit's optional CG support.\n" +
                "This will enable you to use #include " + '"' + "MatEditCG.cginc" + '"' + " everywhere in the Project.\n" +
                "\n" +
                "Do you want to proceed?", "Yes", "No");

            if (!install)
            {
                return;
            }

            Install();
        }

        public static void Install()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string installPath = Path.Combine(projectPath, "MatEdit");

            string versionFile = Path.Combine(installPath, Content.VERSION_FILE);

            string cgFile = Path.Combine(projectPath, Content.MATGUI_CG);

            string cgCurveFile = Path.Combine(installPath, Content.MATGUI_CG_CURVE);
            string cgGradientFile = Path.Combine(installPath, Content.MATGUI_CG_GRADIENT);
            
            if (!Directory.Exists(installPath))
            {
                Directory.CreateDirectory(installPath);
            }

            InstallFile(versionFile, Content.VERSION_FILE);
            InstallFile(cgFile, Content.CG_PATH + Content.MATGUI_CG);
            InstallFile(cgCurveFile, Content.CG_PATH + Content.MATGUI_CG_CURVE);
            InstallFile(cgGradientFile, Content.CG_PATH + Content.MATGUI_CG_GRADIENT);
        }

        private static void InstallFile(string path, string name)
        {
            string file = Content.Get(name);
            File.Copy(file, path, true);
        }

        internal static string GetInstalledVersionNumber()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string installPath = Path.Combine(projectPath, "MatEdit");

            string versionFile = Path.Combine(installPath, Content.VERSION_FILE);


            if (File.Exists(versionFile))
            {
                XmlDocument xmlInstalledVersion = new XmlDocument();
                xmlInstalledVersion.Load(versionFile);

                XmlNodeList installedVersionNodes = xmlInstalledVersion.GetElementsByTagName("version");

                foreach (XmlNode xn in installedVersionNodes)
                {
                    return xn.InnerText;
                }
            }

            return "-1";
        }

        public static void AskUninstall()
        {
            bool uninstall = EditorUtility.DisplayDialog("MatEdit | Uninstallation",
                "You're about to uninstall MatEdit's optional CG support.\n" +
                "You wont delete the Files which are located in your Asset folder!\n" +
                "\n" +
                "Do you want to proceed?", "Yes", "No");

            if (!uninstall)
            {
                return;
            }

            Uninstall();
        }

        public static void Uninstall()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string installPath = Path.Combine(projectPath, "MatEdit");

            string cgFile = Path.Combine(projectPath, Content.MATGUI_CG);

            if (Directory.Exists(installPath))
            {
                Directory.Delete(installPath, true);
            }

            if (File.Exists(cgFile))
            {
                File.Delete(cgFile);
            }
        }
    }
}