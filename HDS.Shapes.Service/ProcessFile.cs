using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using HDS.FileHandler;

namespace HDS.Shapes.Service
{
    public static class ProcessFile
    {
        // Create a logger for use in this class
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void ProcessInput(string fullPath)
        {
            IEnumerable<string> content = GetContent(fullPath);
            if (content == null) return;

            //Validate it has data
            if (!content.Any())
            {
                //No results, 
                if(Log.IsInfoEnabled)
                    Log.InfoFormat("File was empty: '{0}'", Path.GetFileName(fullPath));

                //Move file
                try
                {
                    FileUtil.MoveFile(fullPath,
                                      Path.Combine(ConfigurationManager.AppSettings[Constants.keyShapesWatchFolder], "Processed"),
                                      5);
                    if (Log.IsInfoEnabled)
                        Log.InfoFormat("File moved to processed folder: '{0}'", Path.GetFileName(fullPath));
                }
                catch (Exception ex)
                {
                    Log.Error(
                        "Moving input file to processed folder failed! Please move it manually to processed folder.",
                        ex);
                    return;
                }
                //exit
                return;
            }

           
            //process file here
            IEnumerable<Shape> shapes = Transform.CreateShapes(content).OrderBy(s=>s.Area()).ThenBy(s=>s.Color);
            IEnumerable<String> outputAll = Transform.CreateOutput(shapes);
            IEnumerable<String> outputTriangles = Transform.CreateOutput(shapes.Where(s => s.ToString().Equals("Triangle")));
            IEnumerable<String> outputPurple = Transform.CreateOutput(shapes.Where(s => s.Color.ToLower().Equals("purple")));
            

            //Drop file
            DropTransformedFile(fullPath, outputAll, "All");
            DropTransformedFile(fullPath, outputTriangles, "Triangles");
            DropTransformedFile(fullPath, outputPurple, "Purple");

        }

        private static IEnumerable<string> GetContent(string fullPath)
        {
            IEnumerable<string> content = new List<string>();
            //Read the file
            try
            {
                content = FileUtil.ReadFile(fullPath, System.Text.Encoding.ASCII, 5).ToList();

                if (Log.IsInfoEnabled)
                    Log.InfoFormat("File contents read: '{0}' lines", content.Count());
            }
            catch (Exception ex)
            {
                Log.Error("Reading input file failed! Look for file in the input folder.", ex);
                return null;
            }
            return content;
        }

        private static void DropTransformedFile(string fullPath, IEnumerable<string> transform, string name)
        {
            //Drop the transform file
            try
            {
                var dropName = Path.Combine(ConfigurationManager.AppSettings[Constants.keyShapesDropFolder],
                                            DateTime.Now.ToString("yyyyMMdd") + "_" + name + ".txt");
                FileUtil.WriteFile(dropName,
                                   transform,
                                   System.Text.Encoding.ASCII);

                if (Log.IsInfoEnabled)
                    Log.InfoFormat("File processed and dropped: '{0}'", dropName);
            }
            catch (Exception ex)
            {
                Log.Error("Input file transformed, but dropping transformed file failed! Look for file in input folder.", ex);
                return;
            }

            //finished
            try
            {
                FileUtil.MoveFile(fullPath,
                                  Path.Combine(ConfigurationManager.AppSettings[Constants.keyShapesWatchFolder], "Processed"),
                                  5);
                if (Log.IsInfoEnabled)
                    Log.InfoFormat("File moved to processed folder: '{0}'", Path.GetFileName(fullPath));
            }
            catch (Exception ex)
            {
                Log.Error(
                    "Input file transformed and dropped, but moving it to processed folder failed! Please move it manually to processed folder.",
                    ex);
                return;
            }
        }
    }
}
