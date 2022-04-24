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
    /// Interaction logic for ucTeacherMyClasses.xaml
    /// </summary>
    public partial class ucTeacherMyClasses : UserControl
    {
        General gnr = new General();
        List<classAndBranches> clsList = new List<classAndBranches>();
        int badgeID;
        public ucTeacherMyClasses(int _badgeID=0)
        {
            InitializeComponent();

            badgeID = _badgeID;
            clsList = gnr.getMyClassesAndBranches();

            foreach (var item in clsList)
            {
                Grid grd = new Grid();
                grd.Height = 50;
                grd.Margin = new Thickness(0,10,0,10);
                grd.MouseEnter += Grid_MouseEnter;
                grd.MouseLeave += Grid_MousLeave;
                grd.MouseDown += Grid_MouseDown;

                Border cerceve = new Border();
                cerceve.BorderBrush = Brushes.Gray;
                cerceve.Background = Brushes.Transparent;
                cerceve.CornerRadius = new CornerRadius(5);
                cerceve.BorderBrush = Brushes.Gray;
                cerceve.BorderThickness = new Thickness(2);

                TextBlock txbSinifSube = new TextBlock();
                txbSinifSube.Text = item.sinif + "/" + item.sube;
                txbSinifSube.Foreground = Brushes.DarkBlue;
                txbSinifSube.FontSize = 20;
                txbSinifSube.Padding = new Thickness(10);

                TextBlock txbOgrSayisi = new TextBlock();
                txbOgrSayisi.Text = item.adet.ToString() + " Öğrenci";
                txbOgrSayisi.VerticalAlignment = VerticalAlignment.Top;
                txbOgrSayisi.HorizontalAlignment = HorizontalAlignment.Right;
                txbOgrSayisi.FontSize = 20;
                txbOgrSayisi.Foreground = Brushes.Black;
                txbOgrSayisi.Padding = new Thickness(10);


                TextBlock txbSinifID = new TextBlock();
                txbSinifID.Text = item.sinifID.ToString();
                txbSinifID.Visibility = Visibility.Hidden;


                grd.Children.Add(cerceve);
                grd.Children.Add(txbSinifSube);
                grd.Children.Add(txbOgrSayisi);
                grd.Children.Add(txbSinifID);
                unf.Children.Add(grd);
            }
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
            brd.Background = Brushes.Transparent;
        }

        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            Grid grd = (Grid)sender;
            TextBlock txbSinifID = (TextBlock)grd.Children[3];
            uc_cagir.uc_Ekle(Prm.anaGrid, new ucTeacherMyStudents(1, Convert.ToInt32(txbSinifID.Text), Convert.ToInt32(Prm.okul_id)));


        }
    }
}
