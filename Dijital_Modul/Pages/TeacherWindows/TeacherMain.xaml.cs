using Dijital_Modul.Pages.Class;
using Dijital_Modul.Pages.StudentUserControllers;
using Dijital_Modul.Pages.TeacherUserControllers;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for TeacherMain.xaml
    /// </summary>
    public partial class TeacherMain : Window
    {
        Functions fn = new Functions();
        General gn = new General();
        List<string> teacherFunctions = new List<string>();
        bool kodDurum = false;
        string cagirilanKod = "";
        string kod, parametre;


        public TeacherMain()
        {
            InitializeComponent();

            teacherFunctions.Add("Su_Sayfayi_Ac");
            teacherFunctions.Add("Su_Ogrenciyi_Ac");
            teacherFunctions.Add("Su_Soruyu_Ac");
            teacherFunctions.Add("Rozeti_Alan_Ogrenciler");
           //  teacherFunctions.Add("Tamamlanan_Sorular"); Farklı isim bulalım
            teacherFunctions.Add("Uygulamayi_Kapat");


            txtName.Content = $"{Prm.ad} {Prm.soyad} [{Prm.kullanici_No}]";

            Prm.anaGrid = Content_Icerik;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Prm.pageName = "Metotlar";
            fn.OgretmenSayfaAcik(txtMetotlar, txtOgrencilerim, txtLiderTahtasi, txtRozetler, txtSorular, txtKullanimKilavuzu);
            uc_cagir.uc_Ekle(Content_Icerik, new ucTeacherMethods());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gn.Add_User_Log();
        }

        private void DockPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DockPanel dck = (DockPanel)sender;
            Label lbl = (Label)dck.Children[1];
            gn.LoadPageTeacher(Content_Icerik, lbl.Content.ToString().ToUpper());
            fn.OgretmenSayfaAcik(txtMetotlar, txtOgrencilerim, txtLiderTahtasi, txtRozetler, txtSorular, txtKullanimKilavuzu);
            
        }

        private void DockPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            DockPanel dck = (DockPanel)sender;
            Label lbl = (Label)dck.Children[1];
            lbl.Foreground = Brushes.Yellow;
        }

        private void DockPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            DockPanel dck = (DockPanel)sender;
            Label lbl = (Label)dck.Children[1];
            string sayfaAdi = fn.OgretmenSayfaAdiDegistir(lbl.Content.ToString());
            if (sayfaAdi == Prm.pageName)
            {
                lbl.Foreground = Brushes.LightGreen;
            }
            else
            {
                lbl.Foreground = Brushes.White;
            }
           
        }

        private void txtTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.Enter))
            {
                kod = fn.FunctionControl(txtTerminal.Text, kod, parametre)[0];
                parametre = fn.FunctionControl(txtTerminal.Text, kod, parametre)[1];


                foreach (var item in teacherFunctions)
                {
                    if (item == kod)
                    {
                        kodDurum = true;
                        cagirilanKod = item;
                    }
                }
                if (kodDurum)
                {
                    if (kod == teacherFunctions[0]) // Sayfayı açma fonksiyonu
                    {
                        gn.LoadPageTeacher(Content_Icerik, parametre);
                        Prm.pageName = parametre;
                        fn.OgretmenSayfaAcik(txtMetotlar, txtOgrencilerim, txtLiderTahtasi, txtRozetler, txtSorular, txtKullanimKilavuzu);

                    }
                    else if (kod == teacherFunctions[1]) // Su ogrenciyi acma fonksiyonu
                    {
                        List<classAndBranches> classandbranches= gn.getMyClassesAndBranches();
                        string sinifIDlerim = "";
                        for (int i = 0; i < classandbranches.Count(); i++)
                        {
                            if (i != classandbranches.Count()-1)
                                sinifIDlerim += classandbranches[i].sinifID + ",";
                            else
                                sinifIDlerim += classandbranches[i].sinifID;

                        }
                        string sorgu = $"select k.ID from kullanicilar k inner join kullanici_sinif ks on ks.Kullanicilar_ID = k.ID where k.Yetki_ID=2 and ks.Siniflar_ID in ({sinifIDlerim}) and k.Kullanici_No={parametre};";
                        int controlID = gn.TekilVeriCekInt(sorgu,"ID");
                        if (controlID>0)
                            uc_cagir.uc_Ekle(Prm.anaGrid, new ucTeacher_StudentProgress(parametre));
                        else
                        {
                            MessageBox.Show("Böyle bir öğrenci bulunamadı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else if (kod == teacherFunctions[2]) // Soru çağırma fonksiyonu
                    {
                        String[] parameters = parametre.Split(',');
                        if(parameters.Length == 2)
                        {
                            int sinifID = gn.TekilVeriCekInt($@"SELECT * FROM siniflar where concat(Sinif,'/',Sube) = '{parameters[1]}' ", "ID");

                            string q = $@"Select ss.Sinif_ID from sorular s  inner join Soru_Sinif ss on ss.ID = s.ID where s.Ekleyen in (0,{Prm.kullanici_ID}) and s.Soru_No='{parameters[0]}' and ss.Sinif_ID={sinifID}";
                            int quesControlID = gn.TekilVeriCekInt(q, "Sinif_ID");
                            if (quesControlID > 0)
                            {
                                TeacherQuestionDetails tqd = new TeacherQuestionDetails(parameters[0], sinifID);
                                tqd.Owner = this;
                                tqd.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("Girilen sınıfa ait böyle bir soru bulunamadı!", "Soru bulunamadı", MessageBoxButton.OK, MessageBoxImage.Warning);

                            }
                        }
                        else
                        {
                            MessageBox.Show("Lütfen SoruNo,Sinif/Sube şeklinde kullanın!","Uyarı",MessageBoxButton.OK,MessageBoxImage.Warning);
                        }

                        
                        
                    }
                    else if (kod == teacherFunctions[3]) // Öğrenci Rozetlerini Ac
                    {
                        string sorgu =$@"SELECT * FROM rozetler where Rozet_Adi = '{parametre}' and Ekleyen = {Prm.kullanici_ID}";
                        int badgeID = gn.TekilVeriCekInt(sorgu,"ID");
                        if (badgeID>0)
                        {
                            uc_cagir.uc_Ekle(Content_Icerik,new ucBadges_Students(badgeID));
                        }
                        else
                        {
                            MessageBox.Show("Böyle bir rozet bulunamadı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                         
                        }
                    }
                    else if (kod == teacherFunctions[4]) // Uygulamayı kapatma fonksiyonu
                    {
                        this.Close();
                    }



                }
               
                else
                {
                    MessageBox.Show($"Şu kod bulunamadı: {txtTerminal.Text}", "Kod bulunamadı", MessageBoxButton.OK, MessageBoxImage.Warning);
                 
                }
                txtTerminal.Text = "";
            }
        }
    }
}
