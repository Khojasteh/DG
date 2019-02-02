using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Document.Generator.Helpers
{
    public static class Log
    {
        private static readonly HashSet<string> MissingDocs = new HashSet<string>();

        public static void Error(Exception error)
        {
            Trace.TraceError(error.GetBaseException().Message);
        }

        public static void WarnMisisngDoc(ICRef target)
        {
            if (MissingDocs.Add(target.CRef))
                Trace.TraceWarning($"{Utils.FormatCRef(target.CRef)} has no documentation");
        }

        public static void WarnMisisngTypeParameterDoc(ICRef target, string typeParameterName)
        {
            if (!MissingDocs.Contains(target.CRef) && MissingDocs.Add(target.CRef + '*' + typeParameterName))
                Trace.TraceWarning($"Type parameter '{typeParameterName}' of {Utils.FormatCRef(target.CRef)} has no documentation");
        }

        public static void WarnMisisngParameterDoc(ICRef target, string parameterName)
        {
            if (!MissingDocs.Contains(target.CRef) && MissingDocs.Add(target.CRef + '^' + parameterName))
                Trace.TraceWarning($"Parameter '{parameterName}' of {Utils.FormatCRef(target.CRef)} has no documentation");
        }

        public static void WarnMisisngReturnDoc(ICRef target)
        {
            if (!MissingDocs.Contains(target.CRef) && MissingDocs.Add(target.CRef + '!'))
                Trace.TraceWarning($"Return value of {Utils.FormatCRef(target.CRef)} has no documentation");
        }
    }
}
