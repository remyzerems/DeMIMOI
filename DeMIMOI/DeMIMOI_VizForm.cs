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
using System.Linq;
using System.Windows.Forms;
using GraphVizDotNetLib;

namespace DeMIMOI_Models
{

    /// <summary>
    /// Form that shows DeMIMOI object information
    /// </summary>
    public partial class DeMIMOI_VizForm : Form
    {
        enum DataSourceType
        {
            AllDeMIMOIs,
            DeMIMOIsInCollections
        }

        DataSourceType currentDataSource;
        dynamic comboBoxQuery;

        GraphVizRenderer gv;

        public DeMIMOI_VizForm()
        {
            InitializeComponent();
        }

        private void DeMIMOI_VizForm_Load(object sender, EventArgs e)
        {
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            currentDataSource = DataSourceType.AllDeMIMOIs;
            SelectDataSource(currentDataSource);
        }

        private void SelectDataSource(DataSourceType dataSourceType)
        {
            // Create data sources based on what the user selected
            switch (dataSourceType)
            {
                case DataSourceType.DeMIMOIsInCollections :
                    comboBoxQuery =
                        (from c in DeMIMOI_Collection.Instances
                            from d in DeMIMOI.Instances
                                   where c.Contains(d)
                                    select new {d, FullName = c.Name + "." + d.Name }).ToList();
                break;
                default :
                    comboBoxQuery =
                        (from d in DeMIMOI.Instances
                         select new { d, FullName = d.Name }).ToList();
                break;
            }

            comboBox1.DataSource = comboBoxQuery;
            comboBox1.DisplayMember = "FullName";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex >= 0)
            {
                gv = new GraphVizRenderer();
                pictureBox1.Image = gv.DrawGraphFromDotCode(comboBoxQuery[comboBox1.SelectedIndex].d.GraphVizFullCode());
                gv.Dispose();

                propertyGrid1.SelectedObject = comboBoxQuery[comboBox1.SelectedIndex].d;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            propertyGrid1.Refresh();
        }

        /// <summary>
        /// Calculates the drop down width for automatic size display
        /// </summary>
        /// <param name="myCombo">Combobox object</param>
        /// <returns>The size in pixels the drop down should be</returns>
        /// <see cref="http://stackoverflow.com/questions/4842160/auto-width-of-comboboxs-content"/>
        int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0, temp = 0;
            foreach (var obj in myCombo.Items)
            {
                temp = TextRenderer.MeasureText(obj.ToString(), myCombo.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            return maxWidth;
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            comboBox1.DropDownWidth = DropDownWidth(comboBox1);
        }

        private void UpdateRadioChoice()
        {
            // Select what to display in the combo box
            if (radioButton1.Checked == true)
            {
                currentDataSource = DataSourceType.AllDeMIMOIs;
            }
            else
            {
                currentDataSource = DataSourceType.DeMIMOIsInCollections;
            }

            SelectDataSource(currentDataSource);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRadioChoice();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRadioChoice();
        }
    }
}
