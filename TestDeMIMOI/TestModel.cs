using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeMIMOI_Models;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TestDeMIMOI
{
    [Serializable]
    public class TestModel:DeMIMOI
    {
        public TestModel()
            : base(2, 3, 2, 3)
        {
            Name = "TestModel_" + ID;
        }

        protected override void UpdateInnerSystem(ref DeMIMOI_InputOutput[] new_outputs)
        {
            if (Inputs[0][0].Value != null)
            {
                new_outputs[0].Value = (int)Inputs[0][0].Value * 2;
                new_outputs[1].Value = (int)Inputs[0][1].Value * 4;
            }
        }
    }
}
