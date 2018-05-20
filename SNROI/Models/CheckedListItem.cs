using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.Models
{
    public class CheckedListItem<T> : BaseModel
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
                FirePropertyChanged(nameof(Item));
            }
        }


        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                FirePropertyChanged(nameof(IsChecked));
            }
        }
    }
}
