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

namespace Dijital_Modul.Pages.TeacherWindows
{
    /// <summary>
    /// Interaction logic for TeacherQuestionSelectOption.xaml
    /// </summary>
    public partial class TeacherQuestionSelectOption : Window
    {
        TeacherMain gk = (TeacherMain)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
        string gelenSoruNo;
        int gelenSinifID;
        public TeacherQuestionSelectOption(string soruNo, int sinifID)
        {
            InitializeComponent();
            gelenSoruNo = soruNo;
            txbSoruNo.Text = gelenSoruNo;
            gelenSinifID = sinifID;
        }

        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSorununDetaylarinaGit_Click(object sender, RoutedEventArgs e)
        {
            TeacherQuestionDetails tqd = new TeacherQuestionDetails(gelenSoruNo, gelenSinifID);
            tqd.Owner = gk;
            tqd.ShowDialog();
            this.Close();
        }

        private void btnSoruyuDuzenle_Click(object sender, RoutedEventArgs e)
        {
            TeacherEditQuestion teq = new TeacherEditQuestion(gelenSoruNo);
            teq.Owner = gk;
            teq.ShowDialog();
            this.Close();
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
