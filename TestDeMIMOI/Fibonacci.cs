﻿// Delayed Multiple Input Multiple Output Interface (DeMIMOI) Sample Applications
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
using System.Linq;
using System.Text;
using DeMIMOI_Models;

namespace TestDeMIMOI
{
    /// <summary>
    /// Fibonacci sequence modeled using a DeMIMOI
    /// </summary>
    class Fibonacci:DeMIMOI
    {
        // Create the Fibonacci model with no input and output containing two delays (for step n and step n-1)
        public Fibonacci():base(null, new DeMIMOI_Port(2))
        {
            Name = "Fibonacci";

            // Set Fn = 1 (i.e. F1 = 1)
            Outputs[0][0].Value = 1;
            // Set Fn-1 = 0 (i.e. F0 = 0)
            Outputs[0][1].Value = 0;
        }

        // This function is called by the DeMIMOI model when asked to update the outputs
        protected override void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // We defined our Fibonacci sequence as Fn+1 = Fn + Fn-1
            new_outputs[0].Value = (object)((int)Outputs[0][0].Value + (int)Outputs[0][1].Value);
        }
    }
}
