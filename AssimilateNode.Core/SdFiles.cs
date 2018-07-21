using Json.NETMF;
using Microsoft.SPOT;
using Microsoft.SPOT.IO;
using System.Collections;
using System.IO;
using System.Text;
using System;

namespace AssimilateNode.Core
{
    /// <summary>
    /// Reads, writes, boxes SD file contents
    /// </summary>
    public static class SdFiles
    {

        private const string SD_ROOT = "\\SD";
        private const string SD_VOLUME = "SD";

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

        public static Hashtable ReadToHashtable(string filepath)
        {
            var json = ReadString(filepath);
            if (json == null)
            {
                return null;
            }
            return JsonSerializer.DeserializeString(json) as Hashtable;
        }

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
