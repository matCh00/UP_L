// logika aplikacji

using System;
using System.Windows;
using System.Windows.Controls;

namespace WPF_demo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            StartFunction();
        }
        
        
        // pierwsza funkcja
        public void StartFunction()
        {
            AnswerTextBox.Text = "Hello World!";
        }
        
        
        // funkcje do przycisków - wygenerowana z pliku .xaml
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            // przycisk który został kliknięty
            var button = sender as Button;

            
            // akcje po naciśnięciu danego przycisku
            switch (button.Name)
            {
                case "PowerOnCardButton":
                    AnswerTextBox.Text = "PowerOnCardButton";
                    break;
                
                case "TestButton":
                    AnswerTextBox.Text += "TestButton";
                    break;
                
                case "CloseButton":
                    AnswerTextBox.Clear();
                    break;
                
                case "TransmitCommandButton":
                    AnswerTextBox.Text = CommandTextBox.Text;
                    break;
                
                default: break;
            }
        }
    }
}