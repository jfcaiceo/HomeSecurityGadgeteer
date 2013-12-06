using System;
using Microsoft.SPOT;

namespace GadgeteerApp3
{
    public class Scheduler
    {
        static Scheduler scheduler;
        bool activate = false;
        bool workingForGas = false;

        public static Scheduler Instance()
        {
            if (scheduler == null)
                scheduler = new Scheduler();
            return scheduler;
        }

        public bool canContinue()
        {
            if (!activate)
                return false;
            return true;
        }

        public bool gasContinue()
        {
            if (!activate)
                return false;
            if (workingForGas)
                return false;
            return true;
        }

        public bool Working
        {
            get { return workingForGas; }
            set { workingForGas = value; }
        }

        public bool getActivation()
        {
            return activate;
        }

        public void setActivation(bool activation)
        {
            activate = activation;
        }
    }
}
