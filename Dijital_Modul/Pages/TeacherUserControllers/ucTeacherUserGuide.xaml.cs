using Dijital_Modul.Pages.Class;
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
    /// Interaction logic for ucTeacherUserGuide.xaml
    /// </summary>
    public partial class ucTeacherUserGuide : UserControl
    {
        public ucTeacherUserGuide()
        {
            InitializeComponent();

            Prm.pageName = "KullanimKilavuzu";


            pdfViewer.ItemSource = "Kullanim_Kilavuzlari/Ogretmen_Kilavuz.pdf";

        }
    }
}
