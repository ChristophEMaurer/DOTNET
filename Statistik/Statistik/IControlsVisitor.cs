using System;
using System.Windows.Forms;

namespace CMaurer.Common
{
    /// <summary>
    /// Base class for all visitors that must do something to each control of a form
    /// </summary>
    public interface IControlsVisitor
    {
        void VisitControl(Control item);
        void VisitMenuStrip(MenuStrip item);
        void VisitToolStripItem(ToolStripItem item);
        void VisitToolStripMenuItem(ToolStripMenuItem item);
        void VisitButton(System.Windows.Forms.Button item);
    }
}

