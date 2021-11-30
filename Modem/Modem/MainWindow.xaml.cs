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


namespace Modem
{
    public partial class MainWindow : Window
    {

        // manager portów
        SerialPortManager portManager;

        public MainWindow()
        {
            InitializeComponent();
        }


        // załadowanie portów
        private void Ports(object sender, RoutedEventArgs e)
        {
            List<String> ComList = portManager.GetCOM();

            portsComboBox.Items.Clear();
            portsComboBox.ItemsSource = ComList;
        }


        // połączenie się z portem
        private void Connect(object sender, RoutedEventArgs e)
        {
            if (portsComboBox.SelectedItem != null)
            {
                portManager.Connect(portsComboBox.SelectedItem.ToString());
            }
        }


        // wysłanie wiadomości
        private void Send(object sender, RoutedEventArgs e)
        {
            string message;
            message = sendMessageTextBox.Text;

            Console.WriteLine("SENT MESSAGE: " + message);

            portManager.SendMessage(message);
        }


        // odebranie wiadomości
        private void Receive(object sender, RoutedEventArgs e)
        {
            string message;
            message = portManager.ReadMessage();

            Console.WriteLine("RECEIVED MESSAGE: " + message);
        }
    }
}
