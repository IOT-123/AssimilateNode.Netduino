using System.Collections;
using Json.NETMF;

namespace AssimilateNode.Core
{

    /// <summary>
    /// Methods and constants for config files
    /// </summary>
    public static class Config
    {
        private const string FILE_CONFIG_DEVICE = "config\\device.json";
        private const string FILE_CONFIG_USER_METAS = "config\\user_metas_%i.json";
        private const string FILE_CONFIG_SLAVE_METAS = "config\\slave_metas_%i.json";
        private const string FILE_ADDRESS_WHITELIST = "config\\slave_address_whitelist.json";

        /// <summary>
        /// Saves an object generated from metadata to JSON for each slave
        /// </summary>
        /// <returns>
        /// True if success
        /// </returns>
        /// <param name="slaveAddress">The address of the slave</param>
        /// <param name="rootPairs">The root object to serialize.</param>
        public static bool serializeSlaveMetas(byte slaveAddress, Hashtable rootPairs)
        {
            var pathWrite = Config.FILE_CONFIG_SLAVE_METAS.Replace("%i", slaveAddress.ToString());
            var json = JsonSerializer.SerializeObject(rootPairs);
            return SdFiles.WriteString(pathWrite, json);
        }

        /// <summary>
        /// Creates an array generated from JSON for each slave
        /// </summary>
        /// <returns>
        /// ArrayList of JSON values
        /// </returns>
        /// <param name="slaveAddress">The address of the slave</param>
        public static ArrayList deserializeUserMetas(byte slaveAddress)
        {
            var pathRead = Config.FILE_CONFIG_USER_METAS.Replace("%i", slaveAddress.ToString());
            return SdFiles.ReadToArrayList(pathRead);
        }

        /// <summary>
        /// Creates an object generated from JSON \SD\]config\device.json
        /// </summary>
        /// <returns>
        /// Hashtable of JSON values
        /// </returns>
        public static Hashtable getDeviceHashtable()
        {
            return SdFiles.ReadToHashtable(FILE_CONFIG_DEVICE);
        }

        /// <summary>
        /// Creates an array generated from JSON \SD\]config\slave_address_whitelist.json
        /// </summary>
        /// <returns>
        /// ArrayList of JSON values (slaves to process)
        /// </returns>
        public static ArrayList getWhitelistArrayList()
        {
            return SdFiles.ReadToArrayList(FILE_ADDRESS_WHITELIST);
        }

    }

}
