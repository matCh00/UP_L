using System;
using System.Collections.Generic;
using System.Text;

namespace SoundCard
{
    // nagłówek pliku .wav
    public class WAVheader
    {

        public byte[] riffID;
        public uint size;
        public byte[] wavID;
        public byte[] fmtID;
        public uint fmtSize;
        public ushort format;
        public ushort channels;
        public uint sampleRate;
        public uint bytePerSec;
        public ushort blockSize;
        public ushort bit;
        public byte[] dataID;
        public uint dataSize;

        public override string ToString()
        {
            // rozpoznanie kanału
            string tempChannel;

            if (channels == 1)
            {
                tempChannel = "mono";
            }
                
            else if (channels == 2)
            {
                tempChannel = "stereo";
            }
                
            else
            {
                tempChannel = "unrecognized";
            }

            return "riffID: " + riffID + "\n" +
                   "size: " + size + "\n" +
                   "fmtSize: " + fmtSize + "\n" +
                   "dataSize: " + dataSize + "\n" +
                   "sampleRate: " + sampleRate + "\n" +
                   "channel: " + channels + " " + tempChannel + "\n";
        }

    }
}
