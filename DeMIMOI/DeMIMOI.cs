﻿// Delayed Multiple Input Multiple Output Interface (DeMIMOI) Library
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

namespace DeMIMOI_Models
{
    /// <summary>
    /// DeMIMOI system class
    /// </summary>
    [Serializable]
    public abstract class DeMIMOI : IDeMIMOI_Interface
    {

        static int current_id = 0;  // Current ID to allocate to a new DeMIMOI object
        /// <summary>
        /// Allocates a new DeMIMOI ID
        /// </summary>
        /// <returns>The new ID</returns>
        private static int AllocNewId()
        {
            return current_id++;
        }


        DeMIMOI_InputOutput[] CurrentOutputs;
        DeMIMOI_InputOutput[] CurrentInputs;

        #region Events
        /// <summary>
        /// Event the occurs when one input/output of the <see cref="DeMIMOI"/> model is connected to another input/output
        /// </summary>
        public event DeMIMOI_ConnectionEventHandler Connected;
        /// <summary>
        /// Event the occurs when one input/output of the <see cref="DeMIMOI"/> model is disconnected to another input/output
        /// </summary>
        public event DeMIMOI_ConnectionEventHandler Disconnected;
        #endregion

        /// <summary>
        /// Constructor for XmlSerialization purposes only
        /// </summary>
        public DeMIMOI()
        {
        }

        /// <summary>
        /// Creates a new DeMIMOI object
        /// </summary>
        /// <param name="input_count">Number of inputs of the system</param>
        /// <param name="input_delays_count">Number of delayed input steps</param>
        /// <param name="output_count">Number of outputs of the system</param>
        /// <param name="output_delays_count">Number of delayed output steps</param>
        public DeMIMOI(int input_count, int input_delays_count, int output_count, int output_delays_count)
        {
            Initialize(input_count, input_delays_count, output_count, output_delays_count);
        }

        /// <summary>
        /// Looks at the values of each parameters to check if there's no strange values
        /// </summary>
        /// <param name="input_count">Number of inputs of the system</param>
        /// <param name="input_delays_count">Number of delayed input steps</param>
        /// <param name="output_count">Number of outputs of the system</param>
        /// <param name="output_delays_count">Number of delayed output steps</param>
        void CheckParameters(int input_count, int input_delays_count, int output_count, int output_delays_count)
        {
            if (input_count < 1)
            {
                throw new ArgumentOutOfRangeException("The system must have at least one input !");
            }
            if (input_delays_count < 1)
            {
                throw new ArgumentOutOfRangeException("The system must have at least one input delay !");
            }
            if (output_count < 1)
            {
                throw new ArgumentOutOfRangeException("The system must have at least one output !");
            }
            if (output_delays_count < 1)
            {
                throw new ArgumentOutOfRangeException("The system must have at least one output delay !");
            }
        }

        /// <summary>
        /// Initializes a new DeMIMOI object
        /// </summary>
        /// <param name="input_count">Number of inputs of the system</param>
        /// <param name="input_delays_count">Number of delayed input steps</param>
        /// <param name="output_count">Number of outputs of the system</param>
        /// <param name="output_delays_count">Number of delayed output steps</param>
        void Initialize(int input_count, int input_delays_count, int output_count, int output_delays_count)
        {
            // Check the parameters to see if there's no strange values...
            CheckParameters(input_count, input_delays_count, output_count, output_delays_count);

            // Set name and ID
            ID = AllocNewId();
            Name = "DeMIMOI_" + ID;

            // Save system parameters
            InputCount = input_count;
            InputDelayCount = input_delays_count;
            OutputCount = output_count;
            OutputDelayCount = output_delays_count;
            
            // Initialize inputs and outputs arrays
            InitializeInputs(input_count, input_delays_count);
            InitializeOutputs(output_count, output_delays_count);
        }

        /// <summary>
        /// Initializes inputs arrays
        /// </summary>
        /// <param name="input_count">Number of inputs of the system</param>
        /// <param name="input_delays_count">Number of delayed input steps</param>
        void InitializeInputs(int input_count, int input_delays_count)
        {
            Inputs = new List<DeMIMOI_InputOutput[]>();
            CurrentInputs = new DeMIMOI_InputOutput[input_count];
            for (int j = 0; j < CurrentInputs.Length; j++)
            {
                CurrentInputs[j] = new DeMIMOI_InputOutput(this, DeMIMOI_InputOutputType.INPUT);
            }

            for (int i = 0; i < input_delays_count; i++)
            {
                DeMIMOI_InputOutput[] inputs_0n = new DeMIMOI_InputOutput[input_count];
                for (int j = 0; j < inputs_0n.Length; j++)
                {
                    inputs_0n[j] = new DeMIMOI_InputOutput(this, DeMIMOI_InputOutputType.INPUT);
                    inputs_0n[j].Connected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Connected);
                    inputs_0n[j].Disconnected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Disconnected);
                }
                Inputs.Add(inputs_0n);
            }
        }

        void DeMIMOI_Connected(object sender, DeMIMOI_ConnectionEventArgs e)
        {
            // A Connected event occured on one of the DeMIMOI input or output
            if (Connected != null)
            {
                // Pass this event as a DeMIMOI event
                Connected(this, e);
            }
        }
        
        /// <summary>
        /// Initializes outputs arrays
        /// </summary>
        /// <param name="output_count">Number of outputs of the system</param>
        /// <param name="output_delays_count">Number of delayed output steps</param>
        void InitializeOutputs(int output_count, int output_delays_count)
        {
            Outputs = new List<DeMIMOI_InputOutput[]>();
            CurrentOutputs = new DeMIMOI_InputOutput[output_count];
            for (int j = 0; j < CurrentOutputs.Length; j++)
            {
                CurrentOutputs[j] = new DeMIMOI_InputOutput(this, DeMIMOI_InputOutputType.OUTPUT);
            }

            for (int i = 0; i < output_delays_count; i++)
            {
                DeMIMOI_InputOutput[] outputs_0n = new DeMIMOI_InputOutput[output_count];
                for (int j = 0; j < outputs_0n.Length; j++)
                {
                    outputs_0n[j] = new DeMIMOI_InputOutput(this, DeMIMOI_InputOutputType.OUTPUT);
                    outputs_0n[j].Connected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Connected);
                    outputs_0n[j].Disconnected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Disconnected);
                }
                Outputs.Add(outputs_0n);
            }
        }

        void DeMIMOI_Disconnected(object sender, DeMIMOI_ConnectionEventArgs e)
        {
            // A Disconnected event occured on one of the DeMIMOI input or output
            if (Disconnected != null)
            {
                // Pass this event as a DeMIMOI event
                Disconnected(this, e);
            }
        }

        /// <summary>
        /// Injects the current inputs to the system (i.e. moving to the next step by pushing the step n inputs to n-1)
        /// </summary>
        public void InjectInputs()
        {
            InjectInputs(Inputs[0]);
        }

        /// <summary>
        /// Injects the specified inputs to the system (i.e. moving to the next step by pushing the step n inputs to n-1)
        /// </summary>
        /// <param name="new_inputs">The inputs to inject in the DeMIMOI system</param>
        public void InjectInputs(DeMIMOI_InputOutput[] new_inputs)
        {
            if (InputDelayCount > 1)
            {
                // Push (by cloning to keep the references intact) all the inputs one step in the past
                for (int i = InputDelayCount - 2; i >= 1; i--)
                {
                    for (int j = 0; j < InputCount; j++)
                    {
                        Inputs[i + 1][j].Value = Inputs[i][j].CloneValue();
                    }
                }
                // Set the input values for step n-1 to the specified inputs
                for (int j = 0; j < InputCount; j++)
                {
                    Inputs[1][j].Value = new_inputs[j].CloneValue();
                }
            }
        }

        /// <summary>
        /// Injects the specified outputs to the system (i.e. moving to the next step by pushing the step n outputs to n-1)
        /// </summary>
        /// <param name="new_outputs">The outputs to inject in the DeMIMOI system</param>
        private void InjectOutputs(DeMIMOI_InputOutput[] new_outputs)
        {
            // Push (by cloning to keep the references intact) all the outputs one step in the past
            for (int i = OutputDelayCount - 2; i >= 0 ; i--)
            {
                for (int j = 0; j < OutputCount; j++)
                {
                    Outputs[i + 1][j].Value = Outputs[i][j].CloneValue();
                }
            }
            // Set the output values for step n to the specified outputs
            for (int j = 0; j < OutputCount; j++)
            {
                // Only update outputs of type OUTPUT (for DeMIMOINeuralNetwork compatibility)
                if (Outputs[0][j].Type == DeMIMOI_InputOutputType.OUTPUT)
                {
                    Outputs[0][j].Value = new_outputs[j].CloneValue();
                }
            }
        }

        /// <summary>
        /// The inputs of the DeMIMOI system
        /// </summary>
        public List<DeMIMOI_InputOutput[]> Inputs
        {
            get;
            set;
        }

        /// <summary>
        /// The outputs of the DeMIMOI system
        /// </summary>
        public List<DeMIMOI_InputOutput[]> Outputs
        {
            get;
            set;
        }

        /// <summary>
        /// User defined function (f) that calculates the outputs for n + 1 (Y(n+1) = f(X(n), X(n-1),...X(n-m), Y(n), Y(n-1),..., Y(n-p))
        /// <remarks>Note that the calculated outputs won't be applyed to the system outputs until the <see cref="LatchOutputs"/> function is called</remarks>
        /// </summary>
        /// <param name="new_outputs">Represents Y(n+1), the new outputs of the system</param>
        protected abstract void UpdateInnerSystem(ref DeMIMOI_InputOutput[] new_outputs);

        /// <summary>
        /// Save the current inputs of the system in a temporary buffer
        /// <remarks>Inputs may be linked to other system outputs that can change anytime, so we have to be able to save the inputs</remarks>
        /// </summary>
        private void SaveInputs()
        {
            for (int i = 0; i < CurrentInputs.Length; i++)
            {
                CurrentInputs[i].Value = Inputs[0][i].CloneValue();
            }
        }

        /* Useless for this time ?
        private void SaveOutputs(DeMIMOI_InputOutput[] outputs)
        {
            for (int i = 0; i < CurrentOutputs.Length; i++)
            {
                CurrentOutputs[i].Value = outputs[i].CloneValue();
            }
        }

        private void SaveOutputs()
        {
            for (int i = 0; i < CurrentOutputs.Length; i++)
            {
                CurrentOutputs[i].Value = Outputs[0][i].CloneValue();
            }
        }*/

        /// <summary>
        /// Updates the DeMIMOI
        /// <remarks>
        /// It calls the inner user defined function <see cref="UpdateInnerSystem"/> and manages the delayed inputs/outputs.
        /// Don't forget to call LatchOutputs to publish the new outputs to the outputs of the DeMIMOI !
        /// </remarks>
        /// </summary>
        public void Update()
        {
            // If it's not the first time we update the system
            if (UpdateCount != 0)
            {
                // Inject in the delayed inputs the inputs we used last time on the UpdateInnerSystem
                InjectInputs(CurrentInputs);
            }
            // Save the current inputs to be able to push them at the next step
            SaveInputs();
            // Call the user defined inner function of the DeMIMOI object to calculate the new outputs
            UpdateInnerSystem(ref CurrentOutputs);

            // Be sure the user did not change the CurrentOutputs characteristics
            if (CurrentOutputs.Length != OutputCount)
            {
                throw new InvalidOperationException("UpdateInnerSystem must return an output array of " + OutputCount + " elements, but it contains " + CurrentOutputs.Length + " elements right now...");
            }

            // Count one more system update
            UpdateCount++;
        }

        /// <summary>
        /// Updates the object and publishes the new calculated outputs directly
        /// </summary>
        public void UpdateAndLatch()
        {
            Update();
            LatchOutputs();
        }

        /// <summary>
        /// Represents how many times the system has been updated
        /// </summary>
        public long UpdateCount
        {
            get;
            set;
        }

        /// <summary>
        /// Represents how many times the system outputs have been latched
        /// </summary>
        public long LatchOutputsCount
        {
            get;
            set;
        }

        public virtual void LatchOutputs()
        {
            // Publish the new outputs to the system outputs
            InjectOutputs(CurrentOutputs);

            // Count one more outputs latching
            LatchOutputsCount++;
        }

        /// <summary>
        /// Number of inputs of the system
        /// </summary>
        public int InputCount
        {
            get;
            set;
        }
        
        /// <summary>
        /// Number of delayed inputs steps
        /// </summary>
        public int InputDelayCount
        {
            get;
            set;
        }

        /// <summary>
        /// Number of outputs of the system
        /// </summary>
        public int OutputCount
        {
            get;
            set;
        }

        /// <summary>
        /// Number of delayed outputs steps
        /// </summary>
        public int OutputDelayCount
        {
            get;
            set;
        }

        /// <summary>
        /// DeMIMOI ID number (distinct number for each DeMIMOI object)
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the DeMIMOI object
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /*public void Save(string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(DeMIMOI));
            using (StreamWriter wr = new StreamWriter(filename))
            {
                xs.Serialize(wr, this);
            }
        }*/


        /// <summary>
        /// Creates a full GraphViz script that represents the DeMIMOI system
        /// </summary>
        /// <returns>The GraphViz code</returns>
        public string GraphVizFullCode()
        {
            string code = "digraph " + Name.Replace(" ", "_") + " {\n\trankdir=LR;\n";
            code += GraphVizCode();
            code += "}";

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

            // Computes that kind of output
            //TestModel_1_inputs0[shape = Mrecord, label = "{{<f0> i0(t)|<f1> i1(t)|<f2> i2(t)}|{Modele_1}|{<fo0> o0(..)|<fo1> o1(..)}}"];

            code += Name.Replace(" ", "_") + "_" + ID + "[shape=Mrecord, label=\"{";

            code += "{";
            for (int i = 0; i < Inputs[0].Length; i++)
            {
                code += "<i" + i + "> " + (Inputs[0][i].Name == "" ? "i" + i : Inputs[0][i].Name) + "(t)";
                if (i < Inputs[0].Length - 1)
                {
                    code += "|";
                }
            }
            code += "}|{" + Name + "}|{";
            for (int i = 0; i < Outputs[0].Length; i++)
            {
                code += "<o" + i + "> " + (Outputs[0][i].Name == "" ? "o" + i : Outputs[0][i].Name) + "(..)";
                if (i < Outputs[0].Length - 1)
                {
                    code += "|";
                }
            }
            code += "}}\"];\n";

            code += GenerateLinkGraphVizCode(Inputs, DeMIMOI_InputOutputType.INPUT);
            code += GenerateLinkGraphVizCode(Outputs, DeMIMOI_InputOutputType.OUTPUT);

            code += "\n";


            return code;
        }

        private string GenerateLinkGraphVizCode(List<DeMIMOI_InputOutput[]> input_output, DeMIMOI_InputOutputType argument_type)
        {
            string code = "";

            for (int j = 0; j < input_output.Count; j++)
            {
                for (int i = 0; i < input_output[j].Length; i++)
                {
                    if (input_output[j][i].ConnectedTo != null)
                    {
                        if (input_output[j][i].Type == DeMIMOI_InputOutputType.INPUT)
                        {
                            DeMIMOI ConnectedParent = input_output[j][i].ConnectedTo.Parent;
                            int connected_index = -1;
                            int delay_offset = -1;
                            for (int k = 0; k < ConnectedParent.Outputs.Count; k++)
                            {
                                for (int l = 0; l < ConnectedParent.Outputs[k].Length; l++)
                                {
                                    if (ConnectedParent.Outputs[k][l].ID == input_output[j][i].ConnectedTo.ID)
                                    {
                                        connected_index = l;
                                        delay_offset = k;
                                    }
                                }
                            }
                            code += input_output[j][i].ConnectedTo.Parent.Name.Replace(" ", "_") + "_" + input_output[j][i].ConnectedTo.Parent.ID + ":o" + connected_index + " -> " + Name.Replace(" ", "_") + "_" + ID + (argument_type == DeMIMOI_InputOutputType.INPUT ? ":i" : ":o") + i;
                            if (delay_offset > 0)
                            {
                                code += " [ label=\"t - " + delay_offset + "\" ]";
                            }
                            else
                            {
                                code += " [ label=\"t\" ]";
                            }
                            code += ";\n";
                        }
                    }
                }
            }

            return code;
        }
    }
}
