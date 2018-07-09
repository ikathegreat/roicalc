using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.DataAccess.ObjectBinding;
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

        public static void EditReport(string reportRepxFilePath = "", object dataSource = null)
        {
            var report = CreateXtraReportFromRepxWithDataSource(reportRepxFilePath, dataSource);

            report.ShowDesignerDialog(DXSkinNameHelper.GetUserLookAndFeelFromApplicationTheme());
        }

        public static void PrintReport(string reportRepxFilePath, object dataSource)
        {
            var report = CreateXtraReportFromRepxWithDataSource(reportRepxFilePath, dataSource);
            using (var printTool = new ReportPrintTool(report))
            {
                printTool.PrintDialog(DXSkinNameHelper.GetUserLookAndFeelFromApplicationTheme());
            }
        }


        public static void PreviewReport(string reportRepxFilePath, object dataSource)
        {
            var report = CreateXtraReportFromRepxWithDataSource(reportRepxFilePath, dataSource);
            using (var printTool = new ReportPrintTool(report))
            {
                printTool.ShowPreviewDialog(DXSkinNameHelper.GetUserLookAndFeelFromApplicationTheme());
            }
        }
    }
}
