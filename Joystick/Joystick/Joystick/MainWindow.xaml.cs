using System;
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
using System.Drawing;
using System.Threading;
using SharpDX.DirectInput;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace Joystick
{
    public partial class MainWindow : Window
    {

        DirectInputHelper helper;
        int progressBarMaxSize = 250;
        int maxPadValue = 65535;
        Graphics g;
        private const int MOUSEEVENT_LEFTDOWN = 0x02;
        private const int MOUSEEVENT_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHT_DOWN = 0x08;
        private const int MOUSEEVENTF_RIGHT_UP = 0x10;
        int? prevX = null;
        int? prevY = null;


        public MainWindow()
        {
            InitializeComponent();

            helper = new DirectInputHelper();
            label4.Width = (int)label4.Width;
            label4.Height = 0;
            label5.Width = 0;
            label5.Height = (int)label5.Height;
            label7.Width = 0;
            label7.Height = (int)label5.Height;

            //pictureBox1.BackColor = System.Drawing.Color.White;
            //g = pictureBox1.CreateGraphics();

            // emulacja myszy
            //var inputMonitor = new ControllerAsMouse();
            //inputMonitor.Start();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);


        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };


        public static System.Drawing.Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new System.Drawing.Point(w32Mouse.X, w32Mouse.Y);
        }


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);


        private void moveCursor()
        {
            this.Dispatcher.Invoke(() =>
            {
                /*this.Cursor = new Cursor(Cursor.Current.Handle);
                Cursor.Position = new System.Drawing.Point(Cursor.Position.X + (int)(4 * ((float)(2 * helper.valueX - maxPadValue) / maxPadValue)),
                Cursor.Position.Y + (int)(4 * ((float)(2 * helper.valueY - maxPadValue) / maxPadValue)));*/
                //Cursor.Clip = new Rectangle(this.Location, this.Size);
            });
        }

        private void updateProgress()
        {
            while (true)
            {
                moveCursor();
                if (helper.isAClicked)
                    mouse_event(MOUSEEVENT_LEFTDOWN, helper.valueX, helper.valueY, 0, 0);
                else
                    mouse_event(MOUSEEVENT_LEFTUP, helper.valueX, helper.valueY, 0, 0);

                // oś Y
                label4.Dispatcher.Invoke(() =>
                {
                    label4.Width = (int)label4.Width;
                    label4.Height = Convert.ToInt32(progressBarMaxSize * (Convert.ToDouble(helper.valueY) / maxPadValue));
                    label4.Background = System.Windows.Media.Brushes.White;
                });

                // oś X
                label5.Dispatcher.Invoke(() =>
                {
                    label5.Width = Convert.ToInt32(progressBarMaxSize * (Convert.ToDouble(helper.valueX) / maxPadValue));
                    label5.Height = (int)label5.Height;
                    label5.Background = System.Windows.Media.Brushes.White;
                });

                // przycisk A
                checkBox1.Dispatcher.Invoke(() =>
                {
                    checkBox1.IsChecked = helper.isAClicked;
                });

                // przycisk B
                checkBox2.Dispatcher.Invoke(() =>
                {
                    checkBox2.IsChecked = helper.isBClicked;
                });

                // przycisk X
                checkBox3.Dispatcher.Invoke(() =>
                {
                    checkBox3.IsChecked = helper.isXClicked;
                });

                // przycisk Y
                checkBox4.Dispatcher.Invoke(() =>
                {
                    checkBox4.IsChecked = helper.isYClicked;
                });

                // suwak
                label7.Dispatcher.Invoke(() =>
                {
                    label7.Width = 250 - Convert.ToInt32(progressBarMaxSize * (Convert.ToDouble(helper.valueTriggers) / maxPadValue));
                    label7.Height = (int)label7.Height;
                });
            }
        }


        private void findGamepad(object sender, EventArgs e)
        {
            helper.connectGamepad();
            textBox1.Text = helper.getGamepadName();
            Thread thread = new Thread(updateProgress);
            thread.Start();
        }


        private void picture_MouseDown(object sender, MouseEventArgs e)
        {
            /*System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Color.Black, float.Parse("4"));
            g.DrawLine(p, new System.Drawing.Point(prevX ?? e.X, prevY ?? e.Y), new System.Drawing.Point(e.X, e.Y));
            prevX = e.X;
            prevY = e.Y;*/
        }

        private void picture_MouseUp(object sender, MouseEventArgs e)
        {
            prevX = null;
            prevY = null;
        }


        public void buttonClickClear(object sender, EventArgs e)
        {
            inkCanvas1.Strokes.Clear();
        }
    }
}
