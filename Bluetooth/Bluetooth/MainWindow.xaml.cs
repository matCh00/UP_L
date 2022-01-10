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
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using Microsoft.Win32;
using System.ComponentModel;




namespace Bluetooth
{
    public partial class MainWindow : Window
    {

        private BluetoothDeviceInfo[] devices;
        private bool isPaired = false;
        private BluetoothDeviceInfo deviceToPair = null;
        private List<BluetoothDeviceInfo> connected = new List<BluetoothDeviceInfo>();


        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonFind_Click(object sender, EventArgs e)
        {
            //czyszczenie listy urządzeń
            listBoxDevices.Items.Clear();

            //wyłączneie przycisku na czas wyszukania
            buttonFind.IsEnabled = false;


            //wyszukanie urządzeń i zrobienie z nich listy
            var bluetoothClient = new BluetoothClient();
            devices = bluetoothClient.DiscoverDevices().ToArray();

            //włączenie przycisku po zakończeniu wyszukiwania i wypisamniu
            //wypisanie do text boxa
            foreach (BluetoothDeviceInfo device in devices)
            {
                listBoxDevices.Items.Add(device.DeviceName);
            }

            buttonFind.IsEnabled = true;

        }

        private void buttonPair_Click(object sender, EventArgs e)
        {
            //wybieramy urządzenie z listy
            foreach (BluetoothDeviceInfo device in devices)
            {
                if (device.DeviceName.Equals(listBoxDevices.SelectedItem))
                    deviceToPair = device;
            }

            //nawiżaywanie połączneia z wybranym urządzeniem
            //deviceToPair.Update();
            deviceToPair.Refresh();
            deviceToPair.SetServiceState(BluetoothService.ObexObjectPush, true);

            //wysłanie podłączenia
            string pin = "1234";
            isPaired = BluetoothSecurity.PairRequest(deviceToPair.DeviceAddress, pin);

            //wypisanie wszystkich sparowanych urządzeń
            if (isPaired)
            {
                connected.Add(deviceToPair);
                listBoxConnected.Items.Clear();
                foreach (var device in connected)
                    listBoxConnected.Items.Add(device.DeviceName);
            }
        }

        private void buttonUnpair_Click(object sender, EventArgs e)
        {
            BluetoothSecurity.RemoveDevice(connected[listBoxConnected.SelectedIndex].DeviceAddress);

            connected.RemoveAt(listBoxConnected.SelectedIndex);
            listBoxConnected.Items.Clear();
            foreach (var device in connected)
                listBoxConnected.Items.Add(device.DeviceName);

            isPaired = false;
            deviceToPair = null;
        }

        private void buttonSendFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.FileName = "openFileDialog";
            openFileDialog.FileOk += new CancelEventHandler(openFileDialog_FileOk);

            openFileDialog.ShowDialog();
        }

        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            Task.Run(() =>
            {
                sendFileMethod(sender, this);
            }
            );
        }

        private void sendFileMethod(object sender, MainWindow window)
        {
            OpenFileDialog dialog = (OpenFileDialog)sender;
            Console.WriteLine(dialog.ToString());
            var address = deviceToPair.DeviceAddress;

            SendFile(address, dialog.FileName);
        }

        public static ObexStatusCode SendFile(BluetoothAddress address, string file_path)
        {
            var obexUri = new Uri("obex://" + address + "/" + file_path);

            ObexWebRequest request = new ObexWebRequest(obexUri);

            ObexWebResponse response = null;

            request.ReadFile(file_path);
            try
            {
                response = (ObexWebResponse)request.GetResponse();
                response.Close();
                MessageBox.Show("Plik wysłany \n Kod odpowiedzi: " + response.StatusCode.ToString());
                return response.StatusCode;
            }
            catch (Exception e)
            {
                MessageBox.Show("Nie udało się wysłać pliku:\n" + e.Message + " " + e.ToString());
            }


            return response.StatusCode;
        }

        private void buttonSendFile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
