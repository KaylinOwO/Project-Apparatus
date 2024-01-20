using Mono.Cecil;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ProjectApparatus
{
    public class Loader
    {
        private static GameObject Hack;
        private static GameObject Thirdperson;

        public static void Init()
        {
            Hack = new GameObject();
            Hack.AddComponent<Hacks>();
            UnityEngine.Object.DontDestroyOnLoad(Hack);

            Thirdperson = new GameObject();
            Thirdperson.AddComponent<Features.Thirdperson>();
            UnityEngine.Object.DontDestroyOnLoad(Thirdperson);

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return LoadAssem();
        }

        public static Assembly LoadAssem()
        {
            byte[] ba;
            string resource = "ProjectApparatus.Resources.0Harmony.dll";
            Assembly curAsm = Assembly.GetExecutingAssembly();
            using (Stream stm = curAsm.GetManifestResourceStream(resource))
            {
                ba = new byte[(int)stm.Length];
                stm.Read(ba, 0, (int)stm.Length);
                return Assembly.Load(ba);
            }
        }

        public static void Unload()
		{
			UnityEngine.Object.Destroy(Loader.Hack);
            UnityEngine.Object.Destroy(Loader.Thirdperson);
        }

	}
}
