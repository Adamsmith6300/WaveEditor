using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVisualizer
{

    /// <summary>Represents wave files, (reads/writes)</summary>
    class RWWaveFile
    {
        private string filepath;
        private FileInfo fileInfo;
        private FileStream fileStream;

        private RiffChunk riffChunk;
        private FmtChunk fmtChunk;
        private DataChunk dataChunk;


        /// <summary>Initializes a new instance of the <see cref="RWWaveFile"/> class.</summary>
        /// <param name="rid">The Chunk id</param>
        /// <param name="rfs">The Chunk size</param>
        /// <param name="rfmt">The Chunk format (WAVE)</param>
        /// <param name="fid">The subChunk id</param>
        /// <param name="fsz">The subChunk Size.</param>
        /// <param name="ftg">The audio format</param>
        /// <param name="fc">The number of channels</param>
        /// <param name="fsr">The sample rate</param>
        /// <param name="fbs">The byte rate</param>
        /// <param name="fba">The block align</param>
        /// <param name="fbps">The bits per sample</param>
        /// <param name="did">The subChunk id 2 (DATA)</param>
        /// <param name="dsz">The data size (bytes)</param>
        public RWWaveFile(Byte[] rid, uint rfs, Byte[] rfmt,
                         Byte[] fid, uint fsz, ushort ftg, ushort fc, uint fsr, uint fbs, ushort fba, ushort fbps,
                         Byte[] did, uint dsz)
        {
            RiffChunk1 = new RiffChunk();
            FmtChunk1 = new FmtChunk();
            DataChunk1 = new DataChunk();
            // RIFF chunk
            RiffChunk1.RiffID = rid;
            RiffChunk1.FileSize = rfs;
            RiffChunk1.RiffFormat = rfmt;

            // fmt chuck
            FmtChunk1.FmtID = fid;
            FmtChunk1.FmtSize = fsz;
            FmtChunk1.FmtTag = ftg;
            FmtChunk1.Channels = fc;
            FmtChunk1.SamplesPerSec = fsr;
            FmtChunk1.AverageBytesPerSec = fbs;
            FmtChunk1.BlockAlign = fba;
            FmtChunk1.BitsPerSample = fbps;

            // data chunk
            DataChunk1.DataID = did;
            DataChunk1.DataSize = dsz;
        }


        /// <summary>Initializes a new instance of the <see cref="RWWaveFile"/> class.</summary>
        /// <param name="newFilePath">The path to the file</param>
        public RWWaveFile(string newFilePath)
        {
            filepath = newFilePath;
            fileInfo = new FileInfo(newFilePath);
            fileStream = fileInfo.OpenRead();
            RiffChunk1 = new RiffChunk(fileStream);
            FmtChunk1 = new FmtChunk(fileStream);
            DataChunk1 = new DataChunk(fileStream, FmtChunk1);
        }


        /// <summary>Prints the wavefile header.</summary>
        public void printWave()
        {
            RiffChunk1.printRiff();
            FmtChunk1.printFmt();
            DataChunk1.printDataChunk();
        }
        internal RiffChunk RiffChunk1 { get => riffChunk; set => riffChunk = value; }
        internal FmtChunk FmtChunk1 { get => fmtChunk; set => fmtChunk = value; }
        internal DataChunk DataChunk1 { get => dataChunk; set => dataChunk = value; }

        /// <summary>Writes the wavefile with the specified name.</summary>
        /// <param name="name">The name of the output file</param>
        public void Write(string name)
        {
            FileStream f = new FileStream(name, FileMode.Create, FileAccess.Write);
            BinaryWriter wr = new BinaryWriter(f);
            try
            {
                printWave();
                wr.Write(RiffChunk1.RiffID);
                wr.Write(RiffChunk1.FileSize);
                wr.Write(RiffChunk1.RiffFormat);
                wr.Write(FmtChunk1.FmtID);
                wr.Write(FmtChunk1.FmtSize);
                wr.Write(FmtChunk1.FmtTag);
                wr.Write(FmtChunk1.Channels);
                wr.Write(FmtChunk1.SamplesPerSec);
                wr.Write(FmtChunk1.AverageBytesPerSec);
                wr.Write(FmtChunk1.BlockAlign);
                wr.Write(FmtChunk1.BitsPerSample);
                wr.Write(DataChunk1.DataID);
                wr.Write(DataChunk1.DataSize);
                f.Seek(44, System.IO.SeekOrigin.Begin);

                if(FmtChunk1.BitsPerSample == 16)
                {
                    short[] shorts = DataChunk1.Data.Select(x => (short)(x)).ToArray();
                    byte[] data = shorts.Select(x => Convert.ToInt16(x))
                        .SelectMany(x => BitConverter.GetBytes(x)).ToArray();
                    wr.Write(data);
                } else
                {
                    float[] flts = DataChunk1.Data.Select(x => (float)(x)).ToArray();
                    byte[] data = flts.SelectMany(x => BitConverter.GetBytes(x)).ToArray();
                    for (int i = 0; i < data.Length; ++i)
                    {
                      wr.Write(data[i]);
                    }

                }

            }
            finally
            {
                if (wr != null)
                {
                    wr.Close();
                }
                if (f != null)
                {
                    f.Close();
                }
            }
           
          

        }


        /// <summary>class representing the riffchunk of the wavefile header</summary>
        public class RiffChunk
        {
            //"RIFF"
            private byte[] riffID;
            //Wave file size minus riffId and this chunk in header
            private uint fileSize;
            //"wave"
            private byte[] riffFormat;

            /// <summary>Initializes a new instance of the <see cref="RiffChunk"/> class.</summary>
            /// <param name="FS">The filestream passed in from the wavefile class</param>
            public RiffChunk(FileStream FS)
            {
                riffID = new byte[4];
                riffFormat = new byte[4];
                Read(FS);
            }

            /// <summary>Initializes a new instance of the <see cref="RiffChunk"/> class.</summary>
            public RiffChunk()
            {
            }

            /// <summary>Reads the specified fs.</summary>
            /// <param name="FS">The filestream passed in from the constructor</param>
            public void Read(FileStream FS)
            {
                FS.Read(riffID, 0, 4);
                Debug.Assert(riffID[0] == 82, "Riff ID Not Valid");
                BinaryReader binRead = new BinaryReader(FS);
                FileSize = binRead.ReadUInt32();
                FS.Read(riffFormat, 0, 4);
            }
            //getter/setters
            public byte[] RiffFormat { get => riffFormat; set => riffFormat = value; }
            public uint FileSize { get => fileSize; set => fileSize = value; }
            public byte[] RiffID { get => riffID; set => riffID = value; }

            /// <summary>Prints the riffchunk part of the wavefile header</summary>
            public void printRiff()
            {
                Debug.WriteLine("RiffChunk---------");
                Debug.WriteLine(
                   "Riff ID: " + Encoding.UTF8.GetString(RiffID, 0, RiffID.Length)
                   + "\nChunkSize(File Size): " + FileSize
                   + "\nFormat: " + Encoding.UTF8.GetString(RiffFormat, 0, RiffID.Length));
            }
        }


        /// <summary>format chunk of the header</summary>
        public class FmtChunk
        {
            private byte[] fmtID;
            private uint fmtSize;
            private ushort fmtTag;
            private ushort channels;
            private uint samplesPerSec;
            private uint averageBytesPerSec;
            private ushort blockAlign;
            private ushort bitsPerSample;

            /// <summary>Initializes a new instance of the <see cref="FmtChunk"/> class.</summary>
            /// <param name="FS">The filestream passed in from the wavefile class</param>
            public FmtChunk(FileStream FS)
            {
                fmtID = new byte[4];
                Read(FS);
            }

            /// <summary>Initializes a new instance of the <see cref="FmtChunk"/> class.</summary>
            public FmtChunk()
            {
            }

            /// <summary>Reads the specified fs.</summary>
            /// <param name="FS">The filestream passed in from the constructor</param>
            public void Read(FileStream FS)
            {
                FS.Read(fmtID, 0, 4);
                Debug.Assert(fmtID[0] == 102, "Format ID Not Valid");
                BinaryReader binRead = new BinaryReader(FS);

                fmtSize = binRead.ReadUInt32();
                fmtTag = binRead.ReadUInt16();
                channels = binRead.ReadUInt16();
                samplesPerSec = binRead.ReadUInt32();
                averageBytesPerSec = binRead.ReadUInt32();
                blockAlign = binRead.ReadUInt16();
                bitsPerSample = binRead.ReadUInt16();
 
                FS.Seek(fmtSize + 20, System.IO.SeekOrigin.Begin);
            }

            //getters/setters
            public byte[] FmtID { get => fmtID; set => fmtID = value; }
            public uint FmtSize { get => fmtSize; set => fmtSize = value; }
            public ushort FmtTag { get => fmtTag; set => fmtTag = value; }
            public ushort Channels { get => channels; set => channels = value; }
            public uint SamplesPerSec { get => samplesPerSec; set => samplesPerSec = value; }
            public uint AverageBytesPerSec { get => averageBytesPerSec; set => averageBytesPerSec = value; }
            public ushort BlockAlign { get => blockAlign; set => blockAlign = value; }
            public ushort BitsPerSample { get => bitsPerSample; set => bitsPerSample = value; }


            /// <summary>Prints the format chunk of the wavefile header</summary>
            public void printFmt()
            {
                Debug.WriteLine("FmtChunk--------");
                Debug.WriteLine(
                    "Fmt ID: " + Encoding.UTF8.GetString(FmtID, 0, FmtID.Length)
                   + "\nFmtSize: " + FmtSize
                   + "\nFmtTag: " + FmtTag
                   + "\nChannels: " + Channels
                   + "\nSampleRate: " + SamplesPerSec
                   + "\nByteRate: " + AverageBytesPerSec
                   + "\nBlockAlign: " + BlockAlign
                   + "\nBitsPerSample: " + BitsPerSample);
            }
        }


        /// <summary>data chunk of the wavefile header</summary>
        public class DataChunk
        {
            private byte[] dataID;
            private uint dataSize;
            private int numSamples;
            private double[] data;

            /// <summary>Initializes a new instance of the <see cref="DataChunk"/> class.</summary>
            /// <param name="FS">The filestream passed in from the wavefile class</param>
            /// <param name="fmt">The format chunk of the header</param>
            public DataChunk(FileStream FS, FmtChunk fmt)
            {
                dataID = new byte[4];
                Read(FS, fmt);
            }

            /// <summary>Initializes a new instance of the <see cref="DataChunk"/> class.</summary>
            public DataChunk()
            {
            }

            /// <summary>Reads the specified fs.</summary>
            /// <param name="FS">The filestream passed in from the constructor</param>
            /// <param name="fmt">The format chunk of the header</param>
            public void Read(FileStream FS, FmtChunk fmt)
            {
                FS.Read(dataID, 0, 4);
                Debug.Assert(dataID[0] == 100, "Data ID Not Valid");
                BinaryReader binRead = new BinaryReader(FS);
                dataSize = binRead.ReadUInt32();
                ushort channels = fmt.Channels;
                ushort bitsPerSample = fmt.BitsPerSample;
                numSamples = (int)dataSize / (bitsPerSample/8);
                data = new double[numSamples];
                FS.Seek(44, System.IO.SeekOrigin.Begin);
                int[] temp = new int[data.Length];
                double[] tempD = new double[data.Length];
                for (int i = 0; i < numSamples; i++)
                {
                    if(bitsPerSample == 16){
                        temp[i] = binRead.ReadInt16();
                    } else {
                        tempD[i] = binRead.ReadSingle();
                    }
                }
                if(bitsPerSample == 16)
                {
                    data = temp.Select(x => (double)(x)).ToArray();
                } else
                {

                    data = tempD;
                }
            }

            /// <summary>Copies data to the wavefile class</summary>
            /// <param name="newData">The new data.</param>
            public void copyData(double[] newData)
            {
                Data = new double[NumSamples];
                Array.Copy(newData, 0, Data, 0, numSamples);
            }
            //getters/setters
            public byte[] DataID { get => dataID; set => dataID = value; }
            public uint DataSize { get => dataSize; set => dataSize = value; }
            public double[] Data { get => data; set => data = value; }
            public int NumSamples { get => numSamples; set => numSamples = value; }

            /// <summary>Prints the data chunk part of the wavefile header.</summary>
            public void printDataChunk()
            {
                Debug.WriteLine("DataChunk--------");
                Debug.WriteLine(
                    "DataID: " + Encoding.UTF8.GetString(DataID, 0, DataID.Length)
                + "\nDataSize: " + DataSize
                   + "\nNumSamples: " + NumSamples
                +"\nDataLength: " + Data.Length);
            }

        }

    }
}
