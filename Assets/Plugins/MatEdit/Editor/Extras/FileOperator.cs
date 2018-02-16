// System
using System.IO;
using System.Text;

namespace MB.MatEdit
{
    public static class FileOperator
    {
        /// <summary>
        /// Reads a string from a file
        /// </summary>
        /// <param name="path">The file which should be read</param>
        /// <returns>The content of the file</returns>
        public static string ReadStringFromFile(string path)
        {
            // Create the content
            string content = "";

            // Abandon if the file is not given
            if (!File.Exists(path))
            {
                return content;
            }

            // Fill the content with the file content
            using (StreamReader sr = File.OpenText(path))
            {
                content = sr.ReadToEnd();
            }

            // Return the content
            return content;
        }
        
        /// <summary>
        /// Writes a string to a file
        /// </summary>
        /// <param name="content">The content which should be written to the file</param>
        /// <param name="path">The path of the file which should be written to</param>
        public static void WriteStringToFile(string content, string path)
        {
            // Open the FileStream and write to it
            using (FileStream sw = File.Open(path, FileMode.OpenOrCreate))
            {
                // Set the Encoding
                Encoding enc = Encoding.UTF8;

                // Convert the string to bytes
                byte[] bytes = enc.GetBytes(content);

                // Clear the file
                sw.SetLength(0);

                // Write the content to the file
                sw.Write(bytes, 0, bytes.Length);
            }
        }

    }
}