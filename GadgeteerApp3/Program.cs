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
        void ProgramStarted()
        {
            Wifi w = new Wifi(wifi_RS21);
            CameraClass c = new CameraClass(display_T35, camera, motion_Sensor, w);
            //Gas g = new Gas(gasSense, w);            
            Debug.Print("Program Started");
        }
    }
}
