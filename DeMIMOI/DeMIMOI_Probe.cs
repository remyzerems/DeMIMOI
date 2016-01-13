// Delayed Multiple Input Multiple Output Interface (DeMIMOI) Library
//
// Copyright © Rémy Dispagne, 2014
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
    /// DeMIMOI_Probe class to describe a kind of a virtual probe to display values of outputs on a GraphViz diagram
    /// </summary>
    public class DeMIMOI_Probe:DeMIMOI
    {
        static int current_id = 0; // Current ID to allocate to a new DeMIMOI_RegressionLearner object
        /// <summary>
        /// Allocates a new DeMIMOI_RegressionLearner ID
        /// </summary>
        private static int AllocNewId()
        {
            return current_id++;
        }

        /// <summary>
        /// Creates a DeMIMOI_Probe
        /// </summary>
        public DeMIMOI_Probe():base(new DeMIMOI_Port(1), null)
        {
            ID = AllocNewId();

            Name = GetType().Name + "_" + ID;

            ProbeInput.Connected += new DeMIMOI_ConnectionEventHandler(ProbeInput_Connected);
        }

        void ProbeInput_Connected(object sender, DeMIMOI_ConnectionEventArgs e)
        {
            // The name is now the same than the output we want to probe
            ProbeInput.Name = e.From.Name;
        }

        /// <summary>
        /// Gets or sets the probe input
        /// </summary>
        public DeMIMOI_InputOutput ProbeInput
        {
            get
            {
                return Inputs[0][0];
            }
            set
            {
                Inputs[0][0] = value;
            }
        }

        protected override void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs)
        {
            // Update the probe only if its connected to something
            if (ProbeInput.ConnectedTo != null)
            {
                if (ProbeInput.Value != null)
                {
                    // If its value is a value type (double, int, etc)
                    if (ProbeInput.Value.GetType().IsValueType)
                    {
                        // Display its value into the diagram
                        Name = "= " + ProbeInput.Value;
                    }
                }
                else
                {
                    Name = "= null";
                }
            }
        }
    }
}
