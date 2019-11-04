using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WaveVisualizer
{
    class Chart2
    {
        private ComplexNum[] fourierSamples;
        private int[] frequencies;
        private double[] amplitudes;
        private Chart chart2;
        bool chartClicked = false;
        private Rectangle r1;
        private Rectangle r2;
        private int fLowBucket;
        private int fHighBucket;
        private int nyquistLimit;
        private ComplexNum[] filter;
        Graphics dc;
        Brush brush = new SolidBrush(Color.FromArgb(70, 200, 20, 20));

        public Chart2(ComplexNum[] fourierSamples, Chart chart2)
        {
            this.fourierSamples = fourierSamples;
            this.frequencies = new int[fourierSamples.Length];
            this.amplitudes = new double[fourierSamples.Length];

            //Debug.WriteLine(fourierSamples.Length);
            for (int i = 0; i < fourierSamples.Length; i++)
            {
                this.frequencies[i] = i;
                this.amplitudes[i] = fourierSamples[i].Re > 0 ? fourierSamples[i].Re:0;
                //Debug.WriteLine(this.frequencies[i] + "---" + this.amplitudes[i]);
            }
            this.nyquistLimit = this.frequencies.Length / 2;
            this.chart2 = chart2;
            drawChart(chart2);

            //typeof(Chart).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, this.chart2, new object[] { true });
            this.chart2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Chart2_MouseDown);
            this.chart2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Chart2_MouseUp);
            this.chart2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart2_MouseMove);
            dc = this.chart2.CreateGraphics();
        }

        private void drawChart(Chart chart2)
        {
            chart2.Series.Clear();
            //Create a series using the data
            Series barSeries = new Series();
            barSeries.Points.DataBindXY(this.frequencies, this.amplitudes);
            //Set the chart type, Column;
            barSeries.ChartType = SeriesChartType.Column;
            //Assign it to the required area
            //barSeries.ChartArea = "First";
            //Add the series to the chart
            chart2.Series.Add(barSeries);
            var chartArea = chart2.ChartAreas[0];

            chartArea.CursorX.IsUserSelectionEnabled = false;
            chartArea.CursorX.IsUserEnabled = false;
            //remove scrollbar
            chartArea.AxisX.ScrollBar.Enabled = false;
            //disable zooming
            chartArea.AxisX.ScaleView.Zoomable = false;
            chartArea.CursorX.LineWidth = 0;
            chartArea.CursorY.LineWidth = 0;
            r1 = new Rectangle();
            r2 = new Rectangle();
            
        }

        public ComplexNum[] generateFilter()
        {
            this.filter = new ComplexNum[this.frequencies.Length];
            if(this.filter.Length > 0)
            {
                for (int i = 0; i <= this.nyquistLimit; i++)
                {
                    int aliasIndex = this.filter.Length - i;
                    if (i >= fLowBucket && i <= fHighBucket)
                    {
                        this.filter[i] = new ComplexNum();
                        this.filter[i].Re = 0.0;
                        this.filter[i].Im = 0.0;
                        if (aliasIndex < this.filter.Length)
                        {
                            this.filter[aliasIndex] = new ComplexNum();
                            this.filter[aliasIndex].Re = 0.0;
                            this.filter[aliasIndex].Im = 0.0;
                        }
                    }
                    else
                    {
                        this.filter[i] = new ComplexNum();
                        this.filter[i].Re = 1.0;
                        this.filter[i].Im = 1.0;
                        if (aliasIndex < this.filter.Length)
                        {
                            this.filter[aliasIndex] = new ComplexNum(); 
                            this.filter[aliasIndex].Re = 1.0;
                            this.filter[aliasIndex].Im = 1.0;
                        }
                    }
                }
                //for(int j = 0; j < this.filter.Length; j++)
                //{
                //    Debug.Write("{"+this.filter[j].Re + ", "+ this.filter[j].Im+"} ");
                //}
                //Debug.WriteLine("\n^^^Filter^^^");
            }
            return this.filter;
            
        }

        private void Chart2_MouseDown(object sender, EventArgs e)
        {
            this.chartClicked = true;
            MouseEventArgs me = (MouseEventArgs)e;
            var chart = (Chart)sender;
            var chartArea = chart.ChartAreas[0];

            chartArea.CursorX.SetCursorPixelPosition(new Point(me.X, me.Y), true);
            int pX = (int)me.X;
            //chartArea.AxisX.Minimum = 0.00;
            //chartArea.AxisX.Maximum
            double yTop = chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.Minimum);
            double yBottom = chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.Maximum);
            //Debug.WriteLine(yBottom + "---" + yTop);
            r1.Y = (int)yBottom + 1;
            r1.Height = (int)(yTop - yBottom);
            r1.X = pX + (int)chartArea.InnerPlotPosition.X;
            r1.Width = pX;
            //r2.X = 100;
            //r2.Y = 100;
            //r2.Width = 200;
            //r2.Height = 300;

            //e.ChartGraphics.Graphics.DrawRectangle(new Pen(Color.Red, 3), r1);
            //e.ChartGraphics.Graphics.DrawRectangle(new Pen(Color.Black, 5), r2);
        }
        private void Chart2_MouseUp(object sender, EventArgs e)
        {
            this.chartClicked = false;
            //e.ChartGraphics.Graphics.DrawRectangle(new Pen(Color.Red, 3), r1);
            //e.ChartGraphics.Graphics.DrawRectangle(new Pen(Color.Black, 5), r2);
        }
        private void Chart2_MouseMove(object sender, EventArgs e)
        {
            if (this.chartClicked)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                var chart = (Chart)sender;

                var chartArea = chart.ChartAreas[0];
                chartArea.CursorX.SetCursorPixelPosition(new Point(me.X, me.Y), true);

                r1.Width = me.X - r1.X;
                fLowBucket = (int)chartArea.AxisX.PixelPositionToValue(r1.X);
                fHighBucket = ((int)chartArea.AxisX.PixelPositionToValue(r1.Width + r1.X));
                if(fHighBucket >= this.nyquistLimit)
                {
                    r1.Width = (int)(chartArea.AxisX.ValueToPixelPosition(this.nyquistLimit) - r1.X);
                }
                dc.FillRectangle(brush, r1);
                //draw rect backwards
                //if (me.X < r1.X)
                //{
                //    int temp = r1.X;
                //    r1.X = me.X;
                //    r1.Width = (temp + (int)chartArea.InnerPlotPosition.X) - me.X;
                //}

            }
        }


    }
}
