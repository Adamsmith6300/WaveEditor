using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
namespace WaveVisualizer
{
    public partial class Form1 : Form
    {

        static int numOfSamples = 44100*180;
        static int[] rawSamples;
        static int sampleRate = 44100;
        static int maxSamplesPerSecond = 100;
        static int initialSamplesPerSecond = 1;

        int selStart = 0;
        int selEnd = numOfSamples;
        int sel = numOfSamples;


        public Form1()
        {
            InitializeComponent();
            setData();
            newChart();
        }
        private void setData()
        {
            //generates random data to be removed when we setup opening wave file
            Random rand = new Random();
            rawSamples = Enumerable.Range(0, numOfSamples).Select(x => rand.Next(1, 6) - 3).ToArray();
            ///////////////////

        }

        private void newChart()
        {
            /*********************
             * START Initial setup
             ********************/
            // clear the chart series points
            chart1.Series.Clear();
            var series = chart1.Series.Add("My Series");
            series.ChartType = SeriesChartType.Spline;
            //series.XValueType = ChartValueType.Double;
            var chartArea = chart1.ChartAreas[series.ChartArea];
            chartArea.AxisX.LabelStyle.Format = "#.###";
            //remove grid lines
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            //axis labels
            //chartArea.AxisX.Title = "Time (s)";
            //chartArea.AxisY.Title = "Frequency";
            chartArea.BorderWidth = 1;
            //allow zooming
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            //minmum zoom to 10
            chartArea.AxisX.ScaleView.MinSize = 1/sampleRate;
            // set view range to [0,max]
            chartArea.AxisX.ScaleView.Zoom(0, rawSamples.Length/sampleRate);
            chartArea.AxisX.ScaleView.Position = 0.00;
            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = rawSamples.Length / sampleRate;
            //reset zoom button
            //chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            // enable autoscroll
            chartArea.CursorX.AutoScroll = true;
            chartArea.CursorX.Interval = 0.01;
            chartArea.CursorX.IsUserSelectionEnabled = true;

            int seconds = rawSamples.Length / sampleRate;
            //plot chart points
            int samplesPerSecond = initialSamplesPerSecond;

            //plot chart points
            for (int i = 0; i < seconds; i++)
            {
                if(samplesPerSecond == 1)
                {
                    chart1.Series["My Series"].Points.AddXY(i, rawSamples[i * sampleRate]);
                } else
                {
                    for (int j = 0; j < samplesPerSecond; j++)
                    {
                        chart1.Series["My Series"].Points.AddXY(((double)j / samplesPerSecond) + (double)i, rawSamples[i * j]);
                    }
                }
                
            }
            /*******************
             * END Initial setup
             ******************/
        }
        private void refreshChart()
        {
            foreach (var ser in chart1.Series)
            {
                ser.Points.Clear();
            }
            int seconds = rawSamples.Length / sampleRate;
            /*
             * once zoomed into 10 seconds or less
             * we start adding data points back into graph up to maxSamplesPerSecond
            */
            int samplesPerSecond = Math.Max(1,(int)(((double)(rawSamples.Length - sel) / rawSamples.Length) * maxSamplesPerSecond));
            if(sel/sampleRate > 10)
            {
                samplesPerSecond = initialSamplesPerSecond;
            }
            //plot chart points
            for (int i = 0; i < seconds; i++)
            {
                if (samplesPerSecond == 1)
                {
                    chart1.Series["My Series"].Points.AddXY(i, rawSamples[i * sampleRate]);
                }
                else
                {
                    for (int j = 0; j < samplesPerSecond; j++)
                    {
                        chart1.Series["My Series"].Points.AddXY(((double)j / samplesPerSecond) + (double)i, rawSamples[i * j]);
                    }
                }

                    
            }
        }
        private void ZoomOutFull()
        {
            sel = numOfSamples;
            selStart = 0;
            selEnd = numOfSamples;
            newChart();
        }



        //event Handlers
        private void Button1_Click(object sender, EventArgs e)
        {
        }
        private void Chart1_MouseDown(object sender, MouseEventArgs me)
        {
           
        }
            private void Chart1_Click(object sender, EventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == MouseButtons.Right)
            {
                ZoomOutFull();
            }
            double newSelStart = chart1.ChartAreas["ChartArea1"].CursorX.SelectionStart;
            double newSelEnd = chart1.ChartAreas["ChartArea1"].CursorX.SelectionEnd;
            if (Math.Abs(selEnd - newSelEnd) > 0 || Math.Abs(newSelStart - selStart) > 0)
            {
                selStart = (int)newSelStart * sampleRate;
                selEnd = (int)newSelEnd * sampleRate;
                Debug.WriteLine(selStart);
                Debug.WriteLine(selEnd);
                int newSel = Math.Abs(selStart - selEnd);
                if(newSel > chart1.ChartAreas["ChartArea1"].AxisX.ScaleView.Size * sampleRate)
                {
                    ZoomOutFull();
                }
                if (newSel > 0)
                {
                    sel = Math.Abs(selEnd - selStart);
                    refreshChart();
                }
                
            }
            
        }

    }
}
