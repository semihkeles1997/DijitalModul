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
using System.Windows.Shapes;

namespace Dijital_Modul.Pages.TeacherWindows
{
    /// <summary>
    /// Interaction logic for TeacherQuestionDetails.xaml
    /// </summary>
    public partial class TeacherQuestionDetails : Window
    {
        List<question> quesList = new List<question>();
        List<feedback> fbList = new List<feedback>();
        List<answer> ansList = new List<answer>();
        List<cevapVerenKullanici> cvkList = new List<cevapVerenKullanici>();
        List<student> cevapVermeyenKullanicilar = new List<student>();

        General gn = new General();
        int gelenSinifID;
        string gelenSoruNo;
        public TeacherQuestionDetails(string quesNo, int sinifID)
        {
            InitializeComponent();

            gelenSinifID = sinifID;
            gelenSoruNo = quesNo;


           cvkList = gn.SoruyaCevapVerenler(gelenSinifID, quesNo);
          
            cevapVermeyenKullanicilar = gn.SoruyaCevapVermeyenler(gelenSinifID,quesNo);

            string q = $@"Select s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, kb.Konu_Basligi_Adi 'Baslik', s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu,
                            s.Soru_Acik_Mi from sorular s inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID 
                                where s.Ekleyen in (0,{Prm.kullanici_ID}) and Soru_No='{quesNo}'";

            
          
                //string query = $"Select * from sorular where Soru_No='{quesNo}'";
                quesList = gn.A_Asking_Question(q);

                foreach (var item in quesList)
                {
                    txblHeader.Text = item.soruNo + ": " + item.baslik;
                    txblContent.Text = item.aciklama;
                    txbBaslik.Text = item.soruNo + " DETAY SAYFASI";

                if (item.imgArray != null)
                {
                    using (MemoryStream ms = new MemoryStream(item.imgArray))
                    {
                        try
                        {
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = ms;
                            image.EndInit();
                            image.Rotation = Rotation.Rotate270;
                            imgQues.Source = image;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Hata kodu: 02x0006 - {ex.GetType()}","Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
               


                if (item.soruAcikMi == false)
                    {
                        txbSoruDurum.Text = "SORU KAPALI";
                        txbSoruDurum.Foreground = Brushes.Red;
                    }
                    else
                    {
                        txbSoruDurum.Text = "SORU AÇIK";
                        txbSoruDurum.Foreground = Brushes.Green;
                    }

                }

                if (cvkList.Count > 0)
                {
                    foreach (var item in cvkList)
                    {
                        Grid grd = new Grid();
                        grd.Width = 150;
                        grd.Height = 150;
                        grd.Background = Brushes.Transparent;
                        grd.Margin = new Thickness(0, 30, 0, 0);
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
                        txbAdSoyad.Text = item.ogrenci.ad + " " + item.ogrenci.soyad;
                        txbAdSoyad.Foreground = Brushes.DarkBlue;
                        txbAdSoyad.FontSize = 12;
                        txbAdSoyad.VerticalAlignment = VerticalAlignment.Top;
                        txbAdSoyad.HorizontalAlignment = HorizontalAlignment.Center;
                        txbAdSoyad.TextWrapping = TextWrapping.Wrap;
                        txbAdSoyad.Margin = new Thickness(0, 65, 0, 0);

                        TextBlock txbOgrNo = new TextBlock();
                        txbOgrNo.Text = item.ogrenci.kullaniciNo.ToString();
                        txbOgrNo.Foreground = Brushes.Black;
                        txbOgrNo.FontSize = 20;
                        txbOgrNo.VerticalAlignment = VerticalAlignment.Top;
                        txbOgrNo.HorizontalAlignment = HorizontalAlignment.Center;
                        txbOgrNo.TextWrapping = TextWrapping.Wrap;
                        txbOgrNo.Margin = new Thickness(0, 20, 0, 0);

                        TextBlock txbCevapVermeSuresi = new TextBlock();
                        txbCevapVermeSuresi.Text = item.cevap.cevapGondermeSuresi;
                        txbCevapVermeSuresi.Foreground = Brushes.Gray;
                        txbCevapVermeSuresi.FontSize = 12;
                        txbCevapVermeSuresi.VerticalAlignment = VerticalAlignment.Top;
                        txbCevapVermeSuresi.HorizontalAlignment = HorizontalAlignment.Center;
                        txbCevapVermeSuresi.TextWrapping = TextWrapping.Wrap;
                        txbCevapVermeSuresi.Margin = new Thickness(0, 110, 0, 0);




                        grd.Children.Add(cerceve);
                        grd.Children.Add(txbAdSoyad);
                        grd.Children.Add(txbOgrNo);
                        grd.Children.Add(txbCevapVermeSuresi);
                        unfCevapEkleyenler.Children.Add(grd);
                    }
                    txbCevapVerenYok.Visibility = Visibility.Hidden;
                }
                else
                {
                    txbCevapVerenYok.Visibility = Visibility.Visible;
                }

                if (cevapVermeyenKullanicilar.Count > 0)
                {
                    txbCevapVermeyenYok.Visibility = Visibility.Hidden;
                    foreach (var item in cevapVermeyenKullanicilar)
                    {
                        Grid grd = new Grid();
                        grd.Width = 150;
                        grd.Height = 150;
                        grd.Background = Brushes.Transparent;
                        grd.Margin = new Thickness(0, 30, 0, 0);
                        grd.MouseEnter += Grid_MouseEnter;
                        grd.MouseLeave += Grid_MousLeave;


                        Border cerceve = new Border();
                        cerceve.Width = 150;
                        cerceve.Height = 150;
                        cerceve.BorderBrush = Brushes.Gray;
                        cerceve.Background = Brushes.LightGray;
                        cerceve.BorderThickness = new Thickness(5);
                        cerceve.CornerRadius = new CornerRadius(75);

                        TextBlock txbAdSoyad = new TextBlock();
                        txbAdSoyad.Text = item.ad + " " + item.soyad;
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




                        grd.Children.Add(cerceve);
                        grd.Children.Add(txbAdSoyad);
                        grd.Children.Add(txbOgrNo);
                        unfCevapEklemeyenler.Children.Add(grd);
                    }
                }
                else
                {
                    txbCevapVermeyenYok.Visibility = Visibility.Visible;
                }
            
            

  



        }

        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            
            Grid grd = (Grid)sender;
            TextBlock txbOgrNo = (TextBlock)grd.Children[2];
            TeacherStudentAnswerFeedback tsaf = new TeacherStudentAnswerFeedback(txbOgrNo.Text, gelenSoruNo);
            tsaf.Owner = Prm.gk;
            tsaf.ShowDialog();
            this.Close();
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

        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnMouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void btnMouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
