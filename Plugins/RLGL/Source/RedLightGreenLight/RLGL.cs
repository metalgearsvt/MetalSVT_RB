using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Helpers;
using ff14bot.Managers;
using TreeSharp;
using System.Windows.Media;

namespace RLGL
{
    public class Variables {
		public static bool WatchOut;
	}
	
	public class RLGL {
		
		private Composite _coroutine;
		public Random rnd;

        public void OnButtonPress()
        {
            SettingsWindow sw = new SettingsWindow();
            sw.Show();
        }

        internal async Task<bool> CheckIfClear() {
			if(Variables.WatchOut == true) {
				int ok = 0;
				int NearCount = CountNearbyPC();
				while(NearCount > 0) {
					if(MovementManager.IsMoving) {
						MovementManager.MoveForwardStop();
					}
					Log(Color.FromRgb(255, 0, 0), "Detected player nearby, sleeping for 2 seconds!");
					ok = 1;
					await Coroutine.Sleep(2000);
					NearCount = CountNearbyPC();
				}
				if(ok != 0) {
					Log(Color.FromRgb(0, 255, 0), "No players detected, resuming.");
				}
			}
			return false;
		}
		
		public int CountNearbyPC() {
			int playersCounted = 0;
			var units = GameObjectManager.GameObjects;
			playersCounted = units.Where(IsUnitPC).Count();
			return playersCounted;
		}
		
		public bool IsUnitPC(ff14bot.Objects.GameObject unit) {
			if(!unit.IsMe && (int)unit.Type == 1) {
				Log(Color.FromRgb(100, 200, 255), "Player: {0} - LoS: {1}", unit.Name, unit.InLineOfSight());
				return true;
			}
			return false;
		}
		
		private void Log(Color color, String msg, params object[] vars) {
			string _out = "[RLGL] " + string.Format(msg, vars);
			Logging.Write(color, _out);
		}
	
		public void Dispose() {}
	
		public void Pulse() {}
	
		public string Author {
			get {
				return "MetalSVT";
			}
		}
	
		public Version Version {
			get {
				return new Version(0, 0, 3);
			}
		}
	
		public string Name {
			get {
				return "RedLightGreenLight";
			}
		}
		
		public void OnInitialize() {
			// Only happens once.
			Variables.WatchOut = false;
			rnd = new Random();
			_coroutine = new ActionRunCoroutine(r => CheckIfClear());
			Log(Color.FromRgb(255, 100, 75), "RLGL initialized!");
		}
	
		public void OnShutdown() {
			
		}
	
		public void OnDisabled() {
			Variables.WatchOut = false;
			TreeHooks.Instance.OnHooksCleared -= OnHooksCleared;
			TreeHooks.Instance.RemoveHook("TreeStart", _coroutine);
		}
		
		public void OnEnabled() {
			TreeHooks.Instance.AddHook("TreeStart", _coroutine);
			TreeHooks.Instance.OnHooksCleared += OnHooksCleared;
		}
	
		private void OnHooksCleared(object sender, EventArgs args) {
			Variables.WatchOut = false;
			TreeHooks.Instance.AddHook("TreeStart", _coroutine);
		}
		
	}
}
