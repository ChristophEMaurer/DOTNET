using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace CMaurer.Common
{
    public class OplListView : ListView
    {
        public delegate void HScrollDelegate(ref Message m);
        public event HScrollDelegate OnHScroll;

        //
        // Balkengrafik
        //
        private bool _balkenGrafik = false;
        protected Brush _balkenHintergrund = Brushes.LightBlue;
        protected Brush _balkenIst = Brushes.Blue;
        protected Brush _balkenText = Brushes.White;
        private int _balkenColumnIndex = -1;

        //
        // Sorting
        //
        private bool _sortable = false;
        private ListViewColumnSorter _lvColumnSorter;
        private int _previouslySortedColumn = -1;

        //
        // Editing
        //
        private bool _inPlaceEditing = false;

        /// <summary>
        /// Event Handler for SubItem events
        /// </summary>
        public delegate void SubItemEventHandler(object sender, SubItemEventArgs e);
        /// <summary>
        /// Event Handler for SubItemEndEditing events
        /// </summary>
        public delegate void SubItemEndEditingEventHandler(object sender, SubItemEndEditingEventArgs e);

        public event SubItemEventHandler SubItemClicked;
        public event SubItemEventHandler SubItemBeginEditing;
        public event SubItemEndEditingEventHandler SubItemEndEditing;

        /// <summary>
        /// Event Args for SubItemClicked event
        /// </summary>
        public class SubItemEventArgs : EventArgs
        {
            public SubItemEventArgs(ListViewItem item, int subItem)
            {
                _subItemIndex = subItem;
                _item = item;
            }
            private int _subItemIndex = -1;
            private ListViewItem _item = null;
            public int SubItem
            {
                get { return _subItemIndex; }
            }
            public ListViewItem Item
            {
                get { return _item; }
            }
        }


        /// <summary>
        /// Event Args for SubItemEndEditingClicked event
        /// </summary>
        public class SubItemEndEditingEventArgs : SubItemEventArgs
        {
            private string _text = string.Empty;
            private bool _cancel = true;

            public SubItemEndEditingEventArgs(ListViewItem item, int subItem, string display, bool cancel) :
                base(item, subItem)
            {
                _text = display;
                _cancel = cancel;
            }
            public string DisplayText
            {
                get { return _text; }
                set { _text = value; }
            }
            public bool Cancel
            {
                get { return _cancel; }
                set { _cancel = value; }
            }
        }


        public OplListView()
        {
        }

        #region Balkengrafik

        public void SetBalkenColumnIndex(int balkenColumnIndex)
        {
            _balkenColumnIndex = balkenColumnIndex;
        }

        //
        // Can only be switched on for now
        //
        [Category("Logbuch"),
         Browsable(true),
         DefaultValue(typeof(bool), "false"),
         Description("Defines whether a bar is displayed for a selected column")]
        public bool BalkenGrafik
        {
            get { return _balkenGrafik; }
            set
            {
                if ((!_balkenGrafik) && (value == true) && (View == View.Details))
                {
                    FullRowSelect = true;
                    MultiSelect = false;

                    OwnerDraw = true;
                    DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(lv_DrawColumnHeader);
                    DrawItem += new DrawListViewItemEventHandler(lv_DrawItem);
                    DrawSubItem += new DrawListViewSubItemEventHandler(lv_DrawSubItem);
                    MouseMove += new MouseEventHandler(lv_MouseMove);
                    MouseUp += new MouseEventHandler(lv_MouseUp);
                }
            }
        }

        private void lv_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawBackground();
            e.DrawText();
        }

        private void lv_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //e.DrawText();
        }

        private void lv_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex == _balkenColumnIndex)
            {
                DrawPercentage(e);
            }
            else
            {
                e.DrawText();
            }
        }

        private void lv_MouseUp(object sender, MouseEventArgs e)
        {
            /*
            // Inhalt der ersten Spalte holen in der geklickten Zeile
             
            ListViewItem clickedItem = GetItemAt(5, e.Y);
            if (clickedItem != null)
            {
                clickedItem.Selected = true;
                clickedItem.Focused = true;
            }
                
            */
        }

        private void lv_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = GetItemAt(e.X, e.Y);
            if (item != null && item.Tag == null)
            {
                Invalidate(item.Bounds);
                item.Tag = "tagged";
            }
        }

        static public int ProzentFromBalkenGrafikData(string s)
        {
            double ist;
            double max;

            return ProzentFromBalkenGrafikData(s, out ist, out max);
        }

        static public int ProzentFromBalkenGrafikData(string s, out double ist, out double total)
        {
            string[] arValues = s.Split('|');

            ist = double.Parse(arValues[0]);
            total = double.Parse(arValues[1]);
            int prozent = 0;

            if (total > 0)
            {
                if (ist <= total)
                {
                    prozent = Convert.ToInt32(ist * 100 / total);
                }
                else
                {
                    prozent = 100;
                }
            }

            return prozent;
        }

        private void DrawPercentage(DrawListViewSubItemEventArgs e)
        {
            double ist;
            double max;
            int prozent = ProzentFromBalkenGrafikData(e.SubItem.Text, out ist, out max);

            if (ist > max)
            {
                ist = max;
            }

            if (max > 0 && ist <= max)
            {
                Rectangle rect = e.Bounds;
                Graphics g = e.Graphics;

                rect.Inflate(-1, -1);

                int totalWidth = rect.Width;
                e.Graphics.FillRectangle(_balkenHintergrund, rect);

                rect.Width = (int)((double)rect.Width / max * ist);
                e.Graphics.FillRectangle(_balkenIst, rect);

                //if (prozent > 0)
                {
                    string s;
                    if (prozent >= 10)
                    {
                        s = string.Format("{0}%", prozent);
                    }
                    else
                    {
                        s = string.Format(" {0}%", prozent);
                    }
                    e.Graphics.DrawString(s, new Font("Courier New", 8), _balkenText,
                        rect.Left + totalWidth / 2, rect.Top - 2);
                }
            }
        }
        #endregion

        #region Sorting
        //
        // Can only be switched on for now
        //
        [Category("Logbuch"),
         Browsable(true),
         DefaultValue(typeof(bool), "false"),
         Description("Defines whether this listview has sortable columns")]
        public bool Sortable
        {
            get { return _sortable; }
            set
            {
                if (!_sortable && value)
                {
                    _sortable = true;
                    _lvColumnSorter = new ListViewColumnSorter();
                    this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnColumnClick);
                    this.ListViewItemSorter = _lvColumnSorter;
                }
                else if (_sortable && !value)
                {
                    _sortable = false;
                    this.ColumnClick -= new System.Windows.Forms.ColumnClickEventHandler(this.OnColumnClick);
                    this.ListViewItemSorter = null;
                }
            }
        }

        private void OnColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListViewColumnClicked(sender, e);
        }

        protected void ListViewColumnClicked(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == _lvColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (_lvColumnSorter.Order == SortOrder.Ascending)
                {
                    _lvColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    _lvColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _lvColumnSorter.SortColumn = e.Column;
                _lvColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.Sort();

            SetSortIcons(this, _previouslySortedColumn, e.Column, _lvColumnSorter.Order);
        }

        private void SetSortIcons(ListView _listView, int previouslySortedColumn, int newSortColumn, SortOrder order)
        {
            //
            // http://www.thebitguru.com/articles/16-How%2520to%2520Set%2520ListView%2520Column%2520Header%2520Sort%2520Icons%2520in%2520C%23/209-Run-down
            //

            IntPtr hHeader = Win32Interop.SendMessage(_listView.Handle, Win32Interop.LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
            IntPtr newColumn = new IntPtr(newSortColumn);
            IntPtr prevColumn = new IntPtr(previouslySortedColumn);
            Win32Interop.HDITEM hdItem;
            IntPtr rtn;

            // Only update the previous item if it existed and if it was a different one.
            if (previouslySortedColumn != -1 && previouslySortedColumn != newSortColumn)
            {
                // Clear icon from the previous column.
                hdItem = new Win32Interop.HDITEM();
                hdItem.mask = Win32Interop.HDI_FORMAT;
                rtn = Win32Interop.SendMessageITEM(hHeader, Win32Interop.HDM_GETITEM, prevColumn, ref hdItem);
                hdItem.fmt &= ~Win32Interop.HDF_SORTDOWN & ~Win32Interop.HDF_SORTUP;
                rtn = Win32Interop.SendMessageITEM(hHeader, Win32Interop.HDM_SETITEM, prevColumn, ref hdItem);
            }

            // Set icon on the new column.
            hdItem = new Win32Interop.HDITEM();
            hdItem.mask = Win32Interop.HDI_FORMAT;
            rtn = Win32Interop.SendMessageITEM(hHeader, Win32Interop.HDM_GETITEM, newColumn, ref hdItem);
            if (order == SortOrder.Ascending)
            {
                // add HDF_STRING
                hdItem.fmt &= ~Win32Interop.HDF_SORTDOWN;
                hdItem.fmt |= Win32Interop.HDF_SORTUP;
            }
            else if (order == SortOrder.Descending)
            {
                // add HDF_STRING
                hdItem.fmt &= ~Win32Interop.HDF_SORTUP;
                hdItem.fmt |= Win32Interop.HDF_SORTDOWN;
            }
            rtn = Win32Interop.SendMessageITEM(hHeader, Win32Interop.HDM_SETITEM, newColumn, ref hdItem);
            _previouslySortedColumn = newSortColumn;
        }
        #endregion

        #region Watermark
        private IntPtr GetBitmap(ListView lv, string resourceName)
        {
            Bitmap bitmap = null;
            Bitmap bitmap2 = null;

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            bitmap = new Bitmap(stream);

            stream = assembly.GetManifestResourceStream(resourceName);
            bitmap2 = new Bitmap(stream);

            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(lv.BackColor);
            g.DrawImage(bitmap2, 0, 0, bitmap2.Width, bitmap2.Height);
            g.Dispose();

            return bitmap.GetHbitmap();
        }

        public void SetDoubleBuffer()
        {
            Win32Interop.SendMessage(this.Handle,
                Win32Interop.LVM_SETEXTENDEDLISTVIEWSTYLE,
                Win32Interop.LVS_EX_DOUBLEBUFFER,
                Win32Interop.LVS_EX_DOUBLEBUFFER);
        }

        public void SetWatermark(Bitmap bitmap)
        {
            Win32Interop.SendMessage(this.Handle,
                Win32Interop.LVM_SETEXTENDEDLISTVIEWSTYLE,
                Win32Interop.LVS_EX_DOUBLEBUFFER,
                Win32Interop.LVS_EX_DOUBLEBUFFER);

            SetWatermark2(this, bitmap);
        }

        public void RemoveWatermark()
        {
            Win32Interop.LVBKIMAGE lv = new Win32Interop.LVBKIMAGE();
            lv.hbm = IntPtr.Zero;
            lv.ulFlags = Win32Interop.LVBKIF_TYPE_WATERMARK;
            Win32Interop.SendMessage(this.Handle, Win32Interop.LVM_SETBKIMAGE, 0, lv);

            lv.ulFlags = Win32Interop.LVBKIF_SOURCE_NONE;
            Win32Interop.SendMessage(this.Handle, Win32Interop.LVM_SETBKIMAGE, 0, lv);
        }

        private void SetWatermark1(ListView listView, Bitmap bitmap)
        {
            Win32Interop.LVBKIMAGE lv = new Win32Interop.LVBKIMAGE();
            lv.hbm = bitmap.GetHbitmap();
            lv.ulFlags = Win32Interop.LVBKIF_TYPE_WATERMARK;
            IntPtr lvPTR = Marshal.AllocCoTaskMem(Marshal.SizeOf(lv));
            Marshal.StructureToPtr(lv, lvPTR, false);
            Win32Interop.SendMessage((int)listView.Handle, Win32Interop.LVM_SETBKIMAGE, 0, (int)lvPTR);
            Marshal.FreeCoTaskMem(lvPTR);
        }

        private void SetWatermark2(ListView listView, Bitmap bitmap)
        {
            Win32Interop.LVBKIMAGE lv = new Win32Interop.LVBKIMAGE();
            lv.hbm = bitmap.GetHbitmap();
            lv.ulFlags = Win32Interop.LVBKIF_TYPE_WATERMARK;

            Win32Interop.SendMessage(listView.Handle, Win32Interop.LVM_SETTEXTBKCOLOR, 0, Win32Interop.CLR_NONE);
            Win32Interop.SendMessage(listView.Handle, Win32Interop.LVM_SETBKIMAGE, 0, lv);
        }

        #endregion

        #region Interop
        protected void HandleHorizontalScroll(ref Message m)
        {
            if (OnHScroll != null)
            {
                OnHScroll(ref m);
            }
        }


        /// <summary>
        /// Gets and Sets the Horizontal Scroll position of the control.
        /// </summary>
        [Category("Logbuch"),
         Browsable(true),
         Description("Gets the horizontal scroll position of the control")]
        public int HScrollPos
        {
            get { return Win32Interop.GetScrollPos((IntPtr)this.Handle, Win32Interop.SB_HORZ); }
        }
        #endregion

        #region Editing

        [Category("Logbuch"),
             Browsable(true),
             DefaultValue(typeof(bool), "false"),
             Description("Defines whether one can edit in place or not")]
        public bool InPlaceEditing
        {
            get { return _inPlaceEditing; }
            set
            {
                if ((!_inPlaceEditing) && (value == true) && (View == View.Details))
                {
                }
            }
        }

		private bool _doubleClickActivation = false;
		/// <summary>
		/// Is a double click required to start editing a cell?
		/// </summary>
		public bool DoubleClickActivation
		{
			get {  return _doubleClickActivation; }
			set { _doubleClickActivation = value; }    
		}


		/// <summary>
		/// Retrieve the order in which columns appear
		/// </summary>
		/// <returns>Current display order of column indices</returns>
		public int[] GetColumnOrder()
		{
			IntPtr lPar	= Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) * Columns.Count);

            IntPtr res = Win32Interop.SendMessage(Handle, Win32Interop.LVM_GETCOLUMNORDERARRAY, new IntPtr(Columns.Count), lPar);
			if (res.ToInt32() == 0)	// Something went wrong
			{
				Marshal.FreeHGlobal(lPar);
				return null;
			}

			int	[] order = new int[Columns.Count];
			Marshal.Copy(lPar, order, 0, Columns.Count);

			Marshal.FreeHGlobal(lPar);

			return order;
		}


		/// <summary>
		/// Find ListViewItem and SubItem Index at position (x,y)
		/// </summary>
		/// <param name="x">relative to ListView</param>
		/// <param name="y">relative to ListView</param>
		/// <param name="item">Item at position (x,y)</param>
		/// <returns>SubItem index</returns>
		public int GetSubItemAt(int x, int y, out ListViewItem item)
		{
			item = this.GetItemAt(x, y);
		
			if (item !=	null)
			{
				int[] order = GetColumnOrder();
				Rectangle lviBounds;
				int	subItemX;

				lviBounds =	item.GetBounds(ItemBoundsPortion.Entire);
				subItemX = lviBounds.Left;
				for (int i=0; i<order.Length; i++)
				{
					ColumnHeader h = this.Columns[order[i]];
					if (x <	subItemX+h.Width)
					{
						return h.Index;
					}
					subItemX += h.Width;
				}
			}
			
			return -1;
		}


		/// <summary>
		/// Get bounds for a SubItem
		/// </summary>
		/// <param name="Item">Target ListViewItem</param>
		/// <param name="SubItem">Target SubItem index</param>
		/// <returns>Bounds of SubItem (relative to ListView)</returns>
		public Rectangle GetSubItemBounds(ListViewItem Item, int SubItem)
		{
			int[] order = GetColumnOrder();

			Rectangle subItemRect = Rectangle.Empty;
			if (SubItem >= order.Length)
				throw new IndexOutOfRangeException("SubItem  " +SubItem + " out of range");

			if (Item == null)
				throw new ArgumentNullException("Item");
			
			Rectangle lviBounds = Item.GetBounds(ItemBoundsPortion.Entire);
			int	subItemX = lviBounds.Left;

			ColumnHeader col;
			int i;
			for (i=0; i<order.Length; i++)
			{
				col = this.Columns[order[i]];
                if (col.Index == SubItem)
                {
                    break;
                }
				subItemX += col.Width;
			} 
			subItemRect	= new Rectangle(subItemX, lviBounds.Top, this.Columns[order[i]].Width, lviBounds.Height);
			return subItemRect;
        }

        #endregion


        /*        protected override void WndProc(ref Message m)
        {
            const Int32 WM_HSCROLL = 0x114;

            if (m.Msg == WM_HSCROLL)
            {
                //if (m.WParam.ToInt32() == 5)//SB_THUMBTRACK       = 5,
                {
                    HandleHorizontalScroll(ref m);
                }
            }

            base.WndProc(ref m);
        }
*/

		protected override void	WndProc(ref	Message	msg)
		{
			switch (msg.Msg)
			{
				// Look	for	WM_VSCROLL,WM_HSCROLL or WM_SIZE messages.
                case Win32Interop.WM_VSCROLL:
                    HandleHorizontalScroll(ref msg);
                    EndEditing(false);
                    break;

                case Win32Interop.WM_HSCROLL:
                case Win32Interop.WM_SIZE:
                    EndEditing(false);
					break;

                case Win32Interop.WM_NOTIFY:
					// Look for WM_NOTIFY of events that might also change the
					// editor's position/size: Column reordering or resizing
                    Win32Interop.NMHDR h = (Win32Interop.NMHDR)Marshal.PtrToStructure(msg.LParam, typeof(Win32Interop.NMHDR));
                    if (h.code == Win32Interop.HDN_BEGINDRAG ||
                        h.code == Win32Interop.HDN_ITEMCHANGINGA ||
                        h.code == Win32Interop.HDN_ITEMCHANGINGW)
                    {
                        EndEditing(false);
                    }
#if false
                    else if (h.code == Win32Interop.HDN_ITEMCHANGEDW)
                    {
                        int headerHandle = (IntPtr)Win32Interop.SendMessage((int)Handle, Win32Interop.LVM_GETHEADER,0,0);
                    int right=0;
                    int left=0;
                    Win32Interop.TOOLINFO ti = new Win32Interop.TOOLINFO();
                    ti.cbSize=Marshal.SizeOf(ti);
                    ti.uFlags=Win32Interop.TTF_SUBCLASS;
                    ti.hwnd=headerHandle;
                    for (int i=0; i<this.Columns.Count; i++)
                    {
                    if (i<this.Columns.Count)
                    right+=this.Columns[i].Width;
                    else
                    right=this.Width;
                    ti.uId=i;
                    if (i<this.Columns.Count)
                    ti.lpszText=this.Columns[i].Text;
                    else
                    ti.lpszText="";
                    Rectangle rect= new System.Drawing.Rectangle(left,0,right,20);
                    ti.rect=rect;
                    // SendMessage(toolTipHandle, TTM_DELTOOLW, 0, ref ti);
                    Win32Interop.SendMessage(toolTipHandle, Win32Interop.TTM_ADDTOOLW, 0, ref ti);
                    left=right;
                    }
#endif
                    break;
			}

			base.WndProc(ref msg);
		}


		#region Initialize editing depending of DoubleClickActivation property
		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if (DoubleClickActivation)
			{
				return;
			} 

			EditSubitemAt(new Point(e.X, e.Y));
		}
		
		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick (e);

			if (!DoubleClickActivation)
			{
				return;
			} 

			Point pt = this.PointToClient(Cursor.Position);
		
			EditSubitemAt(pt);
		}

		///<summary>
		/// Fire SubItemClicked
		///</summary>
		///<param name="p">Point of click/doubleclick</param>
		private void EditSubitemAt(Point p)
		{
			ListViewItem item;
			int idx = GetSubItemAt(p.X, p.Y, out item);
			if (idx >= 0)
			{
				OnSubItemClicked(new SubItemEventArgs(item, idx));
			}
		}

		#endregion

		#region In-place editing functions
		// The control performing the actual editing
		private Control _editingControl;
		// The LVI being edited
		private ListViewItem _editItem;
		// The SubItem being edited
		private int _editSubItem;

		protected void OnSubItemBeginEditing(SubItemEventArgs e)
		{
			if (SubItemBeginEditing != null)
				SubItemBeginEditing(this, e);
		}
		protected void OnSubItemEndEditing(SubItemEndEditingEventArgs e)
		{
			if (SubItemEndEditing != null)
				SubItemEndEditing(this, e);
		}
		protected void OnSubItemClicked(SubItemEventArgs e)
		{
			if (SubItemClicked != null)
				SubItemClicked(this, e);
		}


		/// <summary>
		/// Begin in-place editing of given cell
		/// </summary>
		/// <param name="c">Control used as cell editor</param>
		/// <param name="Item">ListViewItem to edit</param>
		/// <param name="SubItem">SubItem index to edit</param>
		public void StartEditing(Control c, ListViewItem Item, int SubItem)
		{
			OnSubItemBeginEditing(new SubItemEventArgs(Item, SubItem));

			Rectangle rcSubItem = GetSubItemBounds(Item, SubItem);

			if (rcSubItem.X < 0)
			{
				// Left edge of SubItem not visible - adjust rectangle position and width
				rcSubItem.Width += rcSubItem.X;
				rcSubItem.X=0;
			}
			if (rcSubItem.X+rcSubItem.Width > this.Width)
			{
				// Right edge of SubItem not visible - adjust rectangle width
				rcSubItem.Width = this.Width-rcSubItem.Left;
			}

			// Subitem bounds are relative to the location of the ListView!
			rcSubItem.Offset(Left, Top);

			// In case the editing control and the listview are on different parents,
			// account for different origins
			Point origin = new Point(0,0);
			Point lvOrigin  = this.Parent.PointToScreen(origin);
			Point ctlOrigin  = c.Parent.PointToScreen(origin);

			rcSubItem.Offset(lvOrigin.X-ctlOrigin.X, lvOrigin.Y-ctlOrigin.Y);

			// Position and show editor
			c.Bounds = rcSubItem;
			c.Text = Item.SubItems[SubItem].Text;
			c.Visible = true;
			c.BringToFront();
			c.Focus();

			_editingControl = c;
			_editingControl.Leave += new EventHandler(_editControl_Leave);
			_editingControl.KeyPress += new KeyPressEventHandler(_editControl_KeyPress);

			_editItem = Item;
			_editSubItem = SubItem;
		}


		private void _editControl_Leave(object sender, EventArgs e)
		{
			// cell editor losing focus
			EndEditing(true);
		}

		private void _editControl_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			switch (e.KeyChar)
			{
				case (char)(int)Keys.Escape:
				{
					EndEditing(false);
					break;
				}

				case (char)(int)Keys.Enter:
				{  
					EndEditing(true);
					break;
				}
			}
		}

		/// <summary>
		/// Accept or discard current value of cell editor control
		/// </summary>
		/// <param name="AcceptChanges">Use the _editingControl's Text as new SubItem text or discard changes?</param>
		public void EndEditing(bool AcceptChanges)
		{
			if (_editingControl == null)
				return;

			SubItemEndEditingEventArgs e = new SubItemEndEditingEventArgs(
				_editItem,		// The item being edited
				_editSubItem,	// The subitem index being edited
				AcceptChanges ?
					_editingControl.Text :	// Use editControl text if changes are accepted
					_editItem.SubItems[_editSubItem].Text,	// or the original subitem's text, if changes are discarded
				!AcceptChanges	// Cancel?
			);

			OnSubItemEndEditing(e);

			_editItem.SubItems[_editSubItem].Text = e.DisplayText;

			_editingControl.Leave -= new EventHandler(_editControl_Leave);
			_editingControl.KeyPress -= new KeyPressEventHandler(_editControl_KeyPress);

			_editingControl.Visible = false;

			_editingControl = null;
			_editItem = null;
			_editSubItem = -1;
		}
		#endregion
    }
}
