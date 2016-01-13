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

        private IList<IDeMIMOI_Interface> _collection;

        // Same as _collection but with all DeMIMOI_Collection objects deployed
        private IList<IDeMIMOI_Interface> _flat_collection;

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

            _collection = new List<IDeMIMOI_Interface>();
            _flat_collection = new List<IDeMIMOI_Interface>();
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
            // Clear the collection by calling the Remove function so to execute some specific instructions
            foreach (IDeMIMOI_Interface item in _collection)
            {
                _collection.Remove(item);
            }
        }

        public bool Contains(IDeMIMOI_Interface item)
        {
            // Ask the collection to check if it contains "item"
            bool contains = _collection.Contains(item);

            // If it doesn't
            if (contains == false)
            {
                // Check in the collections this current collection has on its list (i.e. search the sub-collections)
                for (int i = 0; i < Count; i++)
                {
                    // If the i-th element is a collection
                    if (this[i] is DeMIMOI_Collection)
                    {
                        // Ask the sub-collection to check if it contains "item"
                        contains = ((DeMIMOI_Collection)this[i]).Contains(item);
                        // If we find one true result, we can say that it contains "item", so leave the loop
                        if (contains == true)
                        {
                            break;
                        }
                    }
                }
            }

            return contains;
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


        protected void RefreshFlatCollection()
        {
            if (_flat_collection == null)
            {
                _flat_collection = new List<IDeMIMOI_Interface>();
            }

            _flat_collection.Clear();
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] is DeMIMOI_Collection)
                {
                    DeMIMOI_Collection cur_coll = this[i] as DeMIMOI_Collection;
                    if (cur_coll.refresh_topological_order == true)
                    {
                        cur_coll.RefreshFlatCollection();
                    }
                    for (int j = 0; j < cur_coll._flat_collection.Count; j++)
                    {
                        if (_flat_collection.Contains(cur_coll._flat_collection[j]) == false)
                        {
                            _flat_collection.Add(cur_coll._flat_collection[j]);
                        }
                    }
                }
                else
                {
                    _flat_collection.Add(this[i]);
                }
            }
        }

        /// <summary>
        /// Updates all the <see cref="DeMIMOI"/> objects of the collection
        /// </summary>
        public virtual void Update()
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
        public virtual void UpdateAndLatchTopologically()
        {
            // If the models topology changed from last time
            if (refresh_topological_order == true || topological_order == null)
            {
                // Refresh the flat collection
                RefreshFlatCollection();

                // Ask for topological map computation
                topological_order = ComputeTopologicalMap(_flat_collection);

                refresh_topological_order = false;
            }

            if (topological_order != null)
            {
                if (EnableMultithreading == false)
                {
                    // Updates each model by its topological order (the one(s) which has to give its output first for the next ones to operate on it)
                    foreach (IEnumerable<OrderedProcess> orderedProcessList in topological_order)
                    {
                        foreach (OrderedProcess orderedProcess in orderedProcessList)
                        {
                            int index = int.Parse(orderedProcess.Name);
                            if (_flat_collection[index] != null)
                            {
                                _flat_collection[index].Update();
                                _flat_collection[index].LatchOutputs();
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
                            if (_flat_collection[index] != null)
                            {
                                _flat_collection[index].Update();
                                _flat_collection[index].LatchOutputs();
                            }
                        });
                    }

                }
            }
        }

        /// <summary>
        /// Computes the topological map of the models contained in the collection
        /// </summary>
        /// <returns>The topological map</returns>
        private IEnumerable<IEnumerable<OrderedProcess>> ComputeTopologicalMap(IList<IDeMIMOI_Interface> referenceCollection)
        {
            DependencyGraph g = new DependencyGraph();

            OrderedProcess[] models = new OrderedProcess[referenceCollection.Count];
            for (int i = 0; i < models.Length; i++)
            {
                models[i] = new OrderedProcess(g, i.ToString());
            }

            // Navigate through all the models in the collection
            for (int i = 0; i < models.Length; i++)
            {
                if (referenceCollection[i] is DeMIMOI)
                {
                    DeMIMOI model = (DeMIMOI)referenceCollection[i];
                    if(model.Inputs != null)
                    {
                        // Through all the inputs of t-i
                        for (int j = 0; j < model.Inputs.Count; j++)
                        {
                            // Through all the delayed inputs
                            for (int k = 0; k < model.Inputs[j].Count; k++)
                            {
                                // If the model is connected to another one and the connection has to be taken into account
                                if (model.Inputs[j][k].ConnectedTo != null && model.Inputs[j][k].IgnoreConnection == false)
                                {
                                    // Get the index of that model and check if it's on the collection at the same time
                                    int connected_index = referenceCollection.IndexOf(model.Inputs[j][k].ConnectedTo.Parent);
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
                                    int connected_index = referenceCollection.IndexOf(model.Outputs[j][k].ConnectedTo.Parent);
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

            if (models.Length > 0)
            {
                // Return the topological map
                return g.CalculateSort();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Publishes all the <see cref="DeMIMOI"/> object outputs of the collection
        /// </summary>
        public virtual void LatchOutputs()
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
        /// Logical OR definition. Generates a combination of the models present in the two collections
        /// </summary>
        /// <param name="lhs">Left operand</param>
        /// <param name="rhs">Right operand</param>
        /// <returns>The OR-ed collection</returns>
        public static DeMIMOI_Collection operator |(DeMIMOI_Collection lhs, DeMIMOI_Collection rhs)
        {
            DeMIMOI_Collection res_col = new DeMIMOI_Collection();

            // If the left operand is not null
            if (lhs != null)
            {
                // Give the new collection its name
                res_col.Name = lhs.Name;

                // Add all the elements present in the left operand after flattening the collection
                DeMIMOI_Collection flat_coll = FlattenCollection(lhs);
                res_col.AddRange(flat_coll.ToArray());
            }

            // If the right operand is not null
            if (rhs != null)
            {
                // If the left operand was not null
                if (lhs != null)
                {
                    // Add the OR text
                    res_col.Name += " OR ";
                }
                // Add the the name of the right operand
                res_col.Name += rhs.Name;

                // Flatten rhs
                DeMIMOI_Collection flat_rhs = FlattenCollection(rhs);

                // Go through all the members of the flattened right operand
                for (int i = 0; i < flat_rhs.Count; i++)
                {
                    // If the resulting collection does not already contain the current model
                    if (res_col.Contains(flat_rhs[i]) == false)
                    {
                        // Add it
                        res_col.Add(flat_rhs[i]);
                    }
                }
            }

            return res_col;
        }
        /*        public static DeMIMOI_Collection operator|(DeMIMOI_Collection lhs, DeMIMOI_Collection rhs)
        {
            DeMIMOI_Collection res_col = new DeMIMOI_Collection();

            // If the left operand is not null
            if (lhs != null)
            {
                // Give the new collection its name
                res_col.Name = lhs.Name;

                // Add all the elements present in the left operand
                res_col.AddRange(lhs);
            }

            // If the right operand is not null
            if (rhs != null)
            {
                // If the left operand was not null
                if (lhs != null)
                {
                    // Add the OR text
                    res_col.Name += " OR ";
                }
                // Add the the name of the right operand
                res_col.Name += rhs.Name;

                // Go through all the members of the right operand
                for(int i = 0;i < rhs.Count;i++)
                {
                    // If the model is a collection
                    if (rhs[i] is DeMIMOI_Collection)
                    {
                        DeMIMOI_Collection rhs_coll = ((DeMIMOI_Collection)rhs[i]);
                        DeMIMOI_Collection rhs_new_coll = SubstituteCollections(rhs_coll);
                        res_col.Add(rhs_new_coll);
                    }
                    else
                    {
                        res_col.Add(rhs[i]);
                    }
                }
            }
            

            return res_col;
        }*/

        private static DeMIMOI_Collection FlattenCollection(DeMIMOI_Collection collection)
        {
            DeMIMOI_Collection res_col = new DeMIMOI_Collection();
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i] is DeMIMOI_Collection)
                {
                    DeMIMOI_Collection tmp_col = FlattenCollection((DeMIMOI_Collection)collection[i]);
                    res_col.AddRange(tmp_col.ToArray());
                }
                else
                {
                    res_col.Add(collection[i]);
                }
            }

            return res_col;
        }

        

        private static void RemoveIdenticals(DeMIMOI_Collection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i] is DeMIMOI_Collection)
                {

                }
                else
                {
                    RemoveIdenticals(collection, collection[i]);
                }
            }
        }

        private static void RemoveIdenticals<T>(DeMIMOI_Collection collection, T element_to_find)  where T : IDeMIMOI_Interface
        {
            int index = collection.IndexOf(element_to_find);
            if (index < 0)
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    if (collection[i] is DeMIMOI_Collection)
                    {
                        RemoveIdenticals(((DeMIMOI_Collection)collection[i]));
                    }
                }
            }
            else
            {

            }
        }

        private static DeMIMOI_Collection SubstituteCollections(DeMIMOI_Collection collection)
        {
            DeMIMOI_Collection substituted_coll = new DeMIMOI_Collection(collection.Name);
            for (int i = 0; i < substituted_coll.Count; i++)
            {
                if (collection[i] is DeMIMOI_Collection)
                {
                    DeMIMOI_Collection substituted_coll_sublevel = SubstituteCollections(((DeMIMOI_Collection)collection[i]));
                    substituted_coll.Add(substituted_coll_sublevel);
                }
                else
                {
                    substituted_coll.Add(collection[i]);
                }
            }

            return substituted_coll;
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
