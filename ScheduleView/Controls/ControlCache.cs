using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScheduleView.Wpf.Controls
{
    internal class ControlCache<T> : IControlCache<T> where T : class, new()
    {
        private readonly Queue<T> freeItems;
        private readonly HashSet<T> rentedItems;

        public ControlCache(int initialControlInstances)
        {
            freeItems = new Queue<T>(100);
            rentedItems = new HashSet<T>(100);

            for (int controlIndex = 0; controlIndex < initialControlInstances; controlIndex++)
            {
                freeItems.Enqueue(new T());
            }
        }

        public ICacheItem<T> Get()
        {
            return new CacheItem<T>(this);
        }

        private T RentItem()
        {
            T item = null;

            if (freeItems.Count > 0)
            {
                item = freeItems.Dequeue();
            }
            else
            {
                item = new T();
            }

            rentedItems.Add(item);

            return item;
        }

        private void ReturnItem(T item)
        {
            rentedItems.Remove(item);
            freeItems.Enqueue(item);
        }

        private class CacheItem<TItem> : ICacheItem<TItem> where TItem : class, new()
        {
            private readonly ControlCache<TItem> controlCache;
            private TItem item;

            public CacheItem(ControlCache<TItem> controlCache)
            {
                this.controlCache = controlCache;
            }

            public TItem Item
            {
                get
                {
                    if (item == null)
                    {
                        item = controlCache.RentItem();
                    }

                    return item;
                }
            }

            public void Dispose()
            {
                if (item != null)
                {
                    controlCache.ReturnItem(item);
                }
            }
        }
    }

    interface IControlCache<out T>
    {
        ICacheItem<T> Get();
    }

    interface ICacheItem<out T> : IDisposable
    {
        T Item { get; }
    }
}
