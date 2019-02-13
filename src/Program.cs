using Document.Generator.Config;
using Document.Generator.Helpers;
using Document.Generator.Models;
using System;
using System.IO;

namespace Document.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new CommandLineOptions();
            try
            {
                settings.Initialize(args);
                GenerateDocuments(settings);
            }
            catch (Mono.Options.OptionException e)
            {
                Console.Write("dg: ");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine(settings.GetUsageHelp());
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Converts XML documentations of specified assembly files to markdown and/or HTML format");
                Console.WriteLine();
                Console.WriteLine(settings.GetUsageHelp());
            }
        }

        private static void GenerateDocuments(Settings settings)
        {
            foreach (string assemblyFile in settings.AssemblyFiles)
            {
                try
                {
                    var input = InputContext.Create(assemblyFile);
                    foreach (var format in settings.OutputFormats)
                    {
                        var outputFolder = settings.FlattenFolder ? settings.OutputFolder : Path.Combine(settings.OutputFolder, format.Subfolder);
                        var output = new OutputContext(input, format, settings.Language, outputFolder, settings.IndexName);
                        output.Compose();
                    }
                }
                catch (Exception error)
                {
                    Log.Error(error);
                }
            }
        }
    }
}
