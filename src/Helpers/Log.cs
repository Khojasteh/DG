using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Document.Generator.Helpers
{
    public static class Log
    {
        private static readonly HashSet<string> MissingDocs = new HashSet<string>();

        private static void Write(TraceEventType type, string message)
        {
            Console.WriteLine($"dg: {type} -> {message}");
        }

        public static void Error(Exception error)
        {
            Write(TraceEventType.Error, error.GetBaseException().Message);
        }

        public static void WarnMisisngDoc(ICRef target)
        {
            if (MissingDocs.Add(target.CRef))
                Write(TraceEventType.Warning, $"{Utils.FormatCRef(target.CRef)} has no documentation");
        }

        public static void WarnMisisngTypeParameterDoc(ICRef target, string typeParameterName)
        {
            if (!MissingDocs.Contains(target.CRef) && MissingDocs.Add(target.CRef + '*' + typeParameterName))
                Write(TraceEventType.Warning, $"Type parameter '{typeParameterName}' of {Utils.FormatCRef(target.CRef)} has no documentation");
        }

        public static void WarnMisisngParameterDoc(ICRef target, string parameterName)
        {
            if (!MissingDocs.Contains(target.CRef) && MissingDocs.Add(target.CRef + '^' + parameterName))
                Write(TraceEventType.Warning, $"Parameter '{parameterName}' of {Utils.FormatCRef(target.CRef)} has no documentation");
        }

        public static void WarnMisisngReturnDoc(ICRef target)
        {
            if (!MissingDocs.Contains(target.CRef) && MissingDocs.Add(target.CRef + '!'))
                Trace.TraceWarning($"Return value of {Utils.FormatCRef(target.CRef)} has no documentation");
        }
    }
}
