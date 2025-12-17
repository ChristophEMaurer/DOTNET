using System;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace CMaurer.Common
{
    /// <summary>
    /// This visitor XAbles any button that might be clicked to execute some action to disable or enable all user actions.
    /// </summary>
    public class XAbleAllCommandsVisitor : IControlsVisitor
    {
        public delegate void XableControlDelegate(Control control);
        public delegate void XableButtonDelegate(Button control);

        private BaseForm _view;
        private Button _abortButton1;
        private Button _abortButton2;
        private Button _abortButton3;
        private bool _enable;

        private XAbleAllCommandsVisitor()
        {
        }

        public XAbleAllCommandsVisitor(BaseForm view, bool enable) :
            this(view, null, null, enable)
        {
        }

        public XAbleAllCommandsVisitor(BaseForm view, Button abortButton1, bool enable) :
            this(view, abortButton1, null, enable)
        {
        }

        /// <summary>
        /// Disable all command-controls the specified abort buttons during a long operation.
        /// If all controls are disabled, the abort buttons are enabled.
        /// If all controls are enabled, the abort buttons are disabled.
        /// </summary>
        /// <param name="view">The form that contains the UI</param>
        /// <param name="abortButton1">The abort button, can be null</param>
        /// <param name="abortButton2">Another abort button, can be null</param>
        /// <param name="enable">Enable or disable all commands</param>
        public XAbleAllCommandsVisitor(BaseForm view, Button abortButton1, Button abortButton2, bool enable)
        {
            _view = view;
            _abortButton1 = abortButton1;
            _abortButton2 = abortButton2;
            _enable = enable;

            _view.MayFormClose = _enable;

            if (_abortButton1 != null)
            {
                _abortButton1.Enabled = !_enable;
            }
            if (_abortButton2 != null)
            {
                _abortButton2.Enabled = !_enable;
            }
            view.Accept(this, view);
        }

        public XAbleAllCommandsVisitor(BaseForm view, Button abortButton1, Button abortButton2, Button abortButton3, bool enable)
        {
            _view = view;
            _abortButton1 = abortButton1;
            _abortButton2 = abortButton2;
            _abortButton3 = abortButton3;
            _enable = enable;

            _view.MayFormClose = _enable;

            if (_abortButton1 != null)
            {
                _abortButton1.Enabled = !_enable;
            }
            if (_abortButton2 != null)
            {
                _abortButton2.Enabled = !_enable;
            }
            if (_abortButton3 != null)
            {
                _abortButton3.Enabled = !_enable;
            }

            view.Accept(this, view);
        }


        public void VisitButton(Button control)
        {
            if ((control != _abortButton1) && (control != _abortButton2) && (control != _abortButton3))
            {
                if (control.InvokeRequired)
                {
                    XableButtonDelegate d = new XableButtonDelegate(VisitButton);
                    _view.Invoke(d, control);
                }
                else
                {
                    control.Enabled = _enable;
                }
            }
        }

        public void VisitControl(Control control)
        {
        }

        public void VisitMenuStrip(MenuStrip item)
        {
        }

        public void VisitToolStripItem(ToolStripItem item)
        {
        }

        public void VisitToolStripMenuItem(ToolStripMenuItem item)
        {
        }
    }
}

