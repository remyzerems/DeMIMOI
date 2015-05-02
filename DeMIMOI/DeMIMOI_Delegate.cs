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

using System;
using System.Collections.Generic;

namespace DeMIMOI_Models
{
    /// <summary>
    /// Delegate function definition for DeMIMOI_Delegate
    /// </summary>
    /// <param name="sender">DeMIMOI object that is to be updated</param>
    /// <param name="new_outputs">New outputs to be computed by this function that'll be the next DeMIMOI outputs</param>
    public delegate void DeMIMOI_UpdateDelegate(DeMIMOI sender, ref List<DeMIMOI_InputOutput> new_outputs);

    /// <summary>
    /// DeMIMOI_Delegate class that represents a DeMIMOI model you can use to deport the inner F function to a specific external element (ex: controls of a form such as trackbars...)
    /// </summary>
    [Serializable]
    public class DeMIMOI_Delegate:DeMIMOI
    {
        static int current_id = 0; // Current ID to allocate to a new DeMIMOINeuralNetwork object
        /// <summary>
        /// Allocates a new DeMIMOINeuralNetwork ID
        /// </summary>
        private static int AllocNewId()
        {
            return current_id++;
        }

        /// <summary>
        /// Gets or sets the delegate function that will be called when the model needs to be updated
        /// </summary>
        public DeMIMOI_UpdateDelegate UpdateDelegate
        {
            get;
            set;
        }

        /// <summary>
        /// Initialize the DeMIMOI_Delegate object
        /// </summary>
        /// <param name="input_port">Input port that describes the input of the model</param>
        /// <param name="output_port">Output port that describes the output of the model</param>
        /// <param name="update_delegate">Delegate function to call when the model has to be updated</param>
        private void Initialize(DeMIMOI_Port input_port, DeMIMOI_Port output_port, DeMIMOI_UpdateDelegate update_delegate)
        {
            ID = AllocNewId();
            Name = GetType().Name + "_" + ID;

            UpdateDelegate = update_delegate; 
        }

        /// <summary>
        /// Creates a DeMIMOI_Delegate object
        /// </summary>
        /// <param name="input_port">Input port that describes the input of the model</param>
        /// <param name="output_port">Output port that describes the output of the model</param>
        public DeMIMOI_Delegate(DeMIMOI_Port input_port, DeMIMOI_Port output_port)
            : base(input_port, output_port)
        {
            Initialize(input_port, output_port, null);
        }

        /// <summary>
        /// Creates the DeMIMOI_Delegate object
        /// </summary>
        /// <param name="input_port">Input port that describes the input of the model</param>
        /// <param name="output_port">Output port that describes the output of the model</param>
        /// <param name="update_delegate">Delegate function to call when the model has to be updated</param>
        public DeMIMOI_Delegate(DeMIMOI_Port input_port, DeMIMOI_Port output_port, DeMIMOI_UpdateDelegate update_delegate)
            :base(input_port, output_port)
        {
            Initialize(input_port, output_port, update_delegate);
        }

        // Function that is called by the DeMIMOI model to ask for new outputs
        protected override void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs)
        {
            if (UpdateDelegate != null)
            {
                // Simply call the delegate providing and provide some more information for output computing
                UpdateDelegate(this, ref new_outputs);
            }
        }
    }
}
