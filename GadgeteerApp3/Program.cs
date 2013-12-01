using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp3
{
    public partial class Program
    {
        const int waitTime = 30000;
        Wifi w;
        

        void ProgramStarted()
        {
            w = new Wifi(wifi_RS21);
            CameraClass c = new CameraClass(camera, motion_Sensor, w);
            Devices d = new Devices(button, w);
            
            //Gas g = new Gas(gasSense, w);            
            Debug.Print("Program Started");
            button.ButtonPressed += button_ButtonPressed;
        }

        void button_ButtonPressed(GTM.Button sender, GTM.Button.ButtonState state)
        {Debug.Print("boton apretado program");
            if (CameraClass.getActivation())
            {
                CameraClass.setActivation(false);
                sender.LEDMode = Button.LEDModes.Off;
            }
            else
            {
                sender.LEDMode = Button.LEDModes.On;
                Thread.Sleep(waitTime);
                CameraClass.setActivation(true);
            }
        }
    }
}
