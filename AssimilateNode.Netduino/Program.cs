using AssimilateNode.I2cBus;
using AssimilateNode.Networking;
using System.Threading;
using System;
using AssimilateNode.Core;
using System.Collections;
using Microsoft.SPOT;

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
                startI2cReadLoop(deviceConfig, i2cComs);
            };
            Initializer.NetworkTimeout += (s, e) =>
            {
                Debug.Print("Timout connecting to network;");
                startI2cReadLoop(deviceConfig, i2cComs);
            };
            networking.InitializeNetwork();
            Thread.Sleep(Timeout.Infinite);
        }

        private static void startI2cReadLoop(Hashtable deviceConfig, I2cCommunication i2cComs)
        {
            Debug.Print("startI2cReadLoop");
            var readingInterval = Convert.ToInt16(deviceConfig[DeviceJsonKeys.SENSOR_INTERVAL].ToString());
            new Timer(onI2cLoop, i2cComs, 0, readingInterval);
        }

        private static void setTime(Hashtable deviceConfig, NetworkFeatures networking)
        {
            Debug.Print("setTime");
            var ntpServer = deviceConfig[DeviceJsonKeys.NTP_SERVER_NAME].ToString();
            var timeOffset = Convert.ToInt16(deviceConfig[DeviceJsonKeys.TIME_ZONE].ToString());
            networking.SetTime(ntpServer, timeOffset);
        }

        private static void onI2cPropertyReceived(PropertyReceivedventArgs args)
        {
            Debug.Print("onI2cPropertyReceived");
            var slaveAddress = args.slaveAddress;
            var propertyIndex = args.propertyIndex;
            var role = args.role;
            var name = args.name;
            var value = args.value;
        }

        private static void onI2cSlaveCyleComplete()
        {
            Debug.Print("onI2cSlaveCyleComplete");
        }

        private static void onI2cLoop(object i2cComs)
        {
            ((I2cCommunication)i2cComs).getProperties();
        }


    }


}
