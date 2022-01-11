using System;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SerialPortAutoConnection
{
    class Communicator_SerialPort_Check
    {
        #region Param
        public static int MyBaudRate = 9600;
        public static int MyDataBite = 8;
        public static int allSerialPortCounter = 0;
        public static string currentSerialPort = "";
        public static string currentState = "";

        public static Stopwatch SW = new Stopwatch();
        public static bool StopThread_FindStatusSerial = false;
        public static List<string> List_SerilPortName = new List<string>();
        #endregion
        #region enum StatusSerialState
        public enum StatusSerialState
        {
            findSerials, searchInAllSerials, listenToSerial, conected, searchInLastSerials, connectionChecker
        };
        #endregion
        #region Func: FindActiveSerail
        public static void FindActiveSerail()
        {
            StatusSerialState SerialState = StatusSerialState.findSerials;
            while (!StopThread_FindStatusSerial)
            {
                switch (SerialState)
                {
                    #region findSerials_State
                    case StatusSerialState.findSerials:
                        {
                            currentState = "Find serials";
                            List_SerilPortName = Communicator_SerialPort.GetAllPorts();
                            allSerialPortCounter = 0;
                            if (Properties.Settings.Default.Last_SerialPort != "")
                            {
                                currentSerialPort = Properties.Settings.Default.Last_SerialPort;
                                SerialState = StatusSerialState.searchInLastSerials;
                                currentState = "Searching";

                            }
                            else if (List_SerilPortName.Count > 0)
                            {
                                SerialState = StatusSerialState.searchInAllSerials;
                                currentState = "Searching";
                            }
                        }
                        break;
                    #endregion
                    #region searchInLastSerials_State
                    case StatusSerialState.searchInLastSerials:
                        {
                            SerialState = StatusSerialState.listenToSerial;
                            currentState = "Listen to serials";
                            Communicator_SerialPort.StopThread = false;
                            //Communicator_SerialPort.StatPackCounter = 0;
                            SerialPort SelSerialPort = new SerialPort(currentSerialPort, MyBaudRate, Parity.None, MyDataBite, StopBits.One);
                            Communicator_SerialPort.Instance.Create_Thread_CommunicatorSerial(SelSerialPort);
                            SW.Restart();
                        }
                        break;
                    #endregion
                    #region searchInAllSerials_State
                    case StatusSerialState.searchInAllSerials:
                        {
                            currentSerialPort = List_SerilPortName[allSerialPortCounter];
                            allSerialPortCounter++;
                            SerialState = StatusSerialState.listenToSerial;
                            currentState = "Listen to serials";
                            Communicator_SerialPort.StopThread = false;
                            Communicator_SerialPort.StatPackCounter = 0;
                            SerialPort SelSerialPort = new SerialPort(currentSerialPort, MyBaudRate, Parity.None, MyDataBite, StopBits.One);
                            Communicator_SerialPort.Instance.Create_Thread_CommunicatorSerial(SelSerialPort);
                            SW.Restart();
                        }
                        break;
                    #endregion
                    #region listenToSerial_State
                    case StatusSerialState.listenToSerial:
                        {
                            if (Communicator_SerialPort.StatPackCounter >= 1)
                            {
                                SerialState = StatusSerialState.conected;
                                currentState = "Conected";

                            }
                            else if (SW.Elapsed.TotalMilliseconds > 5000)
                            {
                                Communicator_SerialPort.StopThread = true;
                                Communicator_SerialPort.Instance.Close();
                                Communicator_SerialPort.Instance.IsOpen = false;
                                if (allSerialPortCounter >= List_SerilPortName.Count)
                                {
                                    SerialState = StatusSerialState.findSerials;
                                    currentState = "Find serials";
                                }
                                else
                                {
                                    SerialState = StatusSerialState.searchInAllSerials;
                                    currentState = "Searching";
                                }
                            }
                        }
                        break;
                    #endregion
                    #region conected_State
                    case StatusSerialState.conected:
                        {
                            Properties.Settings.Default.Last_SerialPort = currentSerialPort;
                            Properties.Settings.Default.Save();
                            currentState = "Connect";

                            SerialState = StatusSerialState.connectionChecker;
                            //StopThread_FindStatusSerial = true;
                        }
                        break;
                    #endregion
                    #region connectionChecker
                    case StatusSerialState.connectionChecker:
                        {
                            if ((Communicator_SerialPort.StatPackCounter > 0))
                            {
                                Communicator_SerialPort.StatPackCounter = 0;
                                Thread.Sleep(15000);
                                SerialState = StatusSerialState.connectionChecker;
                                currentState = "Connect";
                            }
                            else
                            {
                                currentState = "Searching";
                                Communicator_SerialPort.StopThread = true;
                                Communicator_SerialPort.Instance.Close();
                                Communicator_SerialPort.Instance.IsOpen = false;
                                SerialState = StatusSerialState.findSerials;
                            }
                        }
                        break;
                    #endregion
                    default:
                        break;
                }
                Thread.Sleep(100);
            }
        }
        #endregion
    }

}
