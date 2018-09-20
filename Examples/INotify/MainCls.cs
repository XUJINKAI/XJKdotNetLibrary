using System.Collections.ObjectModel;

namespace INotify
{
    class MainCls : BaseAbsCls
    {
        public string MainString1 { get; set; }
        public ElementCls Element1 { get; set; }
        public ObservableCollection<ElementCls> List { get; set; }
    }
}
