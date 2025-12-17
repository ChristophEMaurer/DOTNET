using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;

using CMaurer.SerialLib;

public partial class SerialTestView : Form
{
    private QueuedSerialPort _serial = new QueuedSerialPort();
    private bool _started = false;

    public SerialTestView()
    {
        InitializeComponent();
    }

    private void RovinTestView_Load(object sender, EventArgs e)
    {
        PopulateControls();
        EnableControls(false);
    }

    private void PopulateControls()
    {
        cbParity.Items.Add(Parity.None.ToString());
        cbParity.Items.Add(Parity.Even.ToString());
        cbParity.Items.Add(Parity.Mark.ToString());
        cbParity.Items.Add(Parity.Odd.ToString());
        cbParity.Items.Add(Parity.Space.ToString());
        cbParity.SelectedIndex = 0;

        cbStopbits.Items.Add(StopBits.None.ToString());
        cbStopbits.Items.Add(StopBits.One.ToString());
        cbStopbits.Items.Add(StopBits.OnePointFive.ToString());
        cbStopbits.Items.Add(StopBits.Two.ToString());
        cbStopbits.SelectedIndex = 1;

        cbBaudrate.Items.Add("2400");
        cbBaudrate.Items.Add("4800");
        cbBaudrate.Items.Add("9600");
        cbBaudrate.Items.Add("19200");
        cbBaudrate.Items.Add("38400");
        cbBaudrate.Items.Add("57600");
        cbBaudrate.Items.Add("115200");
        cbBaudrate.Items.Add("230400");
        cbBaudrate.SelectedIndex = 6;
    }

    private void Control2Object()
    {
        int baudRate;
        int dataBits;
        int readTimeout;

        _serial.PortName = txtPortname.Text;
        if (int.TryParse(cbBaudrate.Text, out baudRate))
        {
            _serial.BaudRate = baudRate;
        }
        _serial.Parity = (Parity)Enum.Parse(typeof(Parity), cbParity.Text);
        if (int.TryParse(txtDatabits.Text, out dataBits))
        {
            _serial.DataBits = dataBits;
        }
        _serial.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cbStopbits.Text);
        if (int.TryParse(txtReadTimeout.Text, out readTimeout))
        {
            _serial.ReadTimeout = readTimeout;
        }
    }

    private void SerialStop()
    {
        if (_started)
        {
            try
            {
                _serial.Stop();
                _started = false;
                EnableControls(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    private void EnableControls(bool started)
    {
        cmdStart.Enabled = !started;
        cmdStop.Enabled = started;
        cmdSend.Enabled = started;
        cmdSendAndWait.Enabled = started;
    }

    private void SerialStart()
    {
        if (!_started)
        {
            try
            {
                Control2Object();
                _serial.Start();
                _started = true;
                EnableControls(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    private void cmdStop_Click(object sender, EventArgs e)
    {
        SerialStop();
    }

    private void cmdStart_Click(object sender, EventArgs e)
    {
        SerialStart();
    }

    private string GetRequest()
    {
        string text = txtCommand.Text;
        if (chkCrLf.Checked)
        {
            text = text + "\r\n";
        }

        return text;
    }

    private void cmdSendAndWait_Click(object sender, EventArgs e)
    {
        if (_started)
        {
            string response = _serial.SendCommandAndWait(GetRequest());
            if (response.Length > 0)
            {
                lbResponses.Items.Insert(0, response);
            }
        }
    }

    private void cmdSend_Click(object sender, EventArgs e)
    {
        if (_started)
        {
            _serial.SendCommand(GetRequest());
        }
    }

    private void cmdClose_Click(object sender, EventArgs e)
    {
        SerialStop();
        Close();
    }
}
