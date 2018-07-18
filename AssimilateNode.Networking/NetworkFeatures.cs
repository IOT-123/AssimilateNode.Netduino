using System.Threading;
using System.Net;
using Microsoft.SPOT;
using Microsoft.SPOT.Net.NetworkInformation;
using System;
using System.Net.Sockets;

namespace AssimilateNode.Networking
{
    public class NetworkFeatures
    {

        NetworkInterface[] _interfaces;

        public bool IsRunning { get; set; }

        public void Verify()
        {
            this.IsRunning = true;
            bool goodToGo = false;
            try
            {
                goodToGo = InitializeNetwork();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
            if (goodToGo)
            {
                MakeWebRequest("http://google.com");
                Debug.Print("Network.Verify:  done.");
            }
            //this.IsRunning = false;
        }

        public bool SetTime()
        {
            var result = UpdateTimeFromNtpServer("us.pool.ntp.org", 10);
            Debug.Print(result ? "Networking.SetTime: Success" : "Networking.SetTime: Fail");
            return result;

        }

        protected bool InitializeNetwork()
        {
            if (Microsoft.SPOT.Hardware.SystemInfo.SystemID.SKU == 3)
            {
                Debug.Print("Wireless tests run only on Device");
                return false;
            }
            Debug.Print("Getting all the network interfaces.");
            _interfaces = NetworkInterface.GetAllNetworkInterfaces();
            // loop through each network interface
            foreach (var net in _interfaces)
            {
                // debug out
                ListNetworkInfo(net);

                switch (net.NetworkInterfaceType)
                {
                    case (NetworkInterfaceType.Ethernet):
                        Debug.Print("Found Ethernet Interface");
                        break;
                    case (NetworkInterfaceType.Wireless80211):
                        Debug.Print("Found 802.11 WiFi Interface");
                        break;
                    case (NetworkInterfaceType.Unknown):
                        Debug.Print("Found Unknown Interface");
                        break;
                }
                // check for an IP address, try to get one if it's empty
                return CheckIPAddress(net);
            }
            // if we got here, should be false.
            return false;
        }

        protected void MakeWebRequest(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "GET";
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                if (HttpStatusCode.OK == httpResponse.StatusCode)
                {
                    Debug.Print("Networking.MakeWebRequest: OK");
                }
                else
                {
                    Debug.Print("Networking.MakeWebRequest: " + httpResponse.StatusCode.ToString());
                }
            }
            catch
            {
                Debug.Print("Networking.MakeWebRequest: Fail");
            }
        }

        protected bool CheckIPAddress(NetworkInterface net)
        {
            int timeout = 10000; // timeout, in milliseconds to wait for an IP. 10,000 = 10 seconds
            // check to see if the IP address is empty (0.0.0.0). IPAddress.Any is 0.0.0.0.
            if (net.IPAddress == IPAddress.Any.ToString())
            {
                Debug.Print("Networking.CheckIPAddress: No IP Address");
                if (net.IsDhcpEnabled)
                {
                    Debug.Print("Networking.CheckIPAddress: DHCP is enabled, attempting to get an IP Address");
                    // ask for an IP address from DHCP [note this is a static, not sure which network interface it would act on]
                    int sleepInterval = 10;
                    int maxIntervalCount = timeout / sleepInterval;
                    int count = 0;
                    while (IPAddress.GetDefaultLocalAddress() == IPAddress.Any && count < maxIntervalCount)
                    {
                        Debug.Print("Networking.CheckIPAddress: Polling IP Address");
                        Thread.Sleep(10);
                        count++;
                    };
                    // if we got here, we either timed out or got an address, so let's find out.
                    if (net.IPAddress == IPAddress.Any.ToString())
                    {
                        Debug.Print("Failed to get an IP Address in the alotted time.");
                        return false;
                    }
                    Debug.Print("Networking.CheckIPAddress: Got IP Address: " + net.IPAddress.ToString());
                    return true;
                }
                else
                {
                    Debug.Print("Networking.CheckIPAddress: DHCP is not enabled, and no IP address is configured, bailing out.");
                    return false;
                }
            }
            else
            {
                Debug.Print("Networking.CheckIPAddress: Already had IP Address: " + net.IPAddress.ToString());
                return true;
            }

        }

        protected void ListNetworkInfo(NetworkInterface net)
        {
            try
            {
                Debug.Print("MAC Address: " + BytesToHexString(net.PhysicalAddress));
                Debug.Print("DHCP enabled: " + net.IsDhcpEnabled.ToString());
                Debug.Print("Dynamic DNS enabled: " + net.IsDynamicDnsEnabled.ToString());
                Debug.Print("IP Address: " + net.IPAddress.ToString());
                Debug.Print("Subnet Mask: " + net.SubnetMask.ToString());
                Debug.Print("Gateway: " + net.GatewayAddress.ToString());

                if (net is Wireless80211)
                {
                    var wifi = net as Wireless80211;
                    Debug.Print("SSID:" + wifi.Ssid.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.Print("ListNetworkInfo exception:  " + e.Message);
            }

        }

        private static string BytesToHexString(byte[] bytes)
        {
            string hexString = string.Empty;

            // Create a character array for hexadecimal conversion.
            const string hexChars = "0123456789ABCDEF";

            // Loop through the bytes.
            for (byte b = 0; b < bytes.Length; b++)
            {
                if (b > 0)
                    hexString += "-";

                // Grab the top 4 bits and append the hex equivalent to the return string.        
                hexString += hexChars[bytes[b] >> 4];

                // Mask off the upper 4 bits to get the rest of it.
                hexString += hexChars[bytes[b] & 0x0F];
            }

            return hexString;
        }

        public bool UpdateTimeFromNtpServer(string server, int timeZoneOffset)
        {
            try
            {
                var currentTime = GetNtpTime(server, timeZoneOffset);
                Microsoft.SPOT.Hardware.Utility.SetLocalTime(currentTime);
                return true;
            }
            catch (Exception ex)
            {
                //Debug.Print(ex.InnerException.Message);
                return false;
            }
        }


        /// <summary>
        /// Get DateTime from NTP Server
        /// Based on:
        /// http://weblogs.asp.net/mschwarz/archive/2008/03/09/wrong-datetime-on-net-micro-framework-devices.aspx
        /// </summary>
        /// <param name="timeServer">Time Server (NTP) address</param>
        /// <param name="timeZoneOffset">Difference in hours from UTC</param>
        /// <returns>Local NTP Time</returns>
        private DateTime GetNtpTime(String timeServer, int timeZoneOffset)
        {
            // Find endpoint for TimeServer
            var ep = new IPEndPoint(Dns.GetHostEntry(timeServer).AddressList[0], 123);
            // Make send/receive buffer
            var ntpData = new byte[48];
            // Connect to TimeServer
            using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                // Set 10s send/receive timeout and connect
                s.SendTimeout = s.ReceiveTimeout = 10000; // 10,000 ms
                s.Connect(ep);
                // Set protocol version
                ntpData[0] = 0x1B;
                // Send Request
                s.Send(ntpData);
                // Receive Time
                s.Receive(ntpData);
                // Close the socket
                s.Close();
            }
            const byte offsetTransmitTime = 40;
            ulong intpart = 0;
            ulong fractpart = 0;
            for (var i = 0; i <= 3; i++)
                intpart = (intpart << 8) | ntpData[offsetTransmitTime + i];
            for (var i = 4; i <= 7; i++)
                fractpart = (fractpart << 8) | ntpData[offsetTransmitTime + i];
            ulong milliseconds = (intpart * 1000 + (fractpart * 1000) / 0x100000000L);
            var timeSpan = TimeSpan.FromTicks((long)milliseconds * TimeSpan.TicksPerMillisecond);
            var dateTime = new DateTime(1900, 1, 1);
            dateTime += timeSpan;
            var offsetAmount = new TimeSpan(timeZoneOffset, 0, 0);
            var networkDateTime = (dateTime + offsetAmount);
            return networkDateTime;
        }

    }

}
