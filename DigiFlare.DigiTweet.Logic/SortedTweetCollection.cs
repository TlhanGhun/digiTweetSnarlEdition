using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet
{
    public class SortedTweetCollection<T> : ObservableCollection<T>
        where T : Tweet
    {
        #region Constructor

        public SortedTweetCollection()
            : base()
        {
        }

        #endregion

        #region Public Methods

        protected override void InsertItem(int index, T item)
        {
            int sortedIndex = GetIndex(item);
            base.InsertItem(sortedIndex, item);
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotSupportedException("SortedTweetCollection will automatically sort items by date descending on insert");
        }

        protected override void SetItem(int index, T item)
        {
            throw new NotSupportedException("SortedTweetCollection will automatically sort items by date descending on insert");
        }

        public void Add(IList<T> collection)
        {
            foreach (T item in collection)
            {
                this.Add(item);
            }
        }

        new public void Remove(T removeItem)
        {
            foreach (T item in this)
            {
                if (item.Id.Equals(removeItem.Id))
                {
                    base.Remove(item);
                    return;
                }
            }
        }

        public void RefreshTweet(T oldStatus, T newStatus)
        {
            foreach (T item in this)
            {
                if (item.Id.Equals(oldStatus.Id))
                {
                    this.Remove(oldStatus);
                    this.Add(newStatus);
                    return;
                }
            }
        }

        public T GetNewest()
        {
            if (this.Count == 0)
            {
                return null;
            }
            return this[0];
        }

        public T GetOldest()
        {
            if (this.Count == 0)
            {
                return null;
            }
            return this[this.Count - 1];
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Finds the appropriate index to insert the item into the sorted collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private int GetIndex(T item)
        {
            // return index 0 when collection is empty
            if (this.Count == 0)
            {
                return 0;
            }

            // find the index of the given item
            for (int i = 0; i < this.Count; i++)
            {
                if (item.CreatedAt >= this[i].CreatedAt)
                {
                    return i;
                }
            }
            return this.Count;
        }

        #endregion

        // test
        //public void NotifyRebind()
        //{
        //    System.Collections.Specialized.NotifyCollectionChangedEventArgs args = new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset);
        //    OnCollectionChanged(args);
        //}
    }
}
