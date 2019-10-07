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
        int sampleRate = 44100;
        static int maxSamplesPerSecond = 100;
        int maxChartValY = 100000;
        int minChartValY = -100000;

        int selStart = 0;
        int selEnd = 0;
        int sel = 0;


        ComplexNum[] fourierSamples;
        private string filename;
        private RWWaveFile waveFile;

        public Form1()
        {
            InitializeComponent();
        }

        private void newChart()
        {
            /*********************
             * START Initial setup
             ********************/
            short[] rawSamples = waveFile.DataChunk1.Data;
            uint sampleRate = waveFile.FmtChunk1.SamplesPerSec;
            uint bytesPerSec = waveFile.FmtChunk1.AverageBytesPerSec;
            uint bitsPerSample = waveFile.FmtChunk1.BitsPerSample;
            uint dataSize = bitsPerSample == 8 ? (uint)rawSamples.Length : waveFile.DataChunk1.DataSize;
            ushort numChannels = waveFile.FmtChunk1.Channels;

            // clear the chart series points
            chart1.Series.Clear();
            //Debug.WriteLine(rawSamples.Length);
            var series = chart1.Series.Add("My Series");
            var series2 = chart1.Series.Add("My Series2");
            series2.Color = Color.Red;
            series.ChartType = SeriesChartType.Spline;
            series2.ChartType = SeriesChartType.Spline;
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
            chartArea.AxisX.ScaleView.Zoomable = false;
            //chartArea.AxisY.ScaleView.Zoomable = true;
            chart1.MouseWheel += chart1_MouseWheel;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            //minmum zoom to 10
            chartArea.AxisX.ScaleView.MinSize = 0.01;
            // set view range to [0,max]
            chartArea.AxisX.ScaleView.Zoom(1, dataSize/ bytesPerSec);
            chartArea.AxisX.ScaleView.Position = 1.00;
            chartArea.AxisX.Minimum = 0.00;
            chartArea.AxisX.Maximum = dataSize/ bytesPerSec;
            //chartArea.AxisY.Maximum = 100000;
            //chartArea.AxisY.Minimum = -100000;
            //reset zoom button
            //chartArea.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            // enable autoscroll
            //chartArea.CursorX.AutoScroll = true;
            chartArea.CursorX.Interval = 0.01;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            refreshChart();
            /*******************
             * END Initial setup
             ******************/
        }
        private void refreshChart()
        {

            short[] rawSamples = waveFile.DataChunk1.Data;
            uint sampleRate = waveFile.FmtChunk1.SamplesPerSec;
            uint bytesPerSec = waveFile.FmtChunk1.AverageBytesPerSec;
            uint bitsPerSample = waveFile.FmtChunk1.BitsPerSample;
            uint dataSize = bitsPerSample == 8 ? (uint) rawSamples.Length : waveFile.DataChunk1.DataSize;
            ushort numChannels = waveFile.FmtChunk1.Channels;

            foreach (var ser in chart1.Series)
            {
                ser.Points.Clear();
            }
            int seconds = (int)dataSize / (int)bytesPerSec;
            int samplesPerSecond = (int)sampleRate * 2;
            if (bitsPerSample == 8)
            {
                samplesPerSecond = (int)sampleRate;
            }
            Debug.WriteLine(seconds);
            //plot chart points
            if (numChannels == 2)
            {
                int i = 0, j = 0;
                for (i = 0; i < seconds; i++)
                {
                    if (samplesPerSecond == 1)
                    {
                        chart1.Series["My Series"].Points.AddXY(i, rawSamples[i * bytesPerSec]);
                        chart1.Series["My Series2"].Points.AddXY(i, rawSamples[i * bytesPerSec + 1]);
                    }
                    else
                    {
                        for (j = 0; j < samplesPerSecond; j+=samplesPerSecond/100)
                        {
                            double sample = rawSamples[(i*samplesPerSecond) + j];
                            double sample2 = rawSamples[(i*samplesPerSecond) + j + 1];
                            //if (sample > 0 || sample2 > 0)
                            //{
                            //    sample = sample > maxChartValY ? maxChartValY : sample;
                            //    sample2 = sample2 > maxChartValY ? maxChartValY : sample2;
                            //}
                            //else
                            //{
                            //    sample = sample < minChartValY ? minChartValY : sample;
                            //    sample2 = sample2 < minChartValY ? minChartValY : sample2;
                            //}
                            chart1.Series["My Series"].Points.AddXY((double)i+((double)j/samplesPerSecond), sample);
                            chart1.Series["My Series2"].Points.AddXY((double)i+((double)j/samplesPerSecond), sample2);

                        }
                        if (i == seconds - 1)
                        {
                            //Debug.WriteLine(rawSamples[i * j]);
                            //Debug.WriteLine(rawSamples[i * j + 1]);
                        }
                    }

                }
                    //Debug.WriteLine(rawSamples[i * j]);
                    //Debug.WriteLine(rawSamples[i * j + 1]);
            }
            else
            {
                for (int i = 0; i < seconds; i++)
                {
                    if (samplesPerSecond == 1)
                    {
                        chart1.Series["My Series"].Points.AddXY(i, rawSamples[i * bytesPerSec]);
                    }
                    else
                    {
                        for (int j = 0; j < samplesPerSecond; j += samplesPerSecond / 100)
                        {
                            double sample = rawSamples[(i * samplesPerSecond) + j];
                            if (sample > 0)
                            {
                                sample = sample > maxChartValY ? maxChartValY : sample;
                            }
                            else
                            {
                                sample = sample < minChartValY ? minChartValY : sample;
                            }
                            chart1.Series["My Series"].Points.AddXY((double)i + ((double)j / samplesPerSecond), sample);
                        }
                    }

                }
            }
        }
      
        
        private void dft(int start, int end)
        {
            int N = end - start;
            //int[] samples = rawSamples[start, end];
            for (int f = 0; f < N; f++)
            {
                fourierSamples[f] = new ComplexNum();
                for (int t = 0; t < N; t++)
                {
                    //ComplexNum getters/setters needed
                    //fourierSamples[f].re += rawSamples[start + t] * Math.Cos((2 * Math.PI * t * f) / N);
                    //fourierSamples[f].im -= rawSamples[start + t] * Math.Sin((2 * Math.PI * t * f) / N);
                }
                //ComplexNum getters/setters needed
                //fourierSamples[f].re = fourierSamples[f].re / N;
                //fourierSamples[f].im = fourierSamples[f].im / N;
            }
            ///setup chart with fourier samples
        }

        //event Handlers
        private void Button1_Click(object sender, EventArgs e)
        {
        }
        private void Chart1_MouseDown(object sender, MouseEventArgs me)
        {

        }
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;

            try
            {
                if (e.Delta < 0) // Scrolled down.
                {

                    //ZoomOutFull();
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) * 2.00;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) * 2.00;
                    selStart = (int)posXStart * sampleRate;
                    selEnd = (int)posXFinish * sampleRate;
                    sel = Math.Abs(((int)posXStart * sampleRate) - ((int)posXFinish * sampleRate));
                    refreshChart();
                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                    //xAxis.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0) // Scrolled up.
                {
                    var xMin = xAxis.ScaleView.ViewMinimum;
                    var xMax = xAxis.ScaleView.ViewMaximum;
                    var posXStart = xAxis.PixelPositionToValue(e.Location.X) - (xMax - xMin) / 2.00;
                    var posXFinish = xAxis.PixelPositionToValue(e.Location.X) + (xMax - xMin) / 2.00;
                    selStart = (int)posXStart * sampleRate;
                    selEnd = (int)posXFinish * sampleRate;
                    sel = Math.Abs(((int)posXStart * sampleRate) - ((int)posXFinish * sampleRate));
                    refreshChart();
                    xAxis.ScaleView.Zoom(posXStart, posXFinish);
                }
            }
            catch { }
        }
        private void Chart1_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            var posXStart = xAxis.PixelPositionToValue(me.Location.X);
            var posXFinish = xAxis.PixelPositionToValue(me.Location.X);
            dft((int)posXStart, (int)posXFinish);

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();

            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                filename = fileDlg.FileName;
                waveFile = new RWWaveFile(filename);
                sel = (int) waveFile.DataChunk1.DataSize;
                selEnd = (int) waveFile.DataChunk1.DataSize;
                newChart();
                //Debug.WriteLine(waveFile.DataChunk1.Data[waveFile.DataChunk1.Data.Length-32000]);
                //Debug.WriteLine(waveFile.DataChunk1.Data[10*32000]);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "wav (*.wav)|*.wav|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Wav File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                filename = saveFileDialog1.FileName;
                waveFile.Write(filename);
            }
        }
    }
}
