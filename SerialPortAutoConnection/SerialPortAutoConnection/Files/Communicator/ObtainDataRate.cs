using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortAutoConnection
{
    public class ObtainDataRate
    {
        public static ObtainDataRate BoudRate = new ObtainDataRate(100000, 2000);
        #region Temp for obtain rate of  data recive from network Filds
        //public static ObtainDataRate BoudRate = new ObtainDataRate(100000, 2000);                       
        int ArrayLenth, MyCounter = 0, maxRefreshTime;
        int[] RecievedDataLenth;
        //System.UInt64[] RecievedPaketTime;
        double Totallengh, LastRate;
        DateTime[] RecievedDateTime;
        #endregion
        public double Data
        {
            set
            {
                //RecievedDateTime[MyCounter] = DateTime.Now;
                RecievedDataLenth[MyCounter++] = (int)value;
                if (MyCounter >= ArrayLenth || (RecievedDateTime[MyCounter - 1] - RecievedDateTime[0]).TotalMilliseconds > maxRefreshTime)
                {
                    Totallengh = 0;
                    for (int i = 0; i < MyCounter - 1; i++) Totallengh += RecievedDataLenth[i];
                    LastRate = Totallengh / (RecievedDateTime[MyCounter - 1] - RecievedDateTime[0]).TotalMilliseconds;
                    MyCounter = 0;
                }
            }
        }
        public double Rate
        {
            get { return LastRate; }
            set { LastRate = value; }
        }
        public DateTime Datetime
        {
            set { RecievedDateTime[MyCounter] = value; }
        }
        public ObtainDataRate(int RefreshCounter, int MaxRefreshTime)
        {
            ArrayLenth = RefreshCounter;
            maxRefreshTime = MaxRefreshTime;
            //RecievedPaketTime = new System.UInt64[ArrayLenth];
            RecievedDateTime = new DateTime[ArrayLenth];
            RecievedDataLenth = new int[ArrayLenth];
            Totallengh = 0;
            LastRate = 0;
        }
    }
}
