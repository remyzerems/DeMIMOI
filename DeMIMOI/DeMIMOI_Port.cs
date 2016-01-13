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

using System.Collections.Generic;

namespace DeMIMOI_Models
{
    public class DeMIMOI_Port
    {
        public DeMIMOI_Port(params int[] io_delay_count)
        {
            IODelayCount = io_delay_count;
        }

        public DeMIMOI_Port(List<List<DeMIMOI_InputOutput>> model_input_output)
        {
            if (model_input_output != null)
            {
                IODelayCount = new int[model_input_output.Count];
                for (int i = 0; i < model_input_output.Count; i++)
                {
                    IODelayCount[i] = model_input_output[i].Count;
                }
            }
            else
            {
                IODelayCount = null;
            }
        }

        public int[] IODelayCount
        {
            get;
            set;
        }

        public DeMIMOI_Port Add(params int[] new_io_delay_count)
        {
            int curLength = 0;
            if (IODelayCount != null)
            {
                curLength = IODelayCount.Length;
            }
            int[] newIODelayCount = new int[curLength + new_io_delay_count.Length];
            for (int i = 0; i < curLength; i++)
            {
                newIODelayCount[i] = IODelayCount[i];
            }
            for (int i = 0; i < new_io_delay_count.Length; i++)
            {
                newIODelayCount[curLength + i] = new_io_delay_count[i];
            }

            return new DeMIMOI_Port(newIODelayCount);
        }

        public DeMIMOI_Port Add(DeMIMOI_Port new_input_output_port)
        {
            return Add(new_input_output_port.IODelayCount);
        }

    }
}
