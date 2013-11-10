using System;
using Microsoft.SPOT;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp3
{
    public class CameraClass
    {
        private static Gadgeteer.Modules.GHIElectronics.Display_T35 display_T35;

        private GT.Picture pictureForService;
        private Gadgeteer.Modules.GHIElectronics.Camera camera;
        private Motion_Sensor motion_Sensor;
        private Wifi wifi;
        private static bool activate = true;
        private bool sendingPicture = false;

        public CameraClass(Display_T35 display_T35, Gadgeteer.Modules.GHIElectronics.Camera camera, Motion_Sensor motion_Sensor, Wifi wf)
        {
            this.wifi = wf;
            this.camera = camera;
            this.camera.CurrentPictureResolution = Camera.PictureResolution.Resolution160x120;
            CameraClass.display_T35 = display_T35;
            this.motion_Sensor = motion_Sensor;
            this.motion_Sensor.Motion_Sensed += new GTM.Motion_Sensor.Motion_SensorEventHandler(motion_Sensor_Motion_Sensed);
            this.camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);
            this.camera.DebugPrintEnabled = false;
        }

        public static void activateSystem()
        {
            activate = true;
        }

        public static void inactivateSystem()
        {
            activate = false;
        }

        public static void displayText(string text)
        {
            display_T35.SimpleGraphics.Clear();
            
            display_T35.SimpleGraphics.DisplayText(text, Resources.GetFont(Resources.FontResources.NinaB), GT.Color.White, 0, 0);
        }

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            display_T35.SimpleGraphics.DisplayImage(picture, 0, 0);
            wifi.SendPictureData(picture);
            Debug.Print("foto");
            sendingPicture = false;
        }

        void motion_Sensor_Motion_Sensed(GTM.Motion_Sensor sender, GTM.Motion_Sensor.Motion_SensorState state)
        {
            if (!activate)
                return;
            if (sendingPicture)
                return;
            sendingPicture = true;
            Debug.Print("nuevo motion sensor");
            camera.TakePicture();
        }
    }
}
