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
using System.Windows.Forms;
using DeMIMOI_Models;
using DeMIMOI_Controls;

namespace TestDeMIMOI
{
    public partial class ChartDemo : Form
    {
        // Declaration of the models we'll going to use
        DeMIMOI_Collection collection;
        MathOperation sine;
        MathOperation cosine;
        DeMIMOI_Chart chart;

        public ChartDemo()
        {
            InitializeComponent();
        }

        private void ChartDemo_Load(object sender, EventArgs e)
        {
            // Initialize two math operations (sine and cosine)
            sine = new MathOperation(Math.Sin);
            cosine = new MathOperation(Math.Cos);

            // Initialize the DeMIMOI chart control
            chart = new DeMIMOI_Chart(new DeMIMOI_Port(1, 1), chart1);
            // Set the timestep unit using the timer interval so that the timestep is now seconds
            chart.TimestepUnit = (double)timer1.Interval / 1000.0;

            // Create the collection that will contain all the models
            collection = new DeMIMOI_Collection();
            // Add the models to the collection
            collection.Add(sine);
            collection.Add(cosine);
            collection.Add(chart);

            // Create the connections between the math operations and the chart
            chart.Inputs[0][0].ConnectTo(sine.Outputs[0][0]);
            chart.Inputs[1][0].ConnectTo(cosine.Outputs[0][0]);

            // Generate the graphviz code if we want to give a look of the system architecture
            textBox1.Text = collection.GraphVizFullCode();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Update the whole system using topological map
            collection.UpdateAndLatchTopologically();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Toggle the timer
            timer1.Enabled = !timer1.Enabled;

            // Change the button text for each case
            if (timer1.Enabled == true)
            {
                button1.Text = "Stop";
            }
            else
            {
                button1.Text = "Run";
            }
        }
    }
}
