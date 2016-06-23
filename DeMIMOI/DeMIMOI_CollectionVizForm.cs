// Delayed Multiple Input Multiple Output Interface (DeMIMOI) Library
//
// Copyright © Rémy Dispagne, 2016
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
using System.Windows.Forms;
using GraphVizDotNetLib;

namespace DeMIMOI_Models
{
    /// <summary>
    /// Form that shows DeMIMOI collections graphical structure
    /// </summary>
    public partial class DeMIMOI_CollectionVizForm : Form
    {
        // GraphViz renderer object
        GraphVizRenderer gv;

        public DeMIMOI_CollectionVizForm()
        {
            InitializeComponent();
        }

        private void DeMIMOI_VizForm_Load(object sender, EventArgs e)
        {
            // Setup the panel to auto resize to fit the form
            panel1.AutoScroll = true;
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

            // Set the combobox data source
            comboBox1.DataSource = DeMIMOI_Collection.Instances;
            comboBox1.DisplayMember = "Name";
        }

        void UpdateGraph()
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                // Create the renderer
                gv = new GraphVizRenderer();
                // Generate the graph based on the selected collection graphviz code
                pictureBox1.Image = gv.DrawGraphFromDotCode(DeMIMOI_Collection.Instances[comboBox1.SelectedIndex].GraphVizFullCode());
                // Dispose the renderer
                gv.Dispose();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGraph();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateGraph();
        }
    }
}
