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
    /// Persistence class to manage model saving, loading to/from files
    /// </summary>
    public class DeMIMOI_Persistence
    {
        /// <summary>
        /// Save an object as an XML file
        /// </summary>
        /// <typeparam name="T">Type of any serializable object (including DeMIMOI)</typeparam>
        /// <param name="object_to_save">Object to save as XML</param>
        /// <param name="filename">Filename of the file to be saved</param>
        public static void SaveXml<T>(T object_to_save, string filename)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (StreamWriter wr = new StreamWriter(filename))
            {
                xs.Serialize(wr, object_to_save);
            }
        }

        /// <summary>
        /// Load an object from an XML file
        /// </summary>
        /// <typeparam name="T">Type of any serializable object (including DeMIMOI)</typeparam>
        /// <param name="filename">Filename of the file to load</param>
        /// <returns>The loaded object</returns>
        public static T LoadXml<T>(string filename)
        {
            object new_one;
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (StreamReader wr = new StreamReader(filename))
            {
                new_one = xs.Deserialize(wr);
            }

            return (T)new_one;
        }

        /// <summary>
        /// Save an object as a binary file
        /// </summary>
        /// <typeparam name="T">Type of any serializable object (including DeMIMOI)</typeparam>
        /// <param name="object_to_save">Object to save in the binary file</param>
        /// <param name="filename">Filename of the file to be saved</param>
        public static void Save<T>(T object_to_save, string filename)
        {
            using (StreamWriter wr = new StreamWriter(filename))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(wr.BaseStream, object_to_save);
            }
        }

        /// <summary>
        /// Load an object from a binary file
        /// </summary>
        /// <typeparam name="T">Type of any serializable object (including DeMIMOI)</typeparam>
        /// <param name="filename">Filename of the file to load</param>
        /// <returns>The loaded object</returns>
        public static T Load<T>(string filename)
        {
            object new_one;
            using (StreamReader wr = new StreamReader(filename))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                new_one = formatter.Deserialize(wr.BaseStream);
            }

            return (T)new_one;
        }
    }
}
