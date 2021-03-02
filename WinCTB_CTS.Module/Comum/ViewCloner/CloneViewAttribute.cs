using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinCTB_CTS.Module.Comum.ViewCloner
{
    public enum CloneViewType
    {
        DetailView,
        ListView,
        LookupListView
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CloneViewAttribute : Attribute
    {
        readonly string _viewId;
        readonly CloneViewType _viewType;

        public CloneViewAttribute(CloneViewType viewType, string viewId)
        {
            _viewType = viewType;
            _viewId = viewId;
        }

        public string DetailView { get; set; }


        public string ViewId => _viewId;


        public CloneViewType ViewType => _viewType;
    }
}
