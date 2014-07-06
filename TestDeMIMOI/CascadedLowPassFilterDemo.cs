// Delayed Multiple Input Multiple Output Interface (DeMIMOI) Sample Applications
//
// Copyright © Rémy Dispagne, 2014
// All rights reserved. 3-BSD License:
//
//   Redistribution and use in source and binary forms, with or without
//   modification, are permitted provided that the following conditions are met:
//
//      * Redistributions of source code must retain the above copyright
//        notice, this list of conditions and the following disclaimer.
//
//      * Redistributions in binary form must reproduce the above copyright
//        notice, this list of conditions and the following disclaimer in the
//        documentation and/or other materials provided with the distribution.
//
//      * Neither the name of the Accord.NET Framework authors nor the
//        names of its contributors may be used to endorse or promote products
//        derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DeMIMOI_Models;

namespace TestDeMIMOI
{
    public partial class CascadedLowPassFilterDemo : Form
    {
        DeMIMOI_Collection lpf_cascade;
        LowPassFilter[] lpf;

        public CascadedLowPassFilterDemo()
        {
            InitializeComponent();
        }

        private void CascadedLowPassFilter_Load(object sender, EventArgs e)
        {
            // Instantiate the DeMIMOI collection to store and easily manage all the filters
            lpf_cascade = new DeMIMOI_Collection("5th order LPF");

            // Instantiate the low pass filter
            lpf = new LowPassFilter[5];

            for (int i = 0; i < lpf.Length; i++)
            {
                // Create each filter
                lpf[i] = new LowPassFilter();

                // Set the sampling frequency according to the timer interval
                lpf[i].SamplingFrequency = 1000 / timer1.Interval;
                // Set the cutoff frequency to 1Hz
                lpf[i].CutoffFrequency = 1;

                // Add the filter to the collection
                lpf_cascade.Add(lpf[i]);
            }

            // Chain the filters by simply connecting them
            for (int i = 1; i < lpf.Length; i++)
            {
                // Connect the input of the i-th filter to the i-1-th filter output
                lpf[i].Inputs[0][0].ConnectTo(ref lpf[i - 1].Outputs[0][0]);
            }

            textBox1.Text = lpf_cascade.GraphVizFullCode();

            // Enable the timer to run the filter animation
            timer1.Enabled = true;
        }

        /// <summary>
        /// Updates the filter
        /// </summary>
        void UpdateFilter()
        {
            // Set the input value of the model
            lpf[0].Inputs[0][0].Value = (double)trackBar1.Value;
            // Update the filter and latch the ouputs
            lpf_cascade.UpdateAndLatch();
            // Update the controls of the form
            UpdateControls();
        }

        /// <summary>
        /// Updates the controls of the form
        /// </summary>
        void UpdateControls()
        {
            // This has to be done in two steps because teh Value property is of object type
            // The compiler does not allow a direct cast from object to int whereas the data stored in Value is of double type

            /* Straightforward using a double cast */
            // Set the values of each filter stage to the trackbars
            trackBar_stage1.Value = (int)((double)lpf[0].Outputs[0][0].Value);

            trackBar_stage2.Value = (int)((double)lpf[1].Outputs[0][0].Value);

            trackBar_stage3.Value = (int)((double)lpf[2].Outputs[0][0].Value);

            trackBar_stage4.Value = (int)((double)lpf[3].Outputs[0][0].Value);
            
            trackBar_stage5.Value = (int)((double)lpf[4].Outputs[0][0].Value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update the filter in fixed intervals
            UpdateFilter();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
