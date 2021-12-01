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
using System.IO;
using System.IO.Ports;
using System.Threading;


namespace Modem
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }


        // główne zmienne
        SerialPort serialPort;
        Thread reader;


// początek
///////////////////////////////////////////////////////////////////////////
// okno aplikacji



        // przycisk połączenie się z portem
        private void ConnectButton(object sender, RoutedEventArgs e)
        {
            if (portTextBox.Text != null)
            {
                Connect(portTextBox.Text);
            }
        }



        private void CallButton(object sender, RoutedEventArgs e)
        {
            string number = callTextBox.Text;

            serialPort.Write("ATDT " + number + Environment.NewLine);
        }



        private void PickUpButton(object sender, RoutedEventArgs e)
        {

            serialPort.Write("ATA" + Environment.NewLine);
        }



        // przycisk wysłanie wiadomości
        private void SendButton(object sender, RoutedEventArgs e)
        {
            string message;
            message = sendMessageTextBox.Text;

            Console.WriteLine("SENT MESSAGE: " + message);

            SendMessage(message);

            receiveMessageTextBox.Text += "Komputer2: ";
            receiveMessageTextBox.Text += message;
        }



// okno aplikacji
///////////////////////////////////////////////////////////////////////////
// modem



        // łączenie z modemem
        public void Connect(string COM)
        {
            Console.WriteLine(COM);

            if (serialPort != null)
                if (serialPort.IsOpen)
                    serialPort.Close();


            serialPort = new SerialPort(COM);


            if (serialPort != null)
                serialPort.Open();


            if (serialPort.IsOpen)
            {
                serialPort.DtrEnable = true;
                serialPort.Handshake = Handshake.RequestToSend;

                Console.WriteLine(serialPort.PortName);
                Console.WriteLine(serialPort.BaudRate);
                Console.WriteLine(serialPort.Parity);
                Console.WriteLine(serialPort.DataBits);
                Console.WriteLine(serialPort.StopBits);
                Console.WriteLine(serialPort.Handshake);
                Console.WriteLine(serialPort.DtrEnable);


                reader = new Thread(Read);
                reader.Start();
            }

            serialPort.Write("ATS0=1" + Environment.NewLine);
        }



        // wysyłanie wiadomości
        public void SendMessage(string message)
        {
            if (serialPort.IsOpen)
                serialPort.Write(message + Environment.NewLine);

            else
                Console.WriteLine("CONNECTING TO SERIAL PORT FAILED");
        }



        // ciągłe odbieranie wiadomości
        private void Read()
        {
            while (serialPort.IsOpen)
            {
                try
                {
                    string message = serialPort.ReadLine();
                    Console.Write(message);
                    receiveMessageTextBox.Text += message;
                }
                catch (TimeoutException) { }
            }
        }
    }
}
