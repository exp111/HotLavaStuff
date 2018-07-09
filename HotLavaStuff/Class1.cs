using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace HotLavaStuff
{
    public class Class1
    {
		public static GameObject _loadObject;

		public static void Load()
		{
			_loadObject = new GameObject();
			_loadObject.AddComponent<Hack>();
			UnityEngine.Object.DontDestroyOnLoad(_loadObject);
		}

		public static void Unload()
		{
			GameObject.Destroy(_loadObject);
		}
	}
}
