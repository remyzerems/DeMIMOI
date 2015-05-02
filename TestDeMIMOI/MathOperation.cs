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

using System.Collections.Generic;
using DeMIMOI_Models;

namespace TestDeMIMOI
{
    // Delegate to manage a math operation
    delegate double MathAction(double num);

    /// <summary>
    /// Class that represents a math operation in the DeMIMOI world
    /// </summary>
    class MathOperation:DeMIMOI
    {
        /// <summary>
        /// The math operation delegate variable
        /// </summary>
        MathAction ma;

        /// <summary>
        /// Constructor of a math operation DeMIMOI model
        /// </summary>
        /// <param name="mathAction">The math operation to execute</param>
        public MathOperation(MathAction mathAction)
            : base(null, new DeMIMOI_Port(1))
        {
            ma = mathAction;

            // Set the name of the operation to the DeMIMOI model
            Name = ma.Method.Name;

            // Initialise the first value
            Outputs[0][0].Value = ma(0);
        }

        protected override void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // Generate the time value using the number of times the model has been updated
            double cur_t = ((double)UpdateCount) / 10;

            // Calculate the new output value using the math operation and the time value
            new_outputs[0].Value = ma(cur_t);
        }
    }
}
