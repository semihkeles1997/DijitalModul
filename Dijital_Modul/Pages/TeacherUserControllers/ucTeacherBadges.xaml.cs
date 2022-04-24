using Dijital_Modul.Pages.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for ucTeacherBadges.xaml
    /// </summary>
    public partial class ucTeacherBadges : UserControl
    {
        List<badge> badgeList = new List<badge>();
        General gnr = new General();
        public ucTeacherBadges()
        {
            InitializeComponent();
            badgeList = gnr.teacherMyAllBadges();
            badgeList.Count();
         
            var imgBadge = "https://dijitalmodul.com/Gorseller/rozet.png"; //  @"../../Pages/Images/Rozetlerim.png";
            foreach (var item in badgeList)
            {
                Grid grd = new Grid();
                grd.Height = 200;
                grd.Width = 220;
                grd.Margin = new Thickness(5, 0, 0, 0);
                grd.MouseEnter += Grid_MouseEnter;
                grd.MouseLeave += Grid_MousLeave;
                grd.MouseDown += Grid_MouseDown;


                Border brd = new Border();
                brd.Height = 180;
                brd.BorderThickness = new Thickness(2);
                brd.BorderBrush = Brushes.Black;
                brd.CornerRadius = new CornerRadius(10);
                grd.Children.Add(brd);

                TextBlock txbRozetBaslik = new TextBlock();
                txbRozetBaslik.Text = item.rozetAdi;
                txbRozetBaslik.FontSize = 16;
                txbRozetBaslik.VerticalAlignment = VerticalAlignment.Top;
                txbRozetBaslik.HorizontalAlignment = HorizontalAlignment.Center;
                txbRozetBaslik.Foreground = Brushes.Black;
                txbRozetBaslik.Margin = new Thickness(0, 20, 0, 0);
                txbRozetBaslik.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);

                TextBlock txbRozetID = new TextBlock();
                txbRozetID.Text = item.rozetID.ToString();
                txbRozetID.Visibility = Visibility.Hidden;
                grd.Children.Add(txbRozetID);


                TextBlock txbAciklama = new TextBlock();
                txbAciklama.Text = item.aciklama;
                txbAciklama.FontSize = 16;
                txbAciklama.VerticalAlignment = VerticalAlignment.Top;
                txbAciklama.HorizontalAlignment = HorizontalAlignment.Center;
                txbAciklama.Margin = new Thickness(0, 100, 0, 0);
                txbAciklama.Foreground = Brushes.Black;
                txbAciklama.TextWrapping = TextWrapping.Wrap;
                txbAciklama.Padding = new Thickness(5, 0, 5, 0);


                UniformGrid grdSorular = new UniformGrid();
                grdSorular.Columns = 4;
                grdSorular.Margin = new Thickness(0, 130, 0, 0);
                grdSorular.Height = 50;
                grdSorular.VerticalAlignment = VerticalAlignment.Center;
                grdSorular.HorizontalAlignment = HorizontalAlignment.Center;
                foreach (var soruAdi in item.soruListesi)
                {
                    TextBlock soruNo = new TextBlock();
                    soruNo.FontSize = 12;
                    soruNo.Inlines.Add(soruAdi);
                    soruNo.Padding = new Thickness(3);
                    soruNo.VerticalAlignment = VerticalAlignment.Bottom;
                    soruNo.HorizontalAlignment = HorizontalAlignment.Center;
                    soruNo.Foreground = Brushes.Black;
                    grdSorular.Children.Add(soruNo);
                }


                grd.Children.Add(grdSorular);


                Image dynamicImage = new Image();
                dynamicImage.Width = 50;
                dynamicImage.Height = 50;
                dynamicImage.Margin = new Thickness(0, 50, 0, 0);
                dynamicImage.VerticalAlignment = VerticalAlignment.Top;
                dynamicImage.HorizontalAlignment = HorizontalAlignment.Center;

          
                


                if (item.denemeImageByte !=null)
                {
                    using (MemoryStream ms = new MemoryStream(item.denemeImageByte))
                    {
                        try
                        {
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();
                            image.Rotation = Rotation.Rotate270;
                            dynamicImage.Source = image;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Hata kodu: 02x0004 - {ex.GetType()}", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
                else
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imgBadge);
                    bitmap.EndInit();
                    dynamicImage.Source = bitmap;
                }


                grd.Children.Add(dynamicImage);
                grd.Children.Add(txbAciklama);
                grd.Children.Add(txbRozetBaslik);
               
                unf.Children.Add(grd);

            }
        }

        private void btnYeniRozetEkle_Click(object sender, RoutedEventArgs e)
        {
            uc_cagir.uc_Ekle(Prm.anaGrid, new ucTeacherAddBadge());
        }

        private void btnYeniRozetEkle_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void btnYeniRozetEkle_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            Grid grd = (Grid)sender;
            TextBlock txbRozetID = (TextBlock)grd.Children[1];
            uc_cagir.uc_Ekle(Prm.anaGrid, new ucBadges_Students(Convert.ToInt32(txbRozetID.Text)));
            
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            Grid grd = (Grid)sender;
            Border brd = (Border)grd.Children[0];
            brd.Background = Brushes.Cyan;
            //grd.Background = Brushes.Cyan;
        }

        private void Grid_MousLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Grid grd = (Grid)sender;
            //grd.Background = Brushes.LightGray;
            Border brd = (Border)grd.Children[0];
            brd.Background = Brushes.Transparent;
        }

    }
}
