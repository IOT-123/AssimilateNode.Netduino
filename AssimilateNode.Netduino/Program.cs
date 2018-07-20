using AssimilateNode.I2cBus;
using AssimilateNode.Networking;
using System.Threading;
using System;
using AssimilateNode.Core;

namespace AssimilateNode.Netduino
{
    public class Program
    {

    public static void Main()
        {
            var deviceConfig = Config.getDeviceHashtable();
            var i2cComs = new I2cCommunication();
            i2cComs.printMetadata();
            i2cComs.getMetadata();
            i2cComs.printMetadata();
            var networking = new NetworkFeatures();
            networking.InitializeNetwork();
            var ntpServer = deviceConfig[DeviceJsonKeys.NTP_SERVER_NAME].ToString();
            var timeOffset = Convert.ToInt16(deviceConfig[DeviceJsonKeys.TIME_ZONE].ToString());
            networking.SetTime(ntpServer, timeOffset);


            // THREAD? MQTT init
            // THREAD Server init

            var readingInterval = Convert.ToInt16(deviceConfig[DeviceJsonKeys.SENSOR_INTERVAL].ToString());
            new Timer(I2cLoop, i2cComs, 0, readingInterval);
            Thread.Sleep(Timeout.Infinite);
        }

        static void I2cLoop(object i2cComs)
        {
            ((I2cCommunication)i2cComs).getProperties();
        }


    }


}
