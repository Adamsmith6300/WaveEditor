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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;

namespace WaveVisualizer
{

    /// <summary>The main form for the application</summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class Form1 : Form
    {
        private ComplexNum[] fourierSamples;
        private ComplexNum[][] dftThreadSamples;
        private FourierChart fourierChart;
        private RWWaveFile waveFile;
        private WaveForm wf;
        double[] rawSamples;
        double[] cutSamples;
        double[] windowingSamples;
        uint sampleRate;
        uint bytesPerSec;
        uint bitsPerSample;
        ushort numChannels;
        int dataSize;

        static int maxSamples = 596;
        int selStart = 0;
        int selEnd = 0;
        int sel = 0;
        long posXStart;
        long posXFinish;
        double zeroThreshold = 0.01;
        
        private string filename;
        uint bitDepth = 16;
        int multiplier = 1;
        bool dialogOpen = false;
        bool newRecording = false;


        /// <summary>represents the data retured from recording in the dll</summary>
        public struct RecordData
        {
            public uint len;
            public IntPtr ip;
        }

        /// <summary>represents the data returned from convolution in the dll</summary>
        public struct FilterData
        {
            public uint len;
            public IntPtr ip;
            public double openCLElapsed;
        }

        /// <summary>represents the wavefile header data returned from GetWaveForm() in the dll</summary>
        public struct WaveForm
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        /// <summary>Initializes a new instance of the <see cref="Form1"/> class. Sets up some of the styling and attaches event listeners to components.</summary>
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.hScrollBar1.Visible = false;
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Chart1_MouseDown);
            this.chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Chart1_MouseUp);
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
            this.comboBox5.SelectedIndex = 0;
            this.Text = "Wave Editor";
            this.AutoSize = true;

            //start recording button styles
            this.sRec.TabStop = false;
            this.sRec.FlatStyle = FlatStyle.Flat;
            this.sRec.FlatAppearance.BorderSize = 0;
            this.sRec.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            this.sRec.ForeColor = Color.FromArgb(255, 0, 206, 209);
            //stop recording button styles
            this.stRec.TabStop = false;
            this.stRec.FlatStyle = FlatStyle.Flat;
            this.stRec.FlatAppearance.BorderSize = 0;
            this.stRec.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            this.stRec.ForeColor = Color.FromArgb(255, 0, 206, 209);
            //start play button styles
            this.sPlay.TabStop = false;
            this.sPlay.FlatStyle = FlatStyle.Flat;
            this.sPlay.FlatAppearance.BorderSize = 0;
            this.sPlay.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            this.sPlay.ForeColor = Color.FromArgb(255, 0, 206, 209);
            //stop play button styles
            this.stPlay.TabStop = false;
            this.stPlay.FlatStyle = FlatStyle.Flat;
            this.stPlay.FlatAppearance.BorderSize = 0;
            this.stPlay.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            this.stPlay.ForeColor = Color.FromArgb(255, 0, 206, 209);
            //filter button styles
            this.filterButton.TabStop = false;
            this.filterButton.FlatStyle = FlatStyle.Flat;
            this.filterButton.FlatAppearance.BorderSize = 0;
            this.filterButton.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            this.filterButton.ForeColor = Color.FromArgb(255, 0, 206, 209);
            //dft button styles
            this.dftButton.TabStop = false;
            this.dftButton.FlatStyle = FlatStyle.Flat;
            this.dftButton.FlatAppearance.BorderSize = 0;
            this.dftButton.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);
            this.dftButton.ForeColor = Color.FromArgb(255, 0, 206, 209);
        }
        /// <summary>Sets up the time domain chart.</summary>
        private void newChart()
        {
            rawSamples = waveFile.DataChunk1.Data;
            sampleRate = waveFile.FmtChunk1.SamplesPerSec;
            bytesPerSec = waveFile.FmtChunk1.AverageBytesPerSec;
            bitsPerSample = waveFile.FmtChunk1.BitsPerSample;
            numChannels = waveFile.FmtChunk1.Channels;
            dataSize = rawSamples.Length / numChannels;
            chart1.Series.Clear();
            var series = chart1.Series.Add("My Series");
            var series2 = chart1.Series.Add("My Series2");
            series.Color = Color.FromArgb(255, 0, 206, 209);
            series.ChartType = SeriesChartType.Spline;
            series2.ChartType = SeriesChartType.Spline;
            var chartArea = chart1.ChartAreas[series.ChartArea];
            chartArea.BackColor = Color.Black;
            chartArea.AxisX.LabelStyle.Format = "#.###";
            chartArea.AxisY.Title = "Amplitude";
            chartArea.AxisX.Title = "Time(s)";
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisY.MajorGrid.Enabled = false;
            chartArea.BorderWidth = 1;
            chartArea.AxisX.ScrollBar.Enabled = false; 
            chartArea.AxisX.ScaleView.Zoomable = false;
            chart1.MouseWheel += chart1_MouseWheel;
            chartArea.AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;
            
            chartArea.CursorX.Interval = 0.001;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorX.IsUserEnabled = true;

            selStart = 0;
            selEnd = Math.Min(dataSize/numChannels, selEnd + 1000);
            sel = selEnd;

            this.hScrollBar1.Value = selStart;
            this.hScrollBar1.SmallChange = maxSamples;
            this.hScrollBar1.LargeChange = maxSamples;
            this.hScrollBar1.Maximum = (dataSize) - maxSamples;
            this.hScrollBar1.Minimum = 0;
            this.hScrollBar1.Visible = true;

            refreshChart();
        }
        /// <summary>Used to refresh the time domain chart upon changes</summary>
        private void refreshChart()
        {
            foreach (var ser in chart1.Series)
            {
                ser.Points.Clear();
            }
            maxSamples = chart1.Size.Width;

            //plot chart points
            if (numChannels == 2)
            {
                int i;
                int jump = sel / maxSamples;
                int start = Math.Max(0, selStart - sel);
                int end = (selEnd + sel) > dataSize ? dataSize : selEnd + sel;

                for (i = start; i < end - 1; i += jump) 
                {
                    double sample = rawSamples[i];
                    double sample2 = rawSamples[i + 1];
                    chart1.Series["My Series"].Points.AddXY((double)i /sampleRate, sample);
                    chart1.Series["My Series2"].Points.AddXY((double)i /sampleRate, sample2);
                }
            }
            else
            {
                int i;
                int jump = sel / maxSamples;
                int start = Math.Max(0,selStart - sel);
                int end = (selEnd + sel) > dataSize ? dataSize : selEnd + sel;
                for (i = start; i < end; i += jump)
                {
                    double sample = rawSamples[i];
                    chart1.Series["My Series"].Points.AddXY((double)i / sampleRate, sample);
                }
            }
            chart1.ChartAreas[0].AxisX.Minimum = 0.000;
            chart1.ChartAreas[0].AxisX.Maximum =  ((dataSize)/ sampleRate) + 1;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom((double)selStart / sampleRate, (double)selEnd / sampleRate);
           
        }
        /// <summary>Single thread DFT function.</summary>
        /// <param name="start">The start index in the samples array.</param>
        /// <param name="end">The end index in the samples array.</param>
        private void dftSingle(long start, long end)
        {
            long N = end - start;
            fourierSamples = new ComplexNum[N];
            applyWindowing(start, N);
            for (long f = 0; f < N; f++)
            {
                fourierSamples[f] = new ComplexNum();
                for (int t = 0; t < N; t++)
                {
                    fourierSamples[f].Re += windowingSamples[t] * Math.Cos((2 * Math.PI * t * f ) / N);
                    fourierSamples[f].Im -= windowingSamples[t] * Math.Sin((2 * Math.PI * t * f ) / N);
                }
                if (Math.Abs(fourierSamples[f].Re) < zeroThreshold)
                {
                    fourierSamples[f].Re = 0.0;
                }

                if (Math.Abs(fourierSamples[f].Im) < zeroThreshold)
                {
                    fourierSamples[f].Im = 0.0;
                }
            }
        }
        /// <summary>Initial function called for DFT using multiple threads.</summary>
        /// <param name="start">The start index of the samples array.</param>
        /// <param name="end">The end index of the samples array.</param>
        private void dftMulti(long start, long end)
        {
            long N = end - start;
            int threadCount = 4;
            N = (int)Math.Round((N / (double)threadCount),
             MidpointRounding.AwayFromZero) * threadCount;
            applyWindowing(start, N);
            ThreadStart[] childRefs = new ThreadStart[threadCount];
            Thread[] childThreads = new Thread[threadCount];
            dftThreadSamples = new ComplexNum[threadCount][];
            long numberOfBins = N / threadCount;
            childRefs[0] = new ThreadStart(() => dftThreadProc(0, start, N, numberOfBins));
            childThreads[0] = new Thread(childRefs[0]);
            childThreads[0].Start();
            childRefs[1] = new ThreadStart(() => dftThreadProc(1, start, N, numberOfBins));
            childThreads[1] = new Thread(childRefs[1]);
            childThreads[1].Start();
            childRefs[2] = new ThreadStart(() => dftThreadProc(2, start, N, numberOfBins));
            childThreads[2] = new Thread(childRefs[2]);
            childThreads[2].Start();
            childRefs[3] = new ThreadStart(() => dftThreadProc(3, start, N, numberOfBins));
            childThreads[3] = new Thread(childRefs[3]);
            childThreads[3].Start();
            foreach (Thread thread in childThreads)
            {
                thread.Join();
            }
            combineDfts(N, numberOfBins);
        }
        /// <summary>Thread proc for multithread DFT</summary>
        /// <param name="threadIndex">Index of the thread.</param>
        /// <param name="startRawSamples">The start index in the raw samples array.</param>
        /// <param name="N">The total number of samples with which DFT is being applied to.</param>
        /// <param name="chunkSize">Size of this threads chunk.</param>
        private void dftThreadProc(int threadIndex, long startRawSamples, long N, long chunkSize)
        {
            ComplexNum[] dftSamples = new ComplexNum[chunkSize];
            for (long f = 0; f < chunkSize; f++)
            {
                dftSamples[f] = new ComplexNum();
                for (int t = 0; t < N; t++)
                {
                    dftSamples[f].Re += windowingSamples[t] * Math.Cos((2 * Math.PI * t * (f+(threadIndex*chunkSize))) / N);
                    dftSamples[f].Im -= windowingSamples[t] * Math.Sin((2 * Math.PI * t * (f+(threadIndex*chunkSize))) / N);
                }
                if (Math.Abs(dftSamples[f].Re) < zeroThreshold)
                {
                    dftSamples[f].Re = 0.0;
                }

                if (Math.Abs(dftSamples[f].Im) < zeroThreshold)
                {
                    dftSamples[f].Im = 0.0;
                }
            }
            dftThreadSamples[threadIndex] = dftSamples;
        }
        /// <summary>Combines the fourier samples from each thread into one array of complex numbers (fourier samples).</summary>
        /// <param name="N">The total number of samples with which DFT is being applied to.</param>
        /// <param name="chunkSize">The number of samples in each thread chunk.</param>
        private void combineDfts(long N, long chunkSize)
        {
            fourierSamples = new ComplexNum[N];
            Array.Copy(dftThreadSamples[0], 0, fourierSamples, 0, chunkSize);
            Array.Copy(dftThreadSamples[1], 0, fourierSamples, chunkSize, chunkSize);
            Array.Copy(dftThreadSamples[2], 0, fourierSamples, chunkSize * 2, chunkSize);
            Array.Copy(dftThreadSamples[3], 0, fourierSamples, chunkSize * 3, chunkSize);
        }
        /// <summary>applies inverse dft on a set of samples.</summary>
        /// <param name="A">The array of samples (ComplexNum).</param>
        /// <returns>The new samples as an array of doubles.</returns>
        private double[] idft(ComplexNum[] A)
        {
            int n = A.Length;
            double[] newSamples = new double[n];
            for (int t = 0; t < n; t++)
            {
                double re = 0.0;
                double im = 0.0;
                for (int f = 0; f < n; f++)
                {
                    re += A[f].Re * Math.Cos((2 * Math.PI * t * f) / n);
                    im += A[f].Im * Math.Sin((2 * Math.PI * t * f) / n);
                }
                if (Math.Abs(re) < zeroThreshold )
                {
                    re = 0.0;
                }

                if (Math.Abs(im) < zeroThreshold )
                {
                    im = 0.0;
                }
                newSamples[t] = (re + im)/n;
            }
            return newSamples;
        }
        /// <summary>Applies windowing.</summary>
        /// <param name="start">The start index in the rawSamples array.</param>
        /// <param name="N">The number of samples.</param>
        private void applyWindowing(long start, long N)
        {
            windowingSamples = new double[N];
            double[] w = new double[N];
            Array.Copy(rawSamples, start, windowingSamples, 0, N);
            int selectedIndex = comboBox2.SelectedIndex;
            long m = N / 2;
            double r;
            double pi = Math.PI;
            switch (selectedIndex)
            {
                case 0:
                    //rectangular
                    break;
                case 1:
                    //hamming
                    r = pi / m;
                    for (long n = -m; n < m; n++)
                    {
                        w[m + n] = 0.54f + 0.46f * Math.Cos(n * r);
                        windowingSamples[m + n] = (w[m + n] * windowingSamples[m + n]);
                    }
                    break;
                case 2:
                    //hanning
                    r = pi / (m + 1);
                    for (long n = -m; n < m; n++)
                    {
                        w[m + n] = 0.5f + 0.5f * Math.Cos(n * r);
                        windowingSamples[m + n] = (w[m + n] * windowingSamples[m + n]);
                    }
                    break;
                default:
                    //rectangular
                    break;
            }
        }
        /// <summary>Applies a filter to samples.</summary>
        /// <param name="fWeights">The filter weights.</param>
        /// <param name="samples">The samples.</param>
        /// <returns>The new samples.</returns>
        private double[] applyFilter(double[] fWeights, double[] samples)
        {
            for(int i = 0; i < samples.Length; i++)
            {
                double temp = 0.0;
                for (int j = 0; j < fWeights.Length; j++)
                {
                    double rs = 0.0;
                    if(i + j >= samples.Length)
                    {
                        rs = 0.0;
                    } else
                    {
                        rs = samples[i + j];
                    }
                    temp += (rs * fWeights[j]);
                }
                samples[i] = temp;
            }
            return samples;
        }
        /// <summary>Cuts out a section from the raw samples array.</summary>
        /// <param name="posXStart">The position in the rawSamples array to start the cut.</param>
        /// <param name="posXFinish">The position in the rawSamples array to end the cut.</param>
        private void cutRawSamples(int posXStart, int posXFinish)
        {
            //cut & save raw samples
            int length = (int)Math.Abs(posXFinish - posXStart);
            cutSamples = new double[length];
            Array.Copy(rawSamples, posXStart, cutSamples, 0, length);
            //make new arr for raw samples minus the cut
            double[] newRawSamples = new double[rawSamples.Length - length];
            //copy left side of cut into new arr
            Array.Copy(rawSamples, 0, newRawSamples, 0, posXStart);
            //copy right side of cut int arr
            Array.Copy(rawSamples, posXFinish, newRawSamples, posXStart, (rawSamples.Length-(posXStart + length)));
            //reinitialize rawsamples
            rawSamples = new double[newRawSamples.Length];
            //copy new raw samples into old raw samples variable
            Array.Copy(newRawSamples, 0, rawSamples, 0, newRawSamples.Length);
            dataSize = rawSamples.Length / numChannels;
            DataFormats.Format myFormat = DataFormats.GetFormat("cutData");
            ClipboardData cd = new ClipboardData();
            cd.Sr = this.sampleRate;
            cd.Cs = cutSamples;
            DataObject myDataObject = new DataObject(myFormat.Name, cd);
            Clipboard.SetDataObject(myDataObject);
            refreshChart();
        }
        /// <summary>Copies a section of the raw samples.</summary>
        /// <param name="posXStart">The position in the rawSamples array to start the copy.</param>
        /// <param name="posXFinish">The position in the rawSamples array to end the copy.</param>
        private void copyRawSamples(int posXStart, int posXFinish)
        {
            //cut & save raw samples
            int length = (int)Math.Abs(posXFinish - posXStart);
            cutSamples = new double[length];
            Array.Copy(rawSamples, posXStart, cutSamples, 0, length);

            DataFormats.Format myFormat = DataFormats.GetFormat("cutData");
            ClipboardData cd = new ClipboardData();
            cd.Sr = this.sampleRate;
            cd.Cs = cutSamples;
            DataObject myDataObject = new DataObject(myFormat.Name, cd);
            Clipboard.SetDataObject(myDataObject);
        }
        /// <summary>Pastes rawSamples from the clipboard. If there are any.</summary>
        /// <param name="posX">The position to paste the samples in the rawSamples array.</param>
        private void pasteRawSamples(int posX)
        {
            DataFormats.Format myFormat = DataFormats.GetFormat("cutData");
            IDataObject myRetrievedObject = Clipboard.GetDataObject();
            ClipboardData csData = (ClipboardData)myRetrievedObject.GetData(myFormat.Name);

            if (csData != null)
            {
                if (csData.Cs == null || csData.Cs.Length <= 0 || csData.Sr <= 0) return;

                int length = csData.Cs.Length;
                double[] newCutSamples = csData.Cs;
                if (csData.Sr > this.sampleRate)
                {
                    //downSample
                    newCutSamples = downSample(csData);
                    int factor = (int)(csData.Sr / this.sampleRate);
                    length = csData.Cs.Length / factor;
                }
                if (csData.Sr < this.sampleRate)
                {
                    //upSample
                    newCutSamples = upSample(csData);
                    int factor = (int)(this.sampleRate/ csData.Sr);
                    length = csData.Cs.Length * factor;
                }
                double[] newRawSamples = new double[rawSamples.Length + length];
                //paste left side of cut into new arr
                Array.Copy(rawSamples, 0, newRawSamples, 0, posX);
                //paste cutout section
                Array.Copy(newCutSamples, 0, newRawSamples, posX + 1, newCutSamples.Length);
                //paste right side of cut int arr
                Array.Copy(rawSamples, posX, newRawSamples, (posX + length), rawSamples.Length - posX);
                //reinitialize rawsamples
                rawSamples = new double[newRawSamples.Length];
                //copy new raw samples into old raw samples variable
                Array.Copy(newRawSamples, 0, rawSamples, 0, newRawSamples.Length);
                dataSize = rawSamples.Length / numChannels;
                refreshChart();
            }
            
        }
        /// <summary>Upsamples sound data.</summary>
        /// <param name="csData">The sample data.</param>
        /// <returns>the new samples array after upsampling.</returns>
        private double[] upSample(ClipboardData csData)
        {
            int factor = (int)(this.sampleRate / csData.Sr);
            double[] newSamples = new double[csData.Cs.Length * factor];
            for(int i = 0; i < csData.Cs.Length; ++i)
            {
                for(int j = 0; j < factor; ++j)
                {
                    newSamples[(i * factor) + j] = csData.Cs[i];
                }
            }
            ComplexNum[] nyquistFilter = genSampleFilter((int)this.sampleRate, csData);
            double[] nyquistFilterWeights = idft(nyquistFilter);
            newSamples = filterSelect(nyquistFilterWeights, newSamples);
            return newSamples;
        }
        /// <summary>Downsamples sound data.</summary>
        /// <param name="csData">The sample data.</param>
        /// <returns>the new samples array after downsampling.</returns>
        private double[] downSample(ClipboardData csData)
        {
            ComplexNum[] nyquistFilter = genSampleFilter((int)this.sampleRate, csData);
            double[] nyquistFilterWeights = idft(nyquistFilter);
            double[] samples = filterSelect(nyquistFilterWeights, csData.Cs);
            int factor = (int)(csData.Sr/this.sampleRate);
            double[] newSamples = new double[csData.Cs.Length / factor];
            for (int i = 0; i < newSamples.Length; ++i)
            {
                newSamples[i] = samples[i*factor];
            }
            return newSamples;
        }
        /// <summary>Generates a filter to filter out aliases for upsampling/downsampling.</summary>
        /// <param name="newSampleRate">The new sample rate to determine the nyquist limit.</param>
        /// <param name="csData">The sample data.</param>
        /// <returns>ComplexNum array representing the filter.</returns>
        private ComplexNum[] genSampleFilter(int newSampleRate, ClipboardData csData)
        {
            ComplexNum[] filter = new ComplexNum[csData.Cs.Length/10];
            int freq = newSampleRate / 2;
            int bin = (freq * filter.Length) / newSampleRate;
            for(int i = 0; i < filter.Length; ++i)
            {
                filter[i] = new ComplexNum();
                if (i <= bin)
                {
                    filter[i].Re = 1.0;
                    filter[i].Im = 1.0;
                } else
                {
                    filter[i].Re = 0.0;
                    filter[i].Im = 0.0;
                }
            }
            return filter;
        }
        /// <summary>Calls different filter function depending on users selection.</summary>
        /// <param name="filterWeights">The filter weights.</param>
        /// <param name="samples">The samples.</param>
        /// <returns>samples after convolution.</returns>
        private double[] filterSelect(double[] filterWeights, double[] samples)
        {
            int selectedIndex = comboBox1.SelectedIndex;
            double[] newSamples = new double[samples.Length];
            switch (selectedIndex)
            {
                case 0:
                    {
                        newSamples = applyFilter(filterWeights, samples);
                        break;
                    }
                case 1:
                    {
                        IntPtr diptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * samples.Length);
                        Marshal.Copy(samples, 0, diptr, samples.Length);
                        FilterData fd = openCLApplyFilter(filterWeights, filterWeights.Length, diptr, samples.Length);
                        newSamples = new double[(int)fd.len];
                        Marshal.Copy(fd.ip, newSamples, 0, (int)fd.len);
                        Marshal.FreeHGlobal(diptr);
                        break;
                    }
                case 2:
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        newSamples = applyFilter(filterWeights, samples);
                        watch.Stop();
                        double elapsedBasic = (double)watch.ElapsedMilliseconds / 1000.00;

                        IntPtr diptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(double)) * samples.Length);
                        Marshal.Copy(samples, 0, diptr, samples.Length);
                        FilterData fd = openCLApplyFilter(filterWeights, filterWeights.Length, diptr, samples.Length);
                        newSamples = new double[(int)fd.len];
                        Marshal.Copy(fd.ip, newSamples, 0, (int)fd.len);
                        var openCLElapsed = fd.openCLElapsed;

                        double diff = elapsedBasic / openCLElapsed;
                        diff = Math.Round(diff, 3);
                        string box_msg;
                        string box_title = "OpenCL vs C# Convolution Performance";
                        if (diff <= 0)
                        {
                            double negDiff = openCLElapsed / elapsedBasic;
                            negDiff = Math.Round(diff, 3);
                            box_msg = "OpenCL is " + negDiff.ToString() + " times slower than basic C#";
                        }
                        else
                        {
                            box_msg = "OpenCL is " + diff.ToString() + " times faster than basic C#";
                        }
                        MessageBox.Show(box_msg, box_title);
                        Marshal.FreeHGlobal(diptr);
                        break;
                    }
                default:
                    {
                        newSamples = applyFilter(filterWeights, samples);
                        break;
                    }
            }
            return newSamples;
        }



        /// <summary>Calls the start recording dll function.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void sRec_Click_1(object sender, EventArgs e)
        {
            if (OpenDialog())
            {
                int sampleRateIndex = comboBox3.SelectedIndex;
                int quantizationIndex = comboBox4.SelectedIndex;
                switch (sampleRateIndex)
                {
                    case 0:
                        sampleRate = 11025;
                        break;
                    case 1:
                        sampleRate = 22050;
                        break;
                    case 2:
                        sampleRate = 44100;
                        break;
                    default:
                        sampleRate = 44100;
                        break;

                }
                switch (quantizationIndex)
                {
                    case 0:
                        bitDepth = 16;
                        break;
                    case 1:
                        bitDepth = 32;
                        break;
                    default:
                        break;
                }
                dialogOpen = true;
                newRecording = true;
                StartRec(bitDepth, sampleRate);
            }
            else
            {
                dialogOpen = false;
            }
        }
        /// <summary>Calls the stop recording dll function.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void stRec_Click(object sender, EventArgs e)
        {
            if (!dialogOpen)
            {
                OpenDialog();
            }
            RecordData rd = StopRec();
            byte[] data = new byte[rd.len];
            //gets recorded data (samples)
            Marshal.Copy(rd.ip, data, 0, (int)rd.len);
            //gets wave fmt/riff data
            byte[] formatTag = new byte[4];
            wf = (WaveForm)Marshal.PtrToStructure(GetWaveform(), typeof(WaveForm));
            waveFile = new RWWaveFile(Encoding.ASCII.GetBytes("RIFF"), (uint)(rd.len / wf.nBlockAlign) + 36, Encoding.ASCII.GetBytes("WAVE"),
                    Encoding.ASCII.GetBytes("fmt "), 16, (ushort)1, wf.nChannels, wf.nSamplesPerSec,
                    wf.nAvgBytesPerSec, wf.nBlockAlign, wf.wBitsPerSample,
                    Encoding.ASCII.GetBytes("data"), rd.len);
            //sets up samples
            if (waveFile.FmtChunk1.BitsPerSample == 16)
            {
                double[] temp = new double[rd.len / (int)waveFile.FmtChunk1.BlockAlign];
                for (int i = 0; i < temp.Length - 1; i++)
                    temp[i] = BitConverter.ToInt16(data, i * (int)waveFile.FmtChunk1.BlockAlign);
                rawSamples = new double[temp.Length];
                rawSamples = temp.Select(x => (x)).ToArray();
            }
            else if (waveFile.FmtChunk1.BitsPerSample == 32)
            {
                double[] temp = new double[rd.len / (int)waveFile.FmtChunk1.BlockAlign];
                for (int i = 0; i < temp.Length; i++)
                    temp[i] = BitConverter.ToInt32(data, i * (int)waveFile.FmtChunk1.BlockAlign);
                rawSamples = temp.Select(x => (x)).ToArray();
                waveFile.FmtChunk1.FmtTag = 3;
            }

            waveFile.DataChunk1.Data = rawSamples;
            if (waveFile.DataChunk1.Data != null)
            {
                sel = (int)waveFile.DataChunk1.DataSize;
                selEnd = (int)waveFile.DataChunk1.DataSize;
                newChart();
            }
            this.Text = "new untitled recording (" + waveFile.FmtChunk1.BitsPerSample + "bits, " + waveFile.FmtChunk1.SamplesPerSec + "Hz)";
        }
        /// <summary>Starts the play dll function.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void sPlay_Click_1(object sender, EventArgs e)
        {
            if (!dialogOpen)
            {
                OpenDialog();
            }
            if (waveFile == null)
                return;
            byte[] data = null;
            //preps samples for dll
            if (waveFile.FmtChunk1.BitsPerSample == 16)
            {
                data = rawSamples.Select(x => (x)).ToArray()
                    .Select(x => Convert.ToInt16(x))
                    .SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            }
            if (waveFile.FmtChunk1.BitsPerSample == 32)
            {
                data = rawSamples.Select(x => (int)(x)).ToArray()
                    .Select(x => Convert.ToInt32(x))
                    .SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            }
            //gets pointer to array, for dll
            IntPtr iptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(byte)) * data.Length);
            Marshal.Copy(data, 0, iptr, data.Length);
            if (wf.Equals(default(WaveForm)))
            {
                bool isSet = SetWaveform((int)waveFile.FmtChunk1.BitsPerSample, (int)waveFile.FmtChunk1.SamplesPerSec, (int)waveFile.FmtChunk1.BlockAlign, (int)waveFile.FmtChunk1.Channels, (int)waveFile.FmtChunk1.AverageBytesPerSec);
            }
            //calls play in the dll, passing the pointer and play details
            PlayStart(false, iptr, data.Length, (int)waveFile.FmtChunk1.BitsPerSample, (int)waveFile.FmtChunk1.SamplesPerSec, (int)waveFile.FmtChunk1.BlockAlign, (int)waveFile.FmtChunk1.Channels, (int)waveFile.FmtChunk1.AverageBytesPerSec);
            Marshal.FreeHGlobal(iptr);
        }
        /// <summary>Calls the stop dll function.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void stPlay_Click_1(object sender, EventArgs e)
        {
            PlayStop();
        }
        /// <summary>Handles the CTRL-C, CTRL-X and CTRL-V event by calling the appropriate copy, cut, paste function.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.X)
            {
                int posXStart = (int)(chart1.ChartAreas[0].CursorX.SelectionStart * sampleRate * numChannels);
                int posXFinish = (int)(chart1.ChartAreas[0].CursorX.SelectionEnd * sampleRate * numChannels);
                cutRawSamples(posXStart, posXFinish);
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                int posX = (int)(chart1.ChartAreas[0].CursorX.Position * sampleRate * numChannels);
                pasteRawSamples(posX);
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                int posXStart = (int)(chart1.ChartAreas[0].CursorX.SelectionStart * sampleRate * numChannels);
                int posXFinish = (int)(chart1.ChartAreas[0].CursorX.SelectionEnd * sampleRate * numChannels);
                copyRawSamples(posXStart, posXFinish);
            }
        }
        /// <summary>Calls the appropriate DFT function depending on the users selection.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void dftButton_Click(object sender, EventArgs e)
        {
            int selectedDFTIndex = comboBox5.SelectedIndex;
            switch (selectedDFTIndex)
            {
                case 0:
                    dftSingle(posXStart, posXFinish);
                    break;
                case 1:
                    dftMulti(posXStart, posXFinish);
                    break;
                case 2:
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    dftSingle(posXStart, posXFinish);
                    watch.Stop();
                    double elapsedSingle = (double)watch.ElapsedMilliseconds / 1000.00;

                    var watchMulti = System.Diagnostics.Stopwatch.StartNew();
                    dftMulti(posXStart, posXFinish);
                    double elapsedMulti = (double)watchMulti.ElapsedMilliseconds / 1000.00;


                    double diff = elapsedSingle / elapsedMulti;
                    diff = Math.Round(diff, 3);
                    string box_msg;
                    string box_title = "Single vs Multithread DFT Performance";
                    if (diff <= 0)
                    {
                        double invDiff = elapsedMulti / elapsedSingle;
                        invDiff = Math.Round(diff, 3);
                        box_msg = "Multithread is " + invDiff.ToString() + " times slower than Single thread";
                    }
                    else
                    {
                        box_msg = "Multithread is " + diff.ToString() + " times faster than Single thread";
                    }
                    MessageBox.Show(box_msg, box_title);


                    break;

            }
            ///setup chart with fourier samples
            fourierChart = new FourierChart(this.fourierSamples, this.chart2);
            fourierChart.setupChart();
        }
        /// <summary>Handles the filter button click by calling the filter select function.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void filterButton_Click(object sender, EventArgs e)
        {
            if (fourierChart != null)
            {
                ComplexNum[] filter = fourierChart.generateFilter();
                double[] filterWeights = idft(filter);
                rawSamples = filterSelect(filterWeights, this.rawSamples);
                waveFile.DataChunk1.Data = rawSamples;
                refreshChart();
            }
        }
        /// <summary>Handles the MouseDown event of time domain chart</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Chart1_MouseDown(object sender, EventArgs e)
        {
            if (this.waveFile == null && this.wf.Equals(default(WaveForm))) { return; }
            MouseEventArgs me = (MouseEventArgs)e;
            var chart = (Chart)sender;
            chart.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(me.X, me.Y), true);
            double pX = chart.ChartAreas[0].CursorX.Position;
            posXStart = (long)(pX * sampleRate);
        }
        /// <summary>Handles the MouseUp event of the time domain chart.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Chart1_MouseUp(object sender, EventArgs e)
        {
            if (this.waveFile == null && this.wf.Equals(default(WaveForm))) { return; }
            MouseEventArgs me = (MouseEventArgs)e;
            var chart = (Chart)sender;
            chart.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(me.X, me.Y), true);
            double pX = chart.ChartAreas[0].CursorX.Position;

            var posXBeg = posXStart;
            var posXEnd = (long)(pX * sampleRate);
            if (posXBeg > posXEnd)
            {
                posXStart = posXEnd;
                posXFinish = posXBeg;
            }
            else
            {
                posXStart = posXBeg;
                posXFinish = posXEnd;
            }
            if (posXFinish > rawSamples.Length) posXFinish = rawSamples.Length - 1;
        }
        /// <summary>Handles the MouseWheel event of the time domain chart.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        private void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            var chart = (Chart)sender;
            var xAxis = chart.ChartAreas[0].AxisX;
            if (this.waveFile == null && this.wf.Equals(default(WaveForm))) { return; }
            try
            {
                if (e.Delta < 0) // zoom out
                {
                    if (Math.Abs(selEnd - selStart) < dataSize)
                    {
                        selStart = Math.Max(0, selStart - 1000);
                        selEnd = Math.Min(dataSize, selEnd + 1000);
                        sel = Math.Abs(selEnd - selStart);
                        this.hScrollBar1.Value = selStart;
                        refreshChart();
                    }
                }
                else if (e.Delta > 0) // zoom in
                {
                    if (Math.Abs(selEnd - selStart) > 5 * maxSamples)
                    {
                        selStart = Math.Max(0, selStart + 1000);
                        selEnd = Math.Min(dataSize, selEnd - 1000);
                        sel = Math.Abs(selEnd - selStart);
                        this.hScrollBar1.Value = selStart;
                        refreshChart();
                    }

                }
            }
            catch { }
        }
        /// <summary>Handles the Scroll event of the time domain scrollbar.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScrollEventArgs"/> instance containing the event data.</param>
        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            selStart = Math.Max(0, this.hScrollBar1.Value);
            selEnd = selStart + sel;
            refreshChart();
        }
        /// <summary>Handles the Click event of the openFileToolStripMenuItem control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDlg = new OpenFileDialog();

            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                filename = fileDlg.FileName;
                waveFile = new RWWaveFile(filename);
                sel = (int)waveFile.DataChunk1.DataSize;
                selEnd = (int)waveFile.DataChunk1.DataSize;
                newChart();
                newRecording = false;
                this.Text = fileDlg.FileName + " (" + waveFile.FmtChunk1.BitsPerSample + "bits, " + waveFile.FmtChunk1.SamplesPerSec + "Hz)";
                refreshChart();
            }
        }
        /// <summary>Handles the Click event of the saveToolStripMenuItem control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "wav (*.wav)|*.wav|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Wav File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                filename = saveFileDialog1.FileName;
                waveFile.DataChunk1.DataSize = (uint)(rawSamples.Length * numChannels * (bitsPerSample / 8));
                waveFile.RiffChunk1.FileSize = (uint)(rawSamples.Length * numChannels * (bitsPerSample / 8)) + 36;
                waveFile.DataChunk1.NumSamples = rawSamples.Length;
                waveFile.DataChunk1.copyData(rawSamples);
                waveFile.printWave();
                waveFile.Write(filename);
                this.Text = filename + " (" + waveFile.FmtChunk1.BitsPerSample + "bits, " + waveFile.FmtChunk1.SamplesPerSec + "Hz)";
            }

        }
        /// <summary>Handles the Click event of the newEditorToolStripMenuItem control. Creates new instance of the editor.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void newEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var info = new System.Diagnostics.ProcessStartInfo(Application.ExecutablePath);
            System.Diagnostics.Process.Start(info);
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {}
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {}
        private void Chart1_Click(object sender, EventArgs e)
        {}
        private void Button1_Click(object sender, EventArgs e)
        {}



        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool OpenDialog();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CloseDialog();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern RecordData StopRec();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool StartRec(uint bitDepth, uint sampleRate);
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr GetWaveform();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetWaveform(int bits, int rate, int blockAlign, int nChannels, int byteRate);
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayPause();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayStart(bool newRec, IntPtr p, int size, int d, int r, int blockAlign, int c, int byteRate);
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool PlayStop();
        [DllImport("CWaveApi.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern FilterData openCLApplyFilter(double[] fWeights, int size, IntPtr pd, int dsize);


    }
}
