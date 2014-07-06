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
using System.Linq;
using System.Text;

namespace DeMIMOI_Models
{
    /// <summary>
    /// Common Interface for DeMIMOI systems
    /// </summary>
    public interface IDeMIMOI_Interface
    {
        /// <summary>
        /// Updates the <see cref="IDeMIMOI_Interface"/>
        /// </summary>
        void Update();

        /// <summary>
        /// Publishes the <see cref="IDeMIMOI_Interface"/> outputs
        /// </summary>
        void LatchOutputs();

        /// <summary>
        /// Updates the <see cref="IDeMIMOI_Interface"/> and publishes the new calculated outputs directly
        /// </summary>
        void UpdateAndLatch();

        /// <summary>
        /// DeMIMOI Interface unique ID
        /// </summary>
        int ID
        {
            get;
            set;
        }

        /// <summary>
        /// DeMIMOI Interface name
        /// </summary>
        string Name
        {
            get;
            set;
        }


        /// <summary>
        /// Creates a full GraphViz script that represents a DeMIMOI Interface system
        /// </summary>
        /// <returns>The GraphViz code</returns>
        string GraphVizFullCode();

        /// <summary>
        /// Function that generates the partial GraphViz code corresponding to the DeMIMOI Interface structure
        /// <remarks>This only gives the GraphViz code for the DeMIMOI Interface object only, it's not the full code</remarks>
        /// </summary>
        /// <returns>The GraphViz code</returns>
        string GraphVizCode();
    }
}
