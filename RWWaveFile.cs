using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveVisualizer
{
    class RWWaveFile
    {
        private string filepath;
        private FileInfo fileInfo;
        private FileStream fileStream;

        private RiffChunk riffChunk;
        private FmtChunk fmtChunk;
        private DataChunk dataChunk;

        //constructor
        public RWWaveFile(string newFilePath)
        {
            filepath = newFilePath;
            fileInfo = new FileInfo(newFilePath);
            fileStream = fileInfo.OpenRead();
            RiffChunk1 = new RiffChunk(fileStream);
            FmtChunk1 = new FmtChunk(fileStream);
            DataChunk1 = new DataChunk(fileStream);
        }
        internal RiffChunk RiffChunk1 { get => riffChunk; set => riffChunk = value; }
        internal FmtChunk FmtChunk1 { get => fmtChunk; set => fmtChunk = value; }
        internal DataChunk DataChunk1 { get => dataChunk; set => dataChunk = value; }
        public void Write(string name)
        {
            FileStream f = new FileStream(name, FileMode.Create, FileAccess.Write);
            BinaryWriter wr = new BinaryWriter(f);
            try
            {
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
                f.Seek(40, System.IO.SeekOrigin.Begin);
                for (int i = 0; i < DataChunk1.Data.Length/2; i++)
                {
                    wr.Write(DataChunk1.Data[i]);
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

        //first 12 bytes
        public class RiffChunk
        {
            //"RIFF"
            private byte[] riffID;
            //Wave file size minus riffId and this chunk in header
            private uint fileSize;
            //"wave"
            private byte[] riffFormat;

            public RiffChunk(FileStream FS)
            {
                riffID = new byte[4];
                riffFormat = new byte[4];
                Read(FS);
            }
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
        }

        //24 bytes
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
            public FmtChunk(FileStream FS)
            {
                fmtID = new byte[4];
                Read(FS);
            }

            public void Read(FileStream FS)
            {
                FS.Read(fmtID, 0, 4);
                //Debug.Assert(m_FmtID[0] == 102, "Format ID Not Valid");
                BinaryReader binRead = new BinaryReader(FS);

                fmtSize = binRead.ReadUInt32();
                fmtTag = binRead.ReadUInt16();
                channels = binRead.ReadUInt16();
                samplesPerSec = binRead.ReadUInt32();
                averageBytesPerSec = binRead.ReadUInt32();
                blockAlign = binRead.ReadUInt16();
                bitsPerSample = binRead.ReadUInt16();

                // This accounts for the variable format header size 
                // 12 bytes of Riff Header, 4 bytes for FormatId, 4 bytes for FormatSize & the Actual size of the Format Header 
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
        }

        //8 bytes plus size of data
        public class DataChunk
        {
            private byte[] dataID;
            private uint dataSize;
            private int numSamples;
            private Int16[] data;
            public DataChunk(FileStream FS)
            {
                dataID = new byte[4];
                Read(FS);
            }

            public void Read(FileStream FS)
            {
                FS.Read(dataID, 0, 4);
                Debug.Assert(dataID[0] == 100, "Data ID Not Valid");
                BinaryReader binRead = new BinaryReader(FS);
                dataSize = binRead.ReadUInt32();
                data = new Int16[dataSize / 2];
                FS.Seek(40, System.IO.SeekOrigin.Begin);
                numSamples = (int)(dataSize / 2);
                int i;
                for (i = 0; i < numSamples; i++)
                {
                    data[i] = binRead.ReadInt16();
                    //data[i] += 128;
                }
                //Debug.WriteLine(i);
                Debug.WriteLine(data.Length);
            }

            //getters/setters
            public byte[] DataID { get => dataID; set => dataID = value; }
            public uint DataSize { get => dataSize; set => dataSize = value; }
            //short??
            public short[] Data { get => data; set => data = value; }
            public int NumSamples { get => numSamples; set => numSamples = value; }

        }



    }
}
