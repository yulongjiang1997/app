using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace App1.ViewModel
{
    public class Time : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public double Num1 { get; set; }
        public double Num2 { get; set; }
        public double Num3 { get; set; }
        public double Sum { get; set; }

        double n1, n2, n3;
        public Time()
        {
            n1 = Num1;
            n2 = Num2;
            n3 = Num3;
            Check();
        }

        void Check()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (n1 != Num1)
                    {
                        n1 = Num1;
                        NumSum();
                        PropertyChanged(this, new PropertyChangedEventArgs("Num1"));
                    }

                    if (n2 != Num2)
                    {
                        n2 = Num2;
                        NumSum();
                        PropertyChanged(this, new PropertyChangedEventArgs("Num2"));

                    }

                    if (n3 != Num3)
                    {
                        n3 = Num3;
                        NumSum();
                        PropertyChanged(this, new PropertyChangedEventArgs("Num3"));

                    }
                    Thread.Sleep(100);
                }
            });
        }

        void NumSum()
        {
            Sum = Num1 + Num2 + Num3;
            PropertyChanged(this, new PropertyChangedEventArgs("Sum"));
        }
    }
}
