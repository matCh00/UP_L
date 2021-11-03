using System;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;


namespace SoundCard
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }


        // przycisk Wczytaj
        private void Wczytaj(object sender, RoutedEventArgs e)
        {
            string Name = getFileNameTextBox.Text;

            SoundCardHandler.FilePath = @"C:\Users\matic\Desktop\" + Name;
        }


        // przycisk Czytaj
        private void Czytaj(object sender, RoutedEventArgs e)
        {
            headerTextBox.Text = SoundCardHandler.GetHeader();
        }


        // przycisk PlaySound
        private void PlaySound(object sender, RoutedEventArgs e)
        {
            SoundCardHandler.SimplePlay();
        }


        // przycisk ActiveX
        private void Echo(object sender, RoutedEventArgs e)
        {
            SoundCardHandler.EchoAsync();
        }


        // przycisk Odtwórz .wav / .mp3
        private void Play(object sender, RoutedEventArgs e)
        {
            SoundCardHandler.Play();
        }


        // przycisk Stop
        private void Stop(object sender, RoutedEventArgs e)
        {
            SoundCardHandler.Stop();
        }


        // przycisk DirectSound
        private void DirectSound(object sender, RoutedEventArgs e)
        {
            SoundCardHandler.PlayDirectSound();
        }


        // przycisk Waveform
        private void Waveform(object sender, RoutedEventArgs e)
        {

        }


        // przycisk Rozpocznij nagrywanie
        private void StartRecord(object sender, RoutedEventArgs e)
        {
            SoundCardHandler.startRecord();
        }

        // przycisk Zakończ nagrywanie
        private void StopRecord(object sender, RoutedEventArgs e)
        {
            SoundCardHandler.stopRecord();
        }

    }
}
