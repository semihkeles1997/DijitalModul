using Dijital_Modul.Pages.Class;
using System;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Xamarin.Essentials;

namespace Dijital_Modul.Pages.UserController
{
    /// <summary>
    /// Interaction logic for ucStudentMethods.xaml
    /// </summary>
    public partial class ucStudentMethods : UserControl
    {
        public ucStudentMethods()
        {
            InitializeComponent();
            Prm.pageName = "Metotlar";
            pdfViewer.ItemSource = "Metotlar2017.pdf";
        }

    }
}
