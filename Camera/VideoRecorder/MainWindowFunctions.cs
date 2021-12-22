using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging.Filters;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;

namespace VideoRecorder
{
    internal class MainWindowFunctions : ObservableObject, IDisposable
    {
        // zmienne
        private FilterInfo _currentDevice;
        private BitmapImage _image;
        private string _ipCameraUrl;
        private bool _isDesktopSource;
        private bool _isIpCameraSource;
        private bool _isWebcamSource;
        private IVideoSource _videoSource;
        private VideoFileWriter _writer;
        private bool _recording;
        private DateTime? _firstFrameTime;


        // konstruktor
        public MainWindowFunctions()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            GetVideoDevices();
            StartSourceCommand = new RelayCommand(StartCamera);
            StopSourceCommand = new RelayCommand(StopCamera);
            StartRecordingCommand = new RelayCommand(StartRecording);
            StopRecordingCommand = new RelayCommand(StopRecording);
            SaveSnapshotCommand = new RelayCommand(SaveSnapshot);
        }


        public ObservableCollection<FilterInfo> VideoDevices { get; set; }


        // bitmapa
        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }


        public bool IsDesktopSource
        {
            get { return _isDesktopSource; }
            set { Set(ref _isDesktopSource, value); }
        }

        public bool IsWebcamSource
        {
            get { return _isWebcamSource; }
            set { Set(ref _isWebcamSource, value); }
        }

        public bool IsIpCameraSource
        {
            get { return _isIpCameraSource; }
            set { Set(ref _isIpCameraSource, value); }
        }

        public string IpCameraUrl
        {
            get { return _ipCameraUrl; }
            set { Set(ref _ipCameraUrl, value); }
        }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { Set(ref _currentDevice, value); }
        }

        public bool Original
        {
            get { return _original; }
            set { _original = value; this.OnPropertyChanged("Original"); }
        }
        private bool _original;

        public bool Grayscaled
        {
            get { return _grayscale; }
            set { _grayscale = value; this.OnPropertyChanged("Grayscaled"); }
        }
        private bool _grayscale;

        public bool Thresholded
        {
            get { return _thresholded; }
            set { _thresholded = value; this.OnPropertyChanged("Thresholded"); }
        }
        private bool _thresholded;


        public int Threshold
        {
            get { return _threshold; }
            set { _threshold = value; this.OnPropertyChanged("Threshold"); }
        }
        private int _threshold;


        // komendy
        public ICommand StartRecordingCommand { get; private set; }
        public ICommand StopRecordingCommand { get; private set; }
        public ICommand StartSourceCommand { get; private set; }
        public ICommand StopSourceCommand { get; private set; }
        public ICommand SaveSnapshotCommand { get; private set; }


        // pobranie dostępnych urządzeń
        private void GetVideoDevices()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in devices)
            {
                VideoDevices.Add(device);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("No webcam found");
            }
        }


        // włączenie kamery
        private void StartCamera()
        {
            if (IsDesktopSource)
            {
                var rectangle = new Rectangle();
                foreach (var screen in System.Windows.Forms.Screen.AllScreens)
                {
                    rectangle = Rectangle.Union(rectangle, screen.Bounds);
                }
                _videoSource = new ScreenCaptureStream(rectangle);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();
            }
            else if (IsWebcamSource)
            {
                if (CurrentDevice != null)
                {
                    _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                    _videoSource.NewFrame += video_NewFrame;
                    _videoSource.Start();
                }
                else
                {
                    MessageBox.Show("Current device can't be null");
                }
            }
        }


        // wyłączenie kamery
        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= video_NewFrame;
            }
            Image = null;
        }


        // nowa klatka
        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (_recording)
                {
                    using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                    {
                        _writer.WriteVideoFrame(bitmap);

                        if (_firstFrameTime != null)
                        {
                            _writer.WriteVideoFrame(bitmap, DateTime.Now - _firstFrameTime.Value);
                        }
                        else
                        {
                            _writer.WriteVideoFrame(bitmap);
                            _firstFrameTime = DateTime.Now;
                        
                        }
                    }
                }

                // filtrowanie obrazu
                BitmapImage bi;
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    if (Grayscaled)
                    {
                        using (var grayscaledBitmap = Grayscale.CommonAlgorithms.BT709.Apply(bitmap))
                        {
                            bi = grayscaledBitmap.ToBitmapImage();
                        }
                    }
                    else if (Thresholded)
                    {
                        using (var grayscaledBitmap = Grayscale.CommonAlgorithms.BT709.Apply(bitmap))
                        using (var thresholdedBitmap = new Threshold(Threshold).Apply(grayscaledBitmap))
                        {
                            bi = thresholdedBitmap.ToBitmapImage();
                        }
                    }
                    else  // (Original)
                    {
                        bi = bitmap.ToBitmapImage();
                    }
                }
                bi.Freeze();
                Dispatcher.CurrentDispatcher.Invoke(() => Image = bi);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StopCamera();
            }
        }


        // włączenie nagrywania
        private void StartRecording()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "Video1";
            dialog.DefaultExt = ".avi";
            dialog.AddExtension = true;
            var dialogresult = dialog.ShowDialog();
            if (dialogresult != true)
            {
                return;
            }
            _firstFrameTime = null;
            _writer = new VideoFileWriter();
            _writer.Open(dialog.FileName, (int)Math.Round(Image.Width, 0), (int)Math.Round(Image.Height, 0));
            _recording = true;
        }


        // wyłączenie nagrywania
        private void StopRecording()
        {
            _recording = false;
            _writer.Close();
            _writer.Dispose();
        }


        // zapisanie obrazu
        private void SaveSnapshot()
        {
            var dialog = new SaveFileDialog();
            dialog.FileName = "Snapshot1";
            dialog.DefaultExt = ".png";
            var dialogresult = dialog.ShowDialog();
            if (dialogresult != true)
            {
                return;
            }
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Image));
            using (var filestream = new FileStream(dialog.FileName, FileMode.Create))
            {
                encoder.Save(filestream);
            }
        }


        // zmiana właściwości obrazu
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }


        public void Dispose()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
            }
            _writer?.Dispose();
        }
    }
}
