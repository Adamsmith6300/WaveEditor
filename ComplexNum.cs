using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVisualizer
{
    class ComplexNum
    {
        double re = 0.00;
        double im = 0.00; 

        public ComplexNum() {}
        public ComplexNum(double nre, double nim) {
            re = nre;
            im = nim;
            //Debug.WriteLine(this.re);
        }
        public double Re { get; set; }
        public double Im { get; set; }
    }
}
