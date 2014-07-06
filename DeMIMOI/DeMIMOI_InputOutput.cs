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

        // Inner own Input/Output value
        object input_output_value;

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


        /// <summary>
        /// DeMIMOI_InputOutput ID
        /// </summary>
        public int ID
        {
            get;
            set;
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

        /// <summary>
        /// DeMIMOI object on which this DeMIMOI_InputOutput is included and used
        /// </summary>
        [XmlIgnore]
        public DeMIMOI Parent
        {
            get;
            protected set;
        }

        /// <summary>
        /// Output DeMIMOI_InputOutput to which an Input DeMIMOI_InputOutput is connected
        /// </summary>
        [XmlIgnore]
        public DeMIMOI_InputOutput ConnectedTo
        {
            get;
            protected set;
        }

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
        public void ConnectTo(ref DeMIMOI_InputOutput input_output)
        {
            // If it's an output and the argument is an input...
            if (Type == DeMIMOI_InputOutputType.OUTPUT && input_output.Type == DeMIMOI_InputOutputType.INPUT)
            {
                // Ok, connect the input to this output
                input_output.ConnectedTo = this;
            } else {
                // If it's an input and the argument is an output...
                if (Type == DeMIMOI_InputOutputType.INPUT && input_output.Type == DeMIMOI_InputOutputType.OUTPUT)
                {
                    // Ok, connect this to the given output
                    ConnectedTo = input_output;
                }
                else
                {
                    // NOK, this makes non sense ! Trying to connect two outputs or two inputs...
                    throw new Exception("Can't connect two inputs or two outputs together ! Connect one output to an input !");
                }
            }
        }

        /// <summary>
        /// Disconnects an input from the output it's connected to
        /// </summary>
        /// <param name="input">An input typed DeMIMOI_InputOutput object to disconnect</param>
        public static void Unplug(ref DeMIMOI_InputOutput input)
        {
            // If it's an input
            if (input.Type == DeMIMOI_InputOutputType.INPUT)
            {
                // Disconnect it
                input.ConnectedTo = null;
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
    }
}
