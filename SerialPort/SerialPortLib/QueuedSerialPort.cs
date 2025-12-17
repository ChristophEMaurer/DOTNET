using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

/*
 * The <c>CMaurer.SerialLib</c> namespace contains the class QueuedSerialPort 
 * which provides queued access to a serial port.
*/
namespace CMaurer.SerialLib
{
    /// <summary>
    /// Represents a queue of commands, which are sent to the serial port by a worker thread.
    /// Every command must have a response, or a timeout will occur.
    /// <example>
    /// This is a simple example of a client sending one string to the serial port
    /// and waiting for a response:
    /// <br>
    /// <code>
    /// public void SendAndWaitForResponseSample()
    /// {
    ///     QueuedSerialPort serial = new QueuedSerialPort();
    /// 
    ///     serial.PortName = "COM1";
    ///     serial.BaudRate = 58000;
    ///     serial.Parity = Parity.None;
    ///     serial.DataBits = 8;
    ///     serial.StopBits = StopBits.One;
    ///     serial.ReadTimeout = 300;
    /// 
    ///     serial.Start();
    /// 
    ///     // Send a text and wait for the response.
    ///     string response = serial.SendCommandAndWait("version");
    /// 
    ///     // Tell the microprocessor to perform a reset. In this case, it will reset without 
    ///     // sending a response beforehand, so we don't wait.
    ///     response = serial.SendCommand("reset");
    ///     
    ///     serial.Stop();
    /// }
    /// </code>
    /// </br>
    /// </example> 
    /// </summary>
    public class QueuedSerialPort
    {
        /// <summary>
        /// Represents a string that is sent to a serial port.
        /// The sender of the command can choose to wait for or to ignore the result.
        /// In any case, after writing to the serial port, an answer is always expected.
        /// If no answer arrives, a TimeoutException is thrown.
        /// </summary>
        internal class CommandItem
        {
            /// <summary>
            /// The plain text that is sent to the serial port.
            /// </summary>
            public string _command;

            /// <summary>
            /// If the sender chooses to wait when sending a command, this will be the answer.
            /// </summary>
            public string _response;

            /// <summary>
            /// If the sender chooses to wait for the result of a command, the sender will wait until 
            /// this AutoResetEvent is signaled, which occurs when the result is read from the serial port.
            /// </summary>
            public AutoResetEvent _are;

            /// <summary>
            /// Creates an instance of a CommandItem with the specified plain text. 
            /// Initializes the response to the empty string.
            /// </summary>
            /// <param name="command">The plain text.</param>
            public CommandItem(string command)
            {
                _command = command;
                _response = "";
            }
        }

        private Queue<CommandItem> _commandQueue;
        private object _lockObject;
        private Thread _workerThread;
        private bool _started = false;
        private SerialPort _serialPort;
        private string _portName = "COM1";
        private int _baudRate = 58000;
        private Parity _parity = Parity.None;
        private int _dataBits = 8;
        private StopBits _stopBits = StopBits.One;
        private int _readTimeout = 300;

        /// <summary>
        /// Initializes a new instance. Creates an instance of a serial port and 
        /// an empty queue.
        /// </summary>
        public QueuedSerialPort()
        {
            _serialPort = new SerialPort();

            _lockObject = new object();
            _commandQueue = new Queue<CommandItem>();
        }

        /// <summary>
        /// Gets or sets the port for communications, including but not limited to all available COM ports.
        /// </summary>
        public string PortName
        {
            set { _portName = value; }
            get { return _portName; }
        }

        /// <summary>
        /// Gets or sets the serial baud rate.
        /// </summary>
        public int BaudRate
        {
            set { _baudRate = value; }
            get { return _baudRate; }
        }

        /// <summary>
        /// Gets or sets the parity-checking protocol.
        /// </summary>
        public System.IO.Ports.Parity Parity
        {
            set { _parity = value; }
            get { return _parity; }
        }

        /// <summary>
        /// Gets or sets the standard length of data bits per byte.
        /// </summary>
        public int DataBits
        {
            set { _dataBits = value; }
            get { return _dataBits; }
        }
        /// <summary>
        /// Gets or sets the standard number of stopbits per byte.
        /// </summary>
        public System.IO.Ports.StopBits StopBits
        {
            set { _stopBits = value; }
            get { return _stopBits; }
        }

        /// <summary>
        /// Gets or sets the number of milliseconds before a time-out occurs when a read operation does not finish.
        /// </summary>
        public int ReadTimeout
        {
            set { _readTimeout = value; }
            get { return _readTimeout; }
        }

        /// <summary>
        /// When the serial port is active, it polls the queue for commands.
        /// If there is a command, it sends the command and immediately waits for a response.
        /// If the command is waited on by the sender, its AutoResetEvent is signaled, which causes the 
        /// waiting sender of the command to continue execution.
        /// Because this is a thread, we cannot throw an exception, so if a timeout occurs,
        /// an empty string is returned.
        /// </summary>
        private void WorkerThread()
        {
            Debug.WriteLine("Queued serial port worker thread started.");

            while (_started)
            {
                CommandItem commandItem = DequeueCommand();

                if (commandItem != null)
                {
                    string response = "";

                    try
                    {
                        _serialPort.WriteLine(commandItem._command);
                        response = _serialPort.ReadLine();
                        commandItem._response = response;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        // Even in case of a timeout, the sender of a command must be signaled or else
                        // he will be waiting indefinitely.
                        if (commandItem._are != null)
                        {
                            commandItem._are.Set();
                        }
                    }
                }
            }

            Debug.WriteLine("Queued serial port worker thread terminated.");
        }

        /// <summary>
        /// Overloaded. Opens the serial port with the current settings for BaudRate etc. and
        /// starts the thread that handles the commands.
        /// If the port is already started, an <see cref="InvalidOperationExeption"/> is thrown.
        /// </summary>
        public void Start()
        {
            Start(_portName, _baudRate);
        }
        
        /// <summary>
        /// Overloaded. Opens the serial port using the specified port name and baud rate, and the current values 
        /// for the other settings and 
        /// starts the thread that handles the commands.
        /// </summary>
        /// <param name="portName">The port to use (for example, COM1). </param>
        /// <param name="baudRate">The baud rate.</param>
        /// <exception cref="System.InvalidOperationExeption">Thrown if the port has already been started.</exception>
        public void Start(string portName, int baudRate)
        {
            if (_started)
            {
                throw new InvalidOperationException("The serial port has already been started.");
            }
            else
            {
                _serialPort.PortName = portName;
                _serialPort.BaudRate = baudRate;
                _serialPort.Parity = _parity;
                _serialPort.DataBits = _dataBits;
                _serialPort.StopBits = _stopBits;
                _serialPort.ReadTimeout = _readTimeout;
                // \n = 10
                _serialPort.NewLine = "\n";
                _serialPort.Open();

                _workerThread = new Thread(new ThreadStart(WorkerThread));

                // _started has to be set to true before the worker thread starts
                // because the worker thread runs only as long as _started is true.
                _started = true;
                _workerThread.Start();
            }
        }

        /// <summary>
        /// Closes the serial port and shuts down the worker thread.
        /// If the serial port is already closed, an <see cref="InvalidOperationExeption"/> Exception is thrown.
        /// </summary>
        public void Stop()
        {
            if (_started)
            {
                // the worker thread continues until _started = false, so 
                // it has to be set to false before calling _workerThread.Join();
                _started = false;
                if (_workerThread != null)
                {
                    _workerThread.Join();
                    _workerThread = null;
                }

                if (_serialPort != null)
                {
                    _serialPort.Close();
                }
            }
            else
            {
                throw new InvalidOperationException("The serial port has not been started.");
            }
        }

        /// <summary>
        /// Sends a plain text to the serial port without waiting for a result. 
        /// There has to be a response or else a
        /// <see cref="TimeoutException"/> will be thrown.
        /// </summary>
        /// <param name="command">The plain text.</param>
        public void SendCommand(string command)
        {
            CommandItem commandItem = new CommandItem(command);

            EnqueueCommand(commandItem);
        }

        /// <summary>
        /// Sends a plain text to the serial port and waits for the response. There has to be a response or else a
        /// <see cref="TimeoutException"/> will be thrown.
        /// </summary>
        /// <param name="command">The plain text.</param>
        /// <returns>The reponse.</returns>
        public string SendCommandAndWait(string command)
        {
            CommandItem commandItem = new CommandItem(command);

            commandItem._are = new AutoResetEvent(false);
            EnqueueCommand(commandItem);
            commandItem._are.WaitOne();

            return commandItem._response;
        }

        /// <summary>
        /// Enqueues the command.
        /// Access to the queue containing the commands is handled by using
        /// the lock statement.
        /// </summary>
        /// <param name="commandItem">The plain text to send to the serial port. If the sender 
        /// wants to wait, the _are member has to be set to an unsignaled instance of an <see cref="AutoResetEvent"/></param>
        private void EnqueueCommand(CommandItem commandItem)
        {
            lock(_lockObject)
            {
                _commandQueue.Enqueue(commandItem);
            }
        }

        /// <summary>
        /// Dequeues a command if there is one in the command queue.
        /// Access to the queue containing the commands is handled by using
        /// the lock statement.
        /// </summary>
        /// <returns>The first <see cref="CommandItem"/> on the queue.</returns>
        private CommandItem DequeueCommand()
        {
            CommandItem commandItem = null;

            lock(_lockObject)
            {
                if (_commandQueue.Count > 0)
                {
                    commandItem = _commandQueue.Dequeue();
                }
            }

            return commandItem;
        }
    }
}

