// Delayed Multiple Input Multiple Output Interface (DeMIMOI) Library
//
// Copyright © Rémy Dispagne, 2016
// cramer at libertysurf.fr
//
//  The DeMIMOI library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.

//  The DeMIMOI library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.

//  You should have received a copy of the GNU General Public License
//  along with the DeMIMOI library.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;

namespace DeMIMOI_Models
{
    /// <summary>
    /// DeMIMOI_Constant class represents a constant element (could be a constant number value, reference image whatsoever constant)
    /// </summary>
    public class DeMIMOI_Constant:DeMIMOI
    {
        static int current_id = 0; // Current ID to allocate to a new DeMIMOI_Constant object
        /// <summary>
        /// Allocates a new DeMIMOI_Constant ID
        /// </summary>
        private static int AllocNewId()
        {
            return current_id++;
        }


        void Initialize(DeMIMOI_InputOutput inputToSetTo, object value)
        {
            ID = AllocNewId();

            Name = GetType().Name + "_" + ID;

            ConstOutput.Value = value;
            ConstOutput.Connected += new DeMIMOI_ConnectionEventHandler(ConstOutput_Connected);

            if (inputToSetTo != null)
            {
                ConstOutput.ConnectTo(inputToSetTo);
            }

            UpdateName();
        }

        /// <summary>
        /// Creates a DeMIMOI_Constant
        /// </summary>
        /// <param name="value">Value of the constant</param>
        public DeMIMOI_Constant(object value)
            : base(null, new DeMIMOI_Port(1))
        {
            Initialize(null, value);
        }

        /// <summary>
        /// Creates a DeMIMOI_Constant and connect it to the desired input to set constant
        /// </summary>
        /// <param name="inputToSetTo">Input to set to the constant</param>
        /// <param name="value">Value of the constant to set</param>
        public DeMIMOI_Constant(DeMIMOI_InputOutput inputToSetTo, object value)
            : base(null, new DeMIMOI_Port(1))
        {
            Initialize(inputToSetTo, value);
        }

        void ConstOutput_Connected(object sender, DeMIMOI_ConnectionEventArgs e)
        {
            // The name is now the same than the input we want to set to the constant
            ConstOutput.Name = e.To.Name;
        }

        /// <summary>
        /// Gets or sets the constant output value
        /// </summary>
        public DeMIMOI_InputOutput ConstOutput
        {
            get
            {
                return Outputs[0][0];
            }
            set
            {
                Outputs[0][0] = value;
            }
        }

        void UpdateName()
        {
            if (ConstOutput.Value != null)
            {
                // If its value is a value type (double, int, etc)
                if (ConstOutput.Value.GetType().IsValueType)
                {
                    // Display its value into the diagram
                    Name = ConstOutput.Value + " =\\>";
                }
            }
            else
            {
                Name = "Null =\\>";
            }
        }

        protected override void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs)
        {
            UpdateName();
            new_outputs[0].Value = ConstOutput.Value;
        }
    }
}
