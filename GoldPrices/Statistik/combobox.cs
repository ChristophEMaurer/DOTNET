
namespace fsd
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Data;

    public class DataGridComboBoxColumn : DataGridTextBoxColumn
    {
        public NoKeyUpCombo ColumnComboBox;
        private CurrencyManager _source;
        private int _rowNum;
        private bool _isEditing;
    		
        public DataGridComboBoxColumn() : base()
        {
            _source = null;
            _isEditing = false;
    	
            ColumnComboBox = new NoKeyUpCombo();
            ColumnComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
    		
            ColumnComboBox.Leave += new EventHandler(LeaveComboBox);
            ColumnComboBox.Enter += new EventHandler(ComboMadeCurrent);
    		
        }
        private void HandleScroll(object sender, EventArgs e)
        {
            if(ColumnComboBox.Visible)
                ColumnComboBox.Hide();

        }
        private void ComboMadeCurrent(object sender, EventArgs e)
        {
            _isEditing = true; 	
        }
    		
        private void LeaveComboBox(object sender, EventArgs e)
        {
            if(_isEditing)
            {
                SetColumnValueAtRow(_source, _rowNum, ColumnComboBox.Text);
                _isEditing = false;
                Invalidate();
            }
            ColumnComboBox.Hide();
            this.DataGridTableStyle.DataGrid.Scroll -= new EventHandler(HandleScroll);
    			
        }

        protected override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible)
        {
            if (this.ReadOnly)
            {
                return;
            }

            base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible);

            _rowNum = rowNum;
            _source = source;
    		
            ColumnComboBox.Parent = this.TextBox.Parent;
            ColumnComboBox.Location = this.TextBox.Location;
            ColumnComboBox.Size = new Size(this.TextBox.Size.Width, ColumnComboBox.Size.Height);
            ColumnComboBox.SelectedIndex = ColumnComboBox.FindStringExact(this.TextBox.Text);
            ColumnComboBox.Text =  this.TextBox.Text;
            this.TextBox.Visible = false;
            ColumnComboBox.Visible = true;
            this.DataGridTableStyle.DataGrid.Scroll += new EventHandler(HandleScroll);
    			
            ColumnComboBox.BringToFront();
            ColumnComboBox.Focus();	
        }

        protected override bool Commit(System.Windows.Forms.CurrencyManager dataSource, int rowNum)
        {
            if(_isEditing)
            {
                _isEditing = false;
                SetColumnValueAtRow(dataSource, rowNum, ColumnComboBox.Text);
            }
            return true;
        }

        protected override void ConcedeFocus()
        {
            base.ConcedeFocus();
        }

        protected override object GetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum)
        {
            object o = base.GetColumnValueAtRow(source, rowNum);

            if (DBNull.Value.Equals(o))
            {
                return DBNull.Value;
            }
            int s =  (int) o;
            DefTableCache ds = (DefTableCache) this.ColumnComboBox.DataSource;

            return ds.getDescriptionForPk(s);
        }

        protected override void SetColumnValueAtRow(System.Windows.Forms.CurrencyManager source, int rowNum, object value)
        {
            object s = value;

            if (DBNull.Value.Equals(s))
            {
                return;
            }

            DefTableCache ds = (DefTableCache) this.ColumnComboBox.DataSource;

            s = ds.getPkForDescription((string) s);
            base.SetColumnValueAtRow(source, rowNum, s);
        }
    }

    public class NoKeyUpCombo : ComboBox
    {
        private const int WM_KEYUP = 0x101;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if(m.Msg == WM_KEYUP)
            {
                //ignore keyup to avoid problem with tabbing & dropdownlist;
                return;
            }
            base.WndProc(ref m);
        }
    }
}
