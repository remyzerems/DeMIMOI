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
using TopologicalSorting;
using System.Collections;

namespace DeMIMOI_Models
{
    /// <summary>
    /// DeMIMOI Collection class
    /// </summary>
    [Serializable]
    public class DeMIMOI_Collection : IList<IDeMIMOI_Interface>, IDeMIMOI_Interface
    {
        static int current_id = 0; // Current ID to allocate to a new DeMIMOI_Collection object
        /// <summary>
        /// Allocates a new DeMIMOI_Collection ID
        /// </summary>
        private static int AllocNewId()
        {
            return current_id++;
        }

        private IList<IDeMIMOI_Interface> _collection = new List<IDeMIMOI_Interface>();

        // Ask for topological map computation
        [NonSerialized]
        private IEnumerable<IEnumerable<OrderedProcess>> topological_order;
        private bool refresh_topological_order = true;

        /// <summary>
        /// Initializes the collection
        /// </summary>
        public void Initialize()
        {
            ID = AllocNewId();
            Name = "DeMIMOI_Collection_" + ID;
        }

        #region Implementation of IEnumerable

        public IEnumerator<IDeMIMOI_Interface> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Adds a <see cref="DeMIMOI"/> object to the collection
        /// </summary>
        /// <param name="item"><see cref="DeMIMOI"/> object to add</param>
        public void Add(IDeMIMOI_Interface item)
        {
            _collection.Add(item);
            if (item != null)
            {
                if (item is DeMIMOI)
                {
                    ((DeMIMOI)item).Connected += new DeMIMOI_ConnectionEventHandler(item_Connected);
                    ((DeMIMOI)item).Disconnected += new DeMIMOI_ConnectionEventHandler(item_Disconnected);
                }
                refresh_topological_order = true;
            }
        }

        public void Clear()
        {
            foreach (IDeMIMOI_Interface item in _collection)
            {
                _collection.Remove(item);
            }
            //_collection.Clear();
        }

        public bool Contains(IDeMIMOI_Interface item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(IDeMIMOI_Interface[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return _collection.IsReadOnly; }
        }

        public void AddRange(IEnumerable<IDeMIMOI_Interface> collection)
        {
            if (collection != null)
            {
                foreach (IDeMIMOI_Interface item in collection)
                {
                    _collection.Add(item);
                }
            }
        }

        public void Insert(int index, IDeMIMOI_Interface item)
        {
            _collection.Insert(index, item);
            if (item != null)
            {
                if (item is DeMIMOI)
                {
                    ((DeMIMOI)item).Connected += new DeMIMOI_ConnectionEventHandler(item_Connected);
                    ((DeMIMOI)item).Disconnected += new DeMIMOI_ConnectionEventHandler(item_Disconnected);
                }
                refresh_topological_order = true;
            }
        }

        public bool Remove(IDeMIMOI_Interface item)
        {
            bool ret = _collection.Remove(item);
            if (item != null)
            {
                if (item is DeMIMOI)
                {
                    ((DeMIMOI)item).Connected -= item_Connected;
                    ((DeMIMOI)item).Disconnected -= item_Disconnected;
                }
                refresh_topological_order = true;
            }

            return ret;
        }

        public void RemoveAt(int index)
        {
            if (index >= 0 && index < _collection.Count)
            {
                IDeMIMOI_Interface item = _collection[index];
                Remove(item);
            }
        }

        public int IndexOf(IDeMIMOI_Interface item)
        {
            return _collection.IndexOf(item);
        }

        public IDeMIMOI_Interface this[int index]
        {
            get { return _collection[index]; }
            set { _collection[index] = value; }
        }



        void item_Disconnected(object sender, DeMIMOI_ConnectionEventArgs e)
        {
            refresh_topological_order = true;
        }

        void item_Connected(object sender, DeMIMOI_ConnectionEventArgs e)
        {
            refresh_topological_order = true;
        }

        /// <summary>
        /// Creates a new collection
        /// </summary>
        public DeMIMOI_Collection()
            : base()
        {
            Name = GetType().Name + "_" + ID;

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
        /// Updates the <see cref="DeMIMOI"/> objects of the collection following its topological order
        /// </summary>
        public void UpdateAndLatchTopologically()
        {
            // If the models topology changed from last time
            if (refresh_topological_order == true || topological_order == null)
            {
                // Ask for topological map computation
                topological_order = ComputeTopologicalMap();

                refresh_topological_order = false;
            }

            if (EnableMultithreading == false)
            {
                // Updates each model by its topological order (the one(s) which has to give its output first for the next ones to operate on it)
                foreach(IEnumerable<OrderedProcess> orderedProcessList in topological_order)
                {
                    foreach (OrderedProcess orderedProcess in orderedProcessList)
                    {
                        int index = int.Parse(orderedProcess.Name);
                        if (this[index] != null)
                        {
                            this[index].Update();
                            this[index].LatchOutputs();
                        }
                    }
                }
            }
            else
            {
                foreach (IEnumerable<OrderedProcess> orderedProcessList in topological_order)
                {
                    Parallel.ForEach(orderedProcessList, orderedProcess =>
                    {
                        int index = int.Parse(orderedProcess.Name);
                        if (this[index] != null)
                        {
                            this[index].Update();
                            this[index].LatchOutputs();
                        }
                    });
                }
                
            }
        }

        /// <summary>
        /// Computes the topological map of the models contained in the collection
        /// </summary>
        /// <returns>The topological map</returns>
        private IEnumerable<IEnumerable<OrderedProcess>> ComputeTopologicalMap()
        {
            DependencyGraph g = new DependencyGraph();

            OrderedProcess[] models = new OrderedProcess[Count];
            for (int i = 0; i < models.Length; i++)
            {
                models[i] = new OrderedProcess(g, i.ToString());
            }

            // Navigate through all the models in the collection
            for (int i = 0; i < models.Length; i++)
            {
                if (this[i] is DeMIMOI)
                {
                    DeMIMOI model = (DeMIMOI)this[i];
                    if(model.Inputs != null)
                    {
                        // Through all the inputs of t-i
                        for (int j = 0; j < model.Inputs.Count; j++)
                        {
                            // Through all the delayed inputs
                            for (int k = 0; k < model.Inputs[j].Count; k++)
                            {
                                // If the model is connected to another one
                                if (model.Inputs[j][k].ConnectedTo != null)
                                {
                                    // Get the index of that model and check if it's on the collection at the same time
                                    int connected_index = this.IndexOf(model.Inputs[j][k].ConnectedTo.Parent);
                                    // If it's in the collection and that it's not a connection to itself (i.e. cyclic connection)
                                    if (connected_index >= 0 && i != connected_index)
                                    {
                                        // Add it to the topological sort algorithm
                                        // Before, check the direction to determine who's before and who's after
                                        if (model.Inputs[j][k].Type == DeMIMOI_InputOutputType.INPUT)
                                        {
                                            models[i].After(models[connected_index]);
                                        }
                                        else
                                        {
                                            models[i].Before(models[connected_index]);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    // Do the same on outputs
                    if (model.Outputs != null)
                    {
                        for (int j = 0; j < model.Outputs.Count; j++)
                        {
                            for (int k = 0; k < model.Outputs[j].Count; k++)
                            {
                                if (model.Outputs[j][k].ConnectedTo != null)
                                {
                                    int connected_index = this.IndexOf(model.Outputs[j][k].ConnectedTo.Parent);
                                    if (connected_index >= 0 && i != connected_index)
                                    {
                                        if (model.Outputs[j][k].Type == DeMIMOI_InputOutputType.OUTPUT)
                                        {
                                            models[i].Before(models[connected_index]);
                                        }
                                        else
                                        {
                                            models[i].After(models[connected_index]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Return the topological map
            return g.CalculateSort();
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
            string code = "digraph g_" + GetType().Name + " {\n\trankdir=LR;\n\tranksep=1.25;\n";
            code += "\tedge [ fontcolor=red, fontsize=9, fontname=\"Times-Roman italic\" ];\n";
            //code += "\tnode [ penwidth=1 ];\n";
            code += GraphVizCode();
            code += "}\n\n";

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
                code = "subgraph cluster_" + GetType().Name + "_" + ID + " {\n";
                code += "label = \"" + Name + "\";\n";
                code += "style=filled;\nfillcolor=lightgrey;\nfontsize=18;\nfontname=\"Times-Roman bold\";\n";
                code += "node [style=filled, fillcolor = white];\n";
                code += "style = rounded;\n";
                code += GraphVizCode();
                code += "}\n\n";
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
                if (this[i] != null)
                {
                    if (this[i] is DeMIMOI_Collection)
                    {
                        code += ((DeMIMOI_Collection)this[i]).GraphVizCode(true);
                    }
                    else
                    {
                        code += this[i].GraphVizCode();
                    }
                }
            }

            return code;
        }
    }
}
