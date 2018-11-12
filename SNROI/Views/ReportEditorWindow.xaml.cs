using DevExpress.XtraReports.UI;
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

namespace SNROI.Views
{
    /// <summary>
    /// Interaction logic for ReportEditorWindow.xaml
    /// </summary>
    public partial class ReportEditorWindow : Window
    {
        public ReportEditorWindow()
        {
            InitializeComponent();
        }

        private XtraReport xtraReportSource;
        public XtraReport XtraReportSource
        {
            get => xtraReportSource ?? (xtraReportSource = new XtraReport());
            set => xtraReportSource = value;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reportDesigner.DocumentSource = XtraReportSource;
        }
    }
}
