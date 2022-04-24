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
using System.Windows.Threading;
using System.Windows.Xps.Packaging;
using Dijital_Modul.Pages.Class;
using Dijital_Modul.Pages.Student;
using Dijital_Modul.Pages.TeacherWindows;
using Dijital_Modul.Pages.UserController;

namespace Dijital_Modul.Pages
{
    
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    /// 

    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();
            Prm.pageName = "Login";
             //autoLoginSilenecek("111","111");

            
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtTerminal_KeyDown(object sender, KeyEventArgs e)
        {

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && Keyboard.IsKeyDown(Key.Enter))
            {
                if (txtTerminal.Text.Trim() == "Giris_Yap();")
                {
                    General gn = new General();
                    string[] control = gn.Login(txtUsername.Text, txtPassword.Password);
                    Prm.sisteme_giris_zamani = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd H:m:s"));
                    if (control[0] == "1")
                    {
                        if (Prm.yetki_id == "1")
                        {
                            TeacherMain tm = new TeacherMain();
                            tm.Show();
                        }
                        else if (Prm.yetki_id == "2")
                        {
                            StudentMain sm = new StudentMain();
                            sm.Show();
                        }
                        this.Close();

                    }
                    else if (control[0] == "0")
                    {
                        MessageBox.Show("Kullanıcı adı ya da şifre hatalı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                       
                    }
                    else
                    {
                        MessageBox.Show($"{control[1]}", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                     
                    }
                }
                else
                {
                    MessageBox.Show("Komut bulunamadı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
                
                }
            }
           

        }
        /*
        private void autoLoginSilenecek(string userId="111", string password="111")
        {
            General gn = new General();
            string[] control = gn.Login(userId, password);
            Prm.sisteme_giris_zamani = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd H:m:s"));
            if (control[0] == "1")
            {
                if (Prm.yetki_id == "1")
                {
                    TeacherMain tm = new TeacherMain();
                    tm.Show();
                }
                else if (Prm.yetki_id == "2")
                {
                    StudentMain sm = new StudentMain();
                    sm.Show();
                }
                this.Close();

            }
            else if (control[0] == "0")
            {
                MessageBox.Show("Kullanıcı adı ya da şifre hatalı!", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
              
            }
            else
            {
                MessageBox.Show($"{control[1]}", "Uyarı", MessageBoxButton.OK, MessageBoxImage.Warning);
              
            }
        }
        */
        private void logo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("www.dijitalmodul.com");
        }
    }
}
