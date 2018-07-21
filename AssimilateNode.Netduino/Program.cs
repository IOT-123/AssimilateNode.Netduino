using AssimilateNode.I2cBus;
using AssimilateNode.Networking;
using System.Threading;
using System;
using AssimilateNode.Core;
using Netduino.Foundation.Network;

namespace AssimilateNode.Netduino
{
    public class Program
    {

    public static void Main()
        {
            var deviceConfig = Config.getDeviceHashtable();
            var i2cComs = new I2cCommunication();
            var networking = new NetworkFeatures();
            // i2c
            i2cComs.printMetadata();
            i2cComs.getMetadata();
            i2cComs.printMetadata();
            I2cCommunication.PropertyReceived += (s, e) =>
            {

            };
            I2cCommunication.SlaveCyleComplete += (s, e) =>
            {

            };
            // network
            Initializer.NetworkConnected += (s, e) =>
            {
                var ntpServer = deviceConfig[DeviceJsonKeys.NTP_SERVER_NAME].ToString();
                var timeOffset = Convert.ToInt16(deviceConfig[DeviceJsonKeys.TIME_ZONE].ToString());
                networking.SetTime(ntpServer, timeOffset);
                // THREAD? MQTT init
                // THREAD Server init
            };
            networking.InitializeNetwork();
            var readingInterval = Convert.ToInt16(deviceConfig[DeviceJsonKeys.SENSOR_INTERVAL].ToString());
            // i2c property loop
            new Timer(I2cLoop, i2cComs, 0, readingInterval);
            Thread.Sleep(Timeout.Infinite);
        }

        static void I2cLoop(object i2cComs)
        {
            ((I2cCommunication)i2cComs).getProperties();
        }


    }


}
