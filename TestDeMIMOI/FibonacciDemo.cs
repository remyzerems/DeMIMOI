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

namespace TestDeMIMOI
{
    public partial class FibonacciDemo : Form
    {
        Fibonacci fibo;

        public FibonacciDemo()
        {
            InitializeComponent();
        }

        private void FibonacciDemo_Load(object sender, EventArgs e)
        {
            // Instantiate the Fibonacci object
            fibo = new Fibonacci();
            // Update the form controls
            UpdateControls();
        }

        /// <summary>
        /// Updates the controls on the form according to the model outputs
        /// </summary>
        void UpdateControls()
        {
            label_n.Text = "Fn = " + fibo.Outputs[0][0].Value;
            label_n1.Text = "Fn-1 = " + fibo.Outputs[1][0].Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Update the model and latch outputs
            fibo.UpdateAndLatch();
            // Update form controls
            UpdateControls();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Modify the outputs of the model to set it to the initial values
            fibo.Outputs[0][0].Value = 1;
            fibo.Outputs[1][0].Value = 0;
            // Update form controls
            UpdateControls();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                fibo.UpdateAndLatch();
            }
            // Update form controls
            UpdateControls();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Bye bye !
            this.Close();
        }
    }
}
