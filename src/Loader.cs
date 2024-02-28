using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ProjectApparatus
{
    public class Loader
    {
        private static GameObject oGui;
        private static GameObject oThirdperson;
        private static GameObject oLog;

        public static void Init()
        {
            oGui = new GameObject();
            oGui.AddComponent<Gui>();
            UnityEngine.Object.DontDestroyOnLoad(oGui);

            oLog = new GameObject();
            oLog.AddComponent<Log>();
            UnityEngine.Object.DontDestroyOnLoad(oLog);

            oThirdperson = new GameObject();
            oThirdperson.AddComponent<Features.Thirdperson>();
            UnityEngine.Object.DontDestroyOnLoad(oThirdperson);

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
            UnityEngine.Object.Destroy(oGui);
            UnityEngine.Object.Destroy(oThirdperson);
            UnityEngine.Object.Destroy(oLog);
        }

	}
}
