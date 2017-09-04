using System.IO.Ports;
using System.Text;

namespace Boerman.Core.Extensions
{
    public static class SerialPortExtensions
    {
        #region PROPERTY EXTENSIONS
        public static SerialPort BaudRate(this SerialPort serialPort, int baudRate)
        {
            serialPort.BaudRate = baudRate;
            return serialPort;
        }

        public static SerialPort BreakState(this SerialPort serialPort, bool breakState)
        {
            serialPort.BreakState = breakState;
            return serialPort;
        }

        public static SerialPort DataBits(this SerialPort serialPort, int dataBits)
        {
            serialPort.DataBits = dataBits;
            return serialPort;
        }

        public static SerialPort DiscardNull(this SerialPort serialPort, bool discardNull)
        {
            serialPort.DiscardNull = discardNull;
            return serialPort;
        }

        public static SerialPort DtrEnable(this SerialPort serialPort, bool dtrEnable)
        {
            serialPort.DtrEnable = dtrEnable;
            return serialPort;
        }

        public static SerialPort Encoding(this SerialPort serialPort, Encoding encoding)
        {
            serialPort.Encoding = encoding;
            return serialPort;
        }

        public static SerialPort Handshake(this SerialPort serialPort, Handshake handshake)
        {
            serialPort.Handshake = handshake;
            return serialPort;
        }

        public static SerialPort NewLine(this SerialPort serialPort, string newLine)
        {
            serialPort.NewLine = newLine;
            return serialPort;
        }

        public static SerialPort Parity(this SerialPort serialPort, Parity parity)
        {
            serialPort.Parity = parity;
            return serialPort;
        }

        public static SerialPort ParityReplace(this SerialPort serialPort, byte parityReplace)
        {
            serialPort.ParityReplace = parityReplace;
            return serialPort;
        }

        public static SerialPort PortName(this SerialPort serialPort, string portName)
        {
            serialPort.PortName = portName;
            return serialPort;
        }

        public static SerialPort ReadBufferSize(this SerialPort serialPort, int readBufferSize)
        {
            serialPort.ReadBufferSize = readBufferSize;
            return serialPort;
        }

        public static SerialPort ReadTimeout(this SerialPort serialPort, int readTimeout)
        {
            serialPort.ReadTimeout = readTimeout;
            return serialPort;
        }

        public static SerialPort ReceivedBytesTreshold(this SerialPort serialPort, int receivedBytesTreshold)
        {
            serialPort.ReceivedBytesThreshold = receivedBytesTreshold;
            return serialPort;
        }

        public static SerialPort RtsEnable(this SerialPort serialPort, bool rtsEnable)
        {
            serialPort.RtsEnable = rtsEnable;
            return serialPort;
        }

        public static SerialPort StopBits(this SerialPort serialPort, StopBits stopBits)
        {
            serialPort.StopBits = stopBits;
            return serialPort;
        }

        public static SerialPort WriteBufferSize(this SerialPort serialPort, int writeBufferSize)
        {
            serialPort.WriteBufferSize = writeBufferSize;
            return serialPort;
        }

        public static SerialPort WriteTimeout(this SerialPort serialPort, int writeTimeout)
        {
            serialPort.WriteTimeout = writeTimeout;
            return serialPort;
        }
        #endregion

        //public static async Task<SerialPort> SetReadFunction(this SerialPort serialPort, Func<object, IAsyncResult> del)
        //{
        //    byte[] buffer = new byte[serialPort.ReadBufferSize];

        //    var result = await serialPort.BaseStream.ReadAsync(buffer, 0, buffer.Length);

        //    // The buffer should be filled right now.
        //    del.
        //}
    }
}
