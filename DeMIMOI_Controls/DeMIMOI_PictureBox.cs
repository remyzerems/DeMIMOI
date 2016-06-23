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
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace DeMIMOI_Controls
{
    // Delegate to handle thread-safe update of the PictureBox
    delegate void UpdatePictureBoxCallback(int index, Bitmap bmp);

    /// <summary>
    /// DeMIMOI PictureBox class to manage images
    /// </summary>
    [Serializable]
    public class DeMIMOI_PictureBox : DeMIMOI
    {
        static int current_id = 0;  // Current ID to allocate to a new DeMIMOI_PictureBox object
        /// <summary>
        /// Allocates a new DeMIMOI_PictureBox ID
        /// </summary>
        /// <returns>The new ID</returns>
        private static int AllocNewId()
        {
            return current_id++;
        }

       public Graphics[] graphics = null;
        //Bitmap[] bitmaps = null;

        void Initialize(int inputCount, PictureBox[] pictureBoxes)
        {
            ID = AllocNewId();
            Name = GetType().Name + ID;

            // If no chart has been specified, use the internal definition
            if (pictureBoxes == null)
            {
                // Initialize internal picture box controls with default values
                PictureBoxes = new PictureBox[inputCount];
                for (int i = 0; i < PictureBoxes.Length; i++)
                {
                    PictureBoxes[i].Top = 0;
                    PictureBoxes[i].Left = 0;
                    PictureBoxes[i].Width = 256;
                    PictureBoxes[i].Height = 128;
                    PictureBoxes[i].Name = "pictureBox" + (i + 1);
                }
            }
            else
            {
                // Picture boxes have been specified by the user, so use these ones
                PictureBoxes = pictureBoxes;
            }

            // Create graphics to make some graphical operations
            graphics = new Graphics[PictureBoxes.Length];
            for (int i = 0; i < PictureBoxes.Length; i++)
            {
                if (PictureBoxes[i] != null)
                {
                    graphics[i] = PictureBoxes[i].CreateGraphics();
                }
            }

            #region I/O Renaming
            for (int i = 0; i < Inputs.Count; i++)
            {
                for (int j = 0; j < Inputs[i].Count; j++)
                {
                    Inputs[i][j].Name = PictureBoxes[i].Name;
                }
            }
            #endregion
        }

        public DeMIMOI_PictureBox(params PictureBox[] pictureBoxes)
            : base(new DeMIMOI_Port(Enumerable.Repeat(1, pictureBoxes.Length).ToArray()), null)
        {
            Initialize(InputCount, pictureBoxes);
        }

        public DeMIMOI_PictureBox()
            : base(new DeMIMOI_Port(1), null)
        {
            Initialize(InputCount, null);
        }

        public PictureBox[] PictureBoxes
        {
            get;
            set;
        }

        /// <summary>
        /// Updates thread-safely the chart
        /// </summary>
        /// <param name="index">Index of the picturebox to update</param>
        /// <param name="bmp">Bitmap to display</param>
        void UpdatePictureBox(int index, Bitmap bmp)
        {
            if (this.PictureBoxes[index].InvokeRequired)
            {
                if (PictureBoxes[index].Parent != null)
                {
                    UpdatePictureBoxCallback d = new UpdatePictureBoxCallback(UpdatePictureBox);
                    if (PictureBoxes[index].Parent.Disposing == false)
                    {
                        PictureBoxes[index].Parent.Invoke(d, new object[] { index, bmp });
                    }
                }
            }
            else
            {
                try
                {
                    graphics[index].DrawImage(bmp, PictureBoxes[index].DisplayRectangle);
                }
                catch (ExternalException extExc)
                {
                    // Ignore this error for a start...
                }
            }
        }

        protected override void UpdateInnerSystem(ref List<DeMIMOI_InputOutput> new_outputs)
        {
            for (int i = 0; i < PictureBoxes.Length; i++)
            {
                if (PictureBoxes[i] != null)
                {
                    if (Inputs[i][0] != null)
                    {
                        if (Inputs[i][0].Value != null)
                        {
                            Bitmap img = (Bitmap)Inputs[i][0].Value;
                            //graphics[i].DrawImage(img, PictureBoxes[i].DisplayRectangle);
                            UpdatePictureBox(i, img);
                        }
                    }
                }
            }
        }
    }
}
