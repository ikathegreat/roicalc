using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DevExpress.DataProcessing;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using SNROI.Models;

namespace SNROI.ViewModels
{
    public class ImageBrowserViewModel : BaseViewModel
    {
        private readonly ObservableCollection<string> imageFilesToCopy = new ObservableCollection<string>();
        private readonly ObservableCollection<string> imageFilesToDelete = new ObservableCollection<string>();

        private ObservableCollection<string> documentImageList;
        public ObservableCollection<string> DocumentImageList
        {
            get => documentImageList ?? (documentImageList = new ObservableCollection<string>());
            set => documentImageList = value;
        }

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

        public void LoadExisingImages()
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
            DocumentImageList.ForEach(x => DisplayImageList.Remove(x));


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
            var messageText = SelectedImageNamesList.Count == 1
                ? "Delete "
                  + SelectedImageNamesList[0]
                : "Delete" + SelectedImageNamesList.Count + " images?";

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

        public ICommand OKCommand => new RelayCommand(OKWindow);

        private void OKWindow()
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
                DocumentImageList.Clear();
                foreach (var selectedFile in SelectedImageNamesList)
                {
                    DocumentImageList.Add(selectedFile);
                }
            }
            FireCloseRequest();
        }

        public ICommand CancelCommand => new RelayCommand(CancelWindow);

        private void CancelWindow()
        {
            FireCloseRequest();
        }
    }
}
