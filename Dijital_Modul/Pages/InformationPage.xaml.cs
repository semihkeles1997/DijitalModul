using Dijital_Modul.Pages.Class;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Dijital_Modul.Pages
{
    /// <summary>
    /// Interaction logic for InformationPage.xaml
    /// </summary>
    public partial class InformationPage : Window
    {
        public InformationPage()
        {
            InitializeComponent();
        }


        void Hata()
        {
            if (Prm.Hata == 0)
            {
                olumlu_bilgi.Visibility = Visibility.Visible;
                img_Olumlu.Visibility = Visibility.Visible;
                BilgiMesajiAlani.Text = Prm.BilgiMesajiAlani;
                olumsuz_bilgi.Visibility = Visibility.Hidden;
                img_Olumsuz.Visibility = Visibility.Hidden;
                BilgiEkraniBaslik.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("Green"));
                BilgiMesajiAlani.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF134187"));
            }
            else
            {
                olumlu_bilgi.Visibility = Visibility.Hidden;
                img_Olumlu.Visibility = Visibility.Hidden;
                BilgiMesajiAlani.Text = Prm.BilgiMesajiAlani;
                olumsuz_bilgi.Visibility = Visibility.Visible;
                img_Olumsuz.Visibility = Visibility.Visible;
                BilgiEkraniBaslik.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                BilgiMesajiAlani.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF40000"));
            }


            // 7 saniye sonra kapan
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timer.Tick += delegate (object sender, EventArgs e)
            {
                ((DispatcherTimer)timer).Stop();
                if (this.ShowActivated)
                {
                    this.Close();
                }
            };
            timer.Start();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;  // Bilgi ekranını sağ alta almak için kullanıyoruz.
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;

            Hata();
        }
    }
}
