using Dijital_Modul.Pages.Class;
using Dijital_Modul.Pages.GeneralWindows;
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
using WECPOFLogic;

namespace Dijital_Modul.Pages.TeacherUserControllers
{
    /// <summary>
    /// Interaction logic for ucTeacherQuestions.xaml
    /// </summary>
    public partial class ucTeacherQuestions : UserControl
    {
        
        List<question> acikSorular = new List<question>();
        List<question> kapaliSorular = new List<question>();
        List<question> ogretmenAcikSorular = new List<question>();
        List<question> ogretmenKapaliSorular = new List<question>();
        List<question> mufredatAcikSorular = new List<question>();
        List<question> mufredatKapaliSorular = new List<question>();

        General gnr = new General();
        int secilenSinifID;

        List<classAndBranches> clsList = new List<classAndBranches>();

        
        public ucTeacherQuestions(int selectedISinifID =0)
        {
            InitializeComponent();
            clsList = gnr.getMyClassesAndBranches();
            cmbSiniflar.Items.Add("Lütfen bir sınıf seçiniz");
            foreach (var item in clsList)
            {
                cmbSiniflar.Items.Add(item.sinif + "/" + item.sube);
            }

            cmbSiniflar.SelectedIndex = selectedISinifID;

        }

        void Get_List(List<question> questions, bool status)
        {
            var imgLock = @"../../Pages/Images/LockQuestion.png";
            var imgUnlock = @"../../Pages/Images/UnlockQuestion.png";
            int sayac = 0;
            foreach (var item in questions)
            {
                sayac++;
                Grid grd = new Grid();
                grd.Name = "brd" + sayac;
                //grd.Background = Brushes.LightGray;
                grd.Height = 100;
                grd.Margin = new Thickness(20, 0, 20, 20);
                grd.MouseEnter += Grid_MouseEnter;
                grd.MouseLeave += Grid_MousLeave;
                grd.MouseDown += Grid_MouseDown;

                Border cerceve = new Border();
                cerceve.Height = 100;
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

                if (status) // Açık sorular
                {
                    bitmap.UriSource = new Uri(imgUnlock, UriKind.Relative);
                    txtblck.Foreground = Brushes.Black;
                }
                else // Kapalı sorular
                {
                    bitmap.UriSource = new Uri(imgLock, UriKind.Relative);
                    txtblck.Foreground = Brushes.Black;
                }
               
                bitmap.EndInit();

                // Set Image.Source  
                dynamicImage.Source = bitmap;
                grd.Children.Add(dynamicImage);
                grd.Children.Add(txtblck);
                unf.Children.Add(grd);
            }

        }

        private void btnMouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void btnMouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void btnYeniSoruEkle_Click(object sender, RoutedEventArgs e)
        {
            uc_cagir.uc_Ekle(Prm.anaGrid, new ucTeacherAddQuestion());
        }
      
        private void Grid_MouseDown(object sender, MouseEventArgs e)
        {
            Grid grd = (Grid)sender;
            TextBlock txbSoruNo = (TextBlock)grd.Children[2];
            TeacherQuestionSelectOption tqso = new TeacherQuestionSelectOption(txbSoruNo.Text, secilenSinifID);
            tqso.Owner = Prm.gk;
            tqso.ShowDialog();
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


        private void btnSordugumSorular_Click(object sender, RoutedEventArgs e)
        {
            // gnr.LoadPageTeacher(Prm.anaGrid, "Sordugum_Sorular");
           

            unf.Children.Clear();

            if (cmbSiniflar.SelectedIndex > 0)
            {
                Prm.pageName = "Sordugum_Sorular";
                Get_List(ogretmenAcikSorular, true);
                Get_List(ogretmenKapaliSorular, false);
                lblBaslik.Content = "SORDUĞUM SORULAR";
            }
            else
            {
                MessageBox.Show("Lütfen bir sınıf seçiniz!", "Sınıf seçilmedi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnMufredatSorulari_Click(object sender, RoutedEventArgs e)
        {
           
            //gnr.LoadPageTeacher(Prm.anaGrid, "Mufredat_Sorulari");

            unf.Children.Clear();

            if (cmbSiniflar.SelectedIndex > 0)
            {
                Prm.pageName = "Mufredat_Sorulari";
                Get_List(mufredatAcikSorular, true);
                Get_List(mufredatKapaliSorular, false);
                lblBaslik.Content = "MÜFREDAT SORULARI";
            }
            else
            {
                MessageBox.Show("Lütfen bir sınıf seçiniz!", "Sınıf seçilmedi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void btnTumSorular_Click(object sender, RoutedEventArgs e)
        {
            //gnr.LoadPageTeacher(Prm.anaGrid, "Sorular");
            unf.Children.Clear();
            if (cmbSiniflar.SelectedIndex > 0)
            {
                Prm.pageName = "Sorular";
                Get_List(acikSorular, true);
                Get_List(kapaliSorular, false);
                lblBaslik.Content = "TÜM SORULAR";
            }
            else
            {
                MessageBox.Show("Lütfen bir sınıf seçiniz!","Sınıf seçilmedi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void cmbSiniflar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            unf.Children.Clear();

            if (cmbSiniflar.SelectedIndex>0)
            {
                secilenSinifID = clsList[cmbSiniflar.SelectedIndex - 1].sinifID;
                acikSorular = gnr.getTeacherAllQuestion(0, secilenSinifID);
                kapaliSorular = gnr.getTeacherAllQuestion(1, secilenSinifID);
                ogretmenAcikSorular = gnr.getTeacherAllQuestion(2, secilenSinifID);
                ogretmenKapaliSorular = gnr.getTeacherAllQuestion(3, secilenSinifID);
                mufredatAcikSorular = gnr.getTeacherAllQuestion(4, secilenSinifID);
                mufredatKapaliSorular = gnr.getTeacherAllQuestion(5, secilenSinifID);

                if (Prm.pageName == "Sorular")
                {
                    Get_List(acikSorular, true);
                    Get_List(kapaliSorular, false);
                    lblBaslik.Content = "TÜM SORULAR";
                }
                else if (Prm.pageName == "Sordugum_Sorular")
                {
                    Get_List(ogretmenAcikSorular, true);
                    Get_List(ogretmenKapaliSorular, false);
                    lblBaslik.Content = "SORDUĞUM SORULAR";
                }
                else if (Prm.pageName == "Mufredat_Sorulari")
                {
                    Get_List(mufredatAcikSorular, true);
                    Get_List(mufredatKapaliSorular, false);
                    lblBaslik.Content = "MÜFREDAT SORULARI";

                }
               
            }
            
        }

        
    }
}
