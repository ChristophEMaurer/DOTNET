using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

using AppFramework;

namespace Windows.Forms
{
    /// <summary>
    /// Basisklassen von allen Views. Enthält den Zugriff 
    /// auf den BusinessLayer
    /// </summary>
    public abstract class DatabaseForm : Form
    {
        protected abstract string GetFormNameForResourceTexts();
        public abstract void DebugPrintControlClicked(Control control);
        public abstract void DebugPrintControlContents(Control control);
        
        private const string FormName = "DatabaseForm";

        protected BusinessLayerBase _businessLayerBase;

        protected DatabaseForm()
        {
        }

        protected DatabaseForm(BusinessLayerBase oBusinessLayer)
        {
            _businessLayerBase = oBusinessLayer;
        }

        public static string BuildFullControlName(Control control)
        {
            StringBuilder sb = new StringBuilder("");

            while (control != null)
            {
                if (!string.IsNullOrEmpty(control.Name))
                {
                    if (!string.IsNullOrEmpty(control.Text))
                    {
                        //
                        // We don't want this: ribbon[""].
                        //
                        sb.Insert(0, control.Name + "[\"" + control.Text + "\"].");
                    }
                }

                if (control is Form)
                {
                    break;
                }

                control = control.Parent;
            }

            return sb.ToString();
        }

        public string GetText(string page, string id)
        {
            return _businessLayerBase.GetText(page, id);
        }

        public string GetTextForTextBox(string page, string id)
        {
            return _businessLayerBase.GetTextForTextBox(page, id);
        }

        /// <summary>
        /// Holt den Text in der eingestellten Sprache. Format ist immer
        /// "FormName_TextID"
        /// Es wird immer automatisch Form.Name vorangestellt. Basisklassen müssen daher ihren Formnamen explizit angeben, 
        /// das immer die unterste Ableitung den endgültigen Form.Name setzt.
        /// </summary>
        /// <param name="id">Die Id des Textes. Form.Name wird vorangestellt</param>
        /// <returns></returns>
        public string GetText(string id)
        {
            return _businessLayerBase.GetText(GetFormNameForResourceTexts(), id);
        }

        public string GetTextForTextBox(string id)
        {
            return _businessLayerBase.GetTextForTextBox(GetFormNameForResourceTexts(), id);
        }

        public int ConvertToInt32(object o)
        {
            return _businessLayerBase.ConvertToInt32(o);
        }

        public long ConvertToInt64(object o)
        {
            return _businessLayerBase.ConvertToInt64(o);
        }

        public string AppTitle()
        {
            return AppTitle("");
        }

        public string AppTitle(string text)
        {
            string windowText = _businessLayerBase.AppTitle(text);

#if DEBUG
            windowText = this.GetType().Name + "-" + windowText;
#endif

            return windowText;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DatabaseForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "DatabaseForm";
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Prompt the User, default button is NO
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        protected bool Confirm(string strText)
        {
            return DialogResult.Yes == System.Windows.Forms.MessageBox.Show(strText, 
                _businessLayerBase.AppTitle(), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        }

        public DialogResult MessageBox(string strText)
        {
            return System.Windows.Forms.MessageBox.Show(strText, _businessLayerBase.AppTitle());
        }

        public DialogResult MessageBox(string strText, MessageBoxButtons buttons)
        {
            return System.Windows.Forms.MessageBox.Show(strText, _businessLayerBase.AppTitle(), buttons);
        }

        // Die muessen abstract sein und nicht virtual, aber dann kann der Designer keine
        // Instanz dieser Klasse erzeugen und alles ist dahin.
        protected virtual bool ValidateInput()
        {
            return true;
        }
        protected virtual void Object2Control() 
        {
        }
        protected virtual void Control2Object() 
        {
        }
        protected virtual void SaveObject() 
        {
        }

        protected virtual void CancelClicked()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected virtual void OKClicked()
        {
            if (ValidateInput())
            {
                Control2Object();
                SaveObject();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected int GetFirstSelectedIndex(ListView lv)
        {
            int index = -1;

            foreach (int i in lv.SelectedIndices)
            {
                index = i;
                break;
            }
            return index;
        }

        protected ListViewItem GetFirstSelectedLVI(ListView lv)
        {
            return GetFirstSelectedLVI(lv, false);
        }

        protected ListViewItem GetFirstSelectedLVI(ListView lv, bool bMessageIfNoneSelected)
        {
            return GetFirstSelectedLVI(lv, bMessageIfNoneSelected, null);
        }

        protected ListViewItem GetFirstSelectedLVI(ListView lv, bool bMessageIfNoneSelected, string lvName)
        {
            ListViewItem lvi = null;
            foreach (ListViewItem l in lv.SelectedItems)
            {
                lvi = l;
                break;
            }

            if ((lvi == null) && bMessageIfNoneSelected)
            {
                if (lvName == null)
                {
                    MessageBox(GetText(FormName, "msg1"));
                }
                else
                {
                    string msg = string.Format(CultureInfo.InvariantCulture, GetText(FormName, "msg2"), lvName);
                    MessageBox(msg);
                }
            }

            return lvi;
        }

        protected int GetFirstSelectedTag(ListView lv)
        {
            return GetFirstSelectedTag(lv, false, GetText(FormName, "eintrag"));
        }
        protected int GetFirstSelectedTag(ListView lv, bool bMessageIfNoneSelected)
        {
            return GetFirstSelectedTag(lv, bMessageIfNoneSelected, GetText(FormName, "eintrag"));
        }

        protected void SetSelectionByTagValue(ListView lv, int tagValue)
        {
            for (int i = 0; i < lv.Items.Count; i++)
            {
                if ((int)lv.Items[i].Tag == tagValue)
                {
                    lv.EnsureVisible(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Return the tag as an int of the first selected entry, or -1 if nothing is selected
        /// </summary>
        /// <param name="lv">The ListView</param>
        /// <param name="bMessageIfNoneSelected">Display a message box if no line is selected</param>
        /// <param name="strText">The message box text</param>
        /// <returns>-1 if nothing is selected or the tag as an int</returns>
        protected int GetFirstSelectedTag(ListView lv, bool bMessageIfNoneSelected, string strText)
        {
            int nID = -1;
            foreach (ListViewItem lvi in lv.SelectedItems)
            {
                nID = (int)lvi.Tag;
                break;
            }

            if (nID == -1 && bMessageIfNoneSelected)
            {
                string msg = GetText(FormName, "msg1");
                if (!string.IsNullOrEmpty(strText))
                {
                    msg = msg + ": " + strText;
                }
                MessageBox(msg);
            }

            return nID;
        }
        protected string GetFirstSelectedTagString(ListView lv)
        {
            return GetFirstSelectedTagString(lv, false, GetText(FormName, "eintrag"));
        }
        protected string GetFirstSelectedTagString(ListView lv, bool bMessageIfNoneSelected)
        {
            return GetFirstSelectedTagString(lv, bMessageIfNoneSelected, GetText(FormName, "eintrag"));
        }
        protected string GetFirstSelectedTagString(ListView lv, bool bMessageIfNoneSelected, string strText)
        {
            string s = null;
            foreach (ListViewItem lvi in lv.SelectedItems)
            {
                s = (string)lvi.Tag;
                break;
            }

            if (s == null && bMessageIfNoneSelected)
            {
                string msg = string.Format(CultureInfo.InvariantCulture, GetText(FormName, "msg2"), strText);
                MessageBox(msg);
            }

            return s;
        }

        protected void DefaultListViewProperties(OplListView lv)
        {
            lv.View = View.Details;
            lv.FullRowSelect = true;
            lv.ShowItemToolTips = true;
            lv.GridLines = false;
            lv.HideSelection = false;

            // 1, 2, 11 erscheint bei Sortierung als 1, 11, 2
            // lv.Sorting = SortOrder.Ascending;

            lv.SetDoubleBuffer();
        }
    }
}
