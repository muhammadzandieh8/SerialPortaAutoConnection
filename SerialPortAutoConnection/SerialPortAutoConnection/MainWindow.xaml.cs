using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace SerialPortAutoConnection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<string> List_SerilPortName = new List<string>();
        public DispatcherTimer _timer;

        public MainWindow()
        {

            InitializeComponent();
            #region Connect To Automaticaly
            Thread ThreadConnectToAutomaticaly = new Thread(Communicator_SerialPort_Check.FindActiveSerail);
            ThreadConnectToAutomaticaly.Start();
            #endregion
            #region Update UI
            Thread UpdateUIThread = new Thread(UpdateUI);
            UpdateUIThread.Start();
            #endregion
            #region Timer
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Set_up_timer;
            _timer.Start();
            #endregion
            Parser parser = new Parser();
            parser.CreateThread();
        }
        public void UpdateUI()
        {
            while (true)
            {
                this.Dispatcher.Invoke((Action)delegate
                {
                    lbl_SelectedCom.Content = Communicator_SerialPort_Check.currentSerialPort;
                    lbl_ConnectionState.Content = Communicator_SerialPort_Check.currentState;
                    lbl_Buadrate.Content = Communicator_SerialPort_Check.MyBaudRate;

                    lbl_Byte1.Content = Parser.Byte1;
                    lbl_Byte2.Content = Parser.Byte2;
                    lbl_Byte3.Content = Parser.Byte3;
                });
                Thread.Sleep(500);
            }
        }
        public void Set_up_timer(object sender, EventArgs eventArgs)
        {
            lbl_DataRate.Content = (Communicator_SerialPort.TotalBoudRate.Rate).ToString("####0.0##") + " KB";//KB/s
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
