partial class SerialTestView
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.grpSerialPort = new System.Windows.Forms.GroupBox();
        this.cbBaudrate = new System.Windows.Forms.ComboBox();
        this.lblReadTimeoutUnit = new System.Windows.Forms.Label();
        this.txtReadTimeout = new System.Windows.Forms.TextBox();
        this.lblReadTimeout = new System.Windows.Forms.Label();
        this.cbStopbits = new System.Windows.Forms.ComboBox();
        this.cbParity = new System.Windows.Forms.ComboBox();
        this.txtDatabits = new System.Windows.Forms.TextBox();
        this.txtPortname = new System.Windows.Forms.TextBox();
        this.lblStopbits = new System.Windows.Forms.Label();
        this.lblDatabits = new System.Windows.Forms.Label();
        this.lblParity = new System.Windows.Forms.Label();
        this.lblBaudrate = new System.Windows.Forms.Label();
        this.lblPortName = new System.Windows.Forms.Label();
        this.grpCommand = new System.Windows.Forms.GroupBox();
        this.lbResponses = new System.Windows.Forms.ListBox();
        this.lblResponse = new System.Windows.Forms.Label();
        this.txtCommand = new System.Windows.Forms.TextBox();
        this.lblCommand = new System.Windows.Forms.Label();
        this.cmdSend = new System.Windows.Forms.Button();
        this.cmdSendAndWait = new System.Windows.Forms.Button();
        this.cmdStart = new System.Windows.Forms.Button();
        this.cmdStop = new System.Windows.Forms.Button();
        this.cmdClose = new System.Windows.Forms.Button();
        this.chkCrLf = new System.Windows.Forms.CheckBox();
        this.grpSerialPort.SuspendLayout();
        this.grpCommand.SuspendLayout();
        this.SuspendLayout();
        // 
        // grpSerialPort
        // 
        this.grpSerialPort.Controls.Add(this.cbBaudrate);
        this.grpSerialPort.Controls.Add(this.lblReadTimeoutUnit);
        this.grpSerialPort.Controls.Add(this.txtReadTimeout);
        this.grpSerialPort.Controls.Add(this.lblReadTimeout);
        this.grpSerialPort.Controls.Add(this.cbStopbits);
        this.grpSerialPort.Controls.Add(this.cbParity);
        this.grpSerialPort.Controls.Add(this.txtDatabits);
        this.grpSerialPort.Controls.Add(this.txtPortname);
        this.grpSerialPort.Controls.Add(this.lblStopbits);
        this.grpSerialPort.Controls.Add(this.lblDatabits);
        this.grpSerialPort.Controls.Add(this.lblParity);
        this.grpSerialPort.Controls.Add(this.lblBaudrate);
        this.grpSerialPort.Controls.Add(this.lblPortName);
        this.grpSerialPort.Location = new System.Drawing.Point(16, 12);
        this.grpSerialPort.Name = "grpSerialPort";
        this.grpSerialPort.Size = new System.Drawing.Size(364, 199);
        this.grpSerialPort.TabIndex = 0;
        this.grpSerialPort.TabStop = false;
        this.grpSerialPort.Text = "Serial port";
        // 
        // cbBaudrate
        // 
        this.cbBaudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cbBaudrate.FormattingEnabled = true;
        this.cbBaudrate.Location = new System.Drawing.Point(96, 42);
        this.cbBaudrate.Name = "cbBaudrate";
        this.cbBaudrate.Size = new System.Drawing.Size(137, 21);
        this.cbBaudrate.TabIndex = 14;
        // 
        // lblReadTimeoutUnit
        // 
        this.lblReadTimeoutUnit.AutoSize = true;
        this.lblReadTimeoutUnit.Location = new System.Drawing.Point(242, 157);
        this.lblReadTimeoutUnit.Name = "lblReadTimeoutUnit";
        this.lblReadTimeoutUnit.Size = new System.Drawing.Size(26, 13);
        this.lblReadTimeoutUnit.TabIndex = 13;
        this.lblReadTimeoutUnit.Text = "[ms]";
        // 
        // txtReadTimeout
        // 
        this.txtReadTimeout.Location = new System.Drawing.Point(96, 154);
        this.txtReadTimeout.Name = "txtReadTimeout";
        this.txtReadTimeout.Size = new System.Drawing.Size(137, 20);
        this.txtReadTimeout.TabIndex = 12;
        this.txtReadTimeout.Text = "300";
        // 
        // lblReadTimeout
        // 
        this.lblReadTimeout.AutoSize = true;
        this.lblReadTimeout.Location = new System.Drawing.Point(6, 154);
        this.lblReadTimeout.Name = "lblReadTimeout";
        this.lblReadTimeout.Size = new System.Drawing.Size(73, 13);
        this.lblReadTimeout.TabIndex = 11;
        this.lblReadTimeout.Text = "Read timeout:";
        // 
        // cbStopbits
        // 
        this.cbStopbits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cbStopbits.FormattingEnabled = true;
        this.cbStopbits.Location = new System.Drawing.Point(96, 120);
        this.cbStopbits.Name = "cbStopbits";
        this.cbStopbits.Size = new System.Drawing.Size(137, 21);
        this.cbStopbits.TabIndex = 10;
        // 
        // cbParity
        // 
        this.cbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cbParity.FormattingEnabled = true;
        this.cbParity.Location = new System.Drawing.Point(96, 68);
        this.cbParity.Name = "cbParity";
        this.cbParity.Size = new System.Drawing.Size(137, 21);
        this.cbParity.TabIndex = 9;
        // 
        // txtDatabits
        // 
        this.txtDatabits.Location = new System.Drawing.Point(96, 94);
        this.txtDatabits.Name = "txtDatabits";
        this.txtDatabits.Size = new System.Drawing.Size(137, 20);
        this.txtDatabits.TabIndex = 8;
        this.txtDatabits.Text = "8";
        // 
        // txtPortname
        // 
        this.txtPortname.Location = new System.Drawing.Point(96, 16);
        this.txtPortname.Name = "txtPortname";
        this.txtPortname.Size = new System.Drawing.Size(137, 20);
        this.txtPortname.TabIndex = 5;
        this.txtPortname.Text = "COM1";
        // 
        // lblStopbits
        // 
        this.lblStopbits.AutoSize = true;
        this.lblStopbits.Location = new System.Drawing.Point(6, 120);
        this.lblStopbits.Name = "lblStopbits";
        this.lblStopbits.Size = new System.Drawing.Size(51, 13);
        this.lblStopbits.TabIndex = 4;
        this.lblStopbits.Text = "Stop bits:";
        // 
        // lblDatabits
        // 
        this.lblDatabits.AutoSize = true;
        this.lblDatabits.Location = new System.Drawing.Point(6, 94);
        this.lblDatabits.Name = "lblDatabits";
        this.lblDatabits.Size = new System.Drawing.Size(52, 13);
        this.lblDatabits.TabIndex = 3;
        this.lblDatabits.Text = "Data bits:";
        // 
        // lblParity
        // 
        this.lblParity.AutoSize = true;
        this.lblParity.Location = new System.Drawing.Point(6, 68);
        this.lblParity.Name = "lblParity";
        this.lblParity.Size = new System.Drawing.Size(36, 13);
        this.lblParity.TabIndex = 2;
        this.lblParity.Text = "Parity:";
        // 
        // lblBaudrate
        // 
        this.lblBaudrate.AutoSize = true;
        this.lblBaudrate.Location = new System.Drawing.Point(6, 42);
        this.lblBaudrate.Name = "lblBaudrate";
        this.lblBaudrate.Size = new System.Drawing.Size(53, 13);
        this.lblBaudrate.TabIndex = 1;
        this.lblBaudrate.Text = "Baudrate:";
        // 
        // lblPortName
        // 
        this.lblPortName.AutoSize = true;
        this.lblPortName.Location = new System.Drawing.Point(6, 16);
        this.lblPortName.Name = "lblPortName";
        this.lblPortName.Size = new System.Drawing.Size(58, 13);
        this.lblPortName.TabIndex = 0;
        this.lblPortName.Text = "Port name:";
        // 
        // grpCommand
        // 
        this.grpCommand.Controls.Add(this.lbResponses);
        this.grpCommand.Controls.Add(this.lblResponse);
        this.grpCommand.Controls.Add(this.txtCommand);
        this.grpCommand.Controls.Add(this.lblCommand);
        this.grpCommand.Location = new System.Drawing.Point(16, 236);
        this.grpCommand.Name = "grpCommand";
        this.grpCommand.Size = new System.Drawing.Size(362, 165);
        this.grpCommand.TabIndex = 1;
        this.grpCommand.TabStop = false;
        this.grpCommand.Text = "Command";
        // 
        // lbResponses
        // 
        this.lbResponses.FormattingEnabled = true;
        this.lbResponses.Location = new System.Drawing.Point(93, 55);
        this.lbResponses.Name = "lbResponses";
        this.lbResponses.Size = new System.Drawing.Size(263, 82);
        this.lbResponses.TabIndex = 11;
        // 
        // lblResponse
        // 
        this.lblResponse.AutoSize = true;
        this.lblResponse.Location = new System.Drawing.Point(4, 55);
        this.lblResponse.Name = "lblResponse";
        this.lblResponse.Size = new System.Drawing.Size(58, 13);
        this.lblResponse.TabIndex = 10;
        this.lblResponse.Text = "Response:";
        // 
        // txtCommand
        // 
        this.txtCommand.Location = new System.Drawing.Point(94, 16);
        this.txtCommand.Name = "txtCommand";
        this.txtCommand.Size = new System.Drawing.Size(262, 20);
        this.txtCommand.TabIndex = 9;
        // 
        // lblCommand
        // 
        this.lblCommand.AutoSize = true;
        this.lblCommand.Location = new System.Drawing.Point(4, 16);
        this.lblCommand.Name = "lblCommand";
        this.lblCommand.Size = new System.Drawing.Size(57, 13);
        this.lblCommand.TabIndex = 5;
        this.lblCommand.Text = "Command:";
        // 
        // cmdSend
        // 
        this.cmdSend.Location = new System.Drawing.Point(110, 444);
        this.cmdSend.Name = "cmdSend";
        this.cmdSend.Size = new System.Drawing.Size(86, 31);
        this.cmdSend.TabIndex = 13;
        this.cmdSend.Text = "Send";
        this.cmdSend.UseVisualStyleBackColor = true;
        this.cmdSend.Click += new System.EventHandler(this.cmdSend_Click);
        // 
        // cmdSendAndWait
        // 
        this.cmdSendAndWait.Location = new System.Drawing.Point(110, 407);
        this.cmdSendAndWait.Name = "cmdSendAndWait";
        this.cmdSendAndWait.Size = new System.Drawing.Size(86, 31);
        this.cmdSendAndWait.TabIndex = 12;
        this.cmdSendAndWait.Text = "SendAndWait";
        this.cmdSendAndWait.UseVisualStyleBackColor = true;
        this.cmdSendAndWait.Click += new System.EventHandler(this.cmdSendAndWait_Click);
        // 
        // cmdStart
        // 
        this.cmdStart.Location = new System.Drawing.Point(16, 407);
        this.cmdStart.Name = "cmdStart";
        this.cmdStart.Size = new System.Drawing.Size(86, 31);
        this.cmdStart.TabIndex = 13;
        this.cmdStart.Text = "Start";
        this.cmdStart.UseVisualStyleBackColor = true;
        this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
        // 
        // cmdStop
        // 
        this.cmdStop.Location = new System.Drawing.Point(16, 444);
        this.cmdStop.Name = "cmdStop";
        this.cmdStop.Size = new System.Drawing.Size(86, 31);
        this.cmdStop.TabIndex = 14;
        this.cmdStop.Text = "Stop";
        this.cmdStop.UseVisualStyleBackColor = true;
        this.cmdStop.Click += new System.EventHandler(this.cmdStop_Click);
        // 
        // cmdClose
        // 
        this.cmdClose.Location = new System.Drawing.Point(286, 455);
        this.cmdClose.Name = "cmdClose";
        this.cmdClose.Size = new System.Drawing.Size(86, 31);
        this.cmdClose.TabIndex = 15;
        this.cmdClose.Text = "Exit";
        this.cmdClose.UseVisualStyleBackColor = true;
        this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
        // 
        // chkCrLf
        // 
        this.chkCrLf.AutoSize = true;
        this.chkCrLf.Location = new System.Drawing.Point(234, 416);
        this.chkCrLf.Name = "chkCrLf";
        this.chkCrLf.Size = new System.Drawing.Size(78, 17);
        this.chkCrLf.TabIndex = 16;
        this.chkCrLf.Text = "Add CR LF";
        this.chkCrLf.UseVisualStyleBackColor = true;
        // 
        // SerialTestView
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(404, 533);
        this.Controls.Add(this.chkCrLf);
        this.Controls.Add(this.cmdSend);
        this.Controls.Add(this.cmdClose);
        this.Controls.Add(this.cmdSendAndWait);
        this.Controls.Add(this.cmdStop);
        this.Controls.Add(this.grpCommand);
        this.Controls.Add(this.grpSerialPort);
        this.Controls.Add(this.cmdStart);
        this.Name = "SerialTestView";
        this.Text = "Christoph Maurer\'s Serial Port Interface";
        this.Load += new System.EventHandler(this.RovinTestView_Load);
        this.grpSerialPort.ResumeLayout(false);
        this.grpSerialPort.PerformLayout();
        this.grpCommand.ResumeLayout(false);
        this.grpCommand.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.GroupBox grpSerialPort;
    private System.Windows.Forms.Label lblParity;
    private System.Windows.Forms.Label lblBaudrate;
    private System.Windows.Forms.Label lblPortName;
    private System.Windows.Forms.Label lblDatabits;
    private System.Windows.Forms.ComboBox cbStopbits;
    private System.Windows.Forms.ComboBox cbParity;
    private System.Windows.Forms.TextBox txtDatabits;
    private System.Windows.Forms.TextBox txtPortname;
    private System.Windows.Forms.Label lblStopbits;
    private System.Windows.Forms.GroupBox grpCommand;
    private System.Windows.Forms.TextBox txtCommand;
    private System.Windows.Forms.Label lblCommand;
    private System.Windows.Forms.ListBox lbResponses;
    private System.Windows.Forms.Label lblResponse;
    private System.Windows.Forms.Button cmdSend;
    private System.Windows.Forms.Button cmdSendAndWait;
    private System.Windows.Forms.Button cmdStart;
    private System.Windows.Forms.Button cmdStop;
    private System.Windows.Forms.Button cmdClose;
    private System.Windows.Forms.Label lblReadTimeoutUnit;
    private System.Windows.Forms.TextBox txtReadTimeout;
    private System.Windows.Forms.Label lblReadTimeout;
    private System.Windows.Forms.ComboBox cbBaudrate;
    private System.Windows.Forms.CheckBox chkCrLf;
}

