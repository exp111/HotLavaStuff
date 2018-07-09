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

		private static float _origMaxBunnyHop = 0;
		private static float _origPerfectBunnyHopBonusPerSecond = 0;

		//public static float _updateTime = 3.0f;
		//private static float _nextUpdateTime = 0;

		private bool init = false;

		public static List<FreeRunToken> _tokens;
		public static List<CollectibleForLevel> _collectibles;

		void Start()
		{
			StartCoroutine(RefreshCoroutine(3));
			_tokens = new List<FreeRunToken>();
			_collectibles = new List<CollectibleForLevel>();
			init = true;
		}

		private IEnumerator<WaitForSeconds> RefreshCoroutine(int seconds)
		{
			do
			{
				yield return new WaitForSeconds(seconds);
				Refresh();
			} while (true);
		}

		void Update()
		{
			if (!init)
			{
				Start();
			}

			/*if (Time.time > _nextUpdateTime)
			{
				Refresh()
				_nextUpdateTime = Time.time + _updateTime;
			}*/

			//Menu Toggle
			if (Input.GetKeyDown(KeyCode.Insert))
				_menuVisible = !_menuVisible;

			//Stuff
			if (Input.GetKeyDown(KeyCode.F1))
			{
				if (_origMaxBunnyHop != -1 && _origPerfectBunnyHopBonusPerSecond != -1)
				{
					State.LocalPlayer.movementSettings.MaxBunnyHop = _origMaxBunnyHop;
					State.LocalPlayer.movementSettings.PerfectBunnyHopBonusPerSecond = _origPerfectBunnyHopBonusPerSecond;
				}
			}
			//TODO: this.m_PlayerController.movementSettings.ForwardSpeed
			if (Input.GetKeyDown(KeyCode.F2))
			{
				Checkpoint.CommandSaveCheckpoint(State.LocalPlayer.transform.position, State.LocalPlayer.transform.rotation);
			}

			if (Input.GetKeyDown(KeyCode.F3))
			{
				if (_origMaxBunnyHop == -1 || _origPerfectBunnyHopBonusPerSecond == -1)
				{
					_origMaxBunnyHop = State.LocalPlayer.movementSettings.MaxBunnyHop;
					_origPerfectBunnyHopBonusPerSecond = State.LocalPlayer.movementSettings.PerfectBunnyHopBonusPerSecond;
				}
				State.LocalPlayer.movementSettings.MaxBunnyHop = 100;
				State.LocalPlayer.movementSettings.PerfectBunnyHopBonusPerSecond = 100;
			}
			if (Input.GetKeyDown(KeyCode.F4))
			{
				GameScore.Current.m_Current = 0;
			}
		}

		void OnGUI()
		{
			if (_menuVisible)
			{
				_menuRect = GUI.Window(1337, _menuRect, MenuFunction, "Unity is shit");

				if (_teleportMenuVisible)
					_teleportMenuRect = GUI.Window(1338, _teleportMenuRect, TeleportMenuFunction, "Teleport");
			}

			
			if (_freeRunTokenESP)
			{ 
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
			if (_tokens != null)
				GUI.Label(new Rect(150, 20, 200, 20), $"Left: {_tokens.Count.ToString()}");
			_collectiblesESP = GUI.Toggle(new Rect(10, 40, 200, 20), _collectiblesESP, "Collectible ESP");
			if (_collectibles != null)
				GUI.Label(new Rect(150, 40, 200, 20), $"Left: {_collectibles.Count.ToString()}");
			_teleportMenuVisible = GUI.Toggle(new Rect(10, 60, 200, 20), _teleportMenuVisible, "Teleport Menu");
			
			
			if (GUI.Button(new Rect(10, 80, _menuRect.width - 20, 20), "Get Daily Gift"))
			{
				Singleton<LevelSingleton>.Instance.m_CharacterCanvas.m_LevelCompleteMenu.m_AwardsScreen.QueueDailyGiftCheck();
			}

			if (GUI.Button(new Rect(10, 100, _menuRect.width - 20, 20), "Refresh"))
			{
				Refresh();
			}

			//Unload
			if (GUI.Button(new Rect(10, _menuRect.height - 25, _menuRect.width - 20, 20), "Unload"))
			{
				Class1.Unload();
			}
		}

		void Refresh()
		{
			_tokens = FindObjectsOfType<FreeRunToken>().ToList();
			_collectibles = FindObjectsOfType<CollectibleForLevel>().ToList();
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