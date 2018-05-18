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

namespace SNROI
{
    /// <summary>
    /// Interaction logic for ReportDesignerWindow.xaml
    /// </summary>
    public partial class ReportDesignerWindow : Window
    {
        public ReportDesignerWindow()
        {
            InitializeComponent();
        }

        public string ReportFilePath { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ReportFilePath)) return;
            if(File.Exists(ReportFilePath))
                reportDesigner.OpenDocument(ReportFilePath);
        }
    }
}
