// System
using System.IO;

namespace MB.MatEdit {
    public static class Templates
    {

        public const string SIMPLE_SHADER_GUI = "ShaderGUI";
        public const string SHADER_GUI_CLASS_NAME = "#CLASS_NAME#";

        private const string TEMPLATE_EXTENSION = ".txt";

        private const string TEMPLATE_ROOT = "Templates";

        internal static string Get(string templateName)
        {
            string template = Content.Get(TEMPLATE_ROOT + "/" + templateName);
    
            if (Path.GetFileNameWithoutExtension(template) == templateName)
            {
                return FileOperator.ReadStringFromFile(template);
            }

            return "";
        }
    }
}