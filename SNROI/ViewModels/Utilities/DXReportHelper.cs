using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using SNROI.Enums;

namespace SNROI.ViewModels.Utilities
{
    public static class DXReportHelper
    {
        private static XtraReport CreateXtraReportFromRepxWithDataSource(string reportRepxFilePath, object dataSource)
        {
            UIServices.SetBusyState();

            var report = XtraReport.FromFile(reportRepxFilePath, true);

            if (dataSource != null)
            {
                var objectDataSource = new ObjectDataSource
                {
                    Constructor = new ObjectConstructorInfo(),
                    DataSource = (dataSource)
                };
                report.DataSource = objectDataSource;
            }

            report.CreateDocument();
            return report;
        }

        /// <summary>
        /// Export a .repx report with a datasource to an exported file type
        /// </summary>
        /// <param name="reportRepxFilePath"></param>
        /// <param name="dataSource"></param>
        /// <param name="reportExportType"></param>
        /// <param name="targetDirectory"></param>
        /// <param name="fileName"></param>
        public static void ExportReport(string reportRepxFilePath, object dataSource,
            ReportExportType reportExportType, string targetDirectory, string fileName /*No Extension*/)
        {

            var report = CreateXtraReportFromRepxWithDataSource(reportRepxFilePath, dataSource);

            var targetFilePath = string.Empty;
            Directory.CreateDirectory(targetDirectory);
            switch (reportExportType)
            {
                case ReportExportType.CSV:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".csv");
                    report.ExportToCsv(targetFilePath);
                    break;
                case ReportExportType.DOCX:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".docx");
                    report.ExportToDocx(targetFilePath);
                    break;
                case ReportExportType.HTML:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".html");
                    report.ExportToHtml(targetFilePath);
                    break;
                case ReportExportType.JPEG:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".jpg");
                    report.ExportToImage(targetFilePath);
                    break;
                case ReportExportType.PDF:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".pdf");
                    report.ExportToPdf(targetFilePath);
                    break;
                case ReportExportType.PNG:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".png");
                    report.ExportToImage(targetFilePath);
                    break;
                case ReportExportType.RTF:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".rtf");
                    report.ExportToRtf(targetFilePath);
                    break;
                case ReportExportType.Text:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".txt");
                    report.ExportToText(targetFilePath);
                    break;
                case ReportExportType.XLSX:
                    targetFilePath = Path.Combine(targetDirectory, fileName + ".xlsx");
                    report.ExportToXlsx(targetFilePath);
                    break;
            }
            if (!File.Exists(targetFilePath))
            {
                throw new FileNotFoundException("Failed to export report");
            }
        }

        /// <summary>
        /// Open WPF DX Report Designer dialog from a .repx file and bound datasource
        /// </summary>
        /// <param name="reportRepxFilePath"></param>
        /// <param name="dataSource"></param>
        public static void EditReport(string reportRepxFilePath = "", object dataSource = null)
        {
            var report = CreateXtraReportFromRepxWithDataSource(reportRepxFilePath, dataSource);
            report.DisplayName = Path.GetFileNameWithoutExtension(reportRepxFilePath);
            //DevExpress.XtraReports.Configuration.Settings.Default.StorageOptions.RootDirectory = Path.GetDirectoryName(reportRepxFilePath);
            //report.ShowDesignerDialog(DXSkinNameHelper.GetUserLookAndFeelFromApplicationTheme());

            DialogService.Instance.ShowReportEditorWindow(report);

        }

        /// <summary>
        /// Open WPF DX print dialog to print a .repx file with bound datasource
        /// </summary>
        /// <param name="reportRepxFilePath"></param>
        /// <param name="dataSource"></param>
        public static void PrintReport(string reportRepxFilePath, object dataSource)
        {
            var report = CreateXtraReportFromRepxWithDataSource(reportRepxFilePath, dataSource);
            PrintHelper.Print(report);
        }

        /// <summary>
        /// Open WPF DX preview dialog to preview a .repx file with bound datasource
        /// </summary>
        /// <param name="reportRepxFilePath"></param>
        /// <param name="dataSource"></param>
        public static void PreviewReport(string reportRepxFilePath, object dataSource)
        {
            var report = CreateXtraReportFromRepxWithDataSource(reportRepxFilePath, dataSource);
            report.DisplayName = Path.GetFileNameWithoutExtension(reportRepxFilePath);
            DialogService.Instance.ShowReportPreviewWindow(report);
        }
    }
}
