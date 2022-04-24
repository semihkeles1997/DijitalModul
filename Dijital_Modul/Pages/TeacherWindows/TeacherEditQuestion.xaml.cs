using Dijital_Modul.Pages.Class;
using Microsoft.Win32;
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
    /// Interaction logic for TeacherEditQuestion.xaml
    /// </summary>
    public partial class TeacherEditQuestion : Window
    {
        public string gelenSoruNo;
        List<question> question = new List<question>();
        List<konubasligi> konuBasliklari = new List<konubasligi>();
        General gnr = new General();
        string eskiResimAdi = "";
        public TeacherEditQuestion(string soruNo)
        {
            InitializeComponent();

           
            gelenSoruNo = soruNo;
            konuBasliklari = gnr.KonuBasliklariniGetir();
            // txtSoruNo.Text = gelenSoruNo;
            string sorgu = $" select s.Soru_No, s.Resim,s.ResimSize ,s.Aciklama, s.Max_Puan, s.Acilma_Zamani, s.Ekleme_Zamani, " +
                $"kb.Konu_Basligi_Adi 'Baslik', s.Soru_Turu, s.Soru_Acik_Mi from sorular s inner join Konu_Basliklari  kb on kb.ID = s.Konu_Basliklari_ID where s.Soru_No='{soruNo}'";
            question = gnr.A_Asking_Question(sorgu);
            dateEklemeZamani.IsEnabled = false;
            txtEklemeSaati.IsEnabled = false;
            txtEklemeDakikasi.IsEnabled = false;
            txtSoruNo.IsEnabled = false;

            foreach (var item in konuBasliklari)
            {
                cmbKonuBasligi.Items.Add(item.kbAdi);
            }

            Button btnYeniKonuBasligi = new Button();
            btnYeniKonuBasligi.Content = "Yeni Konu Başlığı Ekle";
            btnYeniKonuBasligi.BorderThickness = new Thickness(2);
            btnYeniKonuBasligi.BorderBrush = Brushes.DarkBlue;
            btnYeniKonuBasligi.Margin = new Thickness(0, 10, 0, 10);
            btnYeniKonuBasligi.Padding = new Thickness(10);
            btnYeniKonuBasligi.VerticalAlignment = VerticalAlignment.Bottom;
            btnYeniKonuBasligi.HorizontalAlignment = HorizontalAlignment.Right;
            btnYeniKonuBasligi.Width = cmbKonuBasligi.Width;
            cmbKonuBasligi.Items.Add(btnYeniKonuBasligi);


            // 00 - 59 arası dakika
            for (int i = 0; i < 60; i++)
            {
                if (i == 0)
                {
                    for (int a = 0; a < 10; a++)
                    {
                        cmbAcilmaDakikasi.Items.Add(i + "" + a);
                    }
                    i = 10;
                }
                else
                {
                    cmbAcilmaDakikasi.Items.Add(i);
                }
            }


            // 00 - 23 arası saat
            for (int i = 0; i < 24; i++)
            {
                if (i == 0)
                {
                    for (int a = 0; a < 10; a++)
                    {
                        cmbAcilmaSaati.Items.Add(i + "" + a);
                    }
                    i = 10;
                }
                else
                {
                    cmbAcilmaSaati.Items.Add(i);
                }
            }


            foreach (var item in question)
            {
                txtSoruNo.Text = item.soruNo;
                txtAciklama.Text = item.aciklama;
                txtMaxPuan.Text = item.maxPuan.ToString();
            
                if (item.soruAcikMi)
                {
                    checkSoruAcikMi.IsChecked = true;
                }
                dateAcilmaZamani.SelectedDate = item.acilmaZamani;
                dateEklemeZamani.SelectedDate = item.eklemeZamani;
                cmbKonuBasligi.SelectedItem = item.baslik;
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
                            imgSoruResmi.Source = image;
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.ToString());
                        }
                    }
                }
                if (!string.IsNullOrEmpty(item.acilmaZamani.ToString()))
                {
                    DateTime acilmaZamani = item.acilmaZamani.Value;
                    if (acilmaZamani.Hour < 10)
                    {
                        string sec = "0" + acilmaZamani.Hour;
                        cmbAcilmaSaati.SelectedItem = sec;
                    }
                    else
                    {
                        cmbAcilmaSaati.SelectedItem = acilmaZamani.Hour;
                    }
                    if (acilmaZamani.Minute < 10)
                    {
                        string sec = "0" + acilmaZamani.Minute;
                        cmbAcilmaDakikasi.SelectedItem = sec;
                    }
                    else
                    {
                        cmbAcilmaDakikasi.SelectedItem = acilmaZamani.Minute;
                    }
                }

                DateTime eklemeZamani = item.eklemeZamani.Value;
                txtEklemeSaati.Text = eklemeZamani.Hour.ToString();
                if (eklemeZamani.Minute < 10)
                {
                    txtEklemeDakikasi.Text = "0" + eklemeZamani.Minute.ToString();
                }
                else
                {
                    txtEklemeDakikasi.Text = eklemeZamani.Minute.ToString();
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

        string SecilenResimAdi = "";
        string resimAdi = "";
        
        private void btnResimEkle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Belgelerim klasöründe StokTakipProgrami ve içinde Resimler klasörü yoksa oluştur diyoruz. Varsa zaten aşağıdaki işlemleri yapacak.
                if (!Directory.Exists(Prm.BelgelerimYolu + "/DijitalModul/SoruResimleri"))
                {
                    Directory.CreateDirectory(Prm.BelgelerimYolu.Replace('\\','/') + "/DijitalModul/SoruResimleri");  // Belgelerimin içine Resimler adlı klasör oluşturuyoruz.
                }

                // OpenFileDialog ile resim seçme işlemi yapıyoruz.
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Resim seç";
                dialog.InitialDirectory = "";  // Pencere açıldığında ilk olarak karşımıza hangi pencere gelsin onu seçiyoruz. Direkt Belgelerimi seçebiliriz yani.
                dialog.Filter = "Image Files (*.jpg;*.jpeg;)|*.jpg;*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";  // Seçilecek dosyayı filtreliyoruz. Resim seçmek istediğimiz için.
                dialog.FilterIndex = 1;
                if ((bool)dialog.ShowDialog())  // Seçim işlemi başarılı ise buraya girecek.
                {
                    // Openfile dialog ile seçilen resmi oluşturduğumuz klasör içerisine kopyalama işlemi.
                    SecilenResimAdi = dialog.FileName;
               
                    // Belgelerim içindeki resmimizin yolunu uriye çevirip img_UrunResmi yoluna verme.
                    BitmapImage img = new BitmapImage();  // Resmi anlık olarak değiştirmek için kullanıyoruz.
                    img.BeginInit();
                    img.UriSource = new Uri(@"" + SecilenResimAdi);
                    img.EndInit();
                    imgSoruResmi.Source = img;   // resmi burada değiştiriyoruz.
                }
                else
                {
                    MessageBox.Show("Resim eklenirken bir sorun oldu!","Uyarı",MessageBoxButton.OK,MessageBoxImage.Warning);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 02x0005 - {ex.GetType()}", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            DateTime eklemeZamaniDuzenle = dateEklemeZamani.SelectedDate.Value;
            string duzenlenmisEklemeZamani = eklemeZamaniDuzenle.Year + "-" + eklemeZamaniDuzenle.Month + "-" + eklemeZamaniDuzenle.Day;

            List<konubasligi> kb = gnr.KonuBasliklariniGetir(cmbKonuBasligi.Text);
            int kbID = 0;
            foreach (var a in kb)
            {
                kbID = a.kbID;
            }

            question ques = new question();
            ques.soruNo = txtSoruNo.Text;
            ques.imgArray = new byte[0];
            if (!SecilenResimAdi.Equals(""))
            {
                ques.imgArray = gnr.converSmallerImageByteArray(SecilenResimAdi);
            }
            ques.aciklama = txtAciklama.Text;
            ques.maxPuan = Convert.ToInt32(txtMaxPuan.Text);
            ques.soruAcikMi = checkSoruAcikMi.IsChecked.Value;

            DateTime secilenAcilmaZamani = Convert.ToDateTime(dateAcilmaZamani.SelectedDate);
            string acilmaZamaniDuzenlenmis = secilenAcilmaZamani.ToString("yyyy-MM-dd") + " " + cmbAcilmaSaati.Text + ":" + cmbAcilmaDakikasi.Text + ":00";
            
            foreach (var item in question)
            {
                DateTime quesEklemezamani = Convert.ToDateTime(item.eklemeZamani);
                DateTime quesAcilmaZamani = Convert.ToDateTime(item.acilmaZamani);



                if (txtSoruNo.Text == item.soruNo && txtAciklama.Text == item.aciklama && txtMaxPuan.Text == item.maxPuan.ToString() &&
                   secilenAcilmaZamani.ToString("yyyy-MM-dd") == Convert.ToDateTime(item.acilmaZamani).ToString("yyyy-MM-dd") &&
                   cmbAcilmaSaati.SelectedItem.ToString() == quesAcilmaZamani.ToString("HH") &&
                   cmbAcilmaDakikasi.SelectedItem.ToString() == quesAcilmaZamani.ToString("mm") &&
                   cmbKonuBasligi.SelectedItem.ToString() == item.baslik && checkSoruAcikMi.IsChecked == item.soruAcikMi && SecilenResimAdi.Equals(""))
                {
                    MessageBox.Show("Hiçbir şey değişmedi!", "Değişiklik olmadı!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (gnr.TeacherUpdateQuestion(ques, kbID, acilmaZamaniDuzenlenmis))
                    {
                        MessageBox.Show( $"{ques.soruNo} sorusu başarıyla güncellendi!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();

                    }
                    else
                    {
                        MessageBox.Show("Soru güncellenirken bir hata oldu!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

            }
        }


       
    }
}


