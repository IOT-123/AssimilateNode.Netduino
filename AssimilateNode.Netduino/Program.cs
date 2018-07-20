using AssimilateNode.I2cBus;
using AssimilateNode.Networking;
using System.Collections;
using System.Threading;
using System;
using AssimilateNode.Core;

namespace AssimilateNode.Netduino
{
    public class Program
    {

    public static void Main()
        {
            //            Debug.Print(Resources.GetString(Resources.StringResources.String1));
            // read config
            //var deviceConfig = Config.getDeviceHashtable();
            var i2cComs = new I2cCommunication();
            i2cComs.printMetadata();
            i2cComs.getMetadata();
            i2cComs.printMetadata();
            var networking = new NetworkFeatures();
            networking.InitializeNetwork();
            //var ntpServer = deviceConfig[DeviceJsonKeys.NTP_SERVER_NAME].ToString();
            //var timeOffset = Convert.ToInt16(deviceConfig[DeviceJsonKeys.TIME_ZONE].ToString());
            //networking.SetTime(ntpServer, timeOffset);


            // set sensor timeout
            // THREAD? MQTT init
            // THREAD Server init
            // NTP init
            // get metadata
            // THREAD GET SENSORS LOOPS
            Thread.Sleep(Timeout.Infinite);
        }


    }


}
