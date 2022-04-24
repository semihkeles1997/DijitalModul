using Dijital_Modul.Pages.Class;
using Dijital_Modul.Pages.TeacherUserControllers;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Dijital_Modul.Pages.TeacherWindows
{
    /// <summary>
    /// Interaction logic for TeacherStudentAnswerFeedback.xaml
    /// </summary>
    public partial class TeacherStudentAnswerFeedback : Window
    {
        List<answer> cevapList = new List<answer>();
        List<question> soruList = new List<question>();
        General gnr = new General();
        student ogrenciBilgileri = new student();
        List<feedback> ogrenciDonut = new List<feedback>();
        int maxPuan;
        int  gelenKullaniciNo, gelenCevapID;
        string gelenSoruNo;
        bool pop_status = true;
        public TeacherStudentAnswerFeedback(string kullaniciNo, string soruNo)
        {
            InitializeComponent();

            anwer_pdf_viwer.ItemSource = "MetotlarOgretmen.pdf";

            string q = $@"Select s.Soru_No, s.Resim,s.ResimSize ,s.Aciklama, s.Max_Puan, kb.Konu_Basligi_Adi 'Baslik', s.Acilma_Zamani, s.Ekleme_Zamani, s.Soru_Turu,
                            s.Soru_Acik_Mi from sorular s inner join Konu_Basliklari kb on kb.ID = s.Konu_Basliklari_ID 
                                where s.Ekleyen in (0,{Prm.kullanici_ID}) and Soru_No='{soruNo}'";
            soruList = gnr.A_Asking_Question(q);
            ogrenciBilgileri = gnr.TekilOgrenciGetir(kullaniciNo);
            gelenSoruNo = soruNo;
            gelenKullaniciNo = Convert.ToInt32(kullaniciNo);
            ogrenciDonut = gnr.A_Asking_FeedBack(soruNo, kullaniciNo);


            txbOgrBilgileri.Text = $"{ogrenciBilgileri.sinif}/{ogrenciBilgileri.sube} - {ogrenciBilgileri.ad.ToUpper()} {ogrenciBilgileri.soyad.ToUpper()} [{ogrenciBilgileri.kullaniciNo}]";
            txbBaslik.Text = soruNo + " DETAY SAYFASI";
            cevapList = gnr.A_Asking_Answer(soruNo, kullaniciNo);

            txbDonutZamani.IsEnabled = false;
           



            if (ogrenciDonut.Count>0)
            {
                txtDonut.IsEnabled = false;
                txtPuan.IsEnabled = false;
                txbDonutZamani.Visibility = Visibility.Visible;
                btnDuzenle.Visibility = Visibility.Visible;
                btnDonutuGonder.Visibility = Visibility.Collapsed;
                foreach (var item in ogrenciDonut)
                {
                    txtDonut.Text = item.donut;
                    txtPuan.Text = item.puan.ToString();
                    txbDonutZamani.Text = item.donutZamani.ToString();
                    txtDonut.Foreground = Brushes.Green;
                    txtDonut.Background = Brushes.Transparent;
                    btnDonutuGonder.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                txtDonut.IsEnabled = true;
                txtPuan.IsEnabled = true;
                txbDonutZamani.Visibility = Visibility.Hidden;
                btnDonutuGonder.Visibility = Visibility.Visible;
                btnDuzenle.Visibility = Visibility.Collapsed;
                txbCevapGondermeSuresi.Visibility = Visibility.Hidden;
                txbCevap.Text = "";

                
            }


            foreach (var item in soruList)
            {
                txblHeader.Text = item.soruNo + ": " + item.baslik;
                txblContent.Text = item.aciklama;
                txbBaslik.Text = item.soruNo + " DETAY SAYFASI";
                maxPuan = Convert.ToInt32(item.maxPuan);
                txbMaxPuan.Text = "/ "+maxPuan.ToString();

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
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }
                }
            }

            if (cevapList.Count<=0)
            {
                dckAnswerFeedback.Visibility = Visibility.Collapsed;
            }

            foreach (var item in cevapList)
            {
                txbCevap.Text = item.cevap;
                txbCevapGondermeSuresi.Text = $" Gönderme süresi: {item.cevapGondermeSuresi}";
                //txbCevapGondermeSuresi.Text = $" Gönderme süresi: {item.cevapGondermeSuresi} \n Soru açıldı: {item.soruAcilmaZamani}\n Cevaplandı: {item.cevapGondermeZamani}";
                gelenCevapID = Convert.ToInt32(item.answerID);


                if (!string.IsNullOrEmpty(item.puan.ToString()))
                {
                    txtPuan.Text = item.puan.ToString();
                    txtPuan.Foreground = Brushes.Green;
                    txtPuan.Background = Brushes.Transparent;
                    btnDonutuGonder.Visibility = Visibility.Collapsed;
                    btnDuzenle.Visibility = Visibility.Visible;
                }
                else
                {
                    btnDonutuGonder.Visibility = Visibility.Visible;
                    btnDuzenle.Visibility = Visibility.Collapsed;
                }
            }


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
        int puan;
        private void txtPuan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtPuan.Text.Length>0)
            {
                puan = Convert.ToInt32(txtPuan.Text);
                if (puan>maxPuan)
                {
                    txtPuan.Background = Brushes.Red;
                }
                else
                {
                    txtPuan.Background = Brushes.Transparent;
                }
            }
            else
            {
                txtPuan.Background = Brushes.Transparent;
            }
            
        }
        private void txtPuan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextNumeric(e.Text);
        }
        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return reg.IsMatch(str);
        }
        private void btnDuzenle_Click(object sender, RoutedEventArgs e)
        {
            btnDonutuGonder.Visibility = Visibility.Visible;
            btnDuzenle.Visibility = Visibility.Collapsed;
            txtDonut.IsEnabled = true;
            txtPuan.IsEnabled = true;
            txtDonut.Background = Brushes.White;
            txtPuan.Background = Brushes.White;
        }

        private void pop_upClick(object sender, RoutedEventArgs e)
        {
            if (pop_status)
            {
                anwer_pdf_viwer.Visibility = Visibility.Collapsed;
                pop_status = false;
            }
            else
            {
                anwer_pdf_viwer.Visibility = Visibility.Visible;
                pop_status = true;
            }
        }

        private void btnDonutuGonder_Click(object sender, RoutedEventArgs e)
        {
            if (txtPuan.Text.Length>0)
            {
                if (puan <= maxPuan)
                {
                    if (gnr.DonutEkle(gelenSoruNo, gelenCevapID, txtDonut.Text, gelenKullaniciNo, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"),Prm.kullanici_No, puan))
                    {
                        MessageBox.Show("Dönüt ve puan başarıyla eklendi","Başarılı",MessageBoxButton.OK,MessageBoxImage.Information);
                        this.Close();
                        uc_cagir.uc_Ekle(Prm.anaGrid, new ucTeacher_StudentProgress(gelenKullaniciNo.ToString()));
                    }
                    else
                    {
                        MessageBox.Show("Dönüt ve puan eklenirken bir sorun oldu!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    
                    }
                }
                else
                {
                    MessageBox.Show("Maksimum puandan daha fazla puan verilemez!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                 
                }
            }
            else
            {
                MessageBox.Show("Puan boş olamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            
            }
            
        }
    }
}
