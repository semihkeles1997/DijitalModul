using Dijital_Modul.Pages.Class;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dijital_Modul.Pages.StudentUserControllers
{
    /// <summary>
    /// Interaction logic for ucQuestion.xaml
    /// </summary>
    public partial class ucQuestion : UserControl
    {
        List<question> quesList = new List<question>();
        List<feedback> fbList = new List<feedback>();
        List<answer> ansList = new List<answer>();
        General gn = new General();

        public ucQuestion(string quesNo)
        {
            InitializeComponent();
            Prm.pageName = "Soru";

            string q = $@"select s.Soru_No, s.Resim,s.ResimSize, s.Aciklama, s.Max_Puan, s.Acilma_Zamani, 
                                            s.Ekleme_Zamani, s.Soru_Turu, kb.Konu_Basligi_Adi 'Baslik', s.Soru_Acik_Mi
                                            from sorular s inner join Konu_Basliklari kb on 
                                            kb.ID =  s.Konu_Basliklari_ID where s.Ilerlemelerime_Ekle=1 and
                                            s.Soru_No='{quesNo}'";

            //string query = $"Select * from sorular where Soru_No='{quesNo}'";
            quesList = gn.A_Asking_Question(q);

            
            ansList = gn.A_Asking_Answer(quesNo,Prm.kullanici_No);

            fbList = gn.A_Asking_FeedBack(quesNo, Prm.kullanici_No);

            

            foreach (var item in quesList)
            {
                txblHeader.Text = item.soruNo + ": " + item.baslik;
                txblContent.Text = item.aciklama;


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
                            MessageBox.Show($"Hata kodu: 02x0001 - {ex.GetType()}","Uyarı",MessageBoxButton.OK,MessageBoxImage.Warning);
                        }
                    }
                }



            }


           


            if (ansList.Count() == 0)
            {
                donutCizgisi.Visibility = Visibility.Hidden;
                dckPuan.Visibility = Visibility.Hidden;
                dckPuan.IsEnabled = false;
                txtAnswer.BorderBrush = Brushes.LightGray;
                txtAnswer.Text = "Cevabınızı buraya yazınız...";
                dckPuan.IsEnabled = false;
            }
            else
            {
                donutCizgisi.Visibility = Visibility.Visible;
                foreach (var item in ansList)
                {
                    dckPuan.IsEnabled = true;
                    txtAnswer.Text = item.cevap;
                    lblCevapEklemeZamani.Content = item.cevapGondermeZamani;
                    txtAnswer.IsEnabled = false;
                    if (item.puan >= 0)
                    {
                        dckPuan.IsEnabled = true;
                        lblPuan.Foreground = Brushes.Green;
                        lblPuan.Content = item.puan;
                        lblPuanBaslik.Content = "Puan:";
                        lblPuanBaslik.Foreground = Brushes.Black;
                    }
                    else
                    {
                        lblPuanBaslik.Content = "Puanlanmamış!";
                        lblPuanBaslik.Foreground = Brushes.Red;
                    }
                }

                if (fbList.Count() == 0)
                {
                    txblFeedback.Text = "Dönüt verilmemiş...";
                    txblFeedback.Foreground = Brushes.Red;
                }
                else
                {
                    foreach (var item in fbList)
                    {
                        txblFeedback.Text = item.donut;
                        lblDonutZamani.Content = item.donutZamani;
                    }
                    // Dönüt bu kısma gelecek...
                }


            }

        }

        private void txtAnswer_TextChanged(object sender, TextChangedEventArgs e)
        {
            Prm.cevap = txtAnswer.Text;
        }

        private void txtAnswer_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtAnswer_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
