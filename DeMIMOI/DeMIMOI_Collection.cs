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
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace DeMIMOI_Models
{
    /// <summary>
    /// DeMIMOI Collection class
    /// </summary>
    [Serializable]
    public class DeMIMOI_Collection : List<DeMIMOI>, IDeMIMOI_Interface
    {
        static int current_id = 0; // Current ID to allocate to a new DeMIMOI_Collection object
        /// <summary>
        /// Allocates a new DeMIMOI_Collection ID
        /// </summary>
        private static int AllocNewId()
        {
            return current_id++;
        }

        /// <summary>
        /// Initializes the collection
        /// </summary>
        public void Initialize()
        {
            ID = AllocNewId();
            Name = "DeMIMOI_Collection_" + ID;
        }

        /// <summary>
        /// Creates a new collection
        /// </summary>
        public DeMIMOI_Collection()
            : base()
        {
            Initialize();
        }

        /// <summary>
        /// Creates a new collection by specifying its name
        /// </summary>
        /// <param name="name">Name of the collection</param>
        public DeMIMOI_Collection(string name)
            : base()
        {
            Initialize();

            Name = name;
        }

        /// <summary>
        /// Updates all the <see cref="DeMIMOI"/> objects of the collection
        /// </summary>
        public void Update()
        {
            if (EnableMultithreading == false)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    this[i].Update();
                }
            }
            else
            {
                Parallel.ForEach(this, demimoi_model =>
                {
                    demimoi_model.Update();
                });
            }
        }

        /// <summary>
        /// Publishes all the <see cref="DeMIMOI"/> object outputs of the collection
        /// </summary>
        public void LatchOutputs()
        {
            if (EnableMultithreading == false)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    this[i].LatchOutputs();
                }
            }
            else
            {
                Parallel.ForEach(this, demimoi_model =>
                {
                    demimoi_model.LatchOutputs();
                });
            }
        }

        /// <summary>
        /// Updates all the <see cref="DeMIMOI"/> objects and publishes the new calculated outputs directly
        /// </summary>
        public void UpdateAndLatch()
        {
            Update();
            LatchOutputs();
        }

        /// <summary>
        /// DeMIMOI Collection unique ID
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// DeMIMOI Collection name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies if the multithreading update of the contained models is enabled
        /// </summary>
        public bool EnableMultithreading
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a full GraphViz script that represents the collection of DeMIMOI system
        /// </summary>
        /// <returns>The GraphViz code</returns>
        public string GraphVizFullCode()
        {
            string code = "digraph g_" + Name.Replace(" ", "_") + " {\n\trankdir=LR;\n";
            code += GraphVizCode();
            code += "}";

            return code;
        }

        /// <summary>
        /// Function that generates the partial GraphViz code corresponding to the DeMIMOI structure
        /// <remarks>This only gives the GraphViz code for the DeMIMOI object only, it's not the full code</remarks>
        /// </summary>
        /// <param name="is_collection_a_group">Specifies if the collection in itself must be represented as a container or not in the GraphViz representation</param>
        /// <returns>The GraphViz code</returns>
        public string GraphVizCode(bool is_collection_a_group)
        {
            string code = "";
            if (is_collection_a_group)
            {
                code = "subgraph cluster_DeMIMOI_Collection_" + ID + " {\n";
                code += GraphVizCode();
                code += "}";
            }
            else
            {
                code = GraphVizCode();
            }

            return code;
        }

        /// <summary>
        /// Function that generates the partial GraphViz code corresponding to the DeMIMOI structure
        /// <remarks>This only gives the GraphViz code for the DeMIMOI object only, it's not the full code</remarks>
        /// </summary>
        /// <returns>The GraphViz code</returns>
        public string GraphVizCode()
        {
            string code = "";
            for (int i = 0; i < this.Count; i++)
            {
                code += this[i].GraphVizCode();
            }

            return code;
        }
    }
}
