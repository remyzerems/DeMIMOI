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


        List<DeMIMOI_InputOutput> CurrentOutputs;
        List<DeMIMOI_InputOutput> CurrentInputs;

        #region Events
        /// <summary>
        /// Event that occurs when one input/output of the <see cref="DeMIMOI"/> model is connected to another input/output
        /// </summary>
        public event DeMIMOI_ConnectionEventHandler Connected;
        /// <summary>
        /// Event that occurs when one input/output of the <see cref="DeMIMOI"/> model is disconnected to another input/output
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
        public DeMIMOI(DeMIMOI_Port input_port, DeMIMOI_Port output_port)
        {
            Initialize(input_port, output_port);
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
        protected void Initialize(DeMIMOI_Port input_port, DeMIMOI_Port output_port)
        {
            // Check the parameters to see if there's no strange values...
            //CheckParameters(input_count, input_delays_count, output_count, output_delays_count);

            // Set name and ID
            ID = AllocNewId();
            Name = GetType().Name + "_" + ID;
            Description = "";
            
            // Initialize inputs and outputs arrays
            InitializeInputs(input_port);
            InitializeOutputs(output_port);
        }

        /// <summary>
        /// Initializes inputs arrays
        /// </summary>
        /// <param name="input_count">Number of inputs of the system</param>
        /// <param name="input_delays_count">Number of delayed input steps</param>
        protected void InitializeInputs(DeMIMOI_Port input_port)
        {
            if (input_port != null)
            {
                // Create the inputs image for time t
                CurrentInputs = new List<DeMIMOI_InputOutput>();
                for (int j = 0; j < input_port.IODelayCount.Length; j++)
                {
                    CurrentInputs.Add(new DeMIMOI_InputOutput(this, DeMIMOI_InputOutputType.INPUT));
                }

                // Create all the inputs including delayed inputs
                Inputs = new List<List<DeMIMOI_InputOutput>>();
                for (int i = 0; i < input_port.IODelayCount.Length; i++)
                {
                    // Create the delayed inputs for the current input
                    List<DeMIMOI_InputOutput> inputs_i_n = new List<DeMIMOI_InputOutput>();
                    for (int j = 0; j < input_port.IODelayCount[i]; j++)
                    {
                        DeMIMOI_InputOutput io = new DeMIMOI_InputOutput(this, DeMIMOI_InputOutputType.INPUT);
                        io.Connected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Connected);
                        io.Disconnected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Disconnected);
                        inputs_i_n.Add(io);
                    }
                    Inputs.Add(inputs_i_n);
                }
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
        protected void InitializeOutputs(DeMIMOI_Port output_port)
        {
            if (output_port != null)
            {
                // Create the outputs image for time t
                CurrentOutputs = new List<DeMIMOI_InputOutput>();
                for (int j = 0; j < output_port.IODelayCount.Length; j++)
                {
                    CurrentOutputs.Add(new DeMIMOI_InputOutput(this, DeMIMOI_InputOutputType.OUTPUT));
                }

                // Create all the outputs including delayed outputs
                Outputs = new List<List<DeMIMOI_InputOutput>>();
                for (int i = 0; i < output_port.IODelayCount.Length; i++)
                {
                    // Create the delayed outputs for the current output
                    List<DeMIMOI_InputOutput> outputs_i_n = new List<DeMIMOI_InputOutput>();
                    for (int j = 0; j < output_port.IODelayCount[i]; j++)
                    {
                        DeMIMOI_InputOutput io = new DeMIMOI_InputOutput(this, DeMIMOI_InputOutputType.OUTPUT);
                        io.Connected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Connected);
                        io.Disconnected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Disconnected);
                        outputs_i_n.Add(io);
                    }
                    Outputs.Add(outputs_i_n);
                }
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
            if (Inputs != null)
            {
                List<DeMIMOI_InputOutput> new_inputs = new List<DeMIMOI_InputOutput>();
                for (int i = 0; i < Inputs.Count; i++)
                {
                    // Add the input at time t
                    new_inputs.Add(Inputs[i][0]);
                }
                // Inject these inputs
                InjectInputs(new_inputs);
            }
        }

        /// <summary>
        /// Injects the specified inputs to the system (i.e. moving to the next step by pushing the step n inputs to n-1)
        /// </summary>
        /// <param name="new_inputs">The inputs to inject in the DeMIMOI system</param>
        public void InjectInputs(List<DeMIMOI_InputOutput> new_inputs)
        {
            if (Inputs != null)
            {
                // For each input
                for (int i = 0; i < Inputs.Count; i++)
                {
                    // Do not push the input delays if there's no delay input defined
                    if (Inputs[i].Count > 1)
                    {
                        // For each delayed input of the i-th input
                        for (int j = Inputs[i].Count - 2; j >= 1; j--)
                        {
                            // Push (by cloning to keep the references intact) all the inputs one step in the past
                            Inputs[i][j + 1].Value = Inputs[i][j].CloneValue();
                        }
                        // Set the input values for step n-1 to the specified inputs
                        Inputs[i][1].Value = new_inputs[i].CloneValue();
                    }
                }
            }
        }

        /// <summary>
        /// Injects the specified outputs to the system (i.e. moving to the next step by pushing the step n outputs to n-1)
        /// </summary>
        /// <param name="new_outputs">The outputs to inject in the DeMIMOI system</param>
        public void InjectOutputs(List<DeMIMOI_InputOutput> new_outputs)
        {
            if (Outputs != null)
            {
                // For each output
                for (int i = 0; i < Outputs.Count; i++)
                {
                    // For each delayed output of the i-th input
                    for (int j = Outputs[i].Count - 2; j >= 0; j--)
                    {
                        // Push (by cloning to keep the references intact) all the outputs one step in the past
                        Outputs[i][j + 1].Value = Outputs[i][j].CloneValue();
                    }
                    // Only update outputs of type OUTPUT (for DeMIMOINeuralNetwork compatibility)
                    if (Outputs[i][0].Type == DeMIMOI_InputOutputType.OUTPUT)
                    {
                        // Set the output values for step n-1 to the specified outputs
                        Outputs[i][0].Value = new_outputs[i].CloneValue();
                    }
                }
            }
        }

        /// <summary>
        /// The inputs of the DeMIMOI system
        /// </summary>
        public List<List<DeMIMOI_InputOutput>> Inputs
        {
            get;
            set;
        }

        /// <summary>
        /// The outputs of the DeMIMOI system
        /// </summary>
        public List<List<DeMIMOI_InputOutput>> Outputs
        {
            get;
            set;
        }

        /// <summary>
        /// User defined function (f) that calculates the outputs for n + 1 (Y(n+1) = f(X(n), X(n-1),...X(n-m), Y(n), Y(n-1),..., Y(n-p))
        /// <remarks>Note that the calculated outputs won't be applyed to the system outputs until the <see cref="LatchOutputs"/> function is called</remarks>
        /// </summary>
        /// <param name="new_outputs">Represents Y(n+1), the new outputs of the system</param>
        protected abstract void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs);

        /// <summary>
        /// Save the current inputs of the system in a temporary buffer
        /// <remarks>Inputs may be linked to other system outputs that can change anytime, so we have to be able to save the inputs</remarks>
        /// </summary>
        private void SaveInputs()
        {
            if (Inputs != null)
            {
                for (int i = 0; i < CurrentInputs.Count; i++)
                {
                    CurrentInputs[i].Value = Inputs[i][0].CloneValue();
                }
            }
        }

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
            if (Outputs != null)
            {
                if (CurrentOutputs.Count != Outputs.Count)
                {
                    throw new InvalidOperationException("UpdateInnerSystem must return an output array of " + Outputs.Count + " elements, but it contains " + CurrentOutputs.Count + " elements right now...");
                }
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
            get
            {
                if (Inputs != null)
                {
                    return Inputs.Count;
                }
                else
                {
                    return 0;
                }
            }
            //set;
        }
        
        /*
        /// <summary>
        /// Number of delayed inputs steps
        /// </summary>
        public int InputDelayCount
        {
            get;
            set;
        }*/

        /// <summary>
        /// Number of outputs of the system
        /// </summary>
        public int OutputCount
        {
            get
            {
                if (Outputs != null)
                {
                    return Outputs.Count;
                }
                else
                {
                    return 0;
                }
            }
            //set;
        }

        /*
        /// <summary>
        /// Number of delayed outputs steps
        /// </summary>
        public int OutputDelayCount
        {
            get;
            set;
        }*/

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

        /// <summary>
        /// Description of the DeMIMOI object
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Disconnects all the inputs given. If outputs are present they will be ignored
        /// </summary>
        /// <param name="inputs">Inputs to disconnect</param>
        public static void DisconnectAll(List<List<DeMIMOI_InputOutput>> inputs)
        {
            // For each input
            for (int i = 0; i < inputs.Count; i++)
            {
                // For each delayed input of the i-th input
                for (int j = 0; j < inputs[i].Count; j++)
                {
                    // Only inputs can be disconnected
                    if (inputs[i][j].Type == DeMIMOI_InputOutputType.INPUT)
                    {
                        // Disconnect the input
                        DeMIMOI_InputOutput.Unplug(inputs[i][j]);
                    }
                }
            }
        }

        /// <summary>
        /// Disconnects all the inputs of the model
        /// </summary>
        public void DisconnectAllInputs()
        {
            DisconnectAll(Inputs);
        }

        /// <summary>
        /// Returns the input which name is the same as the argument
        /// </summary>
        /// <param name="name">The name to find</param>
        /// <returns>The input found</returns>
        public DeMIMOI_InputOutput GetInputByName(string name)
        {
            return GetInputOutputByName(name, Inputs);
        }

        /// <summary>
        /// Returns the output which name is the same as the argument
        /// </summary>
        /// <param name="name">The name to find</param>
        /// <returns>The output found</returns>
        public DeMIMOI_InputOutput GetOutputByName(string name)
        {
            return GetInputOutputByName(name, Outputs);
        }

        /// <summary>
        /// Returns the input/output which name is the same as the argument
        /// </summary>
        /// <param name="name">The name to find</param>
        /// <param name="inputs_outputs">The input/output list to search in</param>
        /// <returns>The input/output found</returns>
        static DeMIMOI_InputOutput GetInputOutputByName(string name, List<List<DeMIMOI_InputOutput>> inputs_outputs)
        {
            DeMIMOI_InputOutput ret = null;
            for (int i = 0; i < inputs_outputs.Count; i++)
            {
                for (int j = 0; j < inputs_outputs[i].Count; j++)
                {
                    string inputName = "(t";
                    if (inputs_outputs[i][j].Name == "")
                    {
                        if (inputs_outputs[i][j].Type == DeMIMOI_InputOutputType.INPUT)
                        {
                            inputName = "i" + i + inputName;
                        }
                        else
                        {
                            inputName = "o" + i + inputName;
                        }
                    }
                    else
                    {
                        inputName = inputs_outputs[i][j].Name + inputName;
                    }
                    if (j > 0)
                    {
                        inputName += "-" + j;
                    }
                    inputName += ")";

                    if (name.Replace(" ", "") == inputName)
                    {
                        ret = inputs_outputs[i][j];
                        break;
                    }
                }
                if (ret != null)
                {
                    break;
                }
            }

            return ret;
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
            string code = "digraph " + GetType().Name + " {\n\trankdir=LR;\n\tranksep=1.25;\n\tedge [ fontcolor=red, fontsize=9, fontname=\"Times-Roman italic\" ];\n";
            code += GraphVizCode();
            code += "}";

            return code;
        }

        /// <summary>
        /// Function that generates the partial GraphViz code corresponding to the DeMIMOI structure
        /// <remarks>This only gives the GraphViz code for the DeMIMOI object only, it's not the full code</remarks>
        /// </summary>
        /// <returns>The GraphViz code</returns>
        public virtual string GraphVizCode()
        {
            string code = "";

            // Computes that kind of output
            //TestModel_1_inputs0[shape = Mrecord, label = "{{<f0> i0(t)|<f1> i1(t)|<f2> i2(t)}|{Modele_1}|{<fo0> o0(..)|<fo1> o1(..)}}"];

            // Determine the shape of the model
            string shape = "Mrecord";   // Default shape is a rounded rectangle
            // No input or output ?
            if (Inputs == null || Outputs == null)
            {
                // Shape is a rectangle
                shape = "record";
            }

            code += GetType().Name.Replace("`", "_") + "_" + ID + "[shape=" + shape + ", label=\"{";

            if(Inputs != null)
            {
                code += "{";
                // Draw inputs
                for (int i = 0; i < Inputs.Count; i++)
                {
                    code += "{{";
                    for (int j = 0; j < Inputs[i].Count; j++)
                    {
                        if (Inputs[i][j].ConnectedTo != null || j == 0)
                        {
                            if (j > 0 && j < Inputs[i].Count)
                            {
                                code += "|";
                            }
                            code += "<i" + i + "_" + j + "> " + (Inputs[i][j].Name == "" ? "i" + i : Inputs[i][j].Name) + "(t" + (j == 0 ? "" : " - " + j) + ")";
                        }
                    }
                    code += "}";
                    bool isConnected = false;
                    for (int j = 1; j < Inputs[i].Count; j++)
                    {
                        if (Inputs[i][j].ConnectedTo != null)
                        {
                            isConnected = true;
                            break;
                        }
                    }
                    if (Inputs[i].Count > 1 && isConnected == true)
                    {
                        code += "|" + (Inputs[i][0].Name == "" ? "i" + i : Inputs[i][0].Name);
                    }
                    code += "}";
                    if (i < Inputs.Count - 1)
                    {
                        code += "|";
                    }
                }
                code += "}|";
            }
            code += "{" + Name.Replace("\n", "\\n") + (Description != "" ? "\\n" + Description.Replace("\n", "\\n") : "") + "}";

            if (Outputs != null)
            {
                code += "|{";
                // Draw outputs
                for (int i = 0; i < Outputs.Count; i++)
                {
                    code += "{";

                    if (Outputs[i].Count > 1)
                    {
                        code += (Outputs[i][0].Name == "" ? "o" + i : Outputs[i][0].Name) + "|";
                    }
                    code += "{";
                    for (int j = 0; j < Outputs[i].Count; j++)
                    {
                        code += "<o" + i + "_" + j + "> " + (Outputs[i][j].Name == "" ? "o" + i : Outputs[i][j].Name) + "(t" + (j == 0 ? "" : " - " + j) + ")";
                        if (j < Outputs[i].Count - 1)
                        {
                            code += "|";
                        }
                    }
                    code += "}}";
                    if (i < Outputs.Count - 1)
                    {
                        code += "|";
                    }
                }
                code += "}";
            }
            code += "}\"];\n";

            if (Inputs != null)
            {
                code += GenerateLinkGraphVizCode(Inputs, DeMIMOI_InputOutputType.INPUT);
            }
            if (Outputs != null)
            {
                code += GenerateLinkGraphVizCode(Outputs, DeMIMOI_InputOutputType.OUTPUT);
            }

            code += "\n";


            return code;
        }

        protected virtual string GenerateLinkGraphVizCode(List<List<DeMIMOI_InputOutput>> input_output, DeMIMOI_InputOutputType argument_type)
        {
            return GenerateLinkGraphVizCode(input_output, argument_type, true);
        }
        protected virtual string GenerateLinkGraphVizCode(List<List<DeMIMOI_InputOutput>> input_output, DeMIMOI_InputOutputType argument_type, bool show_labels)
        {
            string code = "";

            // For each input/output
            for (int i = 0; i < input_output.Count; i++)
            {
                // For each delay of the i-th input/output
                for (int j = 0; j < input_output[i].Count; j++)
                {
                    if (input_output[i][j].ConnectedTo != null)
                    {
                        if (input_output[i][j].Type == DeMIMOI_InputOutputType.INPUT)
                        {
                            DeMIMOI ConnectedParent = input_output[i][j].ConnectedTo.Parent;
                            int connected_index = -1;
                            int delay_offset = -1;
                            for (int k = 0; k < ConnectedParent.Outputs.Count; k++)
                            {
                                for (int l = 0; l < ConnectedParent.Outputs[k].Count; l++)
                                {
                                    if (ConnectedParent.Outputs[k][l].ID == input_output[i][j].ConnectedTo.ID)
                                    {
                                        connected_index = k;
                                        delay_offset = l;
                                    }
                                }
                            }
                            code += input_output[i][j].ConnectedTo.Parent.GetType().Name.Replace("`", "_") + "_" + input_output[i][j].ConnectedTo.Parent.ID + ":o" + connected_index + "_" + delay_offset + ":e -> " + GetType().Name.Replace("`", "_") + "_" + ID + (argument_type == DeMIMOI_InputOutputType.INPUT ? ":i" : ":o") + i + "_" + j + (argument_type == DeMIMOI_InputOutputType.INPUT ? ":w" : ":e");
                            if (show_labels == true)
                            {
                                if (delay_offset > 0)
                                {
                                    code += " [ label=\"t - " + delay_offset + "\" ]";
                                }
                                else
                                {
                                    code += " [ label=\"t\" ]";
                                }
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
