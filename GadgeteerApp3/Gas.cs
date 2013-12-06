using System;
using Microsoft.SPOT;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp3
{
    public class Gas
    {
        const double maxGas = 2.5;

        private GasSense gs;
        private Wifi wifi;
        private GT.Timer _getReading = new GT.Timer(20000);
        private GT.Timer _preheat = new GT.Timer(15000);
        public Gas(GasSense g, Wifi wifi)
        {
            this.wifi = wifi;
            this.gs = g;
            _getReading.Tick += new GT.Timer.TickEventHandler(_getReading_Tick);
            _preheat.Tick += new GT.Timer.TickEventHandler(_preheat_Tick);
            _getReading.Start();
        }

        void _getReading_Tick(GT.Timer timer)
        {
            if (!Scheduler.Instance().canContinue())
                return;
            gs.SetHeatingElement(true);
            _preheat.Start();
        }

        void _preheat_Tick(GT.Timer timer)
        {
            _preheat.Stop();

            double gs1 = gs.ReadVoltage();
            wifi.SendGasData(gs1);
            if (gs1 > maxGas)
            {
                gasDetected(gs1);
            }

        }

        public void gasDetected(double j)
        {
            Debug.Print("Niveles de gases peligrosos");
        }
    }

}
