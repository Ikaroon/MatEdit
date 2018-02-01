using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace MB.MatEdit {
    public static class Templates {

        private const string TEMPLATE_EXTENSION = ".txt";

        private const string TEMPLATE_ROOT = "Templates";

        internal static string LoadStringFromFile(string path)
        {
            string content = "";
            if (!File.Exists(path))
            {
                return null;
            }
            using (StreamReader sr = File.OpenText(path))
            {
                content = sr.ReadToEnd();
            }
            return content;
        }

        internal static string Get(string templateName)
        {
            string template = Content.Get(TEMPLATE_ROOT + "/" + templateName);
    
            if (Path.GetFileNameWithoutExtension(template) == templateName)
            {
                return LoadStringFromFile(template);
            }

            return "";
        }
    }
}