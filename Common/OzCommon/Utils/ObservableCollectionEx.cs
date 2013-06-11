using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace OzCommon.Utils
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        #region Data

        private Dispatcher dispatcher;

        #endregion

        #region Ctor

        public ObservableCollectionEx(Dispatcher dispatcher = null)
        {
            this.dispatcher = dispatcher ?? Dispatcher.CurrentDispatcher;
        }

        #endregion

        #region Overrides


        protected override void ClearItems()
        {
            
            dispatcher.InvokeIfRequired(() =>
            {
                try
                {
                    base.ClearItems();
                }
                catch
                {
                }
            }, DispatcherPriority.DataBind);

        }



        /// <summary>

        /// Inserts an item

        /// </summary>

        protected override void InsertItem(int index, T item)
        {
            dispatcher.InvokeIfRequired(() =>
            {

                if (index > Count)

                    return;

                try
                {
                    base.InsertItem(index, item);

                }
                catch
                {
                }



            }, DispatcherPriority.DataBind);



        }

        public void AddItems(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>

        /// Moves an item

        /// </summary>

        protected override void MoveItem(int oldIndex, int newIndex)
        {

            dispatcher.InvokeIfRequired(() =>
            {

                Int32 itemCount = Count;


                if (oldIndex >= itemCount |

                    newIndex >= itemCount |

                    oldIndex == newIndex)

                    return;


                try
                {
                    base.MoveItem(oldIndex, newIndex);
                }
                catch
                {

                }

            }, DispatcherPriority.DataBind);

        }


        protected override void RemoveItem(int index)
        {

            dispatcher.InvokeIfRequired(() =>
            {
                if (index >= this.Count)
                    return;
                try
                {
                    base.RemoveItem(index);
                }
                catch
                {

                }

            }, DispatcherPriority.DataBind);

        }







        /// <summary>

        /// Sets an item

        /// </summary>

        protected override void SetItem(int index, T item)
        {
            dispatcher.InvokeIfRequired(() =>
            {

                try
                {
                    base.SetItem(index, item);
                }
                catch
                {

                }

            }, DispatcherPriority.DataBind);

        }

        #endregion

        public void Sort<TKey>(Func<T, TKey> keySelector, ListSortDirection direction)
        {
            switch (direction)
            {
                case ListSortDirection.Ascending:
                    {
                        ApplySort(Items.OrderBy(keySelector));
                        break;
                    }
                case ListSortDirection.Descending:
                    {
                        ApplySort(Items.OrderByDescending(keySelector));
                        break;
                    }
            }
        }

        public void Sort<TKey>(Func<T, TKey> keySelector, IComparer<TKey> comparer)
        {
            ApplySort(Items.OrderBy(keySelector, comparer));
        }

        void ApplySort(IEnumerable<T> sortedItems)
        {
            var sortedItemsList = sortedItems.ToList();

            foreach (var item in sortedItemsList)
            {
                Move(IndexOf(item), sortedItemsList.IndexOf(item));
            }
        }


    }
}
