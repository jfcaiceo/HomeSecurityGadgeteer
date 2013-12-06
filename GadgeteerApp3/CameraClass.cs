using System;
using Microsoft.SPOT;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp3
{
    public enum PhotoType { Central, Devices };

    public class CameraClass
    {
        //private static Gadgeteer.Modules.GHIElectronics.Display_T35 display_T35;

        private GT.Picture pictureForService;
        private Gadgeteer.Modules.GHIElectronics.Camera camera;
        private Motion_Sensor motion_Sensor;
        private Wifi wifi;
        private bool sendingPicture = false;
        GT.Timer timer;
        const int sleepTime = 5000;

        public CameraClass(Gadgeteer.Modules.GHIElectronics.Camera camera, Motion_Sensor motion_Sensor, Wifi wf)
        {
            this.wifi = wf;
            this.camera = camera;
            this.camera.CurrentPictureResolution = Camera.PictureResolution.Resolution160x120;
            this.motion_Sensor = motion_Sensor;
            this.motion_Sensor.Motion_Sensed += new GTM.Motion_Sensor.Motion_SensorEventHandler(motion_Sensor_Motion_Sensed);
            this.camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);
            this.camera.DebugPrintEnabled = false;

            timer = new GT.Timer(sleepTime);
            timer.Tick += timer_Tick;
        }

        void timer_Tick(GT.Timer timer)
        {
            timer.Stop();
        }

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            wifi.SendPictureData(picture.PictureData, PhotoType.Central.ToString());
            Debug.Print("foto");
            sendingPicture = false;
            Scheduler.Instance().Working = false;
        }

        void motion_Sensor_Motion_Sensed(GTM.Motion_Sensor sender, GTM.Motion_Sensor.Motion_SensorState state)
        {
            if (!Scheduler.Instance().canContinue())
                return;
            if (sendingPicture)
                return;
            if (timer.IsRunning)
                return;
            timer.Start();

            sendingPicture = true;
            Scheduler.Instance().Working = true;
            Debug.Print("nuevo motion sensor");
            camera.TakePicture();
        }
    }
}
