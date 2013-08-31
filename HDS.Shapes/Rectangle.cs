using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDS.Shapes
{
    public class Rectangle : Shape
    {
        public double Length { get; set; }
        public double Width { get; set; }

        public override double Area()
        {
            return Length * Width;
        }

        public override string ToString()
        {
            return "Rectangle";
        }
    }
}
