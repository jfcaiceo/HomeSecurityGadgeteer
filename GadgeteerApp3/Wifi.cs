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
        GT.Networking.WebEvent webEventgetpicture;
        GT.Networking.WebEvent webEventActivate;
        GT.Networking.WebEvent webEventInactivate;
        GT.Networking.WebEvent webEventActivateVideo;
        GT.Networking.WebEvent webEventInactivateVideo;
        GT.Networking.WebEvent webEventVideo;
        //public string host = "http://homesecurity-dev.herokuapp.com/";
        public string host = "http://192.168.0.107:3000/";

        public Wifi(Gadgeteer.Modules.GHIElectronics.WiFi_RS21 wifi_RS21)
        {
            this.wifi_RS21 = wifi_RS21;
            GT.Timer t = new GT.Timer(3000);
            t.Tick += new GT.Timer.TickEventHandler(start);
            t.Start();
        }

        void start(GT.Timer timer)
        {
            timer.Stop();

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
            info = wifi_RS21.Interface.Scan("Living fgb");

            wifi_RS21.Interface.Join(info[0], "1234567890");

        }

        void Interface_NetworkAddressChanged(object sender, EventArgs e)
        {
            CameraClass.displayText(wifi_RS21.Interface.NetworkInterface.IPAddress);
            Debug.Print(wifi_RS21.Interface.NetworkInterface.IPAddress);
            /*
            WebServer.StartLocalServer(wifi_RS21.Interface.NetworkInterface.IPAddress, 80);
            Gadgeteer.Networking.HttpRequest wc = WebClient.GetFromWeb(host+"sendIP?ip=" + wifi_RS21.Interface.NetworkInterface.IPAddress);
            webEventgetpicture = WebServer.SetupWebEvent("getpicture");
            webEventgetpicture.WebEventReceived += new WebEvent.ReceivedWebEventHandler(webEventgetpicture_WebEventReceived);

            webEventActivate = WebServer.SetupWebEvent("activate");
            webEventActivate.WebEventReceived += new WebEvent.ReceivedWebEventHandler(webEventActivate_WebEventReceived);

            webEventInactivate = WebServer.SetupWebEvent("inactivate");
            webEventInactivate.WebEventReceived += new WebEvent.ReceivedWebEventHandler(webEventInactivate_WebEventReceived);

            webEventActivateVideo = WebServer.SetupWebEvent("activateVideo");
            webEventActivateVideo.WebEventReceived += new WebEvent.ReceivedWebEventHandler(webEventActivateVideo_WebEventReceived);

            webEventInactivateVideo = WebServer.SetupWebEvent("inactivateVideo");
            webEventInactivateVideo.WebEventReceived += new WebEvent.ReceivedWebEventHandler(webEventInactivateVideo_WebEventReceived);
             */
        }
        /*
        void webEventgetpicture_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            if (CameraClass.getLastPicure() != null)
                responder.Respond(CameraClass.getLastPicure());
        }

        void webEventActivate_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            CameraClass.activateSystem();
            responder.Respond("Sistema Activado");
        }

        void webEventInactivate_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            CameraClass.inactivateSystem();
            responder.Respond("Sistema Desactivado");
        }

        void webEventActivateVideo_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            webEventVideo = WebServer.SetupWebEvent("getVideo", 1);

            webEventVideo.WebEventReceived += new WebEvent.ReceivedWebEventHandler(webEventGetPicturesMulti_WebEventReceived);
            responder.Respond("Video Activado");
        }

        void webEventInactivateVideo_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            WebServer.DisableWebEvent(webEventVideo);
            responder.Respond("Video Desactivado");
        }

        void webEventGetPicturesMulti_WebEventReceived(string path, WebServer.HttpMethod method, Responder responder)
        {
            if (CameraClass.getLastPicure() != null)
                responder.Respond(CameraClass.getLastPicure());
            if (CameraClass.camera.CameraReady)
                CameraClass.camera.TakePicture();
        }
        */
        void wifi_NetworkDown(GT.Modules.Module.NetworkModule sender, GT.Modules.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down event");
        }

        void wifi_NetworkUp(GT.Modules.Module.NetworkModule sender, GT.Modules.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network Up event");
            Debug.Print(sender.NetworkSettings.IPAddress + "");
        }

        public void SendPictureData(Picture picture)
        {
            if (!wifi_RS21.Interface.IsLinkConnected)
                return;

            //Create parameters and get the string body
            Param[] parameters = new Param[2];
            parameters[0] = new Param("filename", Guid.NewGuid() + ".bmp");
            parameters[1] = new Param("file");
            string postData = this.GetPostData(parameters);

            //Get the byte[]
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[][] buffers = new byte[2][];
            buffers[0] = encoding.GetBytes(postData);
            buffers[1] = picture.PictureData;

            SendHttp(buffers, "uploadFile");
        }

        public void SendGasData(double value)
        {
            if (!wifi_RS21.Interface.IsLinkConnected)
                return;
            //Create parameters and get the string body
            Param[] parameters = new Param[2];
            parameters[0] = new Param("gas", value.ToString());
            string postData = this.GetPostData(parameters);

            //Get the byte[]
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[][] buffers = new byte[1][];
            buffers[0] = encoding.GetBytes(postData);

            SendHttp(buffers, "sendGas");
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
            myRequest.GetResponse();
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
