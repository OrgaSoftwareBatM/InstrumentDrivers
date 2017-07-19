using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AIOUSBNet;  // the namespace exposes the AIOUSB Class interface 

// You must also add a reference to the AIOUSBnet.dll in the project settings


namespace Sample1
{
    public partial class Form1 : Form
    {
        // One and only Device Index:
        public UInt32 DeviceIndex;

        public enum refID:uint
        {
		    // product IDs supported by this sample program
		      USB_AO16_16A_RevA	    = 0x8060
		    , USB_AO16_16A			= 0x8070
		    , USB_AO16_16			= 0x8071
		    , USB_AO16_12A			= 0x8072
		    , USB_AO16_12			= 0x8073
		    , USB_AO16_8A			= 0x8074
		    , USB_AO16_8			= 0x8075
		    , USB_AO16_4A			= 0x8076
		    , USB_AO16_4			= 0x8077
		    , USB_AO12_16A			= 0x8078
		    , USB_AO12_16			= 0x8079
		    , USB_AO12_12A			= 0x807a
		    , USB_AO12_12			= 0x807b
		    , USB_AO12_8A			= 0x807c
		    , USB_AO12_8			= 0x807d
		    , USB_AO12_4A			= 0x807e
		    , USB_AO12_4			= 0x807f
	    };	// enum

        // Board configuration:
        //public UInt32 numAnalogOutputs = 16;
        public UInt32 productID;
        public UInt32 numDACBits;
        public UInt32 numDACs;
        public UInt32 numADCs;

        // For DAC data:
        //ushort[] Data = new ushort[] { 0, 0, 0, 0, 0, 0, 0, 0 };  
        
        // Interval Timer fou GUI update:
        // (Note: A Windows Forms timer designed for single threaded environments not a System Timer 55 ms min res)
        static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

        // Array of controls for GUI updates:
        public TrackBar[] DACOutTrack= new TrackBar[16];
        public Button[] DACOutButton = new Button[16];
        public Label[] DACOutLabel = new Label[16];
                
        public Form1()
        {
            InitializeComponent();

            // Arrays of the controls:
            DACOutTrack[0] = trackBar0;
            DACOutTrack[1] = trackBar1;
            DACOutTrack[2] = trackBar2;
            DACOutTrack[3] = trackBar3;
            DACOutTrack[4] = trackBar4;
            DACOutTrack[5] = trackBar5;
            DACOutTrack[6] = trackBar6;
            DACOutTrack[7] = trackBar7;
            DACOutTrack[8] = trackBar8;
            DACOutTrack[9] = trackBar9;
            DACOutTrack[10] = trackBar10;
            DACOutTrack[11] = trackBar11;
            DACOutTrack[12] = trackBar12;
            DACOutTrack[13] = trackBar13;
            DACOutTrack[14] = trackBar14;
            DACOutTrack[15] = trackBar15;
            
            DACOutButton[0] = btnCount0;
            DACOutButton[1] = btnCount1;
            DACOutButton[2] = btnCount2;
            DACOutButton[3] = btnCount3;
            DACOutButton[4] = btnCount4;
            DACOutButton[5] = btnCount5;
            DACOutButton[6] = btnCount6;
            DACOutButton[7] = btnCount7;
            DACOutButton[8] = btnCount8;
            DACOutButton[9] = btnCount9;
            DACOutButton[10] = btnCount10;
            DACOutButton[11] = btnCount11;
            DACOutButton[12] = btnCount12;
            DACOutButton[13] = btnCount13;
            DACOutButton[14] = btnCount14;
            DACOutButton[15] = btnCount15;

            DACOutLabel[0] = lblCount0;
            DACOutLabel[1] = lblCount1;
            DACOutLabel[2] = lblCount2;
            DACOutLabel[3] = lblCount3;
            DACOutLabel[4] = lblCount4;
            DACOutLabel[5] = lblCount5;
            DACOutLabel[6] = lblCount6;
            DACOutLabel[7] = lblCount7;
            DACOutLabel[8] = lblCount8;
            DACOutLabel[9] = lblCount9;
            DACOutLabel[10] = lblCount10;
            DACOutLabel[11] = lblCount11;
            DACOutLabel[12] = lblCount12;
            DACOutLabel[13] = lblCount13;
            DACOutLabel[14] = lblCount14;
            DACOutLabel[15] = lblCount15;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // Called before Form is displayed Initialize resources:
            
            // Initialize default Device Only:
            DeviceIndex = AIOUSB.diOnly; 
           
            // Device data:
            UInt32 Status;
            UInt32 PID = 0;
            UInt32 NameSize = 256;
            string strName = "name";
            UInt32 DIOBytes = 0;
            UInt32 Counters = 0;
            UInt64 SerNum;

            UInt32 ERROR_SUCCESS = 0;
            bool deviceIndexValid = false;

            // Get The Device Information test for valid device found:
            Status = AIOUSB.QueryDeviceInfo(DeviceIndex, out PID, out NameSize, out strName, out DIOBytes, out Counters);
            if (Status == ERROR_SUCCESS && PID >= 0x8060 && PID <= 0x807F) //All AO cards
            {
                deviceIndexValid = true;
            }
            else
            {
                // If Only device is not valid then Launch connect device dialog box:
                // New parent aware subform:
                FormDetect DetectSubForm = new FormDetect(this);
                DetectSubForm.ShowDialog();

                Status = AIOUSB.QueryDeviceInfo(DeviceIndex, out PID, out NameSize, out strName, out DIOBytes, out Counters);
                if (Status == ERROR_SUCCESS && PID >= 0x8060 && PID <= 0x807F)
                    deviceIndexValid = true;
            }

            if (!deviceIndexValid)
            {
                // No valid device found should exit
                // this.Close();
            }

            // Check device status:
            Status = AIOUSB.ClearDevices();  // Cleans up any orphaned indexes
            Status = AIOUSB.GetDevices();
            Status = AIOUSB.GetDeviceSerialNumber(DeviceIndex, out SerNum);

            
            productID = PID; // public copy	
	        
            // Device properties depend on the product ID
	        switch( productID ) 
            {
		        // case CSample0App::USB_AO16_16A:
		        default:
			        numDACBits = 16;
			        numDACs = 16;
			        numADCs = 2;
			        break;

		        case (uint)refID.USB_AO16_16:
			        numDACBits = 16;
			        numDACs = 16;
			        numADCs = 0;
			        break;

		        case (uint)refID.USB_AO16_12A:
			        numDACBits = 16;
			        numDACs = 12;
			        numADCs = 2;
			        break;

		        case (uint)refID.USB_AO16_12:
			        numDACBits = 16;
			        numDACs = 12;
			        numADCs = 0;
			        break;

		        case (uint)refID.USB_AO16_8A:
			        numDACBits = 16;
			        numDACs = 8;
			        numADCs = 2;
			        break;

		        case (uint)refID.USB_AO16_8:
			        numDACBits = 16;
			        numDACs = 8;
			        numADCs = 0;
			        break;

		        case (uint)refID.USB_AO16_4A:
			        numDACBits = 16;
			        numDACs = 4;
			        numADCs = 2;
			        break;

		        case (uint)refID.USB_AO16_4:
			        numDACBits = 16;
			        numDACs = 4;
			        numADCs = 0;
			        break;

		        case (uint)refID.USB_AO12_16A:
			        numDACBits = 12;
			        numDACs = 16;
			        numADCs = 2;
			        break;

		        case (uint)refID.USB_AO12_16:
			        numDACBits = 12;
			        numDACs = 16;
			        numADCs = 0;
			        break;

		        case (uint)refID.USB_AO12_12A:
			        numDACBits = 12;
			        numDACs = 12;
			        numADCs = 2;
			        break;

		        case (uint)refID.USB_AO12_12:
			        numDACBits = 12;
			        numDACs = 12;
			        numADCs = 0;
			        break;

		        case (uint)refID.USB_AO12_8A:
			        numDACBits = 12;
			        numDACs = 8;
			        numADCs = 2;
			        break;

		        case (uint)refID.USB_AO12_8:
			        numDACBits = 12;
			        numDACs = 8;
			        numADCs = 0;
			        break;

		        case (uint)refID.USB_AO12_4A:
			        numDACBits = 12;
			        numDACs = 4;
			        numADCs = 2;
			        break;

                case (uint)refID.USB_AO12_4:
			        numDACBits = 12;
			        numDACs = 4;
			        numADCs = 0;
			        break;
	        }	// switch( productID ) 

            // we use the same D/A count range for both the 12-bit and 16-bit D/As;
            // the reason we can do so is that the 12-bit D/As automatically truncate
            // the LS 4 bits from the count value; so the count range for the 16-bit
            // D/As is 0-0xffff, while that for the 12-bit D/As is 0-0xfff0
            
            const UInt16 INITIAL_COUNTS = 0;
            bool dacError = false;
            for (int channel = 0; channel < numDACs; channel++)
            {
                DACOutTrack[channel].Enabled = (channel < numDACs);
                if (channel < numDACs)
                    if (AIOUSB.DACDirect(DeviceIndex, (ushort)channel, INITIAL_COUNTS) != ERROR_SUCCESS)
                        dacError = true;
            }
            if (dacError)
                MessageBox.Show("Error initializing DACs");

            // Add the event and the event handler for the method that will 
            // process the timer event to the timer:
            myTimer.Tick += new EventHandler(TimerEventProcessor);

            // Set the timer interval in miliseconds and start 55ms min resolution:
            myTimer.Interval = 100;
            myTimer.Start();
        }
        
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            // This is the method that runs when the timer event is raised:
            myTimer.Stop();

            UpdateGUIState();

            myTimer.Enabled = true;
        }
        
        private void UpdateGUIState()
        {
            // Read data and set GUI state:
            // Nothing to do for this sample!
        }

        private void btnCount_Click(object sender, EventArgs e)
        {
            // Need to stop the timer change board and GUI:
            myTimer.Stop();
            
            //Get button sender Tag (index)
            UInt32 iTagindex = 0;
            Button btnSender = sender as Button;
            if (btnSender != null)
            {
                iTagindex = Convert.ToUInt32(btnSender.Tag);
            }

            UInt32 Status; 
            ushort CounterValue;

            CounterValue = (ushort)(DACOutTrack[iTagindex].Value);

            if (iTagindex < numDACs)
                Status = AIOUSB.DACDirect(DeviceIndex, (ushort)iTagindex, CounterValue);
            else
            {
                MessageBox.Show("DAC out of range for this board!!!");
            }
                               
            // Restart timer thread:
            myTimer.Enabled = true;
        }

        private String FormatCount(int Count)
        {
            string formatString;
            double volt;
            volt = Count / 6553.5;
            //formatString =  volt.ToString();
            formatString = String.Format("{0:0.00}", volt);
            //formatString = String.Format("{0,4:X}", Count);
            return (formatString);
        }
        
        private void trackBarCount_Scroll(object sender, EventArgs e)
        {
            // Need to stop the timer change board and GUI:
            myTimer.Stop();

            //Get button sender Tag (index)
            UInt32 iTagindex = 0;
            TrackBar tbarSender = sender as TrackBar;
            if (tbarSender != null)
            {
                iTagindex = Convert.ToUInt32(tbarSender.Tag);
            }
            
            int CounterValue;
            String TempString;
            
            CounterValue = DACOutTrack[iTagindex].Value;
            TempString = FormatCount(CounterValue);
            DACOutLabel[iTagindex].Text = "DAC Counts: " + TempString;
           
            // Restart timer thread:
            myTimer.Enabled = true;
        }

        private void btnDevice_Click_1(object sender, EventArgs e)
        {
            // Launch connect device dialog box
            // Switch between multiple devices or reconnect

            // New parent aware subform:
            FormDetect DetectSubForm = new FormDetect(this);
            DetectSubForm.ShowDialog();
        }

        private void btnUpdateAll_Click(object sender, EventArgs e)
        {
            ushort count;
            UInt32 Status;

            // For DAC data:
            ushort[] Data = new ushort[numDACs * 2]; 

            for (count = 0; count < numDACs; count++)
            {
                Data[count * 2] = count;
                Data[count * 2 + 1] = (ushort)(DACOutTrack[count].Value); 
            }

            Status = AIOUSB.DACMultiDirect(DeviceIndex, Data, numDACs);

            if (Status != 0)
                MessageBox.Show("Error Updating all DACs!!!");
        }

        private void comboBoxRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            byte RangeCode;

            //The ranges in the combo box are listed in order, so that the index is equal
            //to the range code.

            RangeCode = (byte)comboBoxRange.SelectedIndex;

            if (RangeCode >= 0 && RangeCode <= 3)
            {
                if (AIOUSB.DACSetBoardRange(DeviceIndex, RangeCode) != 0) //ERROR_SUCCESS
                    MessageBox.Show("Error setting DAC range");
            }	
        }

        private void lblCount0_Click(object sender, EventArgs e)
        {

        }

    }
}
