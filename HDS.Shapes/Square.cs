using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDS.Shapes
{
    public class Square : Rectangle
    {
        public override double Area()
        {
            return Length * Length;
        }

        public override string ToString()
        {
            return "Square";
        }
    }
}
