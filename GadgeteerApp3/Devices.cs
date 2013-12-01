using System;
using Microsoft.SPOT;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.GHIElectronics;

using Lzf;

namespace GadgeteerApp3
{
    public class Devices
    {
        Gadgeteer.Bluetooth blueT;
        GT.Timer t;
        Button button;
        Wifi wifi;
        bool sending = false;
        LZF lzf = new LZF();

        public Devices(Button button, Wifi wifi)
        {
            this.wifi = wifi;
            blueT = new Gadgeteer.Bluetooth(8);
            this.button = button;
            t = new GT.Timer(100);
            t.Tick += new GT.Timer.TickEventHandler(start);
            t.Start();
            Debug.Print("devices");
        }

        void start(GT.Timer timer)
        {
            Debug.Print("timer start");
            t.Stop();
            button.ButtonPressed += button_ButtonPressed;
            blueT.DeviceInquired += blueT_DeviceInquired;
            blueT.DataReceived += blueT_DataReceived;
            blueT.BluetoothStateChanged += blueT_BluetoothStateChanged;
        }

        private void blueT_BluetoothStateChanged(GT.Bluetooth sender, GT.Bluetooth.BluetoothState btState)
        {
            Debug.Print(btState.ToString());
        }

        void blueT_DeviceInquired(GT.Bluetooth sender, string macAddress, string name)
        {
            Debug.Print("inquired");
            if (name.IndexOf("HomeSecurity") > -1)
            {
                blueT.HostMode.InputPinCode("1234");
                Debug.Print("Conectado a un dispositivo!!!");
                blueT.HostMode.Connect(macAddress);
            }
        }

        void blueT_DataReceived(GT.Bluetooth sender, string data)
        {
            if (sending)
                return;
            if (data.Length < 1000)
                return;
            string[] array = data.Split("\n".ToCharArray());
            int index = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Length > 1000)
                    index = i;
            }
            array[index] = array[index].TrimStart("#".ToCharArray());
            sending = true;
            Debug.Print("foto bloetooth");

            byte[] picbyte = null;
            if (array[index].IndexOf("pic") >= 0)
            {
                string pic = array[index].Substring(array[index].IndexOf("pic") + 3).Trim('\r').Trim('\n');

                /*string len = pic.Substring(0, pic.IndexOf("pic"));
                pic = pic.Substring(pic.IndexOf("pic") + 3);
                int length = Int32.Parse(len);

                byte[] input = Convert.FromBase64String(pic);
                byte[] output = new byte[input.Length * 2];

                int result = lzf.Decompress(input, length, output, output.Length);
                picbyte = new byte[result];
                for (int i = 0; i < result; i++)
                    picbyte[i] = output[i];*/

                picbyte = Convert.FromBase64String(pic);
            }

            Bitmap bmp = new Bitmap(picbyte, Bitmap.BitmapImageType.Jpeg);

            GT.Picture pic2 = new GT.Picture(bmp.GetBitmap(), GT.Picture.PictureEncoding.BMP);

            wifi.SendPictureData(pic2.PictureData, PhotoType.Devices.ToString());
            sending = false;
        }

        void button_ButtonPressed(GTM.Button sender, GTM.Button.ButtonState state)
        {
            blueT.HostMode.InquireDevice();
            Debug.Print("boton");
            
        }
    }
}
