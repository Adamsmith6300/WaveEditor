using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVisualizer
{

    /// <summary>represents clipboard data samples</summary>
    [Serializable]
    public class ClipboardData : Object
    {
        uint sr;
        double[] cs;

        /// <summary>Initializes a new instance of the <see cref="ClipboardData"/> class.</summary>
        public ClipboardData(){}

        /// <summary>Gets or sets the sample rate.</summary>
        /// <value>The sample rate</value>
        public uint Sr { get; set; }

        /// <summary>Gets or sets the cutSamples array</summary>
        /// <value>The cutSamples</value>
        public double[] Cs { get; set; }
    }
}
