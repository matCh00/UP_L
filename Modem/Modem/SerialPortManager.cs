using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace Modem
{
    class SerialPortManager
    {

        // główne zmienne
        SerialPort serialPort;
        Thread reader;


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
        }


        // pobieranie nazwy portu
        public List<String> GetCOM()
        {
            List<String> ComList = new List<String>();

            foreach (string s in SerialPort.GetPortNames())
            {
                ComList.Add(s);
            }

            ComList.Sort();
            return ComList;
        }


        // wysyłanie wiadomości
        public void SendMessage(string message)
        {
            if (serialPort != null)
                serialPort.Write(message);

            else
                Console.WriteLine("ERROR !! CONNECT TO SERIAL PORT");
        }


        // odebranie wiadomości
        public string ReadMessage()
        {
            return " ";
        }


        // odczyt
        private void Read()
        {
            while (serialPort.IsOpen)
            {
                try
                {
                    string message = serialPort.ReadExisting();
                    Console.Write(message);
                }
                catch (TimeoutException) { }
            }
        }


        // wysyłanie pliku
        public void SendFile(string filePath)
        {
            Console.WriteLine("PATH: " + filePath);

            if (serialPort.IsOpen)
            {
                using (FileStream fs = File.OpenRead(filePath))
                {
                    serialPort.Write((new BinaryReader(fs)).ReadBytes((int)fs.Length), 0, (int)fs.Length);
                }
            }
        }
    }
}
