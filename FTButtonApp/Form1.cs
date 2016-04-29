using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using FT32HDIO;

namespace FTButtonApp
{
    /// <summary>
    /// Sample app that demonstrates the use of the FTButton and
    /// controls LEDs
    /// </summary>
    public partial class Form1 : Form
    {
        ft232hdio _ftdio; // the dio controller
        FTButton _ftbutton; // the ftbutton switch
        delegate void setCallback(); // used to update the gui if InvokeRequired


        public Form1()
        {
            InitializeComponent();


            // Open the ft232h controller
            _ftdio = new ft232hdio();
            int dev_index = _ftdio.GetFirstDevIndex();
            if (dev_index < 0)
            {
                throw new Exception("Unable to find an F232H device");
            }
            _ftdio.Open((uint)dev_index);


            // Create the ftbutton
            _ftbutton = new FTButton(_ftdio, ft232hdio.DIO_BUS.AD_BUS, ft232hdio.PIN.PIN0);
            // Subscribe to click event
            _ftbutton.Click_Event += _ftbutton_Click_Event;

        }

        /// <summary>
        /// Init gui state on form load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Init gui
            updategui();
        }

        /// <summary>
        /// Handles the button click event
        /// </summary>
        /// <param name="sender"></param>
        void _ftbutton_Click_Event(object sender)
        {
            // The button should now be disabled
            // Report something is running...
            updategui();

            // Just pretend something is running for some time
            Thread.Sleep(5000);

            // Now enable the button so a new click can be detected
            _ftbutton.Enabled = true;

            // Report we are idle
            updategui();
        }

        /// <summary>
        /// Update the gui depending on the enablement state of the ftbutton
        /// We asume somthing is running when it is enable and idle when disable
        /// </summary>
        void updategui()
        {
            if (InvokeRequired)
            {
                setCallback d = new setCallback(updategui);
                this.Invoke(d, new object[] { });
            }
            else
            {
                if (_ftbutton.Enabled)
                {
                    label1.Text = "Waiting on button click.\r\nButton is enabled.\r\nLED is on";

                    // Toggle leds as if something passed or failed
                    Random ran = new Random();
                    bool pass_fail = Convert.ToBoolean(ran.Next(2));
                    turn_leds_pass_fail(pass_fail);
                    radioButton1.Checked = true;
                }
                else
                {
                    label1.Text = "Something running.\r\nButton is disabled\r\nLED is off";

                    turn_leds(false);
                    radioButton1.Checked = false;
                }
            }
        }

        /// <summary>
        /// Turn either the green or red LEDs
        /// </summary>
        /// <param name="pass_fail"></param>
        void turn_leds_pass_fail(bool pass_fail)
        {
            if (pass_fail)
            {
                _ftdio.SetPin(ft232hdio.DIO_BUS.AC_BUS, ft232hdio.PIN.PIN0, true);
                _ftdio.SetPin(ft232hdio.DIO_BUS.AC_BUS, ft232hdio.PIN.PIN2, true);
            }
            else
            {
                _ftdio.SetPin(ft232hdio.DIO_BUS.AC_BUS, ft232hdio.PIN.PIN1, true);
                _ftdio.SetPin(ft232hdio.DIO_BUS.AC_BUS, ft232hdio.PIN.PIN3, true);
            }
        }

        /// <summary>
        /// Turn all LEDs
        /// </summary>
        /// <param name="state"></param>
        void turn_leds(bool state)
        {
            _ftdio.SetPin(ft232hdio.DIO_BUS.AC_BUS, ft232hdio.PIN.PIN0, state);
            _ftdio.SetPin(ft232hdio.DIO_BUS.AC_BUS, ft232hdio.PIN.PIN1, state);
            _ftdio.SetPin(ft232hdio.DIO_BUS.AC_BUS, ft232hdio.PIN.PIN2, state);
            _ftdio.SetPin(ft232hdio.DIO_BUS.AC_BUS, ft232hdio.PIN.PIN3, state);
        }

    }
}
