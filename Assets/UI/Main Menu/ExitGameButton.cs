﻿using Managers;
using UnityEngine;

namespace UI.Main_Menu
{
	public class ExitGameButton : MonoBehaviour
	{
		public void QuitApplication()
		{
			FindObjectOfType<DiscordManager>().ClearActivity();
			Application.Quit();
		}
	}
}