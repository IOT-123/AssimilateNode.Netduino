using AssimilateNode.I2cBus;
using AssimilateNode.Networking;
using System.Threading;
using System;
using AssimilateNode.Core;
using System.Collections;

namespace AssimilateNode.Netduino
{
    /// <summary>
    /// Entry point on Netduino
    /// </summary>
    public class Program
    {

        /// <summary>
        /// Entry point on Netduino
        /// </summary>
        public static void Main()
        {
            var deviceConfig = Config.getDeviceHashtable();
            var i2cComs = new I2cCommunication();
            var networking = new NetworkFeatures();
            // i2c
            i2cComs.printMetadata();
            i2cComs.getMetadata();
            i2cComs.printMetadata();
            I2cCommunication.PropertyReceived += (s, e) => onI2cPropertyReceived((PropertyReceivedventArgs)e);
            I2cCommunication.SlaveCyleComplete += (s, e) => onI2cSlaveCyleComplete();
            // network
            Initializer.NetworkConnected += (s, e) =>
            {
                setTime(deviceConfig, networking);
                // THREAD? MQTT init
                // THREAD Server init
            };
            Initializer.NetworkTimeout += (s, e) =>
            {

            };
            networking.InitializeNetwork();
            // i2c property loop
            var readingInterval = Convert.ToInt16(deviceConfig[DeviceJsonKeys.SENSOR_INTERVAL].ToString());
            new Timer(I2cLoop, i2cComs, 0, readingInterval);
            Thread.Sleep(Timeout.Infinite);
        }

        private static void setTime(Hashtable deviceConfig, NetworkFeatures networking)
        {
            var ntpServer = deviceConfig[DeviceJsonKeys.NTP_SERVER_NAME].ToString();
            var timeOffset = Convert.ToInt16(deviceConfig[DeviceJsonKeys.TIME_ZONE].ToString());
            networking.SetTime(ntpServer, timeOffset);
        }

        private static void onI2cPropertyReceived(PropertyReceivedventArgs args)
        {
            var slaveAddress = args.slaveAddress;
            var propertyIndex = args.propertyIndex;
            var role = args.role;
            var name = args.name;
            var value = args.value;
        }

        private static void onI2cSlaveCyleComplete()
        {

        }

        static void I2cLoop(object i2cComs)
        {
            ((I2cCommunication)i2cComs).getProperties();
        }


    }


}
