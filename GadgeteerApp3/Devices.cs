using System;
using Microsoft.SPOT;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp3
{
    public class Devices
    {
        Gadgeteer.Bluetooth blueT;
        GT.Timer t;
        Button button;
        Wifi wifi;
        const string bluetoothMessage = "accel";

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
            if (!Scheduler.Instance().canContinue())
                return;
            int index = data.IndexOf(bluetoothMessage);
            if (index > -1)
            {
                wifi.SendData("device", data.Substring(index + bluetoothMessage.Length, data.Length - (index + bluetoothMessage.Length + 1)), "sendMovement");
            }
        }

        void button_ButtonPressed(GTM.Button sender, GTM.Button.ButtonState state)
        {
            blueT.HostMode.InquireDevice();
            Debug.Print("boton");
            
        }
    }
}
