using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XJK.WPF
{
    /// <summary>
    /// ListViewX.xaml 的交互逻辑
    /// </summary>
    public partial class ListViewX : ListView
    {
        //public new IEnumerable ItemsSource
        //{
        //    get
        //    {
        //        return base.ItemsSource;
        //    }
        //    set
        //    {
        //        base.ItemsSource = value;
        //    }
        //}

        public ListViewX()
        {

        }

        public void NextItem()
        {
            SelectedIndex += 1;
            if (SelectedIndex < 1)
            {
                SelectedIndex = Items.Count - 1;
            }
        }

        public void PrevItem()
        {
            SelectedIndex -= 1;
            if (SelectedIndex < 0)
            {
                SelectedIndex = 0;
            }
        }

        private GridViewColumnHeader lastHeaderClicked = null;
        private ListSortDirection lastDirection = ListSortDirection.Ascending;

        private void GridViewColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            ListSortDirection direction;
            if (e.OriginalSource is GridViewColumnHeader clickedHeader)
            {
                if (clickedHeader.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (clickedHeader != lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }
                    string sortString = clickedHeader.Column.GetSortBy();
                    if (!string.IsNullOrEmpty(sortString))
                    {
                        Sort(sortString, direction);

                        lastHeaderClicked = clickedHeader;
                        lastDirection = direction;
                    }
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(ItemsSource ?? Items);
            dataView.SortDescriptions.Clear();
            SortDescription sD = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sD);
            dataView.Refresh();
        }

        public void RefreshView()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource ?? Items);
            view.Refresh();
        }

    }
}
