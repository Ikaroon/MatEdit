using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;

namespace MB.MatEdit
{
    public class MatEdit_Installer
    {
        private static string versionCache;

        [InitializeOnLoadMethod()]
        public static void Install()
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string installPath = Path.Combine(projectPath, "MatEdit");

            string versionFile = Path.Combine(installPath, Basics.VERSION_FILE);

            string cgFile = Path.Combine(projectPath, Basics.MATGUI_CG);

            string cgCurveFile = Path.Combine(installPath, Basics.MATGUI_CG_CURVE);
            string cgGradientFile = Path.Combine(installPath, Basics.MATGUI_CG_GRADIENT);

            if (File.Exists(versionFile))
            {
                if (versionCache == null)
                {
                    string[] checked_VersionFiles = Directory.GetFiles(Application.dataPath, "*" + Basics.VERSION_FILE, SearchOption.AllDirectories);
                    if (checked_VersionFiles.Length == 1)
                    {
                        XmlDocument xmlCurrentVersion = new XmlDocument();
                        xmlCurrentVersion.Load(checked_VersionFiles[0]);

                        XmlNodeList currentVersionNodes = xmlCurrentVersion.GetElementsByTagName("version");

                        foreach (XmlNode xn in currentVersionNodes)
                        {
                            string version = xn.InnerText;
                            versionCache = version;
                        }
                    }
                }

                XmlDocument xmlInstalledVersion = new XmlDocument();
                xmlInstalledVersion.Load(versionFile);

                XmlNodeList installedVersionNodes = xmlInstalledVersion.GetElementsByTagName("version");

                foreach (XmlNode xn in installedVersionNodes)
                {
                    string version = xn.InnerText;
                    if (!version.Equals(versionCache))
                    {
                        if (float.Parse(version) > float.Parse(versionCache))
                        {
                            Debug.LogWarning("<color=red>Downgraded to a MatEdit version which is older than the one currently installed!</color>");
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (!Directory.Exists(installPath))
            {
                Directory.CreateDirectory(installPath);
            }

            InstallFile(versionFile, Basics.VERSION_FILE);
            InstallFile(cgFile, Basics.MATGUI_CG);
            InstallFile(cgCurveFile, Basics.MATGUI_CG_CURVE);
            InstallFile(cgGradientFile, Basics.MATGUI_CG_GRADIENT);

            Debug.Log("<color=red>Installed MatEdit for this project!</color>");
            Debug.Log("<color=red>MatEdit: Uninstall Instructions:</color>\n" +
                "1. Go to project folder\n" +
                "2. Delete the MatEdit folder\n" +
                "3. Delete the MatEdit.cginc\n" +
                "4. Done!\n" +
                "\n" +
                "<color=red>Take care - Delete MatEdit scripts beforehand - otherwise it will reinstall MatEdit during some Unity Events</color>\n");
        }

        private static void InstallFile(string path, string name)
        {
            string[] files = Directory.GetFiles(Application.dataPath, "*" + name, SearchOption.AllDirectories);
            if (files.Length == 1)
            {
                File.Copy(files[0], path, true);
            }
        }
    }
}