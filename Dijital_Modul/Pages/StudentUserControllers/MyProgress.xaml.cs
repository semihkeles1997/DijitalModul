using Dijital_Modul.Pages.Class;
using System;
using System.Collections.Generic;
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
using Xceed.Wpf.AvalonDock.Layout;

namespace Dijital_Modul.Pages.UserController
{
    /// <summary>
    /// Interaction logic for MyProgress.xaml
    /// </summary>
    public partial class MyProgress : UserControl
    {

        List<question> answeredQuesList = new List<question>();
        List<question> unansweredQuesStatTrueList = new List<question>();
        List<question> unansweredQuesStatFalseList = new List<question>();

        int quesCount, answerCount;
        int sayac = 0;
        General gnr = new General();
        
        public MyProgress()
        {
            InitializeComponent();

            Prm.txbMyScore = txbPuan;
            gnr.Update_Score();

            answeredQuesList = gnr.Answered_Questions(Prm.kullanici_No, false);
            unansweredQuesStatTrueList = gnr.Unanswered_Questions(Prm.kullanici_No, true);
            unansweredQuesStatFalseList = gnr.Unanswered_Questions(Prm.kullanici_No, false);


            if (Prm.pageName == "Ilerlemelerim")
            {
                Get_List(unansweredQuesStatTrueList, 0);
                Get_List(unansweredQuesStatFalseList, 1);
                Get_List(answeredQuesList, 2);
            }
            else if (Prm.pageName == "Cevaplarim")
            {
                Get_List(answeredQuesList, 2);
            }
            else if (Prm.pageName == "Cevap_Vermediklerim")
            {
                Get_List(unansweredQuesStatTrueList, 0);
            }

           

           

            quesCount = answeredQuesList.Count() + unansweredQuesStatFalseList.Count() + unansweredQuesStatTrueList.Count();
            answerCount = answeredQuesList.Count();
            
            
            prgrss.Maximum = 100;
            decimal yuzde = Decimal.Divide(answerCount, quesCount)*100;
            prgrss.Value = Convert.ToInt32(yuzde);
            lblProgressVal.Content = prgrss.Value;


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
                grd.Height = 100;
                grd.Margin = new Thickness(20, 0, 20, 20);

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
                unf.Children.Add(grd);
            }

        }

    }
}
