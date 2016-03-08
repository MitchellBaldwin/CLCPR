using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using ZedGraph;

namespace CLCPR
{
    public enum States
    {
        Idle,
        Test,
        Run
    };
    
    public partial class CLCPRMain : Form
    {
        States state = States.Idle;
        public States State
        {
            get { return state; }
            set { state = value; }
        }

        #region Message type definitions
        // Defines
        public const Byte CommFramingByte = 0x00;           // Identifies the end of a serial message 
        public const Byte TextMsgMsgType = 0x00;            // The packet contains a text message (bi-directional)
        public const Byte SetModeMsgType = 0x01;            // The packet is a Set Mode command (Host to CLCPR device)
        
        public const Byte GetCPRFunctionStates = 0x10;      // Get the state flags from the CLCPR device
        public const Byte SetCPRFunctionStates = 0x11;      // Set the state flags on the CLCPR device - use to start/stop functions, e.g.

        public const Byte SensorDataMsgType = 0x18;         // Packet containing sensor measurements (CLCPR device to Host)

        public const Byte SetCPRPhaseTimesMsgType = 0x20;   // Set CPR function phase times
        
        #endregion // Message type definitions

        // Buffers for serial communication with the embedded device
        public const Byte PACKET_SIZE = 30;
        public const Byte ENCODED_PACKET_SIZE = PACKET_SIZE + 1;
        public const Byte COMM_BUFFER_SIZE = ENCODED_PACKET_SIZE + 1;

        Byte[] packetBuffer = new Byte[PACKET_SIZE];
        Byte[] encodedPacketBuffer = new Byte[ENCODED_PACKET_SIZE];
        Byte[] inBuffer = new Byte[COMM_BUFFER_SIZE];
        Byte[] outBuffer = new Byte[COMM_BUFFER_SIZE];
        Byte[] dummy = new Byte[1];

        #region Flags
        // Switch for displaying contents of commands sent to the PHM Main Controller
        Boolean showAllPHMCommandBufferUpdates = true;
        public Boolean ShowAllPHMCommandBufferUpdates
        {
            get { return showAllPHMCommandBufferUpdates; }
            set { showAllPHMCommandBufferUpdates = value; }
        }

        // Switch or displaying contents of the communications buffer incoming from the PHM Main Controller
        Boolean showInBufferUpdates = false;
        public Boolean ShowInBufferUpdates
        {
            get { return showInBufferUpdates; }
            set { showInBufferUpdates = value; }
        }

        // Switch or displaying contents of the communications buffer outgoing to the PHM Main Controller
        Boolean showOutBufferUpdates = false;
        public Boolean ShowOutBufferUpdates
        {
            get { return showOutBufferUpdates; }
            set { showOutBufferUpdates = value; }
        }

        // Flag indicating that a new message has been received from the PHM Main Controller
        Boolean clcprMessageReceived = false;
        public Boolean CLCPRMessageReceived
        {
            get { return clcprMessageReceived; }
            set { clcprMessageReceived = value; }
        }

        // Flag indicating a timeout error occured in the thread handling serial communications with the PHM Main Controller
        Boolean commTimeoutErrorFlag = false;
        public Boolean CommTimeoutErrorFlag
        {
            get { return commTimeoutErrorFlag; }
            set { commTimeoutErrorFlag = value; }
        }

        // Flag indicating a framing error occured in the thread handling serial communications with the PHM Main Controller
        Boolean commFramingErrorFlag = false;
        public Boolean CommFramingErrorFlag
        {
            get { return commFramingErrorFlag; }
            set { commFramingErrorFlag = value; }
        }

        // Flag indicating a framing error occured in the thread handling serial communications with the PHM Main Controller
        Boolean commChecksumErrorFlag = false;
        public Boolean CommChecksumErrorFlag
        {
            get { return commChecksumErrorFlag; }
            set { commChecksumErrorFlag = value; }
        }
        #endregion // Flags

        #region Form level functions
        public CLCPRMain()
        {
            InitializeComponent();
        }

        private void CLCPRMain_Load(object sender, EventArgs e)
        {
            CLCPRSerialPort.PortName = COMPortToolStripComboBox.Text;
            clcprPortDisplayLabel.Text = CLCPRSerialPort.PortName;
            clcprBaudRateDisplayLabel.Text = CLCPRSerialPort.BaudRate.ToString();
            clcprDataBitsDisplayLabel.Text = CLCPRSerialPort.DataBits.ToString();
            clcprParityDisplayLabel.Text = CLCPRSerialPort.Parity.ToString();
            clcprStopBitsDisplayLabel.Text = CLCPRSerialPort.StopBits.ToString();

            CLCPRSerialPort.ReceivedBytesThreshold = COMM_BUFFER_SIZE;
            CLCPRSerialPort.ReadTimeout = 500;
            CLCPRSerialPort.WriteTimeout = 500;


        }

        private void clcprDisplayUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (clcprMessageReceived && showAllPHMCommandBufferUpdates)
            {
                DisplayCommBuffers();
            }
        }

        private void CLCPRMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.comPortTexrApplicationSetting = COMPortToolStripComboBox.Text;
            Properties.Settings.Default.Save();
        }

        #endregion // Form level functions

        #region SerialComms
        private void clcprConnectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (clcprConnectCheckBox.Checked)
            {
                try
                {
                    CLCPRSerialPort.Open();
                    clcprDisplayUpdateTimer.Enabled = true;
                    testModeCheckBox.Enabled = true;
                    commErrorDisplayLabel.Text = "Connected";
                }
                catch (IOException ioe)
                {
                    Console.WriteLine(ioe.GetType().Name + ": " + ioe.Message);
                    clcprConnectCheckBox.Checked = false;
                }
            }
            else
            {
                if (CLCPRSerialPort.IsOpen)
                {
                    //// If the serial port is open and the data feed is active then stop the data feed before closing the port
                    //BuildCommMessage(StopDataFeed, dummy);
                    //SendCommandMessage();
                    //// Give the serial port and embedded system time to process the message to stop the data feed
                    //System.Threading.Thread.Sleep(1000);

                    CLCPRSerialPort.Close();
                    clcprDisplayUpdateTimer.Enabled = false;
                    testModeCheckBox.Enabled = false;
                    commErrorDisplayLabel.Text = "Not connected";
                }
            }
        }

        private void COMPortToolStripComboBox_TextChanged(object sender, EventArgs e)
        {
            Boolean wasOpen = false;

            if (clcprConnectCheckBox.Checked)
            {
                wasOpen = true;
                clcprConnectCheckBox.Checked = false;
            }
            CLCPRSerialPort.PortName = COMPortToolStripComboBox.Text;
            if (wasOpen)
            {
                clcprConnectCheckBox.Checked = true;
            }
        }

        private void CLCPRSerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            Byte messageType = 0xFF;

            if (CLCPRSerialPort.BytesToRead < COMM_BUFFER_SIZE)
            {
                Console.WriteLine("False call to DataReceived");
                return;
            }
            try
            {
                CLCPRSerialPort.Read(inBuffer, 0, COMM_BUFFER_SIZE);
                commTimeoutErrorFlag = false;                           // If no exception was thrown then we should have receiced a complete buffer (COMM_BUFFER_SIZE bits)
                //dataFeedActive = true;

                // Check that the first byte contains the CommStartByte
                if (inBuffer[COMM_BUFFER_SIZE - 1] != CommFramingByte)
                {
                    commFramingErrorFlag = true;
                    Console.WriteLine("Serial port framing error");
                    return;
                }
                else
                {
                    commFramingErrorFlag = false;
                }

                // A properly framed serial packet has arrived, so decode it
                for (int i=0; i<ENCODED_PACKET_SIZE; ++i)
                {
                    encodedPacketBuffer[i] = inBuffer[i];
                }
                packetBuffer = COBSCodec.decode(encodedPacketBuffer);
                
                // Check that the checksums match
                Byte checkSum = 0;
                for (int i = 0; i < PACKET_SIZE - 1; ++i)
                {
                    checkSum += packetBuffer[i];
                }
                if (checkSum == packetBuffer[PACKET_SIZE - 1])
                {
                    commChecksumErrorFlag = false;
                }
                else
                {
                    commChecksumErrorFlag = true;
                    Console.WriteLine("Serial port checksum error");
                    return;
                }

                // If no errors then set flag indicating that a valid message has been received
                clcprMessageReceived = true;
                messageType = packetBuffer[0x00];

                //if (messageType == SensorDataMsgType)
                //{
                //    curDataPoint = new PHTMDataPoint();
                //    curDataPoint.DataTime = new ZedGraph.XDate(DateTime.Now).XLDate;
                //    curDataPoint.PPG1 = inBuffer[0x02] + inBuffer[0x03] * 256;
                //    curDataPoint.CPRaw = (ushort)(inBuffer[0x04] + inBuffer[0x05] * 256);
                //    //curDataPoint.TargetCuffPressure = Int32.Parse(targetPressureDisplayLabel.Text);

                //    dataPointList.Add(curDataPoint);

                //    curDataPoint.CuffPID = inBuffer[0x06];
                //    curDataPoint.BPM = inBuffer[0x07] + inBuffer[0x08] * 256;
                //    curDataPoint.IBI = inBuffer[0x09] + inBuffer[0x0A] * 256;
                //}

            }
            catch (System.TimeoutException)
            {
                commTimeoutErrorFlag = true;
                Console.WriteLine("Serial port timeout");
                clcprMessageReceived = false;
            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe.GetType().Name + ": " + ioe.Message);
                clcprConnectCheckBox.Checked = false;
            }

        }

        // Helper function to format the outBuffer with a CLCPR Device message / command
        private void BuildCommMessage(Byte msgType, Byte[] buffer)
        {
            Byte checkSum = 0;

            for (int i = 0; i < PACKET_SIZE; ++i)
            {
                packetBuffer[i] = 0;
            }

            packetBuffer[0] = msgType;
            checkSum = msgType;
            for (int i = 0; i < buffer.Length - 1; ++i)
            {
                packetBuffer[1 + i] = buffer[i];
                checkSum += buffer[i];
            }
            packetBuffer[PACKET_SIZE - 1] = checkSum;
            
            encodedPacketBuffer = COBSCodec.encode(packetBuffer);
            for (int i=0; i<COMM_BUFFER_SIZE - 1; ++i)
            {
                outBuffer[i] = encodedPacketBuffer[i];
            }
            outBuffer[COMM_BUFFER_SIZE - 1] = 0x00;
        }

        // Helper function to send the contents of the outBuffer to the CLCPR Device
        private void SendCommandMessage()
        {
            try
            {
                CLCPRSerialPort.Write(outBuffer, 0, COMM_BUFFER_SIZE);
            }
            catch (InvalidOperationException ioe)
            {
                Console.WriteLine(ioe.GetType().Name + ": " + ioe.Message);
            }
            if (ShowOutBufferUpdates)
            {
                DisplayOutBufferToConsole();
            }
        }

        private void BuildAndSendCommandMessage(Byte msgType, Byte[] buffer)
        {
            BuildCommMessage(msgType, buffer);
            SendCommandMessage();
        }

        #endregion // SerialComms

        #region Display helper functions
        private void showAllBufferUpdatesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            showAllPHMCommandBufferUpdates = showAllBufferUpdatesCheckBox.Checked;
        }

        // Display contents of both serial communiction buffers
        private void DisplayCommBuffers()
        {
            String CommBufStr = "OUT: ";
            for (int i = 0; i < COMM_BUFFER_SIZE; ++i)
            {
                CommBufStr += outBuffer[i].ToString("X2");
                if (i < outBuffer.Length) CommBufStr += " ";
            }
            outBufferDisplayLabel.Text = CommBufStr;

            CommBufStr = "IN:  ";
            for (int i = 0; i < COMM_BUFFER_SIZE; ++i)
            {
                CommBufStr += inBuffer[i].ToString("X2");
                if (i < inBuffer.Length) CommBufStr += " ";
            }
            inBufferDisplayLabel.Text = CommBufStr;

            String PacketStr = "PKT: ";
            for (int i = 0; i < PACKET_SIZE; ++i)
            {
                PacketStr += packetBuffer[i].ToString("X2");
                if (i < packetBuffer.Length) PacketStr += " ";
            }
            packetDisplayLabel.Text = PacketStr;
            //Console.WriteLine(PacketStr);
        }

        // Display contents of the serial outBuffer
        private void DisplayOutBufferToConsole()
        {
            String CommBufStr = "OUT: ";
            for (int i = 0; i < COMM_BUFFER_SIZE; ++i)
            {
                CommBufStr += outBuffer[i].ToString("X2");
                if (i < outBuffer.Length) CommBufStr += " ";
            }
            //outBufferDisplayLabel.Text = CommBufStr;
            Console.WriteLine(CommBufStr);
        }

        // Display contents of the serial inBuffer
        private void DisplayInBufferToConsole()
        {
            String CommBufStr = "IN:  ";
            for (int i = 0; i < COMM_BUFFER_SIZE; ++i)
            {
                CommBufStr += inBuffer[i].ToString("X2");
                if (i < inBuffer.Length) CommBufStr += " ";
            }
            //inBufferDisplayLabel.Text = CommBufStr;
            Console.WriteLine(CommBufStr);
        }

        // Display contents of the serial inBuffer
        private void DisplayPacketToConsole()
        {
            String PacketStr = "PKT: ";
            for (int i = 0; i < PACKET_SIZE; ++i)
            {
                PacketStr += packetBuffer[i].ToString("X2");
                if (i < packetBuffer.Length) PacketStr += " ";
            }
            //packetDisplayLabel.Text = PacketStr;
            Console.WriteLine(PacketStr);
        }

        #endregion // Display helper functions

        private void testEncodingButton_Click(object sender, EventArgs e)
        {
            Byte[] buf = new Byte[3];
            buf[0] = 0x01;
            buf[1] = 0x55;
            buf[2] = 0xAA;
            BuildAndSendCommandMessage(GetCPRFunctionStates, buf);
        }

        private void sendCPRParametersButton_Click(object sender, EventArgs e)
        {
            Byte[] buf = new Byte[8];

            UInt16 compTime = 333;
            UInt16 decompTime = 333;
            UInt16 inspTime = 1000;
            UInt16 expTime = 4000;

            buf[0] = (Byte)(compTime % 256);
            buf[1] = (Byte)(compTime / 256);
            buf[2] = (Byte)(decompTime % 256);
            buf[3] = (Byte)(decompTime / 256);
            buf[4] = (Byte)(inspTime % 256);
            buf[5] = (Byte)(inspTime / 256);
            buf[6] = (Byte)(expTime % 256);
            buf[7] = (Byte)(expTime / 256);

            BuildAndSendCommandMessage(SetCPRPhaseTimesMsgType, buf);

            for (int i=0; i<8; ++i)
            {
                buf[i] = 0x00;
            }
            buf[0] = 0x0F;
            BuildAndSendCommandMessage(SetCPRFunctionStates, buf);
        }


    }
}
