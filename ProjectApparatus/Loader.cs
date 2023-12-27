using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ProjectApparatus
{
	public class Loader
	{
		public static void Init()
        {
            Loader.Load = new GameObject();
			Loader.Load.AddComponent<Hacks>();
			UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
			AppDomain.CurrentDomain.AssemblyResolve += Loader.CurrentDomain_AssemblyResolve;
        }

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			return Loader.LoadAssem();
		}

		public static Assembly LoadAssem()
		{
			string name = "ProjectApparatus.Resources.0Harmony.dll";
			Assembly result;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
			{
				byte[] array = new byte[(int)manifestResourceStream.Length];
				manifestResourceStream.Read(array, 0, (int)manifestResourceStream.Length);
				result = Assembly.Load(array);
			}
			return result;
		}

		public static void Unload()
		{
			UnityEngine.Object.Destroy(Loader.Load);
		}

		private static GameObject Load;
	}
}
