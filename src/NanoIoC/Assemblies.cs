using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NanoIoC
{
	internal static class Assemblies
	{
		public static IEnumerable<Assembly> AllFromApplicationBaseDirectory(Predicate<Assembly> filter)
		{
			var assemblies = new List<Assembly>();

			var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

			if (Directory.Exists(baseDirectory))
				assemblies.AddRange(AllFromPath(baseDirectory, filter));

			var binPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath;
			if (Directory.Exists(binPath))
				assemblies.AddRange(AllFromPath(binPath, filter));

			return assemblies;
		}

		public static IEnumerable<Assembly> AllFromPath(string path, Predicate<Assembly> filter)
		{
			var assemblyPaths = Directory.GetFiles(path).Where(file =>
					   Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase)
					   || Path.GetExtension(file).Equals(".dll", StringComparison.OrdinalIgnoreCase))
					   .ToArray();

			foreach (var assemblyPath in assemblyPaths)
			{
				Assembly assembly = null;

				try
				{
					assembly = Assembly.LoadFrom(assemblyPath);
				}
				catch (BadImageFormatException)
				{
					
				}
				catch (Exception e)
				{
					Console.WriteLine("==============================");
					Console.WriteLine(e.Message);
					Console.WriteLine("==============================");

					throw e;
				}

				if (assembly != null && filter(assembly))
					yield return assembly;
			}
		}
	}
}