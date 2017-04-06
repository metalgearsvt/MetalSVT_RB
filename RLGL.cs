using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using Buddy.Coroutines;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using TreeSharp;

namespace RLGL {
	public class Variables {
		public static bool WatchOut;
	}
}

namespace RLGL {
	
	public class RLGL : BotPlugin {
		
		private Composite _coroutine;
		public Random rnd;
		
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
			foreach(var unit in units.OrderBy(r=>r.Distance())) {
				if(!unit.IsMe && (int)unit.Type == 1) {
					playersCounted++;
					Log(Color.FromRgb(100, 200, 255), "Player: {0} - LoS: {1}", unit.Name, unit.InLineOfSight());
				}
			}
			return playersCounted;
		}
		
		private void Log(Color color, String msg, params object[] vars) {
			string _out = "[RLGL] " + string.Format(msg, vars);
			Logging.Write(color, _out);
		}
	
		public override void Dispose() {}
	
		public void Pulse() {}
	
		public override string Author {
			get {
				return "MetalSVT";
			}
		}
	
		public override Version Version {
			get {
				return new Version(0, 0, 1);
			}
		}
	
		public override string Name {
			get {
				return "RedLightGreenLight";
			}
		}
		
		public override void OnInitialize() {
			// Only happens once.
			Variables.WatchOut = false;
			rnd = new Random();
			_coroutine = new ActionRunCoroutine(r => CheckIfClear());
			Log(Color.FromRgb(255, 100, 75), "RLGL initialized!");
		}
	
		public override void OnShutdown() {
			
		}
	
		public override void OnDisabled() {
			Variables.WatchOut = false;
			TreeHooks.Instance.OnHooksCleared -= OnHooksCleared;
			TreeHooks.Instance.RemoveHook("TreeStart", _coroutine);
		}
		
		public override void OnEnabled() {
			TreeHooks.Instance.AddHook("TreeStart", _coroutine);
			TreeHooks.Instance.OnHooksCleared += OnHooksCleared;
		}
	
		private void OnHooksCleared(object sender, EventArgs args) {
			Variables.WatchOut = false;
			TreeHooks.Instance.AddHook("TreeStart", _coroutine);
		}
		
	}
}