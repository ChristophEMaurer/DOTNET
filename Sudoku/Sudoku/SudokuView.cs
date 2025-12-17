using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
//using System.Messaging;

namespace Sudoku
{
    public partial class frmMain : Form
    {
        const int OFFSET = 50;
        const int DELTA = 40;

        protected int nFocusXOld = -1;
        protected int nFocusYOld = -1;
        protected int nFocusX = 0;
        protected int nFocusY = 0;

        Sudoku sudoku;

        public frmMain()
        {
            InitializeComponent();
            sudoku = new Sudoku();
            sudoku.Solution += new SolutionEventHandler(OnSolution);

            //string result = ML.Test();
        }

        private void DrawFocus()
        {
            using (Graphics g = this.CreateGraphics())
            {
                DrawFocus(g);
            }
        }

        private void DrawFocus(Graphics g)
        {
            // focus
            int nOffset = 4;

            // remove old focus
            if (nFocusXOld != -1)
            {
                using (Pen pen = new Pen(this.BackColor))
                {
                    g.DrawRectangle(pen,
                        OFFSET + nFocusXOld * DELTA + nOffset, OFFSET + nFocusYOld * DELTA + nOffset,
                        DELTA - 2 * nOffset, DELTA - 2 * nOffset);
                }
            }

            // draw new focus
            g.DrawRectangle(System.Drawing.Pens.Red,
                OFFSET + nFocusX * DELTA + nOffset, OFFSET + nFocusY * DELTA + nOffset,
                DELTA - 2 * nOffset, DELTA - 2 * nOffset);

        }

        private void DrawGrid(Graphics g)
        {
            int i;

            // horizontal lines
            for (i = 0; i < 10; i++)
            {
                if (i % 3 == 0)
                {
                    g.DrawLine(System.Drawing.Pens.Black, OFFSET, OFFSET + i * DELTA - 1, OFFSET + 9 * DELTA, OFFSET + i * DELTA - 1);
                    g.DrawLine(System.Drawing.Pens.Black, OFFSET, OFFSET + i * DELTA + 1, OFFSET + 9 * DELTA, OFFSET + i * DELTA + 1);
                }
                g.DrawLine(System.Drawing.Pens.Black, OFFSET, OFFSET + i * DELTA, OFFSET + 9 * DELTA, OFFSET + i * DELTA);
            }

            // vertical lines
            for (i = 0; i < 10; i++)
            {
                if (i % 3 == 0)
                {
                    g.DrawLine(System.Drawing.Pens.Black, OFFSET + i * DELTA - 1, OFFSET, OFFSET + i * DELTA - 1, OFFSET + 9 * DELTA);
                    g.DrawLine(System.Drawing.Pens.Black, OFFSET + i * DELTA + 1, OFFSET, OFFSET + i * DELTA + 1, OFFSET + 9 * DELTA);
                }
                g.DrawLine(System.Drawing.Pens.Black, OFFSET + i * DELTA, OFFSET, OFFSET + i * DELTA, OFFSET + 9 * DELTA);
            }

            using (Font drawFont = new Font("Arial", 12))
            {
                string s;
                using (SolidBrush drawBrush = new SolidBrush(Color.Black))
                {
                    byte[] x = new byte[1];

                    // x-Axis
                    for (i = 0; i < 9; i++)
                    {
                        x[0] = (byte) (97 + i);
                        s = Encoding.ASCII.GetString(x);
                        g.DrawString(s, drawFont, drawBrush, OFFSET + i * DELTA + 10, 25);
                    }

                    // y-Axis
                    for (i = 0; i < 9; i++)
                    {
                        g.DrawString((i + 1).ToString(), drawFont, drawBrush, 25, OFFSET + i * DELTA + 10);
                    }
                }
            }

            DrawFocus(g);
        }

        private void DrawNumber(Graphics g, int nValue, int i, int j)
        {
            String drawString = nValue.ToString();

            // Create font and brush.
            using (Font drawFont = new Font("Arial", 16))
            {
                using (SolidBrush drawBrush = new SolidBrush(Color.Black))
                {
                    // Create point for upper-left corner of drawing.
                    float x = OFFSET + i * DELTA + 10;
                    float y = OFFSET + j * DELTA + 10;

                    // Draw string to screen.
                    g.DrawString(drawString, drawFont, drawBrush, x, y);
                }
            }
        }

        private void DrawNumbers(Graphics g)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudoku.getValue(i, j) > 0)
                    {
                        DrawNumber(g, sudoku.getValue(i, j), i, j);
                    }
                }
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int c = (int) e.KeyChar;
            int nValue;

            if (c < 49 || c > 57)
            {
                nValue = 0;
            }
            else
            {
                nValue = (int) (c - 48);
            }

            if (this.sudoku.getValue(nFocusX, nFocusY) == 0
                && this.sudoku.IsValidForField(nValue, nFocusX, nFocusY))
            {
                this.sudoku.setValue(nValue, nFocusX, nFocusY);
                this.Invalidate();
            }
            else if (this.sudoku.getValue(nFocusX, nFocusY) != 0)
            {
                this.sudoku.setValue(0, nFocusX, nFocusY);
                this.Invalidate();
            }
        }

        protected void Save()
        {
            FileDialog dlg = new SaveFileDialog();

            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                string strFilename = dlg.FileName;

                this.Text = "Christoph Maurer's Sudoku - " + strFilename;

                FileInfo fi = new FileInfo(strFilename);

                StreamWriter o = new StreamWriter(strFilename);

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        o.WriteLine(this.sudoku.getValue(i, j));
                    }
                }
                o.Close();
            }
        }
        protected void Open()
        {
            FileDialog dlg = new OpenFileDialog();
            DialogResult result = dlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.Reset();

                string strFilename = dlg.FileName;

                this.Text = "Christoph Maurer's Sudoku - " + strFilename;

                FileInfo fi = new FileInfo(strFilename);

                StreamReader o = new StreamReader(strFilename);

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        string strLine = o.ReadLine();
                        int nValue = 0;

                        try
                        {
                            nValue = int.Parse(strLine);
                        }
                        catch
                        {
                        }

                        this.sudoku.setValue(nValue, i, j);
                    }
                }
                o.Close();

                this.Invalidate();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
            this.lstSolution.Items.Clear();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            using (Graphics g = e.Graphics)
            {
                DrawGrid(g);
                DrawNumbers(g);
            }
        }

        private string IntList2String(List<int> arValues)
        {
            StringBuilder sb = new StringBuilder();

            foreach (int x in arValues)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append(x.ToString());
            }

            return sb.ToString();
        }

        private void ShowPossibleValuesForField(int x, int y, bool bAutoSet)
        {
            if (0 <= x && x <= 8 && 0 <= y && y <= 8)
            {
                List<int> intList = this.sudoku.PossibleValues(x, y);

                if (this.chkPossible.Checked)
                {
                    this.txtPossible.Text = IntList2String(intList);
                }

                if (intList.Count == 1 && bAutoSet)
                {
                    this.sudoku.setValue(intList[0], x, y);
                    this.Invalidate();
                }
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            MoveRectangle(args);
        }

        private bool Step()
        {
            this.sudoku.NextTry();
            return true;
        }

        private void cmdStep_Click(object sender, EventArgs e)
        {
            if (Step())
            {
                this.Invalidate();
            }
        }

        private void DrawSudoku()
        {
            using (Graphics g = this.CreateGraphics())
            {
                DrawGrid(g);
                DrawNumbers(g);
            }
        }

            private void OnSolution(object sender, SolutionEventArgs args)
        {
            if (chkHistory.Checked)
            {

                if (args.bDeadEnd)
                {
                    Invalidate();
                }
                else
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        byte[] x = new byte[1];

                        x[0] = (byte)(97 + args.x);
                        string c = Encoding.ASCII.GetString(x);

                        DrawNumbers(g);

                        string s = args.step.ToString() + ": (" + c + "," + (args.y + 1).ToString() + ") =" + args.value.ToString();
                        this.lstSolution.Items.Insert(0, s);
                        if (lstSolution.Items.Count > 1000)
                        {
                            lstSolution.Items.Clear();
                        }
                        lstSolution.Refresh();
                    }
                }
            }
        }

        private void cmdRun_Click(object sender, EventArgs e)
        {
            this.cmdRun.Enabled = false;
            this.sudoku.Solve();
            DrawSudoku();
            this.cmdRun.Enabled = true;
        }

        private void HandleFocus(int x, int y)
        {
            if (x != nFocusX || y != nFocusY)
            {
                nFocusXOld = nFocusX;
                nFocusYOld = nFocusY;
                nFocusX = x;
                nFocusY = y;

                DrawFocus();
            }
        }

        private void MoveRectangle(MouseEventArgs e)
        {
            int x = (e.X - OFFSET) / DELTA;
            int y = (e.Y - OFFSET) / DELTA;

            if (0 <= x && x <= 8 && 0 <= y && y <= 8)
            {
                HandleFocus(x, y);

                if (nFocusX != nFocusXOld || nFocusY != nFocusYOld)
                {
                    ShowPossibleValuesForField(x, y, false);
                }
            }
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = (e.X - OFFSET) / DELTA;
            int y = (e.Y - OFFSET) / DELTA;

            if (0 <= x && x <= 8 && 0 <= y && y <= 8)
            {
                if (nFocusX != nFocusXOld || nFocusY != nFocusYOld)
                {
                    ShowPossibleValuesForField(x, y, false);
                }
            }
        }

        private void chkPossible_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkPossible.Checked)
            {
                this.txtPossible.Text = "";
            }
        }

        private void Reset()
        {
            sudoku = new Sudoku();
            sudoku.Solution += new SolutionEventHandler(OnSolution);
            lstSolution.Items.Clear();
        }
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reset();
            this.Invalidate();
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            int x = nFocusX;
            int y = nFocusY;
            bool bChanged = true;

            switch (e.KeyValue)
            {
                case 37:
                    // links
                    if (x > 0)
                    {
                        x--;
                    }
                    break;

                case 38:
                    // hoch
                    if (y > 0)
                    {
                        y--;
                    }
                    break;

                case 39:
                    // rechts
                    if (x < 8)
                    {
                        x++;
                    }
                    break;

                case 40:
                    // runter
                    if (y < 8)
                    {
                        y++;
                    }
                    break;

                default:
                    bChanged = false;
                    break;
            }

            if (bChanged)
            {
                HandleFocus(x, y);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}

