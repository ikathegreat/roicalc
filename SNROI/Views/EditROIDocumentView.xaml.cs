using DevExpress.Xpf.Grid;
using SNROI.ViewModels;
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
using DevExpress.Xpf.Bars;

namespace SNROI.Views
{
    /// <summary>
    /// Interaction logic for EditROIDocumentView.xaml
    /// </summary>
    public partial class EditROIDocumentView : UserControl
    {
        public EditROIDocumentView()
        {
            InitializeComponent();
        }

        public void SaveDocumentGrids(string directory)
        {
            GridControlMaterials.SaveLayoutToXml(Path.Combine(directory, "MaterialsGrid.xml"));
            GridControlMachines.SaveLayoutToXml(Path.Combine(directory, "MachinesGrid.xml"));
            GridControlPeople.SaveLayoutToXml(Path.Combine(directory, "PeopleGrid.xml"));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            restoreGridLayoutFromXml(GridControlMaterials, "MaterialsGrid.xml");
            restoreGridLayoutFromXml(GridControlMachines, "MachinesGrid.xml");
            restoreGridLayoutFromXml(GridControlPeople, "PeopleGrid.xml");
        }

        private void restoreGridLayoutFromXml(GridControl grid, string xmlFileName)
        {
            var directory = (this.DataContext as ROIDocumentViewModel).DataDirectory;
            var gridXmlPath = Path.Combine(directory, "AppSettings", xmlFileName);
            if (File.Exists(gridXmlPath))
                grid.RestoreLayoutFromXml(gridXmlPath);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Expander.IsExpanded = !Expander.IsExpanded;

        }
    }
}
