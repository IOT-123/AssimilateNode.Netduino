using Json.NETMF;
using Microsoft.SPOT;
using System.Collections;
using System;

namespace SharedClasses
{
    public static class Config
    {
        internal const string FILE_CONFIG_USER_METAS = "/config/user_metas_%i.json";
        internal const string FILE_CONFIG_SLAVE_METAS = "/config/slave_metas_%i.json";
        internal const string FILE_ADDRESS_WHITELIST = "/config/address_whitelist.json";

        internal static void serializeSlaveMetas(byte slaveAddress, Hashtable rootPairs)
        {
            var pathWrite = Config.FILE_CONFIG_SLAVE_METAS.Replace( "%i", slaveAddress.ToString());
            var json = JsonSerializer.SerializeObject(rootPairs);
            Debug.Print(pathWrite);
            Debug.Print(json);
        }

        internal static ArrayList deserializeUserMetas(byte slaveAddress)
        {
            var pathRead = Config.FILE_CONFIG_USER_METAS.Replace( "%i", slaveAddress.ToString());

            return new ArrayList();
        }

        internal static void clearSlaveMetadataFiles()
        {
            //for (byte i = 8; i < 128; i++)
            //{// range of i2c addresses
            //    char path[31];
            //    sprintf(path, DIR_CONFIG_SLAVE_METAS, i);
            //    if (SPIFFS.exists(path))
            //    {
            //        SPIFFS.remove(path);
            //    }
            //}
        }
    }


}
