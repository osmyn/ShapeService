using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDS.Shapes
{
    public class Circle : Shape
    {
        public double Radius { get; set; }

        public override double Area()
        {
            return Math.PI * Radius * Radius;
        }

        public override string ToString()
        {
            return "Circle";
        }
    }
}
