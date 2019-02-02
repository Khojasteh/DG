# DG

DG is a .NET Core console application for converting the XML-based documentation of .NET code into a comprehensive API help. DG can generate documents in both Markdown and HTML formats.

```plain
Usage: dg [OPTIONS] <path-to-assembly>+

Options:
  -m, --markdown             Generates documents in markdown (default)
  -x, --html                 Generates documents in HTML
  -s, --style=VALUE          Custom stylesheet URL for HTML documents
  -d, --outdir=VALUE         Base output directory
  -h, --help                 Shows this message and exit
```

## Supported Tags

DG supports all standard and recommented tags for documentation comments.

- [`<c>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/code-inline)
- [`<code>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/code)
- [`<example>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/example)
- [`<exception>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/exception)
- [`<include>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/include)
- [`<list>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/list)
- [`<para>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/para)
- [`<param>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/param)
- [`<paramref>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/paramref)
- [`<permission>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/permission)
- [`<remarks>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/remarks)
- [`<returns>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/returns)
- [`<see>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/see)
- [`<seealso>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/seealso)
- [`<summary>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/summary)
- [`<typeparam>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/typeparam)
- [`<typeparamref>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/typeparamref)
- [`<value>`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/xmldoc/value)

Additionally, it recognizes the following [Sandcastle](https://github.com/EWSoftware/SHFB)'s custom tags:

- [`<event>`](https://ewsoftware.github.io/XMLCommentsGuide/html/81bf7ad3-45dc-452f-90d5-87ce2494a182.htm)
- [`<note>`](https://ewsoftware.github.io/XMLCommentsGuide/html/4302a60f-e4f4-4b8d-a451-5f453c4ebd46.htm)
- [`<overloads>`](https://ewsoftware.github.io/XMLCommentsGuide/html/5b11b235-2b6c-4dfc-86b0-2e7dd98f2716.htm)
- [`<revisionHistory>`](https://ewsoftware.github.io/XMLCommentsGuide/html/2a973959-9c9a-4b3b-abcb-48bb30382400.htm)
- [`<threadsafety>`](https://ewsoftware.github.io/XMLCommentsGuide/html/fb4625cb-52d0-428e-9c7c-7a0d88e1b692.htm)
