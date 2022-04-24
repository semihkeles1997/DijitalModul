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

namespace Dijital_Modul.Pages.UserController
{
    /// <summary>
    /// Interaction logic for UserGuide.xaml
    /// </summary>
    public partial class UserGuide : UserControl
    {
        public UserGuide()
        {
            InitializeComponent();
            Prm.pageName = "KullanimKilavuzu";



            pdfViewer.ItemSource = "Kullanim_Kilavuzlari/Ogrenci_Kilavuz.pdf";
        }
    }
}
