using Harmony;
using Klei.HotLava.Cameras;
using Klei.HotLava.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HotLavaStuff
{
	class Hack : MonoBehaviour
	{
		//[DllImport("kernel32.dll")]
		//public static extern bool AllocConsole(int pid);

		//UI
		//Menu
		public bool _menuVisible = true;
		public Rect _menuRect = new Rect(10, 10, 300, 300);

		//Options
		public static bool _freeRunTokenESP = false;
		public static string _status = "0";

		bool init = false;

		void Start()
		{
			FileLog.Reset();
			FileLog.Log("Init Start\n");

			HarmonyInstance harmony = HarmonyInstance.Create("exp.hotlavastuff");
			harmony.PatchAll(Assembly.GetExecutingAssembly());

			/*//Console
			AllocConsole(-1);
			var stdout = Console.OpenStandardOutput();
			var sw = new System.IO.StreamWriter(stdout, Encoding.Default);
			sw.AutoFlush = true;
			Console.SetOut(sw);
			Console.SetError(sw);*/

			init = true;
			FileLog.Log("Init Completed\n");
		}

		void Update()
		{
			//Console.WriteLine("Update");
			if (!init)
			{
				Start();
			}

			//Menu Toggle
			if (Input.GetKeyDown(KeyCode.Insert))
				_menuVisible = !_menuVisible;

			//Stuff
		}

		void OnGUI()
		{
			GUI.Label(new Rect(10, 10, 100, 20), _status);
			if (_menuVisible)
			{
				_menuRect = GUI.Window(1337, _menuRect, MenuFunction, "Unity is shit");
			}
		}

		void MenuFunction(int windowID)
		{
			FileLog.Log("Menu Function\n");
			//Drag Queen
			GUI.DragWindow(new Rect(0, 0, _menuRect.width, 20));

			_freeRunTokenESP = GUI.Toggle(new Rect(10, 20, 200, 20), _freeRunTokenESP, "Card ESP");

			//Unload
			if (GUI.Button(new Rect(10, _menuRect.height - 25, _menuRect.width - 20, 20), "Unload"))
			{
				Class1.Unload();
			}
		}
	}
}

namespace HotLavaStuff.HarmonyPatches
{
	#region HarmonyPatches
	[HarmonyPatch(typeof(FreeRunToken), "OnGUI", null)]
	public static class FreeRunTokenESP
	{
		static void Postfix(FreeRunToken __instance)
		{
			FileLog.Log("FreeRunToken OnGUI\n");
			Hack._status = "1";
			if (!Hack._freeRunTokenESP || __instance == null)
				return;

			Hack._status = "2";
			Vector3 pos = Camera.main.WorldToScreenPoint(__instance.transform.position);
			pos.y = Screen.height - pos.y;
			// Make sure it is on screen
			if (pos.z > 0f)
			{
				GUI.Label(new Rect(pos.x, pos.y, 50, 20), "f");
				Hack._status = "3";
			}
		}
	}
	#endregion
}