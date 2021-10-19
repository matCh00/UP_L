using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using PCSC;
using PCSC.Exceptions;
using PCSC.Utils;


namespace WpfLab1
{
    public partial class App : Application
    {

        // zmienne statyczne 
        private static SCardError error;
        private static SCardReader reader;
        private static SCardContext context;
        private static System.IntPtr protocol;

    }
}