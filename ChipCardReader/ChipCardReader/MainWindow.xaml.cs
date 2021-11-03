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
using PCSC;
using PCSC.Exceptions;
using PCSC.Utils;

namespace ChipCardReader
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


            while (true)
            {
                try
                {
                    ConnectReader();
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Nie udało się połączyć, wiadomość błędu:" + e.Message);
                }
            }
        }


        private static SCardError err;
        private static SCardReader reader;
        private static System.IntPtr protocol;
        private static SCardContext context;
        bool Connected = false;
        string command = "";
        byte[] byteCommand;


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // przycisk który został kliknięty
            var button = sender as Button;


            // akcje po naciśnięciu danego przycisku
            switch (button.Name)
            {
                case "PowerOnCardButton":
                    try
                    {
                        ConnectReader();
                        break;
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine("Nie udało się połączyć, wiadomość błędu:" + ee.Message);
                    }
                    break;


                case "TestButton":
                    break;


                case "CloseButton":
                    break;


                case "TransmitCommandButton":

                    // pobranie tekstu
                    string stringCommand = CommandTextBox.Text;

                    // konwersja na tablicebajtów
                    byte[] byteCommand = Encoding.ASCII.GetBytes(stringCommand);


                    switch (CommandTextBox.Text)
                    {
                        case "SELECT TELCOM":
                            /*try
                            {
                                ConnectReader();
                                break;
                            }
                            catch (Exception eE)
                            {
                                AnswerTextBox.Text = "Nie udało się połączyć, wiadomość błędu:" + eE.Message;
                            }

                            byteCommand = new byte[] { 0xA0, 0xA4, 0x00, 0x00, 0x02, 0x7F, 0x10 };
                            sendCommand(byteCommand, "SELECT TELECOM");

                            byteCommand = new byte[] { 0xA0, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0x3A };
                            var response2 = sendCommand(byteCommand, "SELECT TELECOM");

                            byteCommand = new byte[] { 0xA0, 0xC0, 0x00, 0x00, response2[1] };
                            var response3 = sendCommand(byteCommand, "SELECT TELECOM");*/
                            break;



                        case "SELECT SMS":
                            //byteCommand = new byte[] { 0xA0, 0xA4, 0x00, 0x00, 0x02, 0x3F, 0x00 };
                            //sendCommand(byteCommand, "MASTER FILE");

                            byteCommand = new byte[] { 0xA0, 0xA4, 0x00, 0x00, 0x02, 0x7F, 0x10 };
                            sendCommand(byteCommand, "SELECT TELECOM");

                            byteCommand = new byte[] { 0xA0, 0xC0, 0x00, 0x00, 0x20 };
                            sendCommand(byteCommand, "GET RESPONSE");

                            byteCommand = new byte[] { 0xA0, 0xA4, 0x00, 0x00, 0x02, 0x3F, 0x00 };
                            sendCommand(byteCommand, "SELECT MASTER FILE");

                            byteCommand = new byte[] { 0xA0, 0xC0, 0x00, 0x00, 0x20 };
                            sendCommand(byteCommand, "GET RESPONSE");

                            byteCommand = new byte[] { 0xA0, 0xA4, 0x00, 0x00, 0x02, 0x6F, 0x3C };
                            var response = sendCommand(byteCommand, "SELECT SMS");

                            byteCommand = new byte[] { 0xA0, 0xC0, 0x00, 0x00, 0x20 };
                            sendCommand(byteCommand, "GET RESPONSE");

                            byteCommand = new byte[] { 0xA0, 0xB2, 0x00, 0x04, 0xB0 };
                            sendCommand(byteCommand, "READ RECORD");


                            foreach (byte bb in response)
                            {
                                if (bb > 0x19 && bb < 0x7B)
                                {
                                    char anwser = Convert.ToChar(bb);
                                    AnswerTextBox.Text = anwser.ToString();
                                }
                                else
                                {
                                    AnswerTextBox.Text = "{0:X2} " + bb;
                                }
                            }
                            break;



                        case "READ RECORD":
                            /* byteCommand = new byte[] { 0xA0, 0xC0, 0x00, 0x00, 0x20 };
                             sendCommand(byteCommand, "GET RESPONSE");*/

                            byteCommand = new byte[] { 0xA0, 0xB2, 0x00, 0x04, 0xB0 };
                            sendCommand(byteCommand, "READ RECORD");
                            byteCommand = new byte[] { 0xA0, 0xB2, 0x01, 0x04, 0xB0 };
                            sendCommand(byteCommand, "READ RECORD");
                            byteCommand = new byte[] { 0xA0, 0xB2, 0x02, 0x04, 0xB0 };
                            sendCommand(byteCommand, "READ RECORD");
                            byteCommand = new byte[] { 0xA0, 0xB2, 0x03, 0x04, 0xB0 };
                            sendCommand(byteCommand, "READ RECORD");
                            break;

                        default:
                            break;
                    }
                    break;

                default: break;
            }
        }



        public void ConnectReader()
        {
            //defincja kontekstu   
            context = new SCardContext();

            // przekazanie kontekstu
            context.Establish(SCardScope.System);

            // lista czytników
            string[] readerList = context.GetReaders();

            // lista czytników jest pusta
            Boolean emptyList = readerList.Length <= 0;


            if (emptyList)
            {
                // wyjatek
                throw new PCSCException(SCardError.NoReadersAvailable, "Blad czytnika");
            }

            // definicja czytnika
            reader = new SCardReader(context);

            // bład
            err = reader.Connect(readerList[0], SCardShareMode.Shared, SCardProtocol.T0 | SCardProtocol.T1);

            // sprawdza przypisanie
            checkError(err);

            // wybór protokołu 
            switch (reader.ActiveProtocol)
            {
                case SCardProtocol.T0:
                    protocol = SCardPCI.T0;
                    break;

                case SCardProtocol.T1:
                    protocol = SCardPCI.T1;
                    break;

                default:
                    throw new PCSCException(SCardError.ProtocolMismatch, "nieobslugiwany protokol: " + reader.ActiveProtocol.ToString());
            }
            Connected = true;
        }


        // sprawdzenie prawidłowosci przypisania
        void checkError(SCardError err)
        {
            if (err != SCardError.Success)
            {
                throw new PCSCException(err, SCardHelper.StringifyError(err));
            }

        }



        public byte[] sendCommand(byte[] comand, String name)
        {
            // komendy z CommandTextBox

            // byte to string
            //AnswerTextBox.Text = BitConverter.ToString(comand); 

            byte[] recivedBytes = new byte[256];

            //przesyła dane APDU do karty 
            err = reader.Transmit(protocol, comand, ref recivedBytes);

            checkError(err);

            //sprawdza czy wystąpił błąd
            writeResponse(recivedBytes, name);

            return recivedBytes;
        }


        public void writeResponse(byte[] recivedBytes, String responseCode)
        {
            // odpowiedx w AnswerTextBox

            // napisanie odpowiedzi w polu tekstowym
            AnswerTextBox.Text = responseCode + ": ";

            for (int i = 0; i < recivedBytes.Length; i++)
            {
                AnswerTextBox.Text = "{0:X2} " + recivedBytes[i];
            }
            AnswerTextBox.Text = "\n";
        }
    }
}
