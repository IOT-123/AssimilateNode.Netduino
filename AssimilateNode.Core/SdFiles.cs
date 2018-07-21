using Json.NETMF;
using Microsoft.SPOT.IO;
using System.Collections;
using System.IO;
using System.Text;

namespace AssimilateNode.Core
{
    /// <summary>
    /// Reads, writes, boxes SD file contents
    /// </summary>
    public static class SdFiles
    {

        private const string SD_ROOT = "\\SD";
        private const string SD_VOLUME = "SD";

        /// <summary>
        /// Read a string from SD card
        /// </summary>
        /// <returns>
        /// The string or null
        /// </returns>
        /// <param name="filepath">The path on the SD card</param>      
        public static string ReadString(string filepath)
        {
            var fullath = Path.Combine(SD_ROOT, filepath);
            try
            {
                using (FileStream inFileStream = new FileStream(fullath, FileMode.Open))
                {
                    using (StreamReader inStreamReader = new StreamReader(inFileStream))
                    {
                        var content = inStreamReader.ReadToEnd();
                        inStreamReader.Close();
                        inFileStream.Close();
                        return content;
                    }
                }
            } catch {
                return null;
            }
        }

        /// <summary>
        /// Write a string to the  SD card
        /// </summary>
        /// <returns>
        /// True if success
        /// </returns>
        /// <param name="filepath">The path on the SD card</param> 
        /// <param name="content">The content to write</param> 
        public static bool WriteString(string filepath, string content)
        {
            var fullath = Path.Combine(SD_ROOT, filepath);
            try
            {
                var volume = new VolumeInfo(SD_VOLUME);
                if (volume != null)
                {
                    File.WriteAllBytes(fullath, Encoding.UTF8.GetBytes(content));
                    volume.FlushAll();
                    return true;
                }else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Read JSON and box as Hashtable
        /// </summary>
        /// <returns>
        /// The Hashtable
        /// </returns>
        /// <param name="filepath">The path on the SD card</param> 
        public static Hashtable ReadToHashtable(string filepath)
        {
            var json = ReadString(filepath);
            if (json == null)
            {
                return null;
            }
            return JsonSerializer.DeserializeString(json) as Hashtable;
        }

        /// <summary>
        /// Read JSON and box as ArrayList
        /// </summary>
        /// <returns>
        /// The ArrayList
        /// </returns>
        /// <param name="filepath">The path on the SD card</param> 
        public static ArrayList ReadToArrayList(string filepath)
        {
            var json = ReadString(filepath);
            if (json == null)
            {
                return null;
            }
            return JsonSerializer.DeserializeString(json) as ArrayList;
        }

    }
}
