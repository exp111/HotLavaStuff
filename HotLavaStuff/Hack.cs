﻿using Harmony;
using Klei.HotLava;
using Klei.HotLava.Cameras;
using Klei.HotLava.Game;
using Klei.HotLava.Gameplay;
using Klei.HotLava.Online;
using Klei.HotLava.UI;
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

		public bool _teleportMenuVisible = false;
		public Rect _teleportMenuRect = new Rect(350, 10, 250, 500);
		public Vector2 _teleportScrollPosition = Vector2.zero;

		//Options
		public static bool _freeRunTokenESP = false;
		public static bool _collectiblesESP = false;

		bool init = false;

		public static List<FreeRunToken> _tokens;
		public static List<CollectibleForLevel> _collectibles;

		void Start()
		{
			FileLog.Reset();
			FileLog.Log("Init Start\n");

			_tokens = new List<FreeRunToken>();
			_collectibles = new List<CollectibleForLevel>();

			try
			{
				HarmonyInstance harmony = HarmonyInstance.Create("exp.hotlavastuff");
				harmony.PatchAll(Assembly.GetExecutingAssembly());
			} catch (Exception e)
			{
				FileLog.Log($"Failed because: {e}");
			}
			

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
			if (Input.GetKeyDown(KeyCode.F3))
			{
				State.LocalPlayer.movementSettings.CurrentTargetSpeed += 1f;
			}
			//Console.WriteLine("Update");
			try
			{ 
			if (!init)
			{
				Start();
			}

			//Menu Toggle
			if (Input.GetKeyDown(KeyCode.Insert))
				_menuVisible = !_menuVisible;

			//Stuff
			if (Input.GetKeyDown(KeyCode.F1))
			{
				State.LocalPlayer.RigidBody.AddForce(Camera.main.transform.forward * 5);
			}

			//TODO: this.m_PlayerController.movementSettings.ForwardSpeed
			if (Input.GetKeyDown(KeyCode.F2))
			{
				Checkpoint.CommandSaveCheckpoint(State.LocalPlayer.transform.position, State.LocalPlayer.transform.rotation);
			}
			}
			catch (Exception e)
			{
				FileLog.Log($"Failed because: {e}");
			}
		}

		void OnGUI()
		{
			if (State.LocalPlayer != null && State.LocalPlayer.movementSettings != null)
				GUI.Label(new Rect(200, 200, 200, 200), State.LocalPlayer.movementSettings.CurrentTargetSpeed.ToString());
			if (_menuVisible)
			{
				_menuRect = GUI.Window(1337, _menuRect, MenuFunction, "Unity is shit");

				if (_teleportMenuVisible)
					_teleportMenuRect = GUI.Window(1338, _teleportMenuRect, TeleportMenuFunction, "Teleport");
			}

			
			if (_freeRunTokenESP)
			{
				GUI.Label(new Rect(100, 100, 100, 100), _tokens.Count.ToString());

				foreach (var token in _tokens)
				{
					if (token == null || token.gameObject == null) //|| !token.gameObject.activeSelf || !token.gameObject.activeInHierarchy)
					{
						//_tokens.Remove(token);
						continue;
					}
					Vector3 pos = Camera.main.WorldToScreenPoint(token.transform.position);
					pos.y = Screen.height - pos.y;

					if (pos.z > 0f)
					{
						GUI.Label(new Rect(pos.x, pos.y, 50, 20), $"Token: {Vector3.Distance(token.transform.position, Camera.main.transform.position).ToString()}");
					}
				}
			}


			if (_collectiblesESP)
			{
				GUI.Label(new Rect(100, 150, 100, 100), _collectibles.Count.ToString());
				foreach (var collectible in _collectibles)
				{
					if (collectible == null || collectible.gameObject == null) //|| !collectible.gameObject.activeSelf || !collectible.gameObject.activeInHierarchy)
					{
						//_collectibles.Remove(token);
						continue;
					}
					Vector3 pos = Camera.main.WorldToScreenPoint(collectible.transform.position);
					pos.y = Screen.height - pos.y;

					if (pos.z > 0f)
					{
						GUI.Label(new Rect(pos.x, pos.y, 50, 20), $"Collectible ({collectible.m_Unlockable.m_Description}): {Vector3.Distance(collectible.transform.position, Camera.main.transform.position).ToString()}");
					}
				}
			}


		}

		void MenuFunction(int windowID)
		{
			//Drag Queen
			GUI.DragWindow(new Rect(0, 0, _menuRect.width, 20));

			_freeRunTokenESP = GUI.Toggle(new Rect(10, 20, 200, 20), _freeRunTokenESP, "Card ESP");
			_collectiblesESP = GUI.Toggle(new Rect(10, 40, 200, 20), _collectiblesESP, "Collectible ESP");
			_teleportMenuVisible = GUI.Toggle(new Rect(10, 60, 200, 20), _teleportMenuVisible, "Teleport Menu");
			
			
			if (GUI.Button(new Rect(10, 80, _menuRect.width - 20, 20), "Get Daily Gift"))
			{
				Singleton<LevelSingleton>.Instance.m_CharacterCanvas.m_LevelCompleteMenu.m_AwardsScreen.QueueDailyGiftCheck();
			}

			//Unload
			if (GUI.Button(new Rect(10, _menuRect.height - 25, _menuRect.width - 20, 20), "Unload"))
			{
				Class1.Unload();
			}
		}

		void TeleportMenuFunction(int windowID)
		{
			GUI.DragWindow(new Rect(0, 0, _teleportMenuRect.width, 20));

			var players = Player.GetOnlinePlayers();
			_teleportScrollPosition = GUI.BeginScrollView(new Rect(20, 20, _teleportMenuRect.width, _teleportMenuRect.height), _teleportScrollPosition, new Rect(0, 0, _teleportMenuRect.width - 50, players.Count * 20));
			var y = 0;
			foreach (var player in players)
			{
				if (GUI.Button(new Rect(5, y++ * 25, 200, 20), player.SteamName))
				{
					State.LocalPlayer.Teleport(player.m_PlayerController.transform.position, player.m_PlayerController.transform.rotation);
				}
			}
			GUI.EndScrollView();
		}
	}
}

namespace HotLavaStuff.HarmonyPatches
{
	#region HarmonyPatches
	[HarmonyPatch(typeof(FreeRunToken), "OnEnable")]
	public static class FreeRunTokenESP
	{
		static void Postfix(FreeRunToken __instance)
		{
			if (__instance == null)
				return;

			Hack._tokens.Add(__instance);
		}
	}

	[HarmonyPatch(typeof(CollectibleForLevel), "OnEnable")]
	public static class CollectibleESP
	{
		static void Postfix(CollectibleForLevel __instance)
		{
			if (__instance == null)
				return;

			Hack._collectibles.Add(__instance);
		}
	}
	#endregion
}