using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using HarmonyLib;
using Hax;

namespace ProjectApparatus
{
    public class Loader : MonoBehaviour
    {
        private static GameObject Hack;
        private static GameObject Thirdperson;
        private static GameObject HaxGameObjects;
        private static GameObject HaxModules;

        public static void Init()
        {
            Hack = new GameObject();
            Hack.AddComponent<Hacks>();
            UnityEngine.Object.DontDestroyOnLoad(Hack);

            Thirdperson = new GameObject();
            Thirdperson.AddComponent<Features.Thirdperson>();
            UnityEngine.Object.DontDestroyOnLoad(Thirdperson);

            HaxGameObjects = new GameObject();
            HaxModules = new GameObject();

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

        static void AddHaxModules<T>() where T : Component => HaxModules.AddComponent<T>();
        static void AddHaxGameObject<T>() where T : Component => HaxGameObjects.AddComponent<T>();

        static Assembly OnResolveAssembly(object _, ResolveEventArgs args)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName =
                assembly.GetManifestResourceNames()
                        .First(name => name.EndsWith($"{new AssemblyName(args.Name).Name}.dll"));

            if (string.IsNullOrWhiteSpace(resourceName))
            {
                Logger.Write($"Failed to resolve assembly: {args.Name}");
                throw new FileNotFoundException();
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return Assembly.Load(memoryStream.ToArray());
            }
        }

        public static void Load()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;

            LoadHarmonyPatches();
            LoadHaxGameObjects();
            LoadHaxModules();

            AppDomain.CurrentDomain.AssemblyResolve -= OnResolveAssembly;
        }

        static void LoadHarmonyPatches()
        {
            try
            {
                new Harmony("winstxnhdw.lc-hax").PatchAll();
            }
            catch (Exception exception)
            {
                Logger.Write(exception.ToString());
                throw exception;
            }
        }

        static void LoadHaxGameObjects()
        {
            UnityEngine.Object.DontDestroyOnLoad(HaxGameObjects);

            AddHaxGameObject<HaxObjects>();
            AddHaxGameObject<InputListener>();
            AddHaxGameObject<ScreenListener>();
            AddHaxGameObject<GameListener>();
        }

        static void LoadHaxModules()
        {
            UnityEngine.Object.DontDestroyOnLoad(HaxModules);

            //AddHaxModules<ESPMod>();
            //AddHaxModules<SaneMod>();
            AddHaxModules<StunMod>();
            AddHaxModules<FollowMod>();
            //AddHaxModules<WeightMod>();
            //AddHaxModules<StaminaMod>();
            AddHaxModules<PhantomMod>();
            AddHaxModules<TriggerMod>();
            //AddHaxModules<AntiKickMod>();
            //AddHaxModules<CrosshairMod>();
            //AddHaxModules<MinimalGUIMod>();
            AddHaxModules<PossessionMod>();
            //AddHaxModules<ClearVisionMod>();
            //AddHaxModules<InstantInteractMod>();
        }

        public static void Unload()
        {
            UnityEngine.Object.Destroy(HaxModules);
            UnityEngine.Object.Destroy(HaxGameObjects);
            UnityEngine.Object.Destroy(Hack);
            UnityEngine.Object.Destroy(Thirdperson);
        }
    }
}
