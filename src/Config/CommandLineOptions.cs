using Mono.Options;
using System;
using System.IO;
using System.Linq;

namespace Document.Generator.Config
{
    public class CommandLineOptions : Settings
    {
        private readonly OptionSet parser;
        public bool showUsage;

        public CommandLineOptions()
        {
            parser = new OptionSet
            {
                "Usage: dg [OPTIONS] <path-to-assembly>+",
                "",
                "Options:",
                { "m|markdown",   "Generates documents in markdown (default)",   md => Markdown.Enabled = (md != null) },
                { "x|html",       "Generates documents in HTML",                 html => Html.Enabled = (html != null) },
                { "s|style=",     "Custom stylesheet URL of HTML documents",     style => Html.Stylesheet = style },
                { "d|outdir=",    "Base output directory (default cwd)",         folder => OutputFolder = Path.GetFullPath(folder) },
                { "h|help",       "Shows this message and exits",                help => showUsage = (help != null) },
                { "<>",           "Path to assembly file",                       assembly => AssemblyFiles.Add(Path.GetFullPath(assembly)) },
            };
        }

        public void Initialize(string[] args)
        {
            parser.Parse(args);
            if (showUsage)
                throw new OperationCanceledException();
            if (!AssemblyFiles.Any())
                throw new OptionException("No assembly file is specified", null);
            if (!OutputFormats.Any())
                throw new OptionException("No document format is specified", null);
        }

        public string GetUsageHelp()
        {
            using (var textWriter = new StringWriter())
            {
                parser.WriteOptionDescriptions(textWriter);
                return textWriter.ToString();
            }
        }
    }
}
