using System;

namespace NanoIoC
{
	/// <summary>
	/// Add this attribute to an assembly to make sure
	/// it is included in NanoIoC's assembly scanning.
	/// </summary>
	/// <example>
	/// Apply the attribute, typically in AssemblyInfo.(cs|fs|vb), as follows:
	/// <code>[assembly: IncludeInNanoIoCAssemblyScanning]</code>
	/// </example>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class IncludeInNanoIoCAssemblyScanningAttribute : Attribute
	{
	}
}