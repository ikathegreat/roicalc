using SigmaTEK.Dialogs.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;
using SNROI.Models;
using SNROI.Reports;
using SNROI.ViewModels;
using IDialogService = SigmaTEK.Dialogs.IDialogService;

namespace SNROI
{
    public sealed class DialogService
    {
        private static volatile DialogService instance;
        private static readonly object syncRoot = new Object();
        private readonly IDialogService dialogService;

        public static DialogService Instance
        {
            get
            {
                if (instance != null) return instance;
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new DialogService();
                }

                return instance;
            }
        }

        private DialogService()
        {
            dialogService = new SigmaTEK.Dialogs.DialogService();
        }

        /// <summary>
        /// Closes the progress dialog.
        /// </summary>
        public void CloseProgressDialog()
        {
            dialogService.CloseProgressDialog();
        }

        /// <summary>
        /// Displays the text input dialog.
        /// </summary>
        /// <param name="title">(Optional) The dialog title.</param>
        /// <param name="message">(Optional) The dialog message.</param>
        /// <param name="instructions">(Optional) The instructions for the input.</param>
        /// <param name="defaultInput">(Optional) The default input value to display, is also the default return value if the dialog is suppressed.</param>
        /// <returns>
        /// The user input text.
        /// </returns>
        public string InputDialog(string title = "Input", string message = "Please enter some text.", string instructions = "", string defaultInput = "")
        {
            return dialogService.InputDialog(title, message, instructions, defaultInput);
        }

        /// <summary>
        /// Displays the license manager dialog and returns the selected sim ID.
        /// </summary>
        /// <param name="title">(Optional) The dialog title.</param>
        /// <param name="defaultHaspId">(Optional) The default hasp ID to display, is also the default return value if the dialog is suppressed.</param>
        /// <returns></returns>
        public string LicenseManagerDialog(string title = "License Manager", string defaultHaspId = "")
        {
            return dialogService.LicenseManagerDialog(title, defaultHaspId);
        }

        /// <summary>
        /// Displays the open file dialog and returns the selected file path.
        /// </summary>
        /// <param name="title">(Optional) The dialog title.</param>
        /// <param name="filter">(Optional) The file type filter (e.g. "All files (*.*)|*.*").</param>
        /// <param name="defaultFileName">(Optional) The file to display when the dialog opens, is also the default return value if the dialog is suppressed.</param>
        /// <returns>
        /// User selected file path.
        /// </returns>
        public string OpenFileDialog(string title = "Please select a file.", string filter = "All files (*.*)|*.*", string defaultFileName = "")
        {
            return dialogService.OpenFileDialog(title, filter, defaultFileName);
        }

        /// <summary>
        /// Displays the open file dialog and returns the user selected results or an empty SXOpenFileDialogResult object if the dialog is suppressed.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="filter">The file type filter (e.g. "All files (*.*)|*.*").</param>
        /// <param name="defaultFileName">The file to display when the dialog opens.</param>
        /// <param name="filterIndex">The initial filter index.</param>
        /// <returns>
        /// SXOpenFileDialogResult
        /// </returns>
        public SXOpenFileDialogResult OpenFileDialog(string title, string filter, string defaultFileName, int filterIndex)
        {
            var result = dialogService.OpenFileDialog(title, filter, defaultFileName, filterIndex);
            return new SXOpenFileDialogResult { SelectedFiles = result.SelectedFiles, SelectedFilterIndex = result.SelectedFilterIndex };
        }

        /// <summary>
        /// Displays the open files dialog and returns the user selected file paths or an empty string array if the dialog is suppressed.
        /// </summary>
        /// <param name="title">(Optional) The dialog title.</param>
        /// <param name="filter">(Optional) The file type filter (e.g. "All files (*.*)|*.*").</param>
        /// <param name="initialDirectory">(Optional) The directory to start with when the dialog opens.</param>
        /// <returns>
        /// User selected file paths.
        /// </returns>
        public string[] OpenMultiFileDialog(string title = "Please select one or more files.", string filter = "All files (*.*)|*.*", string initialDirectory = "")
        {
            return dialogService.OpenMultiFileDialog(title, filter, initialDirectory);
        }

        /// <summary>
        /// Displays the open files dialog and returns the user selected result or an empty SXOpenFileDialogResult object if the dialog is suppressed.
        /// </summary>
        /// <param name="title">The dialog title.</param>
        /// <param name="filter">The file type filter (e.g. "All files (*.*)|*.*").</param>
        /// <param name="initialDirectory">The directory to start with when the dialog opens.</param>
        /// <param name="filterIndex">The initial filter index.</param>
        /// <returns>
        /// SXOpenFileDialogResult
        /// </returns>
        public SXOpenFileDialogResult OpenMultiFileDialog(string title, string filter, string initialDirectory, int filterIndex)
        {
            var result = dialogService.OpenMultiFileDialog(title, filter, initialDirectory, filterIndex);
            return new SXOpenFileDialogResult { SelectedFiles = result.SelectedFiles, SelectedFilterIndex = result.SelectedFilterIndex };

        }

        /// <summary>
        /// Displays the file save as dialog.
        /// </summary>
        /// <param name="title">(Optional) The dialog title.</param>
        /// <param name="defaultFileName">(Optional) The default file name, is also the default return value if the dialog is suppressed.</param>
        /// <param name="filter">(Optional) The file type filter (e.g. "All files (*.*)|*.*").</param>
        /// <returns>
        /// True if file saved
        /// </returns>
        public string SaveFileDialog(string title = "Save File As", string defaultFileName = "", string filter = "All files (*.*)|*.*")
        {
            return dialogService.SaveFileDialog(title, defaultFileName, filter);
        }

        /// <summary>
        /// Displays the select folder dialog and returns the selected folder path.
        /// </summary>
        /// <param name="title">(Optional) The dialog title.</param>
        /// <param name="defaultFolderPath">(Optional) The folder to display when the dialog opens, is also the default return value if the dialog is suppressed.</param>
        /// <returns>
        /// User selected folder path.
        /// </returns>
        public string SelectFolderDialog(string title = "Please select a folder.", string defaultFolderPath = "")
        {
            return dialogService.SelectFolderDialog(title, defaultFolderPath);
        }

        /// <summary>
        /// Displays the SQL connection dialog and returns the connection string.
        /// </summary>
        /// <param name="title">(Optional) The dialog title.</param>
        /// <param name="defaultConnectionString">(Optional) The connection to display when the dialog opens, is also the default return value if the dialog is suppressed.</param>
        /// <returns>
        /// User selected connection string.
        /// </returns>
        public string SelectSQLDatabaseDialog(string title = "SQL Login", string defaultConnectionString = "")
        {
            return dialogService.SelectSQLDatabaseDialog(title, defaultConnectionString);
        }

        /// <summary>
        /// Sets the state of the progress dialog. If the percentage is supplied then the progress bar is no longer indeterminate.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="progressPercentage">The progress percentage.</param>
        public void SetProgressDialogState(string messageText, double progressPercentage = -1)
        {
            dialogService.SetProgressDialogState(messageText, progressPercentage);
        }

        /// <summary>
        /// Shows a generic message dialog.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="caption">(Optional) The dialog caption.</param>
        public void ShowMessage(string messageText, string caption = "")
        {
            dialogService.ShowMessage(messageText, caption);
        }

        /// <summary>
        /// Shows an exception message dialog.
        /// </summary>
        /// <param name="exception">The exception object.</param>
        /// <param name="caption">(Optional) The dialog caption.</param>
        public void ShowMessageError(Exception exception, string caption = "")
        {
            dialogService.ShowMessageError(exception, caption);
        }

        /// <summary>
        /// Shows an error dialog.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="caption">(Optional) The dialog caption.</param>
        public void ShowMessageError(string messageText, string caption = "")
        {
            dialogService.ShowMessageError(messageText, caption);
        }

        /// <summary>
        /// Shows an information dialog.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="caption">(Optional) The dialog caption.</param>
        public void ShowMessageInfo(string messageText, string caption = "")
        {
            dialogService.ShowMessageInfo(messageText, caption);
        }

        /// <summary>
        /// Shows a Yes/No dialog with boolean result.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="caption">(Optional) The dialog caption.</param>
        /// <param name="defaultValue">(Optional) The default return value if the dialog is suppressed.</param>
        /// <returns>
        /// True if the Yes button is clicked, otherwise false.
        /// </returns>
        public bool ShowMessageQuestion(string messageText, string caption = "", bool defaultValue = false)
        {
            return dialogService.ShowMessageQuestion(messageText, caption);
        }

        /// <summary>
        /// Shows a warning dialog.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="caption">(Optional) The dialog caption.</param>
        public void ShowMessageWarning(string messageText, string caption = "")
        {
            dialogService.ShowMessageWarning(messageText, caption);
        }

        /// <summary>
        /// Displays a Windows 8 style notification message.
        /// </summary>
        /// <param name="messageText">The message text.</param>
        /// <param name="caption">(Optional) The dialog caption.</param>
        /// <param name="playSound">(Optional) The option to play a notification sound.</param>
        /// <returns>
        /// True if the user clicks the notification, otherwise false.
        /// </returns>
        public Task<bool> ShowNotificationMessageAsync(string messageText, string caption = "", bool playSound = true)
        {
            return dialogService.ShowNotificationMessageAsync(messageText, caption, playSound);
        }

        /// <summary>
        /// Shows an indeterminate progress dialog centered to the parent window.
        /// </summary>
        /// <param name="title">(Optional) The title.</param>
        /// <param name="messageText">(Optional) The message text.</param>
        public void ShowProgressDialog(string title = "Please Wait...", string messageText = "Loading...")
        {
            dialogService.ShowProgressDialog(title, messageText);
        }

        private void OpenCustomDialog(Window dialog)
        {
            try
            {
                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowMessageError(ex, string.Format($"Show {dialog.Name} Exception"));
            }
        }


        public void ShowAboutDialog()
        {
            var aboutWindow = new AboutWindow();
            var aboutViewModel = new AboutViewModel();
            aboutWindow.DataContext = aboutViewModel;
            aboutViewModel.ClosingRequest += (sender, e) => aboutWindow.Close();
            OpenCustomDialog(aboutWindow);
        }
        public void ShowReportsDialog(string defaultDataDirectory)
        {
            var printReportWindow = new PrintReportWindow();
            var printReportsViewModel = new PrintReportsViewModel {DataDirectory = defaultDataDirectory};
            printReportWindow.DataContext = printReportsViewModel;
            printReportsViewModel.ClosingRequest += (sender, e) => printReportWindow.Close();
            OpenCustomDialog(printReportWindow);
        }

        public void ShowOpenROIDocumentDialog(string defaultDataDirectory, string documentPath = "")
        {
            var editROIDocWindow = new EditROIDocumentWindow();

            var roiDocumentViewModel = new ROIDocumentViewModel
            {
                DocumentPath = documentPath,
                DataDirectory = defaultDataDirectory
            };
            editROIDocWindow.DataContext = roiDocumentViewModel;

            roiDocumentViewModel.ClosingRequest += (sender, e) => editROIDocWindow.Close();
            roiDocumentViewModel.ROIDocument.FirePropertyChanged(nameof(roiDocumentViewModel.ROIDocument
                .CultureCurrencyPair));
            OpenCustomDialog(editROIDocWindow);
        }


        public void ShowImageBrowserWindow(ObservableCollection<string> docImageNameList, string imageDirectory)
        {
            var imageBrowserWindow = new ImageBrowserWindow();
            var imageBrowserViewModel = new ImageBrowserViewModel { DocumentImageList = docImageNameList, ImageDirectory = imageDirectory };
            imageBrowserViewModel.LoadExisingImages();
            imageBrowserWindow.DataContext = imageBrowserViewModel;
            imageBrowserViewModel.ClosingRequest += (sender, e) => imageBrowserWindow.Close();
            OpenCustomDialog(imageBrowserWindow);
        }

        public void ShowReportEditorDialog(string reportRepxFilePath = "")
        {
            //Todo: restore and save window settings
            ReportDesignerWindow reportEditorWindow;
            using (new WaitCursor())
            {
                reportEditorWindow = new ReportDesignerWindow { ReportFilePath = reportRepxFilePath };
            }
            if (reportEditorWindow != null)
                OpenCustomDialog(reportEditorWindow);
        }
        public void ShowReportPreviewDialog(string reportRepxFilePath = "")
        {
            var report = XtraReport.FromFile(reportRepxFilePath, true);
            //ReportPrintTool tool = new ReportPrintTool(report);
            //PrintHelper.ShowPrintPreviewDialog(this, new XtraReport1());
            // tool.ShowRibbonPreviewDialog();


            var window = new DocumentPreviewWindow();
            window.PreviewControl.DocumentSource = report;
            report.CreateDocument();
            window.Show();
        }
    }


    /// <summary>
    /// Implements <see cref="IOpenFileDialogResult" />
    /// </summary>
    public class SXOpenFileDialogResult : IOpenFileDialogResult
    {
        /// <summary>
        /// Gets or sets the selected files.
        /// </summary>
        /// <value>
        /// The selected files.
        /// </value>
        public string[] SelectedFiles { get; set; }

        /// <summary>
        /// Gets or sets the index of the selected filter.
        /// </summary>
        /// <value>
        /// The index of the selected filter.
        /// </value>
        public int SelectedFilterIndex { get; set; }

        public SXOpenFileDialogResult()
        {
            SelectedFiles = new string[0];
            SelectedFilterIndex = 0;
        }
    }
}
