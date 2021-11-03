using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text;
using NAudio.Wave;
using SlimDX.DirectSound;
using SlimDX.Multimedia;
using WMPLib;
using BufferFlags = SlimDX.DirectSound.BufferFlags;
using CooperativeLevel = SlimDX.DirectSound.CooperativeLevel;
using DirectSound = SlimDX.DirectSound.DirectSound;
using SecondarySoundBuffer = SlimDX.DirectSound.SecondarySoundBuffer;
using SoundBufferDescription = SlimDX.DirectSound.SoundBufferDescription;
using System.Runtime.InteropServices;
using VisioForge.Shared.NAudio.Wave;
using IWavePlayer = VisioForge.Shared.NAudio.Wave.IWavePlayer;
using WaveOut = VisioForge.Shared.NAudio.Wave.WaveOut;
using DirectSoundOut = NAudio.Wave.DirectSoundOut;
using WaveChannel32 = NAudio.Wave.WaveChannel32;
using StoppedEventArgs = NAudio.Wave.StoppedEventArgs;
using WMPLib;
using System.Threading.Tasks;

namespace SoundCard
{
    public class SoundCardHandler
    {
        public static string FilePath;


        // ActiveX
        public static SoundPlayer Player;

        // WindowsMediaPlayer
        public static WindowsMediaPlayer Wplayer;

        // Waveaudio
        public static NAudio.Wave.WaveFileReader wfr;
        public static DirectSoundOut audioOutput;
        
        // DirectSound
        public static NAudio.Wave.WaveFileReader wave;
        public static DirectSoundOut output;

        // nagrywanie dźwięku
        public static String recordFile = @"C:\Users\matic\Desktop\temp\out.wav";
        public static NAudio.Wave.WaveIn recordInstance;
        public static NAudio.Wave.WaveFileWriter recordWriter;


        // zwykłe odtwarzanie
        public static void SimplePlay()
        {
            //SystemSounds.Asterisk.Play();
            //PlaySound(TEXT(FilePath), NULL, SND_FILENAME);

            //string URL = FilePath;
            //WMPlay.settings.autoStart = false;
            //WMPlay.URL = URL;
        }


        public static async Task EchoAsync()
        {
            Wplayer = new WindowsMediaPlayer();
            Wplayer.URL = FilePath;
            Wplayer.controls.play();
        }


        // odtwórz pliki .wav i .mp3
        public static void Play()
        {

            string Ext = Path.GetExtension(SoundCardHandler.FilePath);

            if (Ext == ".wav")
            {
                Player = new SoundPlayer(FilePath);
                Player.Play();
            }

            if (Ext == ".mp3")
            {
                Wplayer = new WindowsMediaPlayer();
                Wplayer.URL = FilePath;
                Wplayer.controls.play();
            }
        }


        // odtwórz za pomocą DirectSound
        public static void PlayDirectSound()
        {
            wave = new NAudio.Wave.WaveFileReader(FilePath);
            output = new NAudio.Wave.DirectSoundOut();
            output.Init(new NAudio.Wave.WaveChannel32(wave));
            output.Play();
        }


        // zatrzymanie odtwarzania niezależnie od sposobu i rodzaju pliku
        public static void Stop()
        {
            Player?.Stop();
            Wplayer?.controls.stop();
            audioOutput?.Stop();
            output?.Stop();
        }


        // czytanie nagłówka pliku .wav
        public static string GetHeader()
        {
            var header = new WAVheader();

            using (var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                try
                {
                    //pobiera kolejno dane pliku .wav
                    header.riffID = binaryReader.ReadBytes(4);
                    header.size = binaryReader.ReadUInt32();
                    header.wavID = binaryReader.ReadBytes(4);
                    header.fmtID = binaryReader.ReadBytes(4);
                    header.fmtSize = binaryReader.ReadUInt32();
                    header.format = binaryReader.ReadUInt16();
                    header.channels = binaryReader.ReadUInt16();
                    header.sampleRate = binaryReader.ReadUInt32();
                    header.bytePerSec = binaryReader.ReadUInt32();
                    header.blockSize = binaryReader.ReadUInt16();
                    header.bit = binaryReader.ReadUInt16();
                    header.dataID = binaryReader.ReadBytes(4);
                    header.dataSize = binaryReader.ReadUInt32();
                }
                finally
                {
                    binaryReader.Close();
                    fileStream.Close();
                }
            }
            return header.ToString();
        }


        
        // pomocnicza funkcja do nagrywania dźwięku
        private static void recordInstance_DataAvailable(object sender, NAudio.Wave.WaveInEventArgs e)
        {
            if (recordWriter == null) return;
            recordWriter.Write(e.Buffer, 0, e.BytesRecorded);
            recordWriter.Flush();
        }


        // pomocnicza funkcja do nagrywania dźwięku
        private static void recordInstance_RecordingStopped(object sender, NAudio.Wave.StoppedEventArgs e)
        {
            recordWriter.Dispose();
            recordWriter = null;
            recordInstance.Dispose();
        }


        // początek nagrywania dźwięku
        public static void startRecord()
        {
            recordInstance = new NAudio.Wave.WaveIn();
            recordInstance.WaveFormat = new NAudio.Wave.WaveFormat(48000, 1);

            recordInstance.DataAvailable += new EventHandler<NAudio.Wave.WaveInEventArgs>(recordInstance_DataAvailable);
            recordInstance.RecordingStopped += new EventHandler<NAudio.Wave.StoppedEventArgs>(recordInstance_RecordingStopped);

            recordWriter = new NAudio.Wave.WaveFileWriter(recordFile, recordInstance.WaveFormat);
            recordInstance.StartRecording();
        }


        // koniec nagrywania dźwięku
        public static void stopRecord()
        {
            recordInstance.StopRecording();
        }
    }
}
