using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace WindowsApplication1
{
    public partial class WatermarkView : Form
    {
        [DllImport("ole32.dll")]
        public extern static void CoUninitialize();

        [DllImport("ole32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern void CoInitialize(int pvReserved);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SendMessage(IntPtr hwnd, uint wMsg, uint wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, LVBKIMAGE lParam);

        private const int LVM_FIRST = 0x1000;
        private const int LVM_SETBKIMAGE = LVM_FIRST + 138;
        private const int LVM_SETTEXTBKCOLOR = LVM_FIRST + 38;
        private const int LVM_SETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 54;
        private const int LVS_EX_DOUBLEBUFFER = 0x10000;
        private const int LVBKIF_TYPE_WATERMARK = 0x10000000;
        private const int LVBKIF_SOURCE_NONE = 0;
        private CheckBox chkWatermark;
        private const uint CLR_NONE = 0xFFFFFFFF;

        [StructLayout(LayoutKind.Sequential)]
        public class LVBKIMAGE
        {
            public int ulFlags;
            public IntPtr hbm;
            public string pszImage;
            public int cchImageMax;
            public int xOffsetPercent;
            public int yOffsetPercent;
        }

        [STAThread]
        static void Main()
        {
            CoInitialize(0);

            // You MUST have EnableVisualStyles() or the watermark will NOT work
            Application.EnableVisualStyles();
            Application.Run(new WatermarkView());

            CoUninitialize();
        }

        public WatermarkView()
        {
            InitializeComponent();
        }

        IntPtr GetBMP()
        {
            Bitmap bitmap = null;
            Bitmap bitmap2 = null;

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("WindowsApplication1.Watermark.png");
            bitmap = new Bitmap(stream);

            stream = assembly.GetManifestResourceStream("WindowsApplication1.Watermark.png");
            bitmap2 = new Bitmap(stream);

            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(lvWatermark.BackColor);
            g.DrawImage(bitmap2, 0, 0, bitmap2.Width, bitmap2.Height);
            g.Dispose();

            return bitmap.GetHbitmap();
        }

        public void SetWatermark1()
        {
            IntPtr hBMP = GetBMP();
            if (!(hBMP == IntPtr.Zero))
            {
                LVBKIMAGE lv = new LVBKIMAGE();
                lv.hbm = hBMP;
                lv.ulFlags = LVBKIF_TYPE_WATERMARK;
                IntPtr lvPTR = Marshal.AllocCoTaskMem(Marshal.SizeOf(lv));
                Marshal.StructureToPtr(lv, lvPTR, false);
                SendMessage((int)this.lvWatermark.Handle, LVM_SETBKIMAGE, 0, (int)lvPTR);
                Marshal.FreeCoTaskMem(lvPTR);
            }
        }

        public void SetWatermark()
        {
            SetWatermark2();
        }

        public void RemoveWatermark()
        {
            LVBKIMAGE lv = new LVBKIMAGE();
            lv.hbm = IntPtr.Zero;
            lv.ulFlags = LVBKIF_TYPE_WATERMARK;
            SendMessage(lvWatermark.Handle, LVM_SETBKIMAGE, 0, lv);

            lv.ulFlags = LVBKIF_SOURCE_NONE;
            SendMessage(lvWatermark.Handle, LVM_SETBKIMAGE, 0, lv);
        }

        public void SetWatermark2()
        {
            IntPtr hBMP = GetBMP();

            LVBKIMAGE lv = new LVBKIMAGE();
            lv.hbm = hBMP;
            lv.ulFlags = LVBKIF_TYPE_WATERMARK;

            SendMessage(lvWatermark.Handle, LVM_SETTEXTBKCOLOR, 0, CLR_NONE);
            SendMessage(lvWatermark.Handle, LVM_SETBKIMAGE, 0, lv);
        }

        private void InitializeComponent()
        {
            this.lvWatermark = new System.Windows.Forms.ListView();
            this.cbStyles = new System.Windows.Forms.ComboBox();
            this.chkWatermark = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lvWatermark
            // 
            this.lvWatermark.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvWatermark.HideSelection = false;
            this.lvWatermark.Location = new System.Drawing.Point(16, 48);
            this.lvWatermark.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lvWatermark.Name = "lvWatermark";
            this.lvWatermark.Size = new System.Drawing.Size(757, 322);
            this.lvWatermark.TabIndex = 3;
            this.lvWatermark.UseCompatibleStateImageBehavior = false;
            // 
            // cbStyles
            // 
            this.cbStyles.FormattingEnabled = true;
            this.cbStyles.Location = new System.Drawing.Point(16, 15);
            this.cbStyles.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cbStyles.Name = "cbStyles";
            this.cbStyles.Size = new System.Drawing.Size(357, 24);
            this.cbStyles.TabIndex = 4;
            this.cbStyles.SelectedIndexChanged += new System.EventHandler(this.cbStyles_SelectedIndexChanged);
            // 
            // chkWatermark
            // 
            this.chkWatermark.AutoSize = true;
            this.chkWatermark.Location = new System.Drawing.Point(451, 16);
            this.chkWatermark.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkWatermark.Name = "chkWatermark";
            this.chkWatermark.Size = new System.Drawing.Size(95, 20);
            this.chkWatermark.TabIndex = 5;
            this.chkWatermark.Text = "Watermark";
            this.chkWatermark.UseVisualStyleBackColor = true;
            this.chkWatermark.CheckedChanged += new System.EventHandler(this.chkWatermark_CheckedChanged);
            // 
            // WatermarkView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(791, 385);
            this.Controls.Add(this.chkWatermark);
            this.Controls.Add(this.cbStyles);
            this.Controls.Add(this.lvWatermark);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "WatermarkView";
            this.Text = "Watermark";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ListView lvWatermark;
        private ComboBox cbStyles;

        private void Form2_Load(object sender, EventArgs e)
        {
            foreach (object o in System.Enum.GetValues(typeof(System.Windows.Forms.View)))
            {
                cbStyles.Items.Add(o.ToString());                
            }

            SendMessage(lvWatermark.Handle, LVM_SETEXTENDEDLISTVIEWSTYLE, LVS_EX_DOUBLEBUFFER, LVS_EX_DOUBLEBUFFER);

            lvWatermark.View = View.Details;
            lvWatermark.Columns.Add("Column1", 100);
            lvWatermark.Columns.Add("Column2", -2);

            for (int i = 0; i < 100; i++)
            {
                ListViewItem lvi = new ListViewItem("Item" + i);
                lvi.SubItems.Add("Subitem");

                lvWatermark.Items.Add(lvi);
            }

            cbStyles.Text = "Details";
        }

        private void cbStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbStyles.Text.Equals("Details"))
            {
                lvWatermark.View = View.Details;
            }
            else if (cbStyles.Text.Equals("SmallIcon"))
            {
                lvWatermark.View = View.SmallIcon;
            }
            else if (cbStyles.Text.Equals("LargeIcon"))
            {
                lvWatermark.View = View.LargeIcon;
            }
            else if (cbStyles.Text.Equals("List"))
            {
                lvWatermark.View = View.List;
            }
            else if (cbStyles.Text.Equals("Tile"))
            {
                lvWatermark.View = View.Tile;
            }
        }

        private void chkWatermark_CheckedChanged(object sender, EventArgs e)
        {
            if (chkWatermark.Checked)
            {
                SetWatermark();
            }
            else
            {
                RemoveWatermark();
            }
        }
    }
}

