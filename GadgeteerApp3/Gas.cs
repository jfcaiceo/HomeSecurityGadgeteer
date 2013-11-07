using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp3
{
    class Gas
    {
        private GasSense gs;
        private GT.Timer _getReading = new GT.Timer(20000);
        private GT.Timer _preheat = new GT.Timer(15000);        
        public Gas(GasSense g){
            this.gs = g;            
            _getReading.Tick += new GT.Timer.TickEventHandler(_getReading_Tick);
            _preheat.Tick += new GT.Timer.TickEventHandler(_preheat_Tick);
            _getReading.Start();
        }

          void _getReading_Tick(GT.Timer timer)
        {
            gs.SetHeatingElement(true);
            _preheat.Start();
            Debug.Print("getReading.");
        }

          void _preheat_Tick(GT.Timer timer)
          {
              _preheat.Stop();
                      
              double gs1 = gs.ReadVoltage();
              if (gs1 > 2.5)
              {
                  gasDetected(gs1);
              }
            
              //Debug.Print("GS1 = " + gs1.ToString());
              //CameraClass.displayText("GS1 = " + gs1.ToString());
               /*
               * Normally I would unncomment this and save the power between readings
               * but I have seen people comment that these sensors need an initial burn-in
               * of about 24-48 before readings stabilize.  This is a one time thing
               * and once they are burned in (cleans up stuff from manufacturing), then
               * you only need about 10 seconds of preheat to get a valid reading.
               * */
              //gasSense.SetHeatingElement(false);
              
              
          }

      

          public void gasDetected(double j)
          {
              Debug.Print("Niveles de gases peligrosos");
              CameraClass.displayText("Niveles de gases peligrosos");
          }
    }

}
