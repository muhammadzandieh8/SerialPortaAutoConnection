using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SerialPortAutoConnection
{
    public class Parser
    {

        #region enum ParseState
        public enum ParseState
        {
            ReadData, Header, ShiftData, Data
        }
        #endregion
        public static int LengthPacket = 5;
        public static Byte[] data = new byte[LengthPacket];

        public static int Byte1;
        public static int Byte2;
        public static int Byte3;

        public void CreateThread()
        {
            Thread myThread = new Thread(Func);
            myThread.Start();
        }

        public void Func()
        {
            SerialByte SB = new SerialByte();
            ParseState CurrentState = ParseState.Header;
            ParseState LastState = ParseState.Header;
            int i = 0;
            int CountRead = 0;
            int i_CountRead = 0;

            while (true)
            {
                switch (CurrentState)
                {
                    #region Header
                    case ParseState.Header:
                        {
                            if (i == 0)
                            {
                                CountRead = 1;
                                i_CountRead = 0;
                                CurrentState = ParseState.ReadData;
                                LastState = ParseState.Header;
                                break;
                            }
                            if (data[0] == 185)/*Check Header*/
                            {
                                CurrentState = ParseState.Data;

                            }
                            else
                            {
                                CurrentState = ParseState.ShiftData;
                            }

                        }
                        break;
                    #endregion
                    #region Data
                    case ParseState.Data:
                        {
                            if (i == 1)
                            {
                                CountRead = 4;
                                i_CountRead = 0;
                                CurrentState = ParseState.ReadData;
                                LastState = ParseState.Data;
                                break;
                            }
                            if (data[LengthPacket - 1] == 85)/*Check Footer*/
                            {
                                Communicator_SerialPort.StatPackCounter++;
                                i = 1;
                                Byte1 = data[i];
                                i++;
                                Byte2 = data[i];
                                i++;
                                Byte3 = data[i];

                                i = 0;
                                CurrentState = ParseState.Header;
                                LastState = ParseState.Header;
                            }
                            else
                            {
                                CurrentState = ParseState.ShiftData;
                            }
                        }
                        break;
                    #endregion
                    #region ReadData
                    case ParseState.ReadData:
                        {
                            if (i_CountRead < CountRead)
                            {
                                if (Communicator_SerialPort.Q_Byte.DataCount > 0)
                                {
                                    SB = Communicator_SerialPort.Q_Byte.Data;
                                    data[i++] = SB.Byte;
                                    i_CountRead++;
                                }
                                else
                                {
                                    Thread.Sleep(10);
                                }
                            }
                            else
                            {
                                CurrentState = LastState;
                            }
                        }
                        break;
                    #endregion
                    #region ShiftData
                    case ParseState.ShiftData:
                        {
                            i--;

                            for (int k = 1; k < LengthPacket; k++)
                            {
                                data[k - 1] = data[k];
                            }
                            CountRead = 1;
                            i_CountRead = 0;
                            CurrentState = ParseState.ReadData;
                            LastState = ParseState.Header;
                        }
                        break;
                    #endregion
                    default:
                        break;
                }
            }
        }
    }

}
