using DevExpress.Mvvm;
using GalaSoft.MvvmLight.CommandWpf;
using SNROI.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace SNROI.ViewModels
{
    public class ImageBrowserViewModel : ViewModelBase
    {
        private readonly ObservableCollection<string> imageFilesToCopy = new ObservableCollection<string>();
        private readonly ObservableCollection<string> imageFilesToDelete = new ObservableCollection<string>();

        private ObservableCollection<string> displayImageList;

        public ObservableCollection<string> DisplayImageList
        {
            get => displayImageList ?? (displayImageList = new ObservableCollection<string>());
            set => displayImageList = value;
        }

        private ObservableCollection<string> selectedImageNamesList;

        public ObservableCollection<string> SelectedImageNamesList
        {
            get => selectedImageNamesList ?? (selectedImageNamesList = new ObservableCollection<string>());
            set => selectedImageNamesList = value;
        }

        public string ImageDirectory { get; set; }

        public ICommand AddImageCommand => new RelayCommand(ImportImage);

        public void LoadExistingImages()
        {
            if (string.IsNullOrEmpty(ImageDirectory))
                return;
            if (!Directory.Exists(ImageDirectory))
                return;

            var existingFiles = Directory.GetFiles(ImageDirectory);
            foreach (var existingFile in existingFiles)
            {
                DisplayImageList.Add(Path.GetFileName(existingFile));
            }
        }

        private void ImportImage()
        {
            var fileArray = DialogService.Instance.OpenMultiFileDialog("Select Images", "Image Files|*.png");
            if (fileArray == null)
                return;
            var fileList = fileArray.ToList();
            foreach (var file in fileList)
            {
                DisplayImageList.Add(Path.GetFileName(file));
                imageFilesToCopy.Add(file);
            }
            //FirePropertyChanged(nameof(DocumentImageList));
        }

        public ICommand RemoveImageCommand => new RelayCommand(DeleteImages);

        private void DeleteImages()
        {
            if (SelectedImageNamesList.Count == 0)
                return;
            var effectOnExistingReportsMessage = "ROI documents using deleted images must be updated to preview, export, or print." + Environment.NewLine + Environment.NewLine;
            var messageText = SelectedImageNamesList.Count == 1
                ? effectOnExistingReportsMessage + "Delete "
                  + SelectedImageNamesList[0] + "?"
                : effectOnExistingReportsMessage + "Delete" + SelectedImageNamesList.Count + " images?";

            if (!DialogService.Instance.ShowMessageQuestion(messageText, "Delete Images"))
                return;

            var imagesToDelete = new List<string>();
            foreach (var selectedImage in SelectedImageNamesList)
            {
                imagesToDelete.Add(selectedImage);
                imageFilesToDelete.Add(selectedImage);
            }
            foreach (var imageToRemove in imagesToDelete)
            {
                DisplayImageList.Remove(imageToRemove);
            }
        }

        private RelayCommand okCommand;
        public RelayCommand OkCommand
        {
            get
            {
                return okCommand
                    ?? (okCommand = new RelayCommand(
                    () =>
                    {
                        if (!string.IsNullOrEmpty(ImageDirectory))
                        {
                            if (!Directory.Exists(ImageDirectory))
                                Directory.CreateDirectory(ImageDirectory);

                            foreach (var file in imageFilesToCopy)
                            {
                                var fileName = Path.GetFileName(file);
                                try
                                {
                                    File.Copy(file, Path.Combine(ImageDirectory, fileName), false);
                                }
                                catch (Exception ex)
                                {
                                    DialogService.Instance.ShowMessageError(ex);
                                }
                            }
                            foreach (var file in imageFilesToDelete)
                            {
                                var fileName = Path.GetFileName(file);
                                try
                                {
                                    File.Delete(Path.Combine(ImageDirectory, fileName));
                                }
                                catch (Exception ex)
                                {
                                    DialogService.Instance.ShowMessageError(ex);
                                }
                            }
                        }
                    }));
            }
        }
    }
}