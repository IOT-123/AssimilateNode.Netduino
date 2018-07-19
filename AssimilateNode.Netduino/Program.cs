using AssimilateNode.I2cBus;
using AssimilateNode.Networking;
using System.Threading;

namespace AssimilateNode.Netduino
{
    public class Program
    {

    public static void Main()
        {
            //            Debug.Print(Resources.GetString(Resources.StringResources.String1));
            var i2cComs = new I2cCommunication();
            i2cComs.printMetadata();
            i2cComs.getMetadata();
            i2cComs.printMetadata();
            var networking = new NetworkFeatures();
            networking.InitializeNetwork();
            networking.SetTime();
            // read config


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
