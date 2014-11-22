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
//      * Neither the name of the DeMIMOI library authors nor the
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

        // Trackbar DeMIMOI models to map the trackbars values to the DeMIMOI world
        DeMIMOI_Delegate DeMIMOI_trackBar1;
        DeMIMOI_Delegate DeMIMOI_trackBar_stage1, DeMIMOI_trackBar_stage2, DeMIMOI_trackBar_stage3, DeMIMOI_trackBar_stage4, DeMIMOI_trackBar_stage5;

        public CascadedLowPassFilterDemo()
        {
            InitializeComponent();
        }

        private void CascadedLowPassFilter_Load(object sender, EventArgs e)
        {
            // Instantiate the DeMIMOI collection to store and easily manage all the filters and the trackbars
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
                lpf[i].Inputs[0][0].ConnectTo(lpf[i - 1].Outputs[0][0]);
            }

            #region Create a DeMIMOI_Delegate for each trackBar control
            // Create the input (user handled) trackbar
            DeMIMOI_trackBar1 = new DeMIMOI_Delegate(null,                      // No inputs (the input is a in fact the trackbar value)
                                                     new DeMIMOI_Port(1),       // One input with one output delay (the trackbar value at instant t)
                                                     DeMIMOI_trackBar1_Update); // The delegate function to call to update the model output
            DeMIMOI_trackBar1.Name = "Trackbar1";

            // Create the output (low pass filter i-th order) trackbars
            DeMIMOI_trackBar_stage1 = new DeMIMOI_Delegate(new DeMIMOI_Port(1),             // One input (the trackbar value to apply)
                                                           null,                            // No output (the output is in fact the trackbar value)
                                                           DeMIMOI_trackBar_stage1_Update); // The delegate function to call to update the trackbar value using the model inputs
            DeMIMOI_trackBar_stage1.Name = "TrackBar Stage1";
            DeMIMOI_trackBar_stage2 = new DeMIMOI_Delegate(new DeMIMOI_Port(1), null, DeMIMOI_trackBar_stage2_Update);
            DeMIMOI_trackBar_stage2.Name = "TrackBar Stage2";
            DeMIMOI_trackBar_stage3 = new DeMIMOI_Delegate(new DeMIMOI_Port(1), null, DeMIMOI_trackBar_stage3_Update);
            DeMIMOI_trackBar_stage3.Name = "TrackBar Stage3";
            DeMIMOI_trackBar_stage4 = new DeMIMOI_Delegate(new DeMIMOI_Port(1), null, DeMIMOI_trackBar_stage4_Update);
            DeMIMOI_trackBar_stage4.Name = "TrackBar Stage4";
            DeMIMOI_trackBar_stage5 = new DeMIMOI_Delegate(new DeMIMOI_Port(1), null, DeMIMOI_trackBar_stage5_Update);
            DeMIMOI_trackBar_stage5.Name = "TrackBar Stage5";
            #endregion

            #region Connect each trackbar to each low pass filter input or output
            // Connect the input of the first LPF to the user handled trackbar
            DeMIMOI_trackBar1.Outputs[0][0].ConnectTo(lpf[0].Inputs[0][0]);

            // Connect each i-th order trackbar to the i-th order LPF output
            DeMIMOI_trackBar_stage1.Inputs[0][0].ConnectTo(lpf[0].Outputs[0][0]);
            DeMIMOI_trackBar_stage2.Inputs[0][0].ConnectTo(lpf[1].Outputs[0][0]);
            DeMIMOI_trackBar_stage3.Inputs[0][0].ConnectTo(lpf[2].Outputs[0][0]);
            DeMIMOI_trackBar_stage4.Inputs[0][0].ConnectTo(lpf[3].Outputs[0][0]);
            DeMIMOI_trackBar_stage5.Inputs[0][0].ConnectTo(lpf[4].Outputs[0][0]);
            #endregion

            // Add all the trackbars models to the collection
            lpf_cascade.Add(DeMIMOI_trackBar1);
            lpf_cascade.Add(DeMIMOI_trackBar_stage1);
            lpf_cascade.Add(DeMIMOI_trackBar_stage2);
            lpf_cascade.Add(DeMIMOI_trackBar_stage3);
            lpf_cascade.Add(DeMIMOI_trackBar_stage4);
            lpf_cascade.Add(DeMIMOI_trackBar_stage5);

            // Display the graphviz code result of this models combination
            textBox1.Text = lpf_cascade.GraphVizFullCode();

            // Enable the timer to run the filter animation
            timer1.Enabled = true;
        }

        #region Trackbar models update delegates (for DeMIMOI_Delegates)
        // This function is automatically called when the DeMIMOI_trackBar1 model needs to be updated
        private void DeMIMOI_trackBar1_Update(DeMIMOI sender, ref List<DeMIMOI_InputOutput> new_outputs)
        {
            new_outputs[0].Value = (double)trackBar1.Value;
        }
        private void DeMIMOI_trackBar_stage1_Update(DeMIMOI sender, ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // Assign to the trakbar control the value of the its model (the model that represents it in the DeMIMOI world)
            trackBar_stage1.Value = (int)((double)sender.Inputs[0][0].Value);
        }
        private void DeMIMOI_trackBar_stage2_Update(DeMIMOI sender, ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // Assign to the trakbar control the value of the its model (the model that represents it in the DeMIMOI world)
            trackBar_stage2.Value = (int)((double)sender.Inputs[0][0].Value);
        }
        private void DeMIMOI_trackBar_stage3_Update(DeMIMOI sender, ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // Assign to the trakbar control the value of the its model (the model that represents it in the DeMIMOI world)
            trackBar_stage3.Value = (int)((double)sender.Inputs[0][0].Value);
        }
        private void DeMIMOI_trackBar_stage4_Update(DeMIMOI sender, ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // Assign to the trakbar control the value of the its model (the model that represents it in the DeMIMOI world)
            trackBar_stage4.Value = (int)((double)sender.Inputs[0][0].Value);
        }
        private void DeMIMOI_trackBar_stage5_Update(DeMIMOI sender, ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // Assign to the trakbar control the value of the its model (the model that represents it in the DeMIMOI world)
            trackBar_stage5.Value = (int)((double)sender.Inputs[0][0].Value);
        }
        #endregion

        // At fixed intervals...
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update the filter and latch the ouputs using the topological order (ie. updating each model turn by turn following how they are connected to each other)
            // It also indirectly updates the trackbar values thanks to the DeMIMOI_Delegates!
            lpf_cascade.UpdateAndLatchTopologically();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
