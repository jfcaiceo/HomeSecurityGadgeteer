using System;
using Microsoft.SPOT;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer;
using System.IO;

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
        public string host = "http://homesecurity-dev.herokuapp.com/";

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
            wifi_RS21.Interface.WirelessConnectivityChanged += Interface_WirelessConnectivityChanged;

            wifi_RS21.UseDHCP();
            GHI.Premium.Net.WiFiNetworkInfo[] info = null;
            info = wifi_RS21.Interface.Scan("Living fgb");

            wifi_RS21.Interface.Join(info[0], "1234567890");

        }

        void Interface_WirelessConnectivityChanged(object sender, GHI.Premium.Net.WiFiRS9110.WirelessConnectivityEventArgs e)
        {
            Debug.Print("cambio de conectividad");
            Debug.Print(e.IsConnected+"");
        }

        void Interface_NetworkAddressChanged(object sender, EventArgs e)
        {
            CameraClass.displayText(wifi_RS21.Interface.NetworkInterface.IPAddress);
            Debug.Print(wifi_RS21.Interface.NetworkInterface.IPAddress);
            WebServer.StartLocalServer(wifi_RS21.Interface.NetworkInterface.IPAddress, 80);
            Gadgeteer.Networking.HttpRequest wc = WebClient.GetFromWeb("http://homesecurity-dev.herokuapp.com/sendIP?ip=" + wifi_RS21.Interface.NetworkInterface.IPAddress);
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
        }

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

        void wifi_NetworkDown(GT.Modules.Module.NetworkModule sender, GT.Modules.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network down event");
        }

        void wifi_NetworkUp(GT.Modules.Module.NetworkModule sender, GT.Modules.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network Up event");
            Debug.Print(sender.NetworkSettings.IPAddress +"");
        }

        private void SendPicture(Picture picture)
        {
            var req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(host + "uploadFile");
            req.Method = "POST";
            req.ContentType = "image/bmp"; 
            
            req.UserAgent = "Gadgeteer";

            //req.Headers.Add("X-AuthKey", AuthKey);

            req.ContentLength = picture.PictureData.Length;

            try
            {
                using (Stream s = req.GetRequestStream())
                {
                    s.Write(picture.PictureData, 0, picture.PictureData.Length);
                }

                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    using (Stream respStream = resp.GetResponseStream())
                    {
                        byte[] respBytes = new byte[respStream.Length];
                        respStream.Read(respBytes, 0, respBytes.Length);
                        string respString = new string(System.Text.Encoding.UTF8.GetChars(respBytes));
                        Debug.Print(respString);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print("exception : " + ex.Message);
            }
            finally
            {
                req.Dispose();
            }
        }

        public void SendPictureData()        {
            GT.Picture picture = CameraClass.getLastPicure();
            POSTContent postData = POSTContent.CreateBinaryBasedContent(picture.PictureData);
            CameraClass.displayText(postData.ToString());
            

            var reqData =
                HttpHelper.CreateHttpPostRequest(host + "uploadFile",
                postData, "image/bmp");

            reqData.ResponseReceived += new HttpRequest.ResponseHandler(reqData_ResponseReceived);
            reqData.SendRequest();
        }
               

        void reqData_ResponseReceived(HttpRequest sender, HttpResponse response)
        {
            if (response.StatusCode != "200")
                CameraClass.displayText(response.StatusCode);
           
        }
    }
}
