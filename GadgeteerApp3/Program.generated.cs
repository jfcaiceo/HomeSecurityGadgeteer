//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.18052
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GadgeteerApp3 {
    using Gadgeteer;
    using GTM = Gadgeteer.Modules;
    
    
    public partial class Program : Gadgeteer.Program {
        
        /// <summary>The Motion_Sensor module using socket 11 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Motion_Sensor motion_Sensor;
        
        /// <summary>The UsbClientDP module using socket 1 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.UsbClientDP usbClientDP;
        
        /// <summary>The Camera (Premium) module using socket 3 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Camera camera;
        
        /// <summary>The WiFi_RS21 (Premium) module using socket 6 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.WiFi_RS21 wifi_RS21;
        
        /// <summary>The GasSense module using socket 9 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.GasSense gasSense;
        
        /// <summary>The Button module using socket 5 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Button button;
        
        /// <summary>The SerCam module (not connected).</summary>
        private Gadgeteer.Modules.GHIElectronics.SerCam serCam;
        
        /// <summary>This property provides access to the Mainboard API. This is normally not necessary for an end user program.</summary>
        protected new static GHIElectronics.Gadgeteer.FEZSpider Mainboard {
            get {
                return ((GHIElectronics.Gadgeteer.FEZSpider)(Gadgeteer.Program.Mainboard));
            }
            set {
                Gadgeteer.Program.Mainboard = value;
            }
        }
        
        /// <summary>This method runs automatically when the device is powered, and calls ProgramStarted.</summary>
        public static void Main() {
            // Important to initialize the Mainboard first
            Program.Mainboard = new GHIElectronics.Gadgeteer.FEZSpider();
            Program p = new Program();
            p.InitializeModules();
            p.ProgramStarted();
            // Starts Dispatcher
            p.Run();
        }
        
        private void InitializeModules() {
            this.motion_Sensor = new GTM.GHIElectronics.Motion_Sensor(11);
            this.usbClientDP = new GTM.GHIElectronics.UsbClientDP(1);
            this.camera = new GTM.GHIElectronics.Camera(3);
            this.wifi_RS21 = new GTM.GHIElectronics.WiFi_RS21(6);
            this.gasSense = new GTM.GHIElectronics.GasSense(9);
            this.button = new GTM.GHIElectronics.Button(5);
            Microsoft.SPOT.Debug.Print("The module \'serCam\' was not connected in the designer and will be null.");
        }
    }
}
