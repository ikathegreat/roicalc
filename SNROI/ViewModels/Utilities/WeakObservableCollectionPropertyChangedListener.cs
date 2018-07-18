using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNROI.ViewModels.Utilities
{
    /// <summary>
    /// https://stackoverflow.com/a/19939747/1157509
    /// </summary>
    public class WeakObservableCollectionPropertyChangedListener<T> : INotifyPropertyChanged where T : INotifyPropertyChanged
    {
        private readonly ObservableCollection<T> collection;
        private readonly string propertyName;
        private readonly Dictionary<T, int> items = new Dictionary<T, int>(new ObjectIdentityComparer());
        public WeakObservableCollectionPropertyChangedListener(ObservableCollection<T> collection, string propertyName = "")
        {
            this.collection = collection;
            this.propertyName = propertyName ?? "";
            AddRange(collection);
            CollectionChangedEventManager.AddHandler(collection, CollectionChanged);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddRange(e.NewItems.Cast<T>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveRange(e.OldItems.Cast<T>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    AddRange(e.NewItems.Cast<T>());
                    RemoveRange(e.OldItems.Cast<T>());
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Reset();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void AddRange(IEnumerable<T> newItems)
        {
            foreach (var item in newItems)
            {
                if (items.ContainsKey(item))
                {
                    items[item]++;
                }
                else
                {
                    items.Add(item, 1);
                    PropertyChangedEventManager.AddHandler(item, ChildPropertyChanged, propertyName);
                }
            }
        }

        private void RemoveRange(IEnumerable<T> oldItems)
        {
            foreach (var item in oldItems)
            {
                items[item]--;
                if (items[item] != 0)
                    continue;
                items.Remove(item);
                PropertyChangedEventManager.RemoveHandler(item, ChildPropertyChanged, propertyName);
            }
        }

        private void Reset()
        {
            foreach (var item in items.Keys.ToList())
            {
                PropertyChangedEventManager.RemoveHandler(item, ChildPropertyChanged, propertyName);
                items.Remove(item);
            }
            AddRange(collection);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            handler?.Invoke(sender, e);
        }

        private class ObjectIdentityComparer : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return object.ReferenceEquals(x, y);
            }
            public int GetHashCode(T obj)
            {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }
        }
    }

    public static class WeakObservableCollectionPropertyChangedListener
    {
        public static WeakObservableCollectionPropertyChangedListener<T> Create<T>(ObservableCollection<T> collection, 
            string propertyName = "") where T : INotifyPropertyChanged
        {
            return new WeakObservableCollectionPropertyChangedListener<T>(collection, propertyName);
        }
    }
}


