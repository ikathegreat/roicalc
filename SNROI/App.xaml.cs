using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using SNROI.Models;
using SNROI.ViewModels;
using SNROI.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

namespace SNROI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            ApplicationThemeHelper.UpdateApplicationThemeName();

            DXSplashScreen.Show<SplashWindow>();

            var mainWindow = new MainWindow();
            var mainWindowViewModel =
                new MainWindowViewModel(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "SNROI"));
            mainWindow.DataContext = mainWindowViewModel;
            LoadWindowSettings(mainWindow);
            mainWindow.Closed += MainWindow_Closed;
            mainWindow.Loaded += MainWindow_Loaded;
            mainWindow.ShowDialog();
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            ApplicationThemeHelper.SaveApplicationThemeName();
        }

        private static void LoadWindowSettings(MainWindow mainWindow)
        {
            if (!(mainWindow.DataContext is MainWindowViewModel mainWindowViewModel))
                return;

            var windowSettingsPath = Path.Combine(mainWindowViewModel.DataDirectory, "AppSettings", "window.xml");
            if (!File.Exists(windowSettingsPath))
                return;
            TextReader reader = new StreamReader(windowSettingsPath);
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(WindowSettings));
                var windowSettings = (WindowSettings)xmlSerializer.Deserialize(reader);

                if (windowSettings.IsMaximized)
                {
                    mainWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    mainWindow.Height = windowSettings.Height;
                    mainWindow.Width = windowSettings.Width;
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                reader.Close();
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (!(sender is MainWindow mainWindow))
                return;

            if (!(mainWindow.DataContext is MainWindowViewModel mainWindowViewModel))
                return;

            var gridpath = Path.Combine(mainWindowViewModel.DataDirectory, "AppSettings", "grid.xml");

            var gridpathDir = Path.GetDirectoryName(gridpath);
            if (!Directory.Exists(gridpathDir))
                Directory.CreateDirectory(gridpathDir);

            mainWindow.GridControlROIDocuments.SaveLayoutToXml(gridpath);

            SaveWindowSettings(mainWindow);

            Shutdown(1);
        }

        private static void SaveWindowSettings(MainWindow mainWindow)
        {
            if (!(mainWindow.DataContext is MainWindowViewModel mainWindowViewModel))
                return;

            var windowSettingsPath = Path.Combine(mainWindowViewModel.DataDirectory, "AppSettings", "window.xml");

            TextWriter writer = new StreamWriter(windowSettingsPath);
            var xmlSerializer = new XmlSerializer(typeof(WindowSettings));

            try
            {
                var windowSettings = new WindowSettings
                {
                    Width = mainWindow.Width,
                    Height = mainWindow.Height,
                    IsMaximized = mainWindow.WindowState == WindowState.Maximized
                };
                xmlSerializer.Serialize(writer, windowSettings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception occured while writing to XML: " + ex.Message);
            }
            finally
            {
                writer.Close();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = sender as MainWindow;
            mainWindow.TableViewROIDocuments.RowDoubleClick += TableViewROIDocuments_RowDoubleClick;

            if (!(mainWindow.DataContext is MainWindowViewModel mainWindowViewModel))
                return;

            var gridpath = Path.Combine(mainWindowViewModel.DataDirectory, "AppSettings", "grid.xml");

            if (File.Exists(gridpath))
            {
                mainWindow.GridControlROIDocuments.RestoreLayoutFromXml(gridpath);
            }
            else
            {
                mainWindow.TableViewROIDocuments.BestFitColumns();
            }
            if (mainWindowViewModel.FSROIDocList.Count > 0)
                mainWindow.GridControlROIDocuments.SelectItem(0);

            DXSplashScreen.Close();
        }

        private void TableViewROIDocuments_RowDoubleClick(object sender, RowDoubleClickEventArgs e)
        {
            if (!(sender is TableView tableView))
                return;
            var mainWWindViewModel = tableView.DataContext as MainWindowViewModel;
            mainWWindViewModel?.OpenROIDocument();
        }
    }
}