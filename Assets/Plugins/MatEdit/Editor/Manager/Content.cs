// System
using System.IO;

// Unity
using UnityEditor;
using UnityEngine;

namespace MB.MatEdit
{
    internal static class Content
    {
        public const string VERSION_FILE = "MATEDIT_VERSION.xml";

        public const string CG_PATH = "CG/";

        public const string MATGUI_CG = "MatEditCG.cginc";

        public const string MATGUI_CG_CURVE = "CurveCG.cginc";
        public const string MATGUI_CG_GRADIENT = "GradientCG.cginc";

        public static string Get(string content)
        {
            string pathParent = Directory.GetParent(content).Name;
            if (pathParent == MatEdit.ROOT_PATH)
            {
                pathParent = "";
            }
            string path = Path.Combine(pathParent, Path.GetFileNameWithoutExtension(content)).Replace(@"\", "/");
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, AssetDatabase.GetAssetPath(Resources.Load(MatEdit.ROOT_PATH + "/" + path)));
        }

        public static string GetAsset(string content)
        {
            return MatEdit.AbsoluteToAssetPath(Get(content));
        }

        public static T GetFile<T>(string content) where T : Object
        {
            string pathParent = Directory.GetParent(content).Name;
            if (pathParent == MatEdit.ROOT_PATH)
            {
                pathParent = "";
            }
            string path = Path.Combine(pathParent, Path.GetFileNameWithoutExtension(content)).Replace(@"\", "/");
            return Resources.Load<T>(MatEdit.ROOT_PATH + "/" + path);
        }
    }
}