using System;
using System.Windows.Forms;

namespace CMaurer.Common
{
    public class SortableListView : OplListView
    {
        public SortableListView()
        {
            View = View.Details;

            Sortable = true;
        }
    }
}

