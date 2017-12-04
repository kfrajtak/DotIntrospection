using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.MSBuild;

namespace DotIntrospection
{
    public class Walker : CSharpSyntaxWalker
    {
        public override void Visit(SyntaxNode node)
        {
            base.Visit(node);
        }
    }

    public class Class1
    {
        public bool CompileSolution(string solutionUrl, string outputDir)
        {
            bool success = true;

            // [Failure] Cannot open project '*.csproj' because the language 'C#' is not supported.
            // https://stackoverflow.com/questions/30601934/using-an-adhocworkspace-results-in-the-language-c-is-not-supported
            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);

            var workspace = MSBuildWorkspace.Create();
            var solution = workspace.OpenSolutionAsync(solutionUrl).Result;
            if (workspace.Diagnostics.Any())
            {
                Console.WriteLine(string.Join(Environment.NewLine, workspace.Diagnostics));
                return false;
            }

            var projectGraph = solution.GetProjectDependencyGraph();
            var projects = projectGraph.GetTopologicallySortedProjects();
            if (!projects.Any())
            {
                Console.WriteLine($"{projects.Count()} projects :(");
                return false;
            }

            foreach (var projectId in projects)
            {
                Compilation projectCompilation = solution.GetProject(projectId).GetCompilationAsync().Result;
                if (null != projectCompilation && !string.IsNullOrEmpty(projectCompilation.AssemblyName))
                {
                    using (var stream = new MemoryStream())
                    {
                        EmitResult result = projectCompilation.Emit(stream);
                        if (result.Success)
                        {
                            string fileName = string.Format("{0}.dll", projectCompilation.AssemblyName);

                            using (FileStream file = File.Create(outputDir + '\\' + fileName))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.CopyTo(file);
                            }
                        }
                        else
                        {
                            success = false;
                        }
                    }
                }
                else
                {
                    success = false;
                }
            }

            return success;
        }
    }
}
