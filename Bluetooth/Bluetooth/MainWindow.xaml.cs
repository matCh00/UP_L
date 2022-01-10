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

        /*
         
        BTHelper helper;


        public MainWindow()
        {
            InitializeComponent();
            helper = new BTHelper();
        }



        private void searchAdapterButton_Click(object sender, EventArgs e)
        {
            helper.refreshAdapters();
            adaptersListBox.Items.Clear();
            if (helper.btAdapters.Length != 0)
            {
                foreach (var device in helper.btAdapters)
                    adaptersListBox.Items.Add(device.Name);
            }

        }



        private void adaptersInfoButton_Click(object sender, EventArgs e)
        {
            if (helper.chosenRadio != null)
                helper.displayAdapterInfo();

        }



        private void adaptersListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            helper.chosenRadio = helper.btAdapters[adaptersListBox.SelectedIndex];
        }



        private void searchDevicesButton_Click(object sender, EventArgs e)
        {
            devicesListBox.Items.Clear();
            if (helper.chosenRadio != null)
            {
                helper.refreshDevices();
                foreach (var device in helper.btDevices)
                {
                    devicesListBox.Items.Add(device.DeviceName.ToString());
                }
            }
        }



        private void devicesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            helper.chosenDevice = helper.btDevices[devicesListBox.SelectedIndex];
        }



        private void deviceInfoButton_Click(object sender, EventArgs e)
        {
            if (helper.chosenRadio != null)
                if (helper.chosenDevice != null)
                    helper.displayDeviceInfo();
        }



        private void connectButton_Click(object sender, EventArgs e)
        {
            if (helper.chosenDevice != null)
                helper.connectDevice();
        }



        private void sendFileButton_Click(object sender, EventArgs e)
        {
            helper.sendFile();
        }
        
        */

        
         
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
            openFileDialog.ShowDialog();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
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
        
        


    }
}
