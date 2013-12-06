using System;
using Microsoft.SPOT;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer;
using System.IO;
using System.Net;

namespace GadgeteerApp3
{
    public class Wifi
    {
        private Gadgeteer.Modules.GHIElectronics.WiFi_RS21 wifi_RS21;
        //public string host = "http://homesecurity-dev.herokuapp.com/";
        public string host = "http://192.168.43.238:3000/";
        GT.Timer t;

        public Wifi(Gadgeteer.Modules.GHIElectronics.WiFi_RS21 wifi_RS21)
        {
            this.wifi_RS21 = wifi_RS21;
            t = new GT.Timer(3000);
            t.Tick += new GT.Timer.TickEventHandler(start);
            t.Start();
        }

        void start(GT.Timer timer)
        {
            t.Stop();

            if (!wifi_RS21.Interface.IsOpen)
                wifi_RS21.Interface.Open();
            if (!wifi_RS21.Interface.NetworkInterface.IsDhcpEnabled)
                wifi_RS21.Interface.NetworkInterface.EnableDhcp();

            // to use hold the network to connect to
            //GHI.Premium.Net.WiFiNetworkInfo[] info = null;
            wifi_RS21.NetworkDown += new GT.Modules.Module.NetworkModule.NetworkEventHandler(wifi_NetworkDown);
            wifi_RS21.NetworkUp += new GT.Modules.Module.NetworkModule.NetworkEventHandler(wifi_NetworkUp);
            wifi_RS21.Interface.NetworkAddressChanged += Interface_NetworkAddressChanged;

            wifi_RS21.UseDHCP();
            GHI.Premium.Net.WiFiNetworkInfo[] info = null;
            info = wifi_RS21.Interface.Scan("AndroidNexus");

            wifi_RS21.Interface.Join(info[0], "17533355k");

        }

        void Interface_NetworkAddressChanged(object sender, EventArgs e)
        {
            Debug.Print(wifi_RS21.Interface.NetworkInterface.IPAddress);
        }

        void wifi_NetworkDown(GT.Modules.Module.NetworkModule sender, GT.Modules.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down event");
        }

        void wifi_NetworkUp(GT.Modules.Module.NetworkModule sender, GT.Modules.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network Up event");
            Debug.Print(sender.NetworkSettings.IPAddress + "");
        }

        public void SendPictureData(byte[] picture, string type)
        {
            if (!wifi_RS21.Interface.IsLinkConnected)
                return;

            //Create parameters and get the string body
            Param[] parameters = new Param[4];
            parameters[0] = new Param("filename", Guid.NewGuid() + ".bmp");
            parameters[1] = new Param("type", type);
            parameters[2] = new Param("device", GetMACAddress(wifi_RS21.NetworkSettings.PhysicalAddress));
            parameters[3] = new Param("file");
            string postData = this.GetPostData(parameters);

            //Get the byte[]
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[][] buffers = new byte[2][];
            buffers[0] = encoding.GetBytes(postData);
            buffers[1] = picture;

            SendHttp(buffers, "uploadFile");
        }

        public void SendGasData(double value)
        {
            if (!wifi_RS21.Interface.IsLinkConnected)
                return;
            //Create parameters and get the string body
            Param[] parameters = new Param[2];
            parameters[0] = new Param("gas", value.ToString());
            parameters[1] = new Param("device", GetMACAddress(wifi_RS21.NetworkSettings.PhysicalAddress));
            string postData = this.GetPostData(parameters);

            //Get the byte[]
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[][] buffers = new byte[1][];
            buffers[0] = encoding.GetBytes(postData);

            SendHttp(buffers, "sendGas");
        }

        public void SendData(string param, string value, string url)
        {
            if (!wifi_RS21.Interface.IsLinkConnected)
                return;
            //Create parameters and get the string body
            Param[] parameters = new Param[1];
            parameters[0] = new Param(param, value);
            string postData = this.GetPostData(parameters);

            //Get the byte[]
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[][] buffers = new byte[1][];
            buffers[0] = encoding.GetBytes(postData);

            SendHttp(buffers, url);
        }

        private void SendHttp(byte[][] buffers, string uriMethod)
        {
            Uri urlUri = new Uri(host + uriMethod);
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(urlUri);
            myRequest.Method = "POST";
            myRequest.AllowWriteStreamBuffering = true;

            int count = 0;
            foreach (byte[] buffer in buffers)
            {
                count += buffer.Length;
            }

            // Get length of content
            myRequest.ContentLength = count;


            // Get request stream 
            Stream newStream = myRequest.GetRequestStream();

            // Send the data.
            foreach (byte[] buffer in buffers)
            {
                newStream.Write(buffer, 0, buffer.Length);
            }

            // Close stream 
            newStream.Close();
            myRequest.KeepAlive = false;

            try
            {
                myRequest.GetResponse();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        private string GetPostData(Param[] parameters)
        {
            string requestBody = "";
            foreach (Param param in parameters)
            {
                requestBody += param.Key + "=";
                if (param.Value != string.Empty)
                {
                    requestBody += param.Value + ";";
                }
            }
            return requestBody;
        }

        private string GetMACAddress(byte[] PhysicalAddress)
        {
            return ByteToHex(PhysicalAddress[0]) + "-"
                                + ByteToHex(PhysicalAddress[1]) + "-"
                                + ByteToHex(PhysicalAddress[2]) + "-"
                                + ByteToHex(PhysicalAddress[3]) + "-"
                                + ByteToHex(PhysicalAddress[4]) + "-"
                                + ByteToHex(PhysicalAddress[5]);
        }

        private string ByteToHex(byte number)
        {
            string hex = "0123456789ABCDEF";
            return new string(new char[] { hex[(number & 0xF0) >> 4], hex[number & 0x0F] });
        }

    }

    public struct Param
    {
        private string key;
        private string value;

        public Param(string key)
        {
            this.key = key;
            this.value = string.Empty;
        }

        public Param(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public string Key
        {
            get { return key; }
        }

        public string Value
        {
            get { return value; }
        }
    }
}
