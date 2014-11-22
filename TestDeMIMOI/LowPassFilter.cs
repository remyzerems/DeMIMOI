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
using System.Linq;
using System.Text;
using DeMIMOI_Models;

namespace TestDeMIMOI
{
    /// <summary>
    /// 1st order Low Pass Filter represented by a DeMIMOI
    /// </summary>
    class LowPassFilter:DeMIMOI
    {
        /// <summary>
        /// Filter constant to set the cutoff frequency according to the sampling period
        /// </summary>
        double alpha;

        /// <summary>
        /// Defines a LowPassFilter : 1 input, 1 input delay, 1 output, 1 output delay
        /// </summary>
        public LowPassFilter()
            : base(new DeMIMOI_Port(1), new DeMIMOI_Port(1))
        {
            Name = "Low Pass Filter";

            // Sets the default input/output
            Inputs[0][0].Value = 0.0;
            Outputs[0][0].Value = 0.0;
        }

        double iCutoffFrequency;
        /// <summary>
        /// Cutoff frequency of the filter
        /// </summary>
        public double CutoffFrequency
        {
            get
            {
                return iCutoffFrequency;
            }
            set
            {
                iCutoffFrequency = value;
                CalculateAlpha();
            }
        }

        double iSamplingFrequency;
        /// <summary>
        /// Sampling frequency of the filter
        /// </summary>
        public double SamplingFrequency
        {
            get
            {
                return iSamplingFrequency;
            }
            set
            {
                iSamplingFrequency = value;
            }
        }

        /// <summary>
        /// Function to update alpha value when CutoffFrequency or SamplingFrequency has been modified
        /// </summary>
        void CalculateAlpha()
        {
            double tmp_term = 2*Math.PI*CutoffFrequency/SamplingFrequency;
            double alpha_tmp = tmp_term / (1 + tmp_term);
            alpha = alpha_tmp;
        }

        // This function is called by the DeMIMOI model when asked to update the outputs
        protected override void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // Calculate the next step output for the filter using the discrete filter formula
            // See http://en.wikipedia.org/wiki/Low-pass_filter for more details
            // y[i + 1] := α * x[i] + (1-α) * y[i]
            new_outputs[0].Value = alpha*((double)Inputs[0][0].Value) + (1-alpha)*((double)Outputs[0][0].Value);
        }
    }
}
