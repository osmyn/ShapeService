using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HDS.Shapes.Service
{
    public class Transform
    {
        // Create a logger for use in this class
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static IEnumerable<Shape> CreateShapes(IEnumerable<string> input)
        {

            foreach (var line in input)
            {
                var properties = line.Split(',');
         
                switch (properties[0].Trim().ToLower())
                {
                    case "circle":
                        yield return new Circle
                            {
                                Radius = double.Parse(properties[1].Trim()),
                                Color = properties[2].Trim()
                            };
                        break;
                    case "triangle":
                        yield return new Triangle
                        {
                            Base = double.Parse(properties[1].Trim()),
                            Height = double.Parse(properties[2].Trim()),
                            Color = properties[3].Trim()
                        };
                        break;
                    case "rectangle":
                        yield return new Rectangle()
                        {
                            Length = double.Parse(properties[1].Trim()),
                            Width = double.Parse(properties[2].Trim()),
                            Color = properties[3].Trim()
                        };
                        break;
                    case "square":
                        yield return new Square()
                        {
                            Length = double.Parse(properties[1].Trim()),
                            Color = properties[2].Trim()
                        };
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Input shape '{0}' is not supported.",
                                                                      properties[0]));
                }
            }

        }

        public static IEnumerable<String> CreateOutput(IEnumerable<Shape> shapes)
        {
            return shapes.Select(shape => string.Format("{0}, {1}, {2}",
                                                        shape.ToString(),
                                                        Math.Round(shape.Area(), 2),
                                                        shape.Color));
        }
    }
}
