using Dijital_Modul.Pages.Class;
using Microsoft.Win32;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;



namespace Dijital_Modul.Pages.TeacherUserControllers
{
    /// <summary>
    /// Interaction logic for ucTeacherAddQuestion.xaml
    /// </summary>
    public partial class ucTeacherAddQuestion : UserControl
    {
      
        private General gn;
        List<konubasligi> konuBasligiListesi;
        List<classAndBranches> clsList = new List<classAndBranches>();
        string questionImagePath = "";
        public ucTeacherAddQuestion()
        {
            InitializeComponent();
            gn = new General();
            cmbKonuBasligi.Items.Add( "Lütfen bir ünite başlığı   seçiniz");
            cmbKonuBasligi.SelectedIndex = 0;
            konuBasligiListesi = gn.KonuBasliklariniGetir();
            foreach (konubasligi kb in konuBasligiListesi)
            {
                cmbKonuBasligi.Items.Add(kb.kbAdi);
            }
            clsList = gn.getMyClassesAndBranches();
            cmbSinif.Items.Add("Lütfen bir sınıf seçiniz");
            cmbSinif.SelectedIndex = 0;
            foreach (var item in clsList)
            {
                cmbSinif.Items.Add(item.sinif + "/" + item.sube);
            }
            
            dateAcilmaZamani.DisplayDateStart = DateTime.Now.Date;

            
        }

        private void btnResimEkle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Belgelerim klasöründe StokTakipProgrami ve içinde Resimler klasörü yoksa oluştur diyoruz. Varsa zaten aşağıdaki işlemleri yapacak.
                if (!Directory.Exists(Prm.BelgelerimYolu + "/DijitalModul/SoruResimleri"))
                {
                    Directory.CreateDirectory(Prm.BelgelerimYolu.Replace('\\', '/') + "/DijitalModul/SoruResimleri");  // Belgelerimin içine Resimler adlı klasör oluşturuyoruz.
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
                   
                    DateTime zaman = DateTime.Now;
                    string format = "dd-MM-yyyy-hh-mm-ss";
                    Random rst = new Random();

                    questionImagePath = dialog.FileName;
                    // Belgelerim içindeki resmimizin yolunu uriye çevirip img_UrunResmi yoluna verme.

                    BitmapImage img = new BitmapImage();  // Resmi anlık olarak değiştirmek için kullanıyoruz.
                    img.BeginInit();
                    img.UriSource = new Uri(@"" + questionImagePath);
                    img.EndInit();
                    questionImage.Source = img;   // resmi burada değiştiriyoruz.
                }
              


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 02x0003 - {ex.GetType()}", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

     

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string questionNo = txtSoruNo.Text.ToString();
            int subjectTitleIndex = cmbKonuBasligi.SelectedIndex;
            int classroomIndex = cmbSinif.SelectedIndex;
            string promotion = txtAciklama.Text.ToString();
            string maxPuan = txtMaxPuan.Text.ToString();
            DatePicker date = dateAcilmaZamani;
            bool on_off_now = cmbHemenAc.IsChecked.Value;


            if (subjectTitleIndex > 0 && classroomIndex > 0 && !maxPuan.Equals("") && !promotion.Equals("") && !questionNo.Equals("") )
            {
                string acilmaZamani = date.SelectedDate.Value.ToString("yyyy-MM-dd H:m:s");

                byte[] qustionImageByte = new byte[] { };
                
                if (!questionImagePath.Equals(""))
                {
                    qustionImageByte = gn.converSmallerImageByteArray(questionImagePath);
                }
                int subjectTitle_ID = konuBasligiListesi[subjectTitleIndex - 1].kbID;
                int class_ID = clsList[classroomIndex - 1].sinifID;
                bool check = gn.addQuestion(questionNo,qustionImageByte,promotion,Convert.ToInt32(maxPuan),acilmaZamani,on_off_now,subjectTitle_ID,true,1,class_ID);
                if (check)
                {
                    MessageBox.Show("Soru kaydedildi","Başarılı",MessageBoxButton.OK,MessageBoxImage.Information);

                    // gn.LoadPageTeacher(Prm.anaGrid,"SORULAR".ToUpper());
                    uc_cagir.uc_Ekle(Prm.anaGrid, new ucTeacherQuestions(classroomIndex));
                }
                else
                {
                    MessageBox.Show("Soru kaydedilirken bir sorun oldu!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            else
            {
                MessageBox.Show("Lütfen boş alanları doldurunuz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        // çalışmıyor
        private void txtMaxPuan_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox puanText = (TextBox)sender;
            int puan = (int)Convert.ToInt64(puanText.Text.ToString());
            if (puan>100)
            {
                puanText.Text = "100";
            }else if (puan<0)
            {
                puanText.Text = "1";
            }
            MessageBox.Show(e.ToString());
        }
        private void cmbHemenAc_Click(object sender, RoutedEventArgs e)
        {
            bool checkHemenAcStatus = cmbHemenAc.IsChecked.Value;
            if (checkHemenAcStatus)
            {
                dateAcilmaZamani.IsEnabled = false;
                dateAcilmaZamani.SelectedDate = DateTime.Now.Date;
            }
            else
            {
                dateAcilmaZamani.IsEnabled = true;
            }
        }
     
    }
}
