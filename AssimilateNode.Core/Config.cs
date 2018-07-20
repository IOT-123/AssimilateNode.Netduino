using System.Collections;
using Json.NETMF;

namespace AssimilateNode.Core
{
    public static class Config
    {
        private const string FILE_CONFIG_DEVICE = "config\\device.json";
        private const string FILE_CONFIG_USER_METAS = "config\\user_metas_%i.json";
        private const string FILE_CONFIG_SLAVE_METAS = "config\\slave_metas_%i.json";
        private const string FILE_ADDRESS_WHITELIST = "config\\slave_address_whitelist.json";

        public static bool serializeSlaveMetas(byte slaveAddress, Hashtable rootPairs)
        {
            var pathWrite = Config.FILE_CONFIG_SLAVE_METAS.Replace("%i", slaveAddress.ToString());
            var json = JsonSerializer.SerializeObject(rootPairs);
            return SdFiles.WriteString(pathWrite, json);
        }

        public static ArrayList deserializeUserMetas(byte slaveAddress)
        {
            var pathRead = Config.FILE_CONFIG_USER_METAS.Replace("%i", slaveAddress.ToString());
            return SdFiles.ReadToArrayList(pathRead);
        }

        public static Hashtable getDeviceHashtable()
        {
            return SdFiles.ReadToHashtable(FILE_CONFIG_DEVICE);
        }

        public static ArrayList getWhitelistArrayList()
        {
            return SdFiles.ReadToArrayList(FILE_ADDRESS_WHITELIST);
        }

    }

}
