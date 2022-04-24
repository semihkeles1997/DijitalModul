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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace Dijital_Modul.Pages.TeacherUserControllers
{
    /// <summary>
    /// Interaction logic for ucTeacherAddBadge.xaml
    /// </summary>
    public partial class ucTeacherAddBadge : UserControl
    {
        General gnr = new General();
        List<classAndBranches> clsList = new List<classAndBranches>();
        List<question> questionuNoList = new List<question>();
        OpenFileDialog dialog = new OpenFileDialog();
        List<question> selectedQuestion = new List<question>();
        string secilenRessimAdi = "";
        List<int> checkboxIndexList = new List<int>();

        public ucTeacherAddBadge()
        {
            InitializeComponent();
            clsList = gnr.getMyClassesAndBranches();
            foreach (classAndBranches item in clsList)
            {
                cmbSinifSecim.Items.Add(item.sinif + "/" + item.sube);
            }
            dtpTarih.DisplayDateStart = DateTime.Now.Date;
            for (int i = 1; i < 25; i++)
            {
                cmb_Hour.Items.Add(i.ToString());
            }
            for (int i = 0; i < 60; i++)
            {
                if (i > 9)
                {
                    cmb_Secound.Items.Add(i.ToString());
                }
                else
                {
                    cmb_Secound.Items.Add($"0{i}");
                }
            }
            dtpTarih.SelectedDate = DateTime.Now.Date;
            cmb_Hour.Text = DateTime.Now.Hour.ToString();
            cmb_Secound.Text = DateTime.Now.Second.ToString();
        }



        string isim = "";
        private void TextBoxGenel_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBox txb = (TextBox)sender;
            isim = txb.Text;
            txb.Text = "";
        }

        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            Border brd = (Border)sender;
            brd.Background = Brushes.Cyan;
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            Border brd = (Border)sender;
            brd.Background = Brushes.Transparent;
        }

        private void btnRozetiEkle_Click(object sender, RoutedEventArgs e)
        {
            string badgeName = txbRozetAdi.Text.ToString();
            string badgePromotion = txbAciklama.Text.ToString();
            int sinifSelecedCmb = cmbSinifSecim.SelectedIndex;
            //int questionSelectedCmb = cmbSoruSecim.SelectedIndex;
            int questionSelectedCmb = checkboxIndexList.Count;

            DateTime date = dtpTarih.SelectedDate.Value;
            string hour = cmb_Hour.SelectedItem.ToString();
            string imagePath = secilenRessimAdi; //btnResimYukle.Content.ToString();          
            string secound = cmb_Secound.SelectedItem.ToString();
            if (!badgeName.Equals("") && sinifSelecedCmb != -1 && questionSelectedCmb > 0 && date != null && !hour.Equals("") && !secound.Equals(""))
            {
                DateTime newDate = new DateTime(date.Year, date.Month, date.Day, Convert.ToInt32(hour), Convert.ToInt32(secound), 0);
                string newDateString = newDate.ToString("yyyy-MM-dd H:m:s");
                int sinifID = clsList[cmbSinifSecim.SelectedIndex].sinifID;
                byte[] imageByte = null;
                int fileSize = 0;
                if (!imagePath.Equals(""))
                {
                    imageByte = gnr.converSmallerImageByteArray(imagePath);
                    fileSize = imageByte.Length;

                }
                for (int i = 0; i < checkboxIndexList.Count; i++)
                {
                    question newQuestion = questionuNoList[checkboxIndexList[i]];
                    selectedQuestion.Add(newQuestion);
                    //MessageBox.Show(newQuestion.soruNo);
                }

                bool status = gnr.addBadges(badgeName, "", badgePromotion, newDateString, selectedQuestion, imageByte, fileSize);

                if (status)
                {
                    uc_cagir.uc_Ekle(Prm.anaGrid, new ucTeacherBadges());
                    MessageBox.Show("Rozet başarı ile kaydedildi", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                else
                {
                    MessageBox.Show("Rozet kaydedilirken hata oldu", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Lütfen boş alanları doldurunuz", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void btnResimYukle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // OpenFileDialog ile resim seçme işlemi yapıyoruz.

                dialog.Title = "Resim seç";
                dialog.InitialDirectory = "";  // Pencere açıldığında ilk olarak karşımıza hangi pencere gelsin onu seçiyoruz. Direkt Belgelerimi seçebiliriz yani.
                dialog.Filter = "Image Files (*.jpg;*.jpeg;)|*.jpg;*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg;";  // Seçilecek dosyayı filtreliyoruz. Resim seçmek istediğimiz için.
                dialog.FilterIndex = 1;


                if ((bool)dialog.ShowDialog())  // Seçim işlemi başarılı ise buraya girecek.
                {
                    DateTime zaman = DateTime.Now;
                    string format = "dd-MM-yyyy-hh-mm-ss";
                    Random rst = new Random();
                    secilenRessimAdi = dialog.FileName.ToString();
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(@"" + secilenRessimAdi);
                    bitmap.EndInit();
                    badgeImage.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata kodu: 02x0002 - {ex.GetType()}", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void cmbSinifSecim_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbSoruSecim.Items.Clear();
            cmbSoruSecim.IsEnabled = true;
            int cls_ID = clsList[cmbSinifSecim.SelectedIndex].sinifID;

            cmbSoruSecim.Items.Clear();
            for (int i = 0; i < 6; i++)
            {
                List<question> newQuestions = new List<question>();
                newQuestions = gnr.getTeacherAllQuestion(i, cls_ID);
                foreach (question item in newQuestions)
                {
                    questionuNoList.Add(item);
                }
            }


            Add_Questions();
        }
        public void Add_Questions()
        {
            DataTable dt = new DataTable();
            DataColumn soruNoColum = new DataColumn("soru_no");



            dt.Columns.Add(soruNoColum);
            int i = 1;
            foreach (question item in questionuNoList)
            {
                CheckBox check = new CheckBox();
                check.Content = item.soruNo;
                check.Checked += Check_Checked;
                check.Unchecked += Check_UnChecked;
                stckQuestions.Children.Add(check);
            }
        }

        private void Check_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox _check = (CheckBox)sender;

            if (checkboxIndexList.Count < 4)
            {
                int index = stckQuestions.Children.IndexOf(_check);
                checkboxIndexList.Add(index);

            }
            
            if(checkboxIndexList.Count ==3)
            {
                
                for (int i = 0; i < stckQuestions.Children.Count; i++)
                {
                    stckQuestions.Children[i].IsEnabled = false;
                }
                foreach (var item in checkboxIndexList)
                {
                    stckQuestions.Children[item].IsEnabled = true;
                }
            }



            //Checkbox_Question_Lock_Unlock(_check.Content.ToString());
            //if (selectedQuestion.Count < 4)
            //{
            //    foreach (question item in questionuNoList)
            //    {
            //        if (item.soruNo.Equals(_check.Content))
            //        {
            //            selectedQuestion.Add(item);

            //        }
            //    }

            //}
            //else
            //{
            //    List<question> _quesList = questionuNoList;


            //}
        }
        private void Check_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox _check = (CheckBox)sender;
            int index = stckQuestions.Children.IndexOf(_check);
            checkboxIndexList.Remove(index);
            for (int i = 0; i < stckQuestions.Children.Count; i++)
            {
                stckQuestions.Children[i].IsEnabled = true;
            }

        }
        void Checkbox_Question_Lock_Unlock(string quesNo)
        {
            List<question> _temp = new List<question>();
            if (selectedQuestion.Count > 3)
            {
                foreach (question _ques in questionuNoList)
                {
                    foreach (var _selected in selectedQuestion)
                    {
                        if (_ques.soruNo != _selected.soruNo)
                        {
                            _temp.Add(_ques);
                        }

                    }
                }
                foreach (var item in stckQuestions.Children)
                {
                    foreach (var item2 in _temp)
                    {
                        CheckBox _chk = (CheckBox)item;
                        if (_chk.Content.ToString() != item2.soruNo)
                        {
                            _chk.IsChecked = false;
                            _chk.IsEnabled = false;
                        }
                    }


                }
            }
            else
            {
                question q = new question();
                q.soruNo = quesNo;
                selectedQuestion.Add(q);
                Open_The_All_Question_Checkbox();
            }

        }
        void Open_The_All_Question_Checkbox()
        {
            foreach (var item in stckQuestions.Children)
            {
                CheckBox _chk = (CheckBox)item;
                _chk.IsEnabled = true;
            }
        }

        /*
void OnChecked(object sender, RoutedEventArgs e)
{
   if (selectedQuestion.Count<4)
   {
       int index = dtg_SoruSecimi.SelectedIndex;
       selectedQuestion.Add(questionuNoList[index]);
   }
   else
   {

       DataGridRow row = (DataGridRow)dtg_SoruSecimi.ItemContainerGenerator.ContainerFromIndex(dtg_SoruSecimi.SelectedIndex);
       CheckBox check = (CheckBox)dtg_SoruSecimi.Columns[0].GetCellContent(row);
       check.IsChecked = false;
       MessageBox.Show("Maksimum 4 soru seçebilirsiniz! ", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
   }


}
void UnChecked(object sender, RoutedEventArgs e)
{
   int index = dtg_SoruSecimi.SelectedIndex;
   selectedQuestion.Remove(questionuNoList[index]);
}


*/
    }
}


