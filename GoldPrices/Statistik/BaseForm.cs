using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Drawing;


namespace CMaurer.Common
{
    /// <summary>
    /// Basisklassen von allen Views. 
    /// </summary>
    public class BaseForm : Form
    {
        public const string ConfigFileName = "FiatKitFileTool.cfg";
        public delegate void ProgressCallbackPercentTextDelegate(int percent, string text);

        public const string LogFileName = @"d:\temp\fkft.txt";

        protected bool _bIgnoreControlEvents;
        protected bool _showWaitCursorDuringPopulate;

        /// <summary>
        /// Derived windows can use this to abort a lenghty action
        /// </summary>
        private bool _abort = false;

        private bool _mayFormClose = true;

        protected BaseForm()
        {
        }

        public void MessageBox(string msg)
        {
            System.Windows.Forms.MessageBox.Show(msg, "Statistik", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        public static string MakeMenuPath(System.Windows.Forms.Control item)
        {
            StringBuilder sb = new StringBuilder(item.Text);
            Control parent = item.Parent;

            while (parent != null && !string.IsNullOrEmpty(parent.Text))
            {
                sb.Insert(0, " > ");
                sb.Insert(0, parent.Text);
                parent = parent.Parent;
            }

            sb.Replace("...", "");

            return sb.ToString();
        }

        protected bool GetValueFromConfigSection(IProgressCallBack gui, Dictionary<string, ConfigSection> data, string section, string key, out string value)
        {
            bool success = false;

            value = "";

            ConfigSection configSection = null;

            if (data.TryGetValue(section, out configSection))
            {
                if (configSection.TryGetValue(key, out value))
                {
                    success = true;
                }
            }

            if (!success)
            {
                gui.ReportError(string.Format("Entry '{0}-{1} could not be found in config file {2}'", section, key, ConfigFileName));
            }

            return success;
        }

        public void DoEvents()
        {
            Application.DoEvents();
        }

        protected bool Abort
        {
            get { return _abort; }
            set { _abort = value; }
        }

        protected ListViewItem GetFirstSelectedListViewItem(ListView listView)
        {
            ListViewItem lvi = null;

            if (listView.SelectedItems.Count > 0)
            {
                lvi = listView.SelectedItems[0];
            }

            return lvi;
        }

        protected bool ShowWaitCursorDuringPopulate
        {
            set { _showWaitCursorDuringPopulate = value; }
        }


        internal bool MayFormClose
        {
            get {return _mayFormClose; }
            set {_mayFormClose = value; }
        }

        protected void DefaultListViewProperties(ListView lv)
        {
            lv.View = View.Details;
            lv.FullRowSelect = true;
            lv.MultiSelect = false;
            lv.HideSelection = false;
        }



        #region XableAllButtonsForLongOperation

        protected void XableAllButtonsForLongOperation(bool enable)
        {
            XableAllButtonsForLongOperation(null, null, enable);
        }

        /// <summary>
        /// Disable all Buttons and enable the abortButton, or enable all and disable the abortButton.
        /// </summary>
        /// <param name="abortButton">This button is set to !enable</param>
        /// <param name="enable">All other buttons are enabled to this value</param>
        protected void XableAllButtonsForLongOperation(Button abortButton, bool enable)
        {
            XableAllButtonsForLongOperation(abortButton, null, enable);
        }

        /// <summary>
        /// </summary>
        /// <param name="abortButton"></param>
        /// <param name="enable"></param>
        protected void XableAllButtonsForLongOperation(Button abortButton1, Button abortButton2, bool enable)
        {
            XAbleAllCommandsVisitor visitor = new XAbleAllCommandsVisitor(this, abortButton1, abortButton2, enable);
        }

        protected void XableAllButtonsForLongOperation(Button abortButton1, Button abortButton2, Button abortButton3, bool enable)
        {
            XAbleAllCommandsVisitor visitor = new XAbleAllCommandsVisitor(this, abortButton1, abortButton2, abortButton3, enable);
        }

        #endregion


        /// <summary>
        /// Set the selection to the line below the passed index, if there is one, otherwise 
        /// select the last line.
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="index"></param>
        protected void EnsureVisibleNearIndex(ListView listView, int index)
        {
            if (index < listView.Items.Count - 1)
            {
                //
                // count = 4:
                // 0  index
                // 1  index 
                // 2  index
                // 3  
                //
                // Focus auf die Zeile darunter
                //
                index++;
            }
            else
            {
                //
                // count = 4:
                // 0
                // 1
                // 2 
                // 3  index in letzter Zeile
                //
                // falls index >= Count ist
                //
                index = listView.Items.Count - 1;
            }

            if ((0 <= index) && (index < listView.Items.Count))
            {
                listView.EnsureVisible(index);
            }
        }

        #region Visitor Design pattern
        public void Accept(IControlsVisitor visitor)
        {
            foreach (Control c in this.Controls)
            {
                Accept(visitor, c);
            }
        }


        public void Accept(IControlsVisitor visitor, Control control)
        {
            visitor.VisitControl(control);

            if (control is MenuStrip)
            {
                MenuStrip menuStrip = (MenuStrip)control;
                visitor.VisitMenuStrip(menuStrip);
            }
            else if (control is System.Windows.Forms.Button)
            {
                System.Windows.Forms.Button button = (System.Windows.Forms.Button)control;
                visitor.VisitButton(button);
            }

            foreach (Control child in control.Controls)
            {
                Accept(visitor, child);
            }
        }

        public void Accept(IControlsVisitor visitor, MenuStrip item)
        {
            visitor.VisitMenuStrip(item);

            foreach (ToolStripMenuItem subitem in item.Items)
            {
                Accept(visitor, subitem);
            }
        }

        public void Accept(IControlsVisitor visitor, ToolStripItem item)
        {
            visitor.VisitToolStripItem(item);
        }

        public void Accept(IControlsVisitor visitor, ToolStripMenuItem item)
        {
            visitor.VisitToolStripMenuItem(item);

            for (int i = 0; i < item.DropDownItems.Count; i++)
            {
                if (item.DropDownItems[i] is ToolStripSeparator)
                {
                }
                else
                {
                    Accept(visitor, item.DropDownItems[i]);
                }
            }
        }

        #endregion

        
        protected void XableAllCommands(bool enable)
        {
            XAbleAllCommandsVisitor visitor = new XAbleAllCommandsVisitor(this, enable);

            Accept(visitor);
        }

        protected void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Name = "BaseForm";
            this.Load += new System.EventHandler(this.BaseForm_Load);
            this.ResumeLayout(false);

        }

        private void BaseForm_Load(object sender, EventArgs e)
        {

        }

        public void AddItem(ListView listView, string folder, string filename)
        {
            ListViewItem lvi = new ListViewItem(folder);
            lvi.SubItems.Add(filename);
            listView.Items.Add(lvi);
        }

        public void AddErrorItem(ListView listView, string folder, string filename, string info)
        {
            ListViewItem lvi = new ListViewItem(folder);
            lvi.ForeColor = Color.Red;
            lvi.SubItems.Add(filename);
            lvi.SubItems.Add(info);
            listView.Items.Add(lvi);
        }

        public void AddItem(ListView listView, string text1, string text2, string text3)
        {
            ListViewItem lvi = new ListViewItem(text1);
            lvi.SubItems.Add(text2);
            lvi.SubItems.Add(text3);
            listView.Items.Add(lvi);
        }

        public void AddMissingItem(ListView listView, string text1)
        {
            ListViewItem lvi = new ListViewItem("missing");
            lvi.ForeColor = Color.Red;
            lvi.SubItems.Add(text1);
            listView.Items.Add(lvi);
        }

        public void WriteLinetoLogFile(string text)
        {
            StreamWriter writer = new StreamWriter(LogFileName);
            writer.WriteLine(text);
            writer.Close();
            writer = null;
        }

        public void AddLineToLog(ListView lvLog, Exception exception)
        {
            AddLineToLog(lvLog, Tools.LogKeyError, exception.Message);
            AddLineToLog(lvLog, Tools.LogKeyError, exception.StackTrace.ToString());
        }

        public void AddLineToLog(ListView lvLog, string key, string text)
        {
            string timestamp = Tools.GetTimeStampInUserReadableFormat();
            string valueTs = timestamp + ": " + text;

            ListViewItem lvi = new ListViewItem(key);
            lvi.SubItems.Add(valueTs);
            lvLog.Items.Add(lvi);

            if (key == Tools.LogKeyError)
            {
                lvi.ForeColor = Color.Red;
            }

            lvLog.EnsureVisible(lvLog.Items.Count - 1);
            Application.DoEvents();
        }

        public void AddLineToLog(ListView lvLog, string key, string text, string text2)
        {
            string timestamp = Tools.GetTimeStampInUserReadableFormat();
            string valueTs = timestamp + ": " + text;

            ListViewItem lvi = new ListViewItem(key);
            lvi.SubItems.Add(valueTs);
            lvi.SubItems.Add(text2);
            lvLog.Items.Add(lvi);

            if (key == Tools.LogKeyError)
            {
                lvi.ForeColor = Color.Red;
            }

            lvLog.EnsureVisible(lvLog.Items.Count - 1);
            Application.DoEvents();
        }

        public void AddMissingItem(ListView listView, string text1, string text2)
        {
            ListViewItem lvi = new ListViewItem(text1);
            lvi.ForeColor = Color.Red;
            lvi.SubItems.Add(text2);
            lvi.SubItems.Add("missing");
            listView.Items.Add(lvi);
        }



        virtual public string GetApplicationTitle()
        {
            return "Missing application title!";
        }

        protected void ClearTextBoxes()
        {
            Action<Control.ControlCollection> func = null;
            //skip_text_change = true;

            func = (controls) =>
            {
                foreach (Control control in controls)
                {
                    if (control is TextBox)
                    {
                        (control as TextBox).Clear();
                    }
                    else
                    {
                        func(control.Controls);
                    }
                }
            };
            func(Controls);
            //skip_text_change = false;
        }

    }
}

