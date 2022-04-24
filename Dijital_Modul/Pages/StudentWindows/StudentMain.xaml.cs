using Dijital_Modul.Pages.Class;
using Dijital_Modul.Pages.StudentUserControllers;
using Dijital_Modul.Pages.UserController;
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
using System.Windows.Shapes;

namespace Dijital_Modul.Pages.Student
{
    /// <summary>
    /// Interaction logic for StudentMain.xaml
    /// </summary>
    public partial class StudentMain : Window
    {
        List<string> studentFunctions = new List<string>();
        bool kodDurum = false;
        string cagirilanKod = "";
        string kod = "", parametre = "";
        General gn = new General();
        Functions fn = new Functions();

        int sayac = 0;
        public StudentMain()
        {
            InitializeComponent();

            studentFunctions.Add("Su_Sayfayi_Ac");
            studentFunctions.Add("Su_Soruyu_Ac");
            studentFunctions.Add("Cevabi_Gonder");
            studentFunctions.Add("Uygulamayi_Kapat");
            txtName.Content = $"{Prm.ad} {Prm. soyad} [{Prm.kullanici_No}]";

            scrlviewer.ScrollToVerticalOffset(sayac);
            
        }
      

        private void txtTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.Enter))
            {
                kod = fn.FunctionControl(txtTerminal.Text, kod, parametre)[0];
                parametre = fn.FunctionControl(txtTerminal.Text, kod, parametre)[1];

                foreach (var item in studentFunctions)
                {
                    if (item == kod)
                    {
                        kodDurum = true;
                        cagirilanKod = item;
                    }
                }
                if (kodDurum)
                {
                    if (kod == studentFunctions[0]) // Sayfayı açma fonksiyonu
                    {
                        gn.LoadPageStudent(Content_Icerik,parametre);
                        Prm.pageName = parametre;
                        fn.SayfaAcik(txtMetotlar, txtIlerlemelerim, txtLiderTahtasi, txtRozetlerim, txtKullanimKilavuzu);

                    }
                    else if (kod == studentFunctions[1]) // Soru çağırma fonksiyonu
                    {
                        bool quesStatus = true;
                        if (Prm.pageName == "Ilerlemelerim" || Prm.pageName == "Metotlar")
                        {
                            string q = $@"select s.Soru_No, s.Resim,s.ResimSize ,s.Aciklama, s.Max_Puan, s.Acilma_Zamani, 
                                            s.Ekleme_Zamani, s.Soru_Turu, kb.Konu_Basligi_Adi 'Baslik', s.Soru_Acik_Mi
                                            from sorular s inner join Konu_Basliklari kb on 
                                            kb.ID =  s.Konu_Basliklari_ID where s.Ilerlemelerime_Ekle=1 and
                                            s.Soru_No='{parametre}'";
                            //string query = $"Select * from sorular where Soru_No='{parametre}'";
                            List<question> ques =  gn.A_Asking_Question(q);
                            if (ques.Count() > 0)
                            {
                                foreach (var item in ques)
                                {
                                    if (item.soruAcikMi == false)
                                    {
                                        quesStatus = false;
                                        break;
                                    }
                                }
                                if (quesStatus == false)
                                {
                                  
                                    MessageBox.Show("Soru açık değil!","Uyarı",MessageBoxButton.OK,MessageBoxImage.Warning);
                                }
                                else
                                {
                                    Prm.soru_acilma_zamani = DateTime.Now.ToString("yyyy-MM-dd H:m:s");
                                    Prm.soruNo = parametre;
                                    uc_cagir.uc_Ekle(Content_Icerik, new ucQuestion(parametre));
                                }
                            }
                            else
                            {
                                MessageBox.Show("Soru bulunamadı ya da soru açık değil!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"{Prm.pageName} sayfası için soru açma fonksiyonu kullanılamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else if (kod == studentFunctions[2]) // Cevabı gönderme fonksiyonu
                    {
                        if (Prm.pageName == "Soru")
                        {
                            List<answer> ans = new List<answer>();
                            ans = gn.A_Asking_Answer(Prm.soruNo, Prm.kullanici_No);
                            if (ans.Count()>0)
                            {
                                MessageBox.Show("Zaten bir cevabınız var!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else
                            {
                                if(Prm.cevap.Length>0)
                                {
                                    if (gn.Add_Answer(Prm.soruNo, Prm.cevap) == true)
                                    {
                                        MessageBox.Show("Cevap başarıyla gönderildi!", "Başarılı", MessageBoxButton.OK, MessageBoxImage.Information);
                                        uc_cagir.uc_Ekle(Content_Icerik, new ucQuestion(Prm.soruNo));
                                    }
                                    else
                                    {
                                        MessageBox.Show("Cevap eklenirken bir sorun oldu!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Cevabınız boş olamaz!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                                
                            }
                        }
                    }
                    else if (kod == studentFunctions[3]) // Uygulamayı kapatma fonksiyonu
                    {
                        this.Close();
                    }


                }
                else
                {
                    MessageBox.Show("Şu kod bulunamadı: " + txtTerminal.Text, "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                txtTerminal.Text = "";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Prm.pageName = "Metotlar";
            fn.SayfaAcik(txtMetotlar, txtIlerlemelerim, txtLiderTahtasi, txtRozetlerim, txtKullanimKilavuzu);
            uc_cagir.uc_Ekle(Content_Icerik, new ucStudentMethods());
        }


      

     

       
        private void scrlviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            sayac += 100;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            gn.Add_User_Log();
        }


      
    }
}
