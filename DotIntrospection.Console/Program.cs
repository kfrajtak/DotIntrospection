using System;
using System.Reflection;

namespace DotIntrospection.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string solutionUrl = @"C:\Temp\mvp\DotIntrospection\DotIntrospection.sln";
            string outputDir = @"";

            try
            {
                bool success = new Class1().CompileSolution(solutionUrl, outputDir);

                if (success)
                {
                    System.Console.WriteLine("Compilation completed successfully.");
                    System.Console.WriteLine("Output directory:");
                    System.Console.WriteLine(outputDir);
                }
                else
                {
                    System.Console.WriteLine("Compilation failed.");
                }
            }
            catch (ReflectionTypeLoadException e)
            {
                System.Console.WriteLine(e.LoaderExceptions);
            }
            catch(AggregateException a)
            {
                System.Console.WriteLine(a.InnerException);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }
    }
}
