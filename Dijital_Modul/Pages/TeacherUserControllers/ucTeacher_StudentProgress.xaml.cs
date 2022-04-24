using Dijital_Modul.Pages.Class;
using Dijital_Modul.Pages.TeacherWindows;
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
    /// Interaction logic for ucTeacher_StudentProgress.xaml
    /// </summary>
    public partial class ucTeacher_StudentProgress : UserControl
    {

        List<question> answeredQuesList = new List<question>();
        List<question> unansweredQuesStatTrueList = new List<question>();
        List<question> unansweredQuesStatFalseList = new List<question>();
        General gnr = new General();
        int quesCount, answerCount;
        int sayac = 0;
        string studentNumber;

        public ucTeacher_StudentProgress(string ogrNo)
        {
            InitializeComponent();

            

            student ogrenci = gnr.TekilOgrenciGetir(ogrNo);
            txtBaslik.Content = ogrenci.sinif.ToUpper()+"/"+ogrenci.sube.ToUpper()+" - "+ogrenci.ad.ToUpper()+" "+ogrenci.soyad.ToUpper()+$" ({ogrenci.kullaniciNo})";

            answeredQuesList = gnr.Answered_Questions(ogrNo, false);
            unansweredQuesStatTrueList = gnr.Unanswered_Questions(ogrNo, true);
            unansweredQuesStatFalseList = gnr.Unanswered_Questions(ogrNo, false);

            studentNumber = ogrNo;
            Get_List(unansweredQuesStatTrueList, 0);
            Get_List(unansweredQuesStatFalseList, 1);
            Get_List(answeredQuesList, 2);


            quesCount = answeredQuesList.Count() + unansweredQuesStatFalseList.Count() + unansweredQuesStatTrueList.Count();
            answerCount = answeredQuesList.Count();


            prgrss.Maximum = 100;
            decimal yuzde = Decimal.Divide(answerCount, quesCount) * 100;
            prgrss.Value = Convert.ToInt32(yuzde);
            lblProgressVal.Content = prgrss.Value;
            studentNumber = ogrNo;

           
        }


        void Get_List(List<question> questions, int status)
        {
            var imgLock = @"../../Pages/Images/LockQuestion.png";
            var imgUnlock = @"../../Pages/Images/UnlockQuestion.png";
            var imgAnswerLock = @"../../Pages/Images/AnswerLock.png";
            
            foreach (var item in questions)
            {
                sayac++;
                Grid grd = new Grid();
                grd.Name = "brd" + sayac;
                grd.Background = Brushes.LightGray;
                grd.Height = 130;
                grd.Margin = new Thickness(20, 0, 20, 20);
                grd.MouseEnter += Grid_MouseEnter;
                grd.MouseLeave += Grid_MousLeave;
                grd.MouseDown += Grid_MouseDown;

                Border cerceve = new Border();
                cerceve.Height = 130;
                cerceve.BorderBrush = Brushes.Gray;
                cerceve.BorderThickness = new Thickness(2);
                cerceve.Background = Brushes.LightGray;
                cerceve.CornerRadius = new CornerRadius(10);
                grd.Children.Add(cerceve);

                TextBlock txtblck = new TextBlock();
                txtblck.FontSize = 16;
                txtblck.Text = item.soruNo;
                txtblck.Foreground = Brushes.LightGreen;
                txtblck.VerticalAlignment = VerticalAlignment.Top;
                txtblck.HorizontalAlignment = HorizontalAlignment.Center;


                Image dynamicImage = new Image();
                dynamicImage.Width = 50;
                dynamicImage.Height = 50;
                dynamicImage.Margin = new Thickness(0, 20, 0, 0);

                // Create a BitmapSource  
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                if (status == 0) // Cevaplanmamış ve açık sorular
                {
                    bitmap.UriSource = new Uri(imgUnlock, UriKind.Relative);
                    txtblck.Foreground = Brushes.Black;
                }
                else if (status == 1) // Cevaplanmamış ve kapalı sorular
                {
                    bitmap.UriSource = new Uri(imgLock, UriKind.Relative);
                    txtblck.Foreground = Brushes.Black;
                }
                else if (status == 2) // Cevaplanmış sorular
                {
                    bitmap.UriSource = new Uri(imgAnswerLock, UriKind.Relative);
                    txtblck.Foreground = Brushes.Green;
                }

                bitmap.EndInit();

                // Set Image.Source  
                dynamicImage.Source = bitmap;
                grd.Children.Add(dynamicImage);


                grd.Children.Add(txtblck);
                TextBlock txbDPDurum = new TextBlock();
                txbDPDurum.FontSize = 12;
                txbDPDurum.VerticalAlignment = VerticalAlignment.Bottom;
                txbDPDurum.HorizontalAlignment = HorizontalAlignment.Right;
                txbDPDurum.Margin = new Thickness(0,0,20,10);
                string sorgu = $"select c.ID from cevaplar c left join donutler d on d.Soru_No=c.Soru_No " +
                    $"where c.Soru_No = '{item.soruNo}' and (c.Puan > 0 or length(d.Donut) > 0) and c.Kullanici_No='{studentNumber}' ";
                if (gnr.TekilVeriCekInt(sorgu, "ID") > 0)
                {
                    txbDPDurum.Text = "Dönüt/Puan verilmiş";
                    txbDPDurum.Foreground = Brushes.Green;
                }
                else
                {
                    txbDPDurum.Text = "Dönüt/Puan verilmemiş";
                    txbDPDurum.Foreground = Brushes.Red;
                }
                grd.Children.Add(txbDPDurum);    
                unf.Children.Add(grd);
            }
        }
        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            Grid grd = (Grid)sender;
            TextBlock txbSoruNo = (TextBlock)grd.Children[2];
          
            TeacherStudentAnswerFeedback tsaf = new TeacherStudentAnswerFeedback(studentNumber,txbSoruNo.Text.ToString());
            tsaf.Owner = Prm.gk;
            tsaf.ShowDialog();
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
            brd.Background = Brushes.LightGray;
        }

    }
}
