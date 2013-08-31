using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDS.Shapes
{
    public class Triangle : Shape
    {
        public double Base { get; set; }
        public double Height { get; set; }

        public override double Area()
        {
            return 0.5D * Base * Height;
        }

        public override string ToString()
        {
            return "Triangle";
        }
    }
}
