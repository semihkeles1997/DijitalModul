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

namespace Dijital_Modul.Pages.StudentUserControllers
{
    /// <summary>
    /// Interaction logic for ucTeacherClassesAndBranches.xaml
    /// </summary>
    public partial class ucTeacherClassesAndBranches : UserControl
    {
        General gnr = new General();
        List<student> studentList = new List<student>();
        List<classAndBranches> clsList = new List<classAndBranches>();
        public ucTeacherClassesAndBranches()
        {
            InitializeComponent();

            studentList = gnr.getMyStudents();
            clsList = gnr.getMyClassesAndBranches();

            Grid grd = new Grid();
            grd.Height = 100;

            Border cerceve = new Border();
            cerceve.Height = 100;
            cerceve.CornerRadius = new CornerRadius(10);
            cerceve.Background = Brushes.LightGray;

            grd.Children.Add(cerceve);
            unf.Children.Add(grd);

        }
    }
}
