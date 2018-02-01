using UnityEditor;
using UnityEngine;
using System.IO;

namespace MB.MatEdit
{
    internal static class Content
    {
        internal const string VERSION_FILE = "MATEDIT_VERSION.xml";

        internal const string CG_PATH = "CG/";

        internal const string MATGUI_CG = "MatEditCG.cginc";

        internal const string MATGUI_CG_CURVE = "CurveCG.cginc";
        internal const string MATGUI_CG_GRADIENT = "GradientCG.cginc";

        internal static string Get(string content)
        {
            string pathParent = Directory.GetParent(content).Name;
            if (pathParent == MatEdit.ROOT_PATH)
            {
                pathParent = "";
            }
            string path = Path.Combine(pathParent, Path.GetFileNameWithoutExtension(content)).Replace(@"\", "/");
            return Path.Combine(Directory.GetParent(Application.dataPath).FullName, AssetDatabase.GetAssetPath(Resources.Load(MatEdit.ROOT_PATH + "/" + path)));
        }

        internal static string GetAsset(string content)
        {
            return MatEdit.AbsoluteToAssetPath(Get(content));
        }
    }
}