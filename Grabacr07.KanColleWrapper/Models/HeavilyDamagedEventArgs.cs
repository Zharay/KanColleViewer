using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grabacr07.KanColleWrapper.Models
{
    public class HeavilyDamagedEventArgs : EventArgs
    {
        public string FleetName { get; private set; }

        public HeavilyDamagedEventArgs(string fleetName)
        {
            this.FleetName = fleetName;
        }
    }
}