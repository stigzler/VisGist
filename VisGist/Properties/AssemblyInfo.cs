﻿using System.Reflection;
using System.Runtime.InteropServices;
using VisGist;

[assembly: AssemblyTitle(Vsix.Name)]
[assembly: AssemblyDescription(Vsix.Description)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(Vsix.Author)]
[assembly: AssemblyProduct(Vsix.Name)]
[assembly: AssemblyCopyright(Vsix.Author)]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: AssemblyVersion(Vsix.Version)]
[assembly: AssemblyFileVersion(Vsix.Version)]

// My additions
[assembly: ProvideCodeBase(AssemblyName = "ICSharpCode.AvalonEdit")]

namespace System.Runtime.CompilerServices
{
    public class IsExternalInit
    { }
}