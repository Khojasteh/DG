using System;

namespace Document.Generator.Formatters
{
    [Flags]
    public enum TextStyles
    {
        None = 0,
        Teletype = 1,
        Strong = 2,
        Emphasize = 4
    }
}
