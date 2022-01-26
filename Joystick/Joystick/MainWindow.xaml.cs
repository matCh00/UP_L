using System;
using System.Windows;
using System.Threading;
using System.Runtime.InteropServices;

namespace Joystick
{
    public partial class MainWindow : Window
    {
        DirectInputHelper helper;
        readonly int progressBarMaxSize = 250;
        readonly int maxPadValue = 65535;
        private const int MOUSEEVENT_LEFTDOWN = 0x02;
        private const int MOUSEEVENT_LEFTUP = 0x04;


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


        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);


        private void moveCursor()
        {
            this.Dispatcher.Invoke(() =>
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(
                    System.Windows.Forms.Cursor.Position.X + (int)(4 * ((float)((2 * helper.valueX) - maxPadValue) / (maxPadValue * 3))),
                    System.Windows.Forms.Cursor.Position.Y + (int)(4 * ((float)((2 * helper.valueY) - maxPadValue) / (maxPadValue * 3))));

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


        public void buttonClickClear(object sender, EventArgs e)
        {
            inkCanvas1.Strokes.Clear();
        }
    }
}
