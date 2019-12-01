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
        Brush brush = new SolidBrush(Color.FromArgb(255, 255, 0, 255));

        public Chart2(ComplexNum[] fourierSamples, Chart chart2)
        {
            this.fourierSamples = fourierSamples;
            this.frequencies = new int[fourierSamples.Length];
            this.amplitudes = new double[fourierSamples.Length];
            this.nyquistLimit = this.frequencies.Length / 2;
            this.chart2 = chart2;
            this.fHighBucket = fourierSamples.Length;
            this.fLowBucket = 0;
        }

        public void setupChart()
        {
            for (int i = 0; i < fourierSamples.Length; i++)
            {
                this.frequencies[i] = i;
                this.amplitudes[i] = fourierSamples[i].Re > 0 ? Math.Sqrt(Math.Pow(fourierSamples[i].Re, 2) + Math.Pow(fourierSamples[i].Im, 2)) : 0;
            }
            drawChart(chart2);
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
            barSeries.Color = Color.LightBlue;
            //Add the series to the chart
            chart2.Series.Add(barSeries);
            var chartArea = chart2.ChartAreas[0];
            chartArea.BackColor = Color.Black;
            chartArea.CursorX.IsUserSelectionEnabled = false;
            chartArea.CursorX.IsUserEnabled = false;
            chartArea.AxisY.Title = "Amplitude";
            chartArea.AxisX.Title = "Frequency Bin #";
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
                        this.filter[i].Re = 1.0;
                        this.filter[i].Im = 1.0;
                        if (aliasIndex < this.filter.Length)
                        {
                            this.filter[aliasIndex] = new ComplexNum();
                            this.filter[aliasIndex].Re = 1.0;
                            this.filter[aliasIndex].Im = 1.0;
                        }
                    }
                    else
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
                }
            }
            this.fHighBucket = fourierSamples.Length;
            this.fLowBucket = 0;
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
            int pX2 = 0;

            double yTop = chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.Minimum);
            double yBottom = chartArea.AxisY.ValueToPixelPosition(chartArea.AxisY.Maximum);
            double xMin = chartArea.AxisX.ValueToPixelPosition(chartArea.AxisX.Minimum);
            double xMax = chartArea.AxisX.ValueToPixelPosition(chartArea.AxisX.Maximum);
            //Debug.WriteLine(xMin);
            //Debug.WriteLine(pX);
            if(pX < xMin)
            {
                pX = (int)xMin;
            }
            r1.Y = (int)yBottom + 1;
            r1.Height = (int)(yTop - yBottom);
            r1.X = pX;//+ (int)chartArea.InnerPlotPosition.X;
            r1.Width = pX;
            r2.X = (int)xMax - pX + (int)xMin;
            r2.Y = (int)yBottom + 1;
            r2.Width = pX2;
            r2.Height = (int)(yTop - yBottom);

            //dc.FillRectangle(brush, r2);
        }
        private void Chart2_MouseUp(object sender, EventArgs e)
        {
            this.chartClicked = false;
        }
        private void Chart2_MouseMove(object sender, EventArgs e)
        {
            if (this.chartClicked)
            {
                MouseEventArgs me = (MouseEventArgs)e;
                var chart = (Chart)sender;

                var chartArea = chart.ChartAreas[0];
                chartArea.CursorX.SetCursorPixelPosition(new Point(me.X, me.Y), true);
                int pX = (int)me.X;
                if (pX > (chartArea.AxisX.ValueToPixelPosition(this.nyquistLimit)))
                {
                    if (r1.X > (chartArea.AxisX.ValueToPixelPosition(this.nyquistLimit)))
                    {
                        return;
                    }
                    pX = (int)chartArea.AxisX.ValueToPixelPosition(this.nyquistLimit);
                }
                
                r1.Width = pX - r1.X;
                fLowBucket = (int)chartArea.AxisX.PixelPositionToValue(r1.X);
                fHighBucket = ((int)chartArea.AxisX.PixelPositionToValue(r1.Width + r1.X));
                if(fHighBucket >= this.nyquistLimit)
                {
                    r1.Width = (int)(chartArea.AxisX.ValueToPixelPosition(this.nyquistLimit) - r1.X);
                }
                r2.X = r2.Right - r1.Width;
                r2.Width = r1.Width;
                dc.FillRectangle(brush, r1);
                dc.FillRectangle(brush, r2);

            }
        }


    }
}
