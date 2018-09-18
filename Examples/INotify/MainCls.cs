using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XJK.NotifyPropertyChanged;

namespace INotify
{
    class MainCls : BaseAbsCls
    {
        public string MainString1 { get; set; }
        public ElementCls Element1 { get; set; }
        public ObservableCollectionEx<ElementCls> List { get; set; }
    }
}
