using GalaSoft.MvvmLight;

namespace SNROI.Models
{
    public class CheckedListItem<T> : ObservableObject
    {
        private bool isChecked;
        private T item;

        public CheckedListItem()
        {

        }

        public CheckedListItem(T item, bool isChecked = false)
        {
            this.item = item;
            this.isChecked = isChecked;
        }

        public T Item
        {
            get => item;
            set
            {
                item = value;
                RaisePropertyChanged(nameof(Item));
            }
        }


        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                RaisePropertyChanged(nameof(IsChecked));
            }
        }
    }
}
