using UnityEngine;
using System.IO;

namespace MB.MatEdit
{
    internal static class MatEdit
    {
        internal const string ROOT_PATH = "MatEdit";

        internal static string ProjectPath()
        {
            return Directory.GetParent(Application.dataPath).FullName;
        }

        internal static string AbsoluteToAssetPath(string path)
        {
            path = Directory.GetParent(path).FullName;
            path = path.Replace(MatEdit.ProjectPath(), "");
            path = path.Replace(@"\", "/");
            path = path.Substring(1, path.Length - 1);
            return path;
        }
    }
}