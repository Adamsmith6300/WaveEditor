using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVisualizer
{

    /// <summary>represents a complex number</summary>
    class ComplexNum
    {
        double re = 0.00;
        double im = 0.00;

        /// <summary>Initializes a new instance of the <see cref="ComplexNum"/> class.</summary>
        public ComplexNum() {}

        /// <summary>Initializes a new instance of the <see cref="ComplexNum"/> class.</summary>
        /// <param name="nre">The new real value</param>
        /// <param name="nim">The new imaginary value</param>
        public ComplexNum(double nre, double nim) {
            re = nre;
            im = nim;
        }

        /// <summary>Gets or sets the re.</summary>
        /// <value>The real component of the complex number</value>
        public double Re { get; set; }

        /// <summary>Gets or sets the im.</summary>
        /// <value>The imaginary component of the complex number</value>
        public double Im { get; set; }
    }
}
