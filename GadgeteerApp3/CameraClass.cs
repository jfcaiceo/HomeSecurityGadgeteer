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
        private static GT.Picture pictureForService;
        private static Gadgeteer.Modules.GHIElectronics.Display_T35 display_T35;
        public static Gadgeteer.Modules.GHIElectronics.Camera camera;
        private Motion_Sensor motion_Sensor;
        private Wifi wifi;
        private static bool activate = false;

        public CameraClass(Display_T35 display_T35, Gadgeteer.Modules.GHIElectronics.Camera camera, Motion_Sensor motion_Sensor, Wifi wf)
        {
            this.wifi = wf;
            CameraClass.camera = camera;
            CameraClass.camera.CurrentPictureResolution = Camera.PictureResolution.Resolution160x120;
            CameraClass.display_T35 = display_T35;
            this.motion_Sensor = motion_Sensor;
            this.motion_Sensor.Motion_Sensed += new GTM.Motion_Sensor.Motion_SensorEventHandler(motion_Sensor_Motion_Sensed);
            CameraClass.camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);
            CameraClass.camera.DebugPrintEnabled = false;


            GT.Timer myTimer = new GT.Timer(1000);
            myTimer.Tick += new GT.Timer.TickEventHandler(timerTick);
        }

        public static GT.Picture getLastPicure()
        {
            return pictureForService;
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

        uint time = 0;

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            Debug.Print("Picture displayed!");
            display_T35.SimpleGraphics.DisplayImage(picture, 0, 0);
            CameraClass.pictureForService = picture;
        }

        void motion_Sensor_Motion_Sensed(GTM.Motion_Sensor sender, GTM.Motion_Sensor.Motion_SensorState state)
        {
            if (!activate)
                return;

            time = 0;
            camera.TakePicture();
            wifi.SendPictureData();
            
        }

        void timerTick(GT.Timer timer)
        {
            timer.Stop();

            time++;
            if (time >= 6)
                time = 1;

            Debug.Print("Timer invoked at " + time);

            if (time == 5)
            {
                Debug.Print("Display cleared!");
                display_T35.SimpleGraphics.Clear();
            }

            timer.Start();
        }
    }
}
