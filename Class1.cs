using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVisualizer
{
    [Serializable]
    public class ClipboardData : Object
    {
        uint sr; 
        short[] cs; 
        public ClipboardData(){}

        public uint Sr { get; set; }
        public short[] Cs { get; set; }
    }
}
