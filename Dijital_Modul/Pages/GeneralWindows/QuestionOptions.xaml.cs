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

namespace Dijital_Modul.Pages.GeneralWindows
{
    /// <summary>
    /// Interaction logic for QuestionOptions.xaml
    /// </summary>
    public partial class QuestionOptions : Window
    {
        public string gelenSoruNo;
        public QuestionOptions(string soruNo)
        {
            InitializeComponent();
            gelenSoruNo = soruNo;
            txbSoruNo.Text = gelenSoruNo;
        }

        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSoruyaGit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{gelenSoruNo} soruya gidilecek...");
        }

        private void btnSoruyuAc_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"{gelenSoruNo} sorusu öğrencilere açılacak...");
        }

        private void btnMouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void btnMouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }


    }
}
