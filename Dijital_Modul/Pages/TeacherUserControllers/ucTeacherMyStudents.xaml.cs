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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dijital_Modul.Pages.TeacherUserControllers
{
    /// <summary>
    /// Interaction logic for ucTeacherMyStudents.xaml
    /// </summary>
    public partial class ucTeacherMyStudents : UserControl
    {
        General gnr = new General();
        List<student> studentList = new List<student>();
        public ucTeacherMyStudents(int secim, int sinifID = 0, int okulID = 0)
        {
            InitializeComponent();

            //var imgBadge = @"../../Pages/Images/Student_Images/user.png";

            if (secim == 0) // Öğretmenin sadece 1 sınıfı varsa
            {
                studentList = gnr.getMyStudents();
            }
            else // Öğretmenin birden fazla sınıfı varsa seçtiği sınıfın listesi
            {
                studentList = gnr.getMyStudents(sinifID, okulID);
            }

            // MessageBox.Show(studentList.Count().ToString());

            foreach (var item in studentList)
            {
                Grid grd = new Grid();
                grd.Width = 150;
                grd.Height = 150;
                grd.Background = Brushes.Transparent;
                grd.Margin = new Thickness(0, 20, 0, 0);
                grd.MouseEnter += Grid_MouseEnter;
                grd.MouseLeave += Grid_MousLeave;
                grd.MouseDown += Grid_MouseDown;

                Border cerceve = new Border();
                cerceve.Width = 150;
                cerceve.Height = 150;
                cerceve.BorderBrush = Brushes.Gray;
                cerceve.Background = Brushes.LightGray;
                cerceve.BorderThickness = new Thickness(5);
                cerceve.CornerRadius = new CornerRadius(75);

                TextBlock txbAdSoyad = new TextBlock();
                txbAdSoyad.Text = item.ad.ToUpper() + " " + item.soyad.ToUpper();
                txbAdSoyad.Foreground = Brushes.DarkBlue;
                txbAdSoyad.FontSize = 12;
                txbAdSoyad.VerticalAlignment = VerticalAlignment.Bottom;
                txbAdSoyad.HorizontalAlignment = HorizontalAlignment.Center;
                txbAdSoyad.TextWrapping = TextWrapping.Wrap;
                txbAdSoyad.Margin = new Thickness(0, 65, 0, 65);

                TextBlock txbOgrNo = new TextBlock();
                txbOgrNo.Text = item.kullaniciNo.ToString();
                txbOgrNo.Foreground = Brushes.Black;
                txbOgrNo.FontSize = 20;
                txbOgrNo.VerticalAlignment = VerticalAlignment.Top;
                txbOgrNo.HorizontalAlignment = HorizontalAlignment.Center;
                txbOgrNo.TextWrapping = TextWrapping.Wrap;
                txbOgrNo.Margin = new Thickness(0, 20, 0, 0);

                TextBlock txbSinifSube = new TextBlock();
                txbSinifSube.Text = item.sinif + "/" + item.sube.ToUpper();
                txbSinifSube.Foreground = Brushes.Black;
                txbSinifSube.FontSize = 20;
                txbSinifSube.VerticalAlignment = VerticalAlignment.Bottom;
                txbSinifSube.HorizontalAlignment = HorizontalAlignment.Center;
                txbSinifSube.TextWrapping = TextWrapping.Wrap;
                txbSinifSube.Margin = new Thickness(0, 110, 0, 20);


                grd.Children.Add(cerceve);
                grd.Children.Add(txbAdSoyad);
                grd.Children.Add(txbOgrNo);
                grd.Children.Add(txbSinifSube);
                unf.Children.Add(grd);
            }
        }

        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            Grid grd = (Grid)sender;
            TextBlock txbOgrNo = (TextBlock)grd.Children[2];
            uc_cagir.uc_Ekle(Prm.anaGrid, new ucTeacher_StudentProgress(txbOgrNo.Text));
           // MessageBox.Show($"Öğrenci No: {txbOgrNo.Text}");
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            Grid grd = (Grid)sender;
            Border brd = (Border)grd.Children[0];
            brd.Background = Brushes.Cyan;
        }

        private void Grid_MousLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Grid grd = (Grid)sender;
            Border brd = (Border)grd.Children[0];
            brd.Background = Brushes.LightGray;
        }

    }
}
