using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using ff14bot.AClasses;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Behavior;
using TreeSharp;
using Action = TreeSharp.Action;

namespace RLGLLoader
{
    public class RLGLLoader : BotPlugin
    {
        public RLGLLoader()
        {
            if (started) return;
            started = true;
            //LoadPlugin();
        }
        #region Meta Data

        private static bool started = false;
        private const string PluginClass = "RLGL.RLGL";
        private static string ProjectName = "RedLightGreenLight";
        private static readonly string PluginAssembly = Path.Combine(Environment.CurrentDirectory, @"Plugins\RLGL\RedLightGreenLight.dll");
		private static readonly string RLGLSettings = Path.Combine(Environment.CurrentDirectory, @"Plugins\RLGL\RLGL.dll");
		private static readonly string greyMagicAssembly = Path.Combine(Environment.CurrentDirectory, @"GreyMagic.dll");
        private static readonly object ObjLock = new object();

        #endregion

        #region Overrides
        public override bool WantButton => true;
        public override string Name => ProjectName;
        private static MethodInfo StartFunc { get; set; }

        private static MethodInfo StopFunc { get; set; }

        private static MethodInfo ButtonFunc { get; set; }

        private static MethodInfo RootFunc { get; set; }
        private static MethodInfo InitFunc { get; set; }
        private static MethodInfo PulseFunc { get; set; }

		public override Version Version {
			get {
				return new Version(0, 0, 3);
			}
		}
		
		public override string Author {
			get {
				return "MetalSVT";
			}
		}
		
        public override void OnButtonPress()
        {
            if (Plugin == null) { LoadPlugin(); }
            if (Plugin != null) { ButtonFunc.Invoke(Plugin, null); }
        }
		
		public override void OnEnabled() {
			if (Plugin == null) {
				LoadPlugin();
			}
			if(Plugin != null) {
				StartFunc.Invoke(Plugin, null);
			}
		}
		
		public override void OnDisabled() {
			if (Plugin == null) {
				LoadPlugin();
			}
			if(Plugin != null) {
				StopFunc.Invoke(Plugin, null);
			}
		}
		
        public override void OnInitialize()
        {
            if (Plugin == null)
            {
                LoadPlugin();
            }
            if (Plugin != null)
            {
                InitFunc.Invoke(Plugin, null);
            }
        }

        public override void OnPulse()
        {
            if (Plugin == null)
            {
                LoadPlugin();
            }
            if (Plugin != null)
            {
                PulseFunc.Invoke(Plugin, null);
            }
        }

        #endregion

        #region Injections

        private static object Plugin { get; set; }

        #endregion

        #region Inject Methods

        private static object Load()
        {
            RedirectAssembly();

            var assembly = LoadAssembly(PluginAssembly);
            if (assembly == null)
            {
                return null;
            }

            Type baseType;
            try
            {
                baseType = assembly.GetType(PluginClass);
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return null;
            }

            object plugin;
            try
            {
                plugin = Activator.CreateInstance(baseType);
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return null;
            }

            Log(plugin != null
                ? "Loaded successfully."
                : "Could not load. This can be due to a new version of Rebornbuddy being released. An update should be ready soon.");

            return plugin;
        }

        private static Assembly LoadAssembly(string path)
        {

            if (!File.Exists(path)) { return null; }
            
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFrom(path);
            }
            catch (Exception e)
            {
                Logging.WriteException(e);
                var typeLoadException = e as ReflectionTypeLoadException;
                if (typeLoadException != null)
                {
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    foreach(var ee in loaderExceptions)
                        Logging.WriteException(ee);
                }
            }

            return assembly;
        }

        private static void LoadPlugin()
        {
            lock (ObjLock)
            {
                if (Plugin != null)
                {
                    return;
                }
                Plugin = Load();

                if (Plugin == null)
                {
                    return;
                }

                StartFunc = Plugin.GetType().GetMethod("OnEnabled");
                StopFunc = Plugin.GetType().GetMethod("OnDisabled");
                ButtonFunc = Plugin.GetType().GetMethod("OnButtonPress");
                InitFunc = Plugin.GetType().GetMethod("OnInitialize");
				PulseFunc = Plugin.GetType().GetMethod("Pulse");
                if (InitFunc != null)
                    InitFunc.Invoke(Plugin, null);

            }
        }

        #endregion

        #region Helper Methods

        private static void Log(string message)
        {
            Logging.Write(Colors.Magenta, $"[RLGLLoader] {message}");
        }

        public static void RedirectAssembly()
        {
            ResolveEventHandler handler = (sender, args) =>
            {
                string name = Assembly.GetEntryAssembly().GetName().Name;
                var requestedAssembly = new AssemblyName(args.Name);
                return requestedAssembly.Name != name ? null : Assembly.GetEntryAssembly();
            };

            AppDomain.CurrentDomain.AssemblyResolve += handler;

            ResolveEventHandler greyMagicHandler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                return requestedAssembly.Name != "GreyMagic" ? null : Assembly.LoadFrom(greyMagicAssembly);
            };

            AppDomain.CurrentDomain.AssemblyResolve += greyMagicHandler;
			
			ResolveEventHandler RLGLSettingsHandler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                return requestedAssembly.Name != "RLGLSettings" ? null : Assembly.LoadFrom(RLGLSettings);
            };

            AppDomain.CurrentDomain.AssemblyResolve += greyMagicHandler;
        }

        #endregion
    }
}