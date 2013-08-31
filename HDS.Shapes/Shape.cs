using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDS.Shapes
{
    public abstract class Shape
    {
        public string Color { get; set; }
        
        public abstract double Area();
    }
}
