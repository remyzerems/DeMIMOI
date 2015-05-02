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
using DeMIMOI_Models;
using System.Windows.Forms.DataVisualization.Charting;
using System;

namespace DeMIMOI_Controls
{
    /// <summary>
    /// DeMIMOI Chart class to manage charts
    /// </summary>
    [Serializable]
    public class DeMIMOI_Chart :DeMIMOI
    {
        static int current_id = 0;  // Current ID to allocate to a new DeMIMOI_Chart object
        /// <summary>
        /// Allocates a new DeMIMOI_Chart ID
        /// </summary>
        /// <returns>The new ID</returns>
        private static int AllocNewId()
        {
            return current_id++;
        }

        /// <summary>
        /// Initialize a DeMIMOI Chart
        /// </summary>
        /// <param name="input_port">The input port that represents the DeMIMOI Chart inputs (i.e. series to draw)</param>
        /// <param name="chart">The chart control to use to draw the series in (can be user defined or if null, the internal one will be used)</param>
        void Initialize(DeMIMOI_Port input_port, Chart chart)
        {
            ID = AllocNewId();
            Name = GetType().Name + ID;

            // If no chart has been specified, use the internal definition
            if (chart == null)
            {
                // Initialize the internal Chart control with default values
                Chart = new Chart();
                Chart.Top = 0;
                Chart.Left = 0;
                Chart.Width = 256;
                Chart.Height = 128;
            }
            else
            {
                // A chart has been specified by the user, so use this one
                Chart = chart;
            }

            // Delete any series that would already be present
            Chart.Series.Clear();

            // Delete any chart area
            Chart.ChartAreas.Clear();

            // Add as many series that we have inputs
            for (int i = 0; i < input_port.IODelayCount.Length; i++)
            {
                // Create a chart area for each input
                Chart.ChartAreas.Add("zone" + i);

                // For each delayed input, create a series on this zone
                for (int j = 0; j < input_port.IODelayCount[i]; j++)
                {
                    string series_name = "Series" + i + "_" + j;
                    Chart.Series.Add(series_name);
                    Chart.Series[series_name].ChartType = SeriesChartType.Line;
                    Chart.Series[series_name].ChartArea = "zone" + i;
                }
            }
            // Put the legend text on the bottom of the chart
            Chart.Legends[0].Docking = Docking.Bottom;

            // Set the timestep unit to 1 (i.e. the default unit is steps)
            TimestepUnit = 1.0;

            // Subscribe to the connection event to be notified when an input of the model is connected
            Connected += new DeMIMOI_ConnectionEventHandler(DeMIMOI_Chart_Connected);
        }

        void DeMIMOI_Chart_Connected(object sender, DeMIMOI_ConnectionEventArgs e)
        {
            int seriesIndex = 0;
            // When an input is connected...
            for (int i = 0; i < Inputs.Count; i++)
            {
                for (int j = 0; j < Inputs[i].Count; j++)
                {
                    // Search the input index that has been connected
                    if (e.To == Inputs[i][j])
                    {
                        // Set the series name to the same name as the output at has been connected to
                        string series_name = e.From.Parent.Name + ".";
                        if (e.From.Name != "")
                        {
                            series_name += e.From.Name;
                        }
                        else
                        {
                            int[] indexes = DeMIMOI_InputOutput.GetInputOutputIndexes(e.From);
                            series_name += "o" + indexes[0] + "(t" + (indexes[1] == 0 ? "" : "-" + indexes[1]);
                        }

                        Chart.Series[seriesIndex].Name = series_name;
                        break;
                    }
                    seriesIndex++;
                }
            }
        }

        public DeMIMOI_Chart(DeMIMOI_Port input_port)
            :base(input_port, null)
        {
            Initialize(input_port, null);
        }

        public DeMIMOI_Chart(DeMIMOI_Port input_port, Chart chart)
            : base(input_port, null)
        {
            Initialize(input_port, chart);
        }

        public Chart Chart
        {
            get;
            set;
        }

        public double TimestepUnit
        {
            get;
            set;
        }

        protected override void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs)
        {
            int seriesIndex = 0;
            
            for (int i = 0; i < Inputs.Count; i++)
            {
                for (int j = 0; j < Inputs[i].Count; j++)
                {
                    Chart.Series[seriesIndex].Points.AddXY(UpdateCount * TimestepUnit, Convert.ToDouble(Inputs[i][j].Value));
                    seriesIndex++;
                }
            }
        }
    }
}
