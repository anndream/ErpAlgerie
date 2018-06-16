using MongoDbGenericRepository.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ErpAlgerie.Pages.Helpers
{
    public class PagingCollectionView : Collection<IDocument>, ITypedList,INotifyPropertyChanged
    {
        private   List<IDocument> _innerList;
        private   int _itemsPerPage;
        private int innerCount;

        public event PropertyChangedEventHandler PropertyChanged;

        public PagingCollectionView()
        {

        }

        private void NotifyPropertyChanged( String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected override void InsertItem(int index, IDocument item)
        {
            base.InsertItem(index, item);
            NotifyPropertyChanged("items");
        }
        protected override void SetItem(int index, IDocument item)
        {
            base.SetItem(index, item);
            NotifyPropertyChanged("items");
        }
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return null;
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return TypeDescriptor.GetProperties(Count > 0 ? this[0].GetType() : typeof(IDocument));
        }
    }
}
