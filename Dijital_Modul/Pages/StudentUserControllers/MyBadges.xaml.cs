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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dijital_Modul.Pages.UserController
{
    /// <summary>
    /// Interaction logic for MyBadges.xaml
    /// </summary>
    public partial class MyBadges : UserControl
    {
        List<badge> badgeList = new List<badge>();
        General gnr = new General();

        List<question> cevapladigimSorular = new List<question>();

        List<badge> aldigimRozetler = new List<badge>();

        List<badge> alamadigimRozetler = new List<badge>();

        public MyBadges()
        {
            InitializeComponent();
           

            badgeList = gnr.studentMyAllBadges();
           

            foreach (var item in badgeList)
            {
                if (badgeControl(item.soruListesi))
                {
                    badge b = new badge();
                    b.soruListesi = item.soruListesi;
                    b.rozetID = item.rozetID;
                    b.rozetAdi = item.rozetAdi;
                    b.resim = item.resim;
                    b.eklemeZamani = item.eklemeZamani;
                    b.bitisSuresi = item.bitisSuresi;
                    b.aciklama = item.aciklama;
                    b.denemeImageByte = item.denemeImageByte;
                    aldigimRozetler.Add(b);
                }
                else
                {
                    badge b = new badge();
                    b.soruListesi = item.soruListesi;
                    b.rozetID = item.rozetID;
                    b.rozetAdi = item.rozetAdi;
                    b.resim = item.resim;
                    b.eklemeZamani = item.eklemeZamani;
                    b.bitisSuresi = item.bitisSuresi;
                    b.aciklama = item.aciklama;
                    b.denemeImageByte = item.denemeImageByte;
                    alamadigimRozetler.Add(b);
                }
            }

            Get_Badges();


        }



        void Get_Badges()
        {
           
            var imgBadge = @"../../Pages/Images/Rozetlerim.png";
            var alinmisRozet = @"../../Pages/Images/checked.png";
            // var alinmamisRozet = @"../../Pages/Images/wrong.png";

            foreach (var item in aldigimRozetler)
            {
                Grid grd = new Grid();
                grd.Height = 220;
                grd.Margin = new Thickness(10,0,0,0);

                Border brd = new Border();
                brd.Height = 200;
                brd.BorderThickness = new Thickness(6);
                brd.BorderBrush = Brushes.Green;
                brd.CornerRadius = new CornerRadius(10);


                DropShadowEffect effect = new DropShadowEffect();
                effect.Color = Colors.Black;
                effect.Direction = -1;
                effect.ShadowDepth = 1;

                brd.Effect = effect;


                TextBlock txbRozetBaslik = new TextBlock();
                txbRozetBaslik.Text = item.rozetAdi;
                txbRozetBaslik.FontSize = 16;
                txbRozetBaslik.VerticalAlignment = VerticalAlignment.Top;
                txbRozetBaslik.HorizontalAlignment = HorizontalAlignment.Center;
                txbRozetBaslik.Foreground = Brushes.Black;
                txbRozetBaslik.Margin = new Thickness(0,20,0,0);
                txbRozetBaslik.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);

               

                Image dynamicImage = new Image();
                dynamicImage.Width = 50;
                dynamicImage.Height = 50;
                dynamicImage.Margin = new Thickness(0, 60, 0, 0);
                dynamicImage.VerticalAlignment = VerticalAlignment.Top;
                dynamicImage.HorizontalAlignment = HorizontalAlignment.Center;

                // Create a BitmapSource  

                if (item.denemeImageByte != null)
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
                    bitmap.UriSource = new Uri(imgBadge, UriKind.Relative);
                    bitmap.EndInit();
                    dynamicImage.Source = bitmap;
                }



                Image imgsuccess = new Image();
                imgsuccess.Width = 30;
                imgsuccess.Height = 30;
                imgsuccess.VerticalAlignment = VerticalAlignment.Top;
                imgsuccess.HorizontalAlignment = HorizontalAlignment.Right;
                imgsuccess.Margin = new Thickness(0,20,10,0);

                // Create a BitmapSource  

                BitmapImage imgBitmap = new BitmapImage();
                imgBitmap.BeginInit();
                imgBitmap.UriSource = new Uri(alinmisRozet, UriKind.Relative);
                imgBitmap.EndInit();
                imgsuccess.Source = imgBitmap;


                TextBlock txbAciklama = new TextBlock();
                txbAciklama.Text = item.aciklama;
                txbAciklama.FontSize = 16;
                txbAciklama.VerticalAlignment = VerticalAlignment.Top;
                txbAciklama.HorizontalAlignment = HorizontalAlignment.Center;
                txbAciklama.Margin = new Thickness(0, 120, 0, 0);
                txbAciklama.Foreground = Brushes.Black;
                txbAciklama.TextWrapping = TextWrapping.Wrap;
                txbAciklama.Padding = new Thickness(10, 0, 10, 0);

                UniformGrid grdSorular = new UniformGrid();
                grdSorular.Columns = 3;
                grdSorular.Margin = new Thickness(0, 150, 0, 0);
                grdSorular.Height = 50;
                grdSorular.VerticalAlignment = VerticalAlignment.Top;
                grdSorular.HorizontalAlignment = HorizontalAlignment.Center;
                foreach (var soruAdi in item.soruListesi)
                {
                    TextBlock soruNo = new TextBlock();
                    soruNo.FontSize = 12;
                    soruNo.Inlines.Add(soruAdi);
                    soruNo.Foreground = Brushes.Green;
                    soruNo.Padding = new Thickness(3);
                    soruNo.VerticalAlignment = VerticalAlignment.Bottom;
                    soruNo.HorizontalAlignment = HorizontalAlignment.Center;
                    grdSorular.Children.Add(soruNo);
                }


                grd.Children.Add(grdSorular);



                grd.Children.Add(brd);
                grd.Children.Add(txbRozetBaslik);
                grd.Children.Add(dynamicImage);
                grd.Children.Add(imgsuccess);
                grd.Children.Add(txbAciklama);

                unf.Children.Add(grd);
            }

           


            foreach (var item in alamadigimRozetler)
            {
                Grid grd = new Grid();
                grd.Height = 200;
                grd.Margin = new Thickness(10, 0, 0, 0);

                Border brd = new Border();
                brd.Height = 180;
                brd.BorderThickness = new Thickness(2);
                brd.BorderBrush = Brushes.Black;
                brd.CornerRadius = new CornerRadius(10);
              

                TextBlock txbRozetBaslik = new TextBlock();
                txbRozetBaslik.Text = item.rozetAdi;
                txbRozetBaslik.FontSize = 16;
                txbRozetBaslik.VerticalAlignment = VerticalAlignment.Top;
                txbRozetBaslik.HorizontalAlignment = HorizontalAlignment.Center;
                txbRozetBaslik.Foreground = Brushes.Black;
                txbRozetBaslik.Margin = new Thickness(0, 20, 0, 0);
                txbRozetBaslik.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);



                Image dynamicImage = new Image();
                dynamicImage.Width = 50;
                dynamicImage.Height = 50;
                dynamicImage.Margin = new Thickness(0, 50, 0, 0);
                dynamicImage.VerticalAlignment = VerticalAlignment.Top;
                dynamicImage.HorizontalAlignment = HorizontalAlignment.Center;

                // Create a BitmapSource  

                if (item.denemeImageByte != null)
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
                    bitmap.UriSource = new Uri(imgBadge, UriKind.Relative);
                    bitmap.EndInit();
                    dynamicImage.Source = bitmap;
                }

                /*
                Image imgWrong = new Image();
                imgWrong.Width = 30;
                imgWrong.Height = 30;
                imgWrong.VerticalAlignment = VerticalAlignment.Top;
                imgWrong.HorizontalAlignment = HorizontalAlignment.Right;
                imgWrong.Margin = new Thickness(0,20,10,0);

                // Create a BitmapSource  

                BitmapImage imgBitmap = new BitmapImage();
                imgBitmap.BeginInit();
                imgBitmap.UriSource = new Uri(alinmamisRozet, UriKind.Relative);
                imgBitmap.EndInit();
                imgWrong.Source = imgBitmap;
                */

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
                grdSorular.Columns = 3;
                grdSorular.Margin = new Thickness(0, 130, 0, 0);
                grdSorular.Height = 50;
                grdSorular.VerticalAlignment = VerticalAlignment.Top;
                grdSorular.HorizontalAlignment = HorizontalAlignment.Center;
                foreach (var soruAdi in item.soruListesi)
                {
                    TextBlock soruNo = new TextBlock();
                    soruNo.FontSize = 12;
                    soruNo.Inlines.Add(soruAdi);
                    soruNo.Padding = new Thickness(3);
                    soruNo.VerticalAlignment = VerticalAlignment.Bottom;
                    soruNo.HorizontalAlignment = HorizontalAlignment.Center;
                    
                  
                    if (gnr.badgeQuesStatus(soruAdi, Prm.kullanici_No))
                    {
                        soruNo.Foreground = Brushes.Green;
                    }
                    else
                    {
                        soruNo.Foreground = Brushes.Black;
                    }

                    grdSorular.Children.Add(soruNo);
                }


                grd.Children.Add(grdSorular);



                grd.Children.Add(brd);
                grd.Children.Add(txbRozetBaslik);
                grd.Children.Add(dynamicImage);
               // grd.Children.Add(imgWrong);
                grd.Children.Add(txbAciklama);

                unf.Children.Add(grd);
            }

        }

        

        bool badgeControl(List<string> soruListesi)
        {
            cevapladigimSorular = gnr.Answered_Questions(Prm.kullanici_No, true);
            bool rozetDurum = true;

            List<string> cevaplarim = new List<string>();
            foreach (var cevap in cevapladigimSorular)
            {
                cevaplarim.Add(cevap.soruNo);
            }

            foreach (var soru in soruListesi)
            {
                if (!cevaplarim.Contains(soru))
                {
                    rozetDurum = false;
                    break;
                }
            }

            return rozetDurum;
        }



    }
}
