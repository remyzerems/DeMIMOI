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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Globalization;

namespace DeMIMOI_Models
{

    /// <summary>
    /// DeMIMOI Input/Ouput type
    /// </summary>
    public enum DeMIMOI_InputOutputType
    {
        INPUT,
        OUTPUT
    }

    /// <summary>
    /// DeMIMOI Input or Output class
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(DeMIMOI_InputOutputConverter))]
    public class DeMIMOI_InputOutput
    {
        static int current_id = 0; // Current ID to allocate to a new DeMIMOI_InputOutput object
        /// <summary>
        /// Allocates a new DeMIMOI_InputOutput ID
        /// </summary>
        private static int AllocNewId()
        {
            return current_id++;
        }

        /// <summary>
        /// Search the inputs or outputs of the parent model of the input given to return the input indexes in the Input array it is contained
        /// </summary>
        /// <param name="input">Input to find the indexes</param>
        /// <param name="arrayToSearchIn">Array of the DeMIMOI to search in (Inputs or Outputs array)</param>
        /// <returns>The indexes found (both to -1 if it does not succeed)</returns>
        private static int[] GetInputOutputIndexes(DeMIMOI_InputOutput input, DeMIMOI_InputOutputType arrayToSearchIn)
        {
            // Initialize the indexes found to -1 (default if not found)
            int[] indexesFound = new int[2];
            indexesFound[0] = -1;
            indexesFound[1] = -1;

            // Create a reference that points the array we should search in
            List<List<DeMIMOI_InputOutput>> demimoi_io_array;
            if (arrayToSearchIn == DeMIMOI_InputOutputType.INPUT)
            {
                demimoi_io_array = input.Parent.Inputs;
            }
            else
            {
                demimoi_io_array = input.Parent.Outputs;
            }

            // Search the selected array to find the input_output in
            for (int i = 0; i < demimoi_io_array.Count; i++)
            {
                for (int j = 0; j < demimoi_io_array[i].Count; j++)
                {
                    // If we find it
                    if (demimoi_io_array[i][j] == input)
                    {
                        // Get the index and leave the whole loop
                        indexesFound[0] = i;
                        indexesFound[1] = j;
                        break;
                    }
                }
                // If we found the indexes, leave the loop
                if (indexesFound[0] != -1)
                {
                    break;
                }
            }

            return indexesFound;
        }

        /// <summary>
        /// Search the inputs or outputs of the parent model of the input given to return the input indexes in the Input array it is contained
        /// </summary>
        /// <param name="input_output">Input to find the indexes</param>
        /// <returns>The indexes found (both to -1 if it does not succeed)</returns>
        public static int[] GetInputOutputIndexes(DeMIMOI_InputOutput input_output)
        {
            // First search in the same array than the input_output.Type indicates
            int[] indexesFound = GetInputOutputIndexes(input_output, input_output.Type);
            // If we didn't find anything, try on the other array (i.e. Output array if input_output is an input and vice versa)
            if (indexesFound[0] == -1)
            {
                if (input_output.Type == DeMIMOI_InputOutputType.INPUT)
                {
                    indexesFound = GetInputOutputIndexes(input_output, DeMIMOI_InputOutputType.OUTPUT);
                }
                else
                {
                    indexesFound = GetInputOutputIndexes(input_output, DeMIMOI_InputOutputType.INPUT);
                }
            }

            return indexesFound;
        }

        // Inner own Input/Output value
        object input_output_value;

        #region Events
        /// <summary>
        /// Event the occurs when the input/output is connected to another one
        /// </summary>
        public event DeMIMOI_ConnectionEventHandler Connected;
        /// <summary>
        /// Event the occurs when the input/output is disconnected from another one
        /// </summary>
        public event DeMIMOI_ConnectionEventHandler Disconnected;
        #endregion

        /// <summary>
        /// Initialize a DeMIMOI_InputOutput
        /// </summary>
        void Initialize()
        {
            // Sets the ID
            ID = AllocNewId();

            // Say that by default it's an Input
            Type = DeMIMOI_InputOutputType.INPUT;

            // No parent by default
            Parent = null;

            // No name (i.e. default name will be used)
            Name = "";

            // Take the connection into account when computing the topological map by default
            IgnoreConnection = false;
        }

        /// <summary>
        /// Constructs a new DeMIMOI_InputOutput object
        /// </summary>
        public DeMIMOI_InputOutput()
        {
            Initialize();
        }

        /// <summary>
        /// Constructs a new DeMIMOI_InputOutput object by specifying its type
        /// </summary>
        /// <param name="type">Input/Output type</param>
        public DeMIMOI_InputOutput(DeMIMOI_InputOutputType type)
        {
            Initialize();

            // Set the specified type
            Type = type;
        }

        /// <summary>
        /// Constructs a new DeMIMOI_InputOutput object by specifying its type and its parent DeMIMOI object
        /// </summary>
        /// <param name="parent">DeMIMOI object in which it is used and included</param>
        /// <param name="type">Input/Output type</param>
        public DeMIMOI_InputOutput(DeMIMOI parent, DeMIMOI_InputOutputType type)
        {
            Initialize();

            // Set the specified type
            Type = type;
            // Set the parent DeMIMOI object
            Parent = parent;
        }

        [DescriptionAttribute("DeMIMOI_InputOutput ID"),
        CategoryAttribute("General")]
        /// <summary>
        /// DeMIMOI_InputOutput ID
        /// </summary>
        public int ID
        {
            get;
            set;
        }

        [DescriptionAttribute("DeMIMOI_InputOutput name"),
        CategoryAttribute("General")]
        /// <summary>
        /// DeMIMOI_InputOutput name
        /// </summary>
        public string Name
        {
            set;
            get;
        }

        /// <summary>
        /// Value of the Input/Output
        /// <remarks>In case it's an Input and it's connected to an Output, it returns the Output value. For all other cases, it returns its own inner value</remarks>
        /// </summary>
        public object Value
        {
            get
            {
                // If it's not connected to an Output
                if (ConnectedTo == null)
                {
                    // Return the inner own value
                    return input_output_value;
                }
                else
                {
                    // It's connected to an output, so return this output value
                    return ConnectedTo.Value;
                }
            }
            set
            {
                // If it's not connected to an Output
                if (ConnectedTo == null)
                {
                    // Change the inner own value
                    input_output_value = value;
                }
                /*else
                {
                    // It's connected to an output, so, changing an output value this way doesn't make any sense...
                    // That's why we shouldn't do this : ConnectedTo.Value = value;
                }*/
                
            }
        }

        [BrowsableAttribute(false)]
        /// <summary>
        /// DeMIMOI object on which this DeMIMOI_InputOutput is included and used
        /// </summary>
        [XmlIgnore]
        public DeMIMOI Parent
        {
            get;
            protected set;
        }

        [DescriptionAttribute("Output DeMIMOI_InputOutput to which an Input DeMIMOI_InputOutput is connected"),
        CategoryAttribute("Connections")]
        /// <summary>
        /// Output DeMIMOI_InputOutput to which an Input DeMIMOI_InputOutput is connected
        /// </summary>
        [XmlIgnore]
        public DeMIMOI_InputOutput ConnectedTo
        {
            get;
            protected set;
        }

        [BrowsableAttribute(false)]
        /// <summary>
        /// Flag that indicates this connection must not be taken into account when computing the topological map
        /// </summary>
        [XmlIgnore]
        public bool IgnoreConnection
        {
            get;
            set;
        }

        [DescriptionAttribute("DeMIMOI_InputOutput type (input, output...)"),
        CategoryAttribute("General")]
        /// <summary>
        /// DeMIMOI_InputOutput type (input, output...)
        /// </summary>
        public DeMIMOI_InputOutputType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Connects two DeMIMOI_InputOutput to each other
        /// <remarks>It has to be one input and one output.</remarks>
        /// </summary>
        /// <param name="input_output">If the current object is an input, this argument must be an output, if the current object is an output, this argument must be an input</param>
        public void ConnectTo(DeMIMOI_InputOutput input_output)
        {
            // If the input we want to connect is defined...
            if (input_output != null)
            {
                // If it's an output and the argument is an input...
                if (Type == DeMIMOI_InputOutputType.OUTPUT && input_output.Type == DeMIMOI_InputOutputType.INPUT)
                {
                    // Ok, connect the input to this output
                    input_output.ConnectedTo = this;

                    // If event signalling is required
                    if (Connected != null)
                    {
                        Connected(this, new DeMIMOI_ConnectionEventArgs(this, input_output));
                    }
                    // If event signalling for the input is required
                    if (input_output.Connected != null)
                    {
                        input_output.Connected(this, new DeMIMOI_ConnectionEventArgs(this, input_output));
                    }
                }
                else
                {
                    // If it's an input and the argument is an output...
                    if (Type == DeMIMOI_InputOutputType.INPUT && input_output.Type == DeMIMOI_InputOutputType.OUTPUT)
                    {
                        // Ok, connect this to the given output
                        ConnectedTo = input_output;

                        // If event signalling is required
                        if (Connected != null)
                        {
                            Connected(this, new DeMIMOI_ConnectionEventArgs(input_output, this));
                        }
                        // If event signalling for the output is required
                        if (input_output.Connected != null)
                        {
                            input_output.Connected(this, new DeMIMOI_ConnectionEventArgs(input_output, this));
                        }
                    }
                    else
                    {
                        // NOK, this makes non sense ! Trying to connect two outputs or two inputs...
                        throw new Exception("Can't connect two inputs or two outputs together ! Connect one output to an input !");
                    }
                }
            }
            else
            {
                // The input (or output) is not defined, so we unplug
                Unplug(this);
            }
        }

        /// <summary>
        /// Disconnects an input from the output it's connected to
        /// </summary>
        /// <param name="input">An input typed DeMIMOI_InputOutput object to disconnect</param>
        public static void Unplug(DeMIMOI_InputOutput input)
        {
            // If it's an input
            if (input.Type == DeMIMOI_InputOutputType.INPUT)
            {
                // If the input is connected to something
                if (input.ConnectedTo != null)
                {
                    DeMIMOI_ConnectionEventArgs eventArg = null;

                    // Before disconnecting, pick up the current value and set it to the input (i.e. once it has been disconnected, the input keeps the last value it had before disconnection)
                    input.input_output_value = input.ConnectedTo.Value;

                    // If event signalling is required
                    if (input.Disconnected != null)
                    {
                        // Prepare the event args
                        eventArg = new DeMIMOI_ConnectionEventArgs(input.ConnectedTo, input);
                    }

                    // Disconnect it
                    input.ConnectedTo = null;
                    // Reset the IgnoreConnection so that it will be taken into account for the new connection it may have in the future
                    input.IgnoreConnection = false;

                    // If event signalling is required
                    if (input.Disconnected != null)
                    {
                        input.Disconnected(input, eventArg);
                    }
                }
            }
            else
            {
                throw new Exception("Can't unplug an output !");
            }            
        }

        /// <summary>
        /// Makes a clone (deep clone) of the value
        /// </summary>
        /// <returns>The cloned value</returns>
        public object CloneValue()
        {
            object new_one = null;

            if (Value != null)
            {
                // Is it a value type ?
                if (Value.GetType().IsValueType)
                {
                    // Yes it is, so we don't need to spend time serializing/deserializing !!
                    new_one = Value;
                }
                else
                {
                    // It's a reference type, so then we have to spend some time serializing/deserializing to make a deep copy...
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, Value);
                        stream.Position = 0;
                        new_one = formatter.Deserialize(stream);
                    }
                }
            }

            return new_one;
        }



        /*internal class DeMIMOI_InputOutputConverter : TypeConverter
        {

            public override PropertyDescriptorCollection
            GetProperties(ITypeDescriptorContext context,
                     object value,
                     Attribute[] filter)
            {
                return TypeDescriptor.GetProperties(value, filter);
            }

            public override bool GetPropertiesSupported(
                     ITypeDescriptorContext context)
            {
                return true;
            }
        }*/

        internal class DeMIMOI_InputOutputConverter : ExpandableObjectConverter
        {

            public override bool CanConvertFrom(
                  ITypeDescriptorContext context, Type t)
            {

                /*if (t == typeof(double))
                {
                    return true;
                }*/
                return base.CanConvertFrom(context, t);
            }

            public override object ConvertFrom(
                  ITypeDescriptorContext context,
                  CultureInfo info,
                   object value) {

              return base.ConvertFrom(context, info, value);
           }

            public override object ConvertTo(
                     ITypeDescriptorContext context,
                     CultureInfo culture,
                     object value,
                     Type destType)
            {
                if (destType == typeof(string) && value is DeMIMOI_InputOutput)
                {
                    DeMIMOI_InputOutput p = (DeMIMOI_InputOutput)value;

                    return "" + p.Value;
                }
                return base.ConvertTo(context, culture, value, destType);
            }
        }
    }

    public delegate void DeMIMOI_ConnectionEventHandler(object sender, DeMIMOI_ConnectionEventArgs e);

    /// <summary>
    /// DeMIMOI Connection Event Arguments to describe a connection/disconnection event of a <see cref="DeMIMOI_InputOutput"/>
    /// </summary>
    public class DeMIMOI_ConnectionEventArgs : EventArgs
    {
        public DeMIMOI_ConnectionEventArgs(DeMIMOI_InputOutput from, DeMIMOI_InputOutput to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// Output from which the event has been thrown
        /// </summary>
        public DeMIMOI_InputOutput From
        {
            get;
            set;
        }

        /// <summary>
        /// Input from which the event has been thrown
        /// </summary>
        public DeMIMOI_InputOutput To
        {
            get;
            set;
        }
    }
}
