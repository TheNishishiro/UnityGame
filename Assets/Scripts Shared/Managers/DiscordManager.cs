﻿using System;
using Discord;
using Objects.Characters;
using Objects.Stage;
using UnityEngine;
using Discord = Discord.Discord;

namespace Managers
{
	public class DiscordManager : MonoBehaviour
	{
		[SerializeField] private bool isEnabled;
		private global::Discord.Discord _discord;
		private ActivityManager _activityManager;
		private bool UseDiscord => isEnabled && !Application.isEditor;
		
		public void Awake()
		{
			var instances = FindObjectsOfType<AchievementManager>();
			if (instances.Length > 1)
			{
				Destroy(gameObject);
				return;
			}
			
			_discord = new global::Discord.Discord(1126131759722012674, (ulong)CreateFlags.NoRequireDiscord);
			_activityManager = _discord.GetActivityManager();
			DontDestroyOnLoad(gameObject);
		}

		private void Update()
		{
			if (!UseDiscord)
				return;
			
			_discord.RunCallbacks();
		}

		public void SetInGame()
		{
			UpdateActivity(
				"In game", 
				null,
				$"{GameData.GetPlayerCharacterId().GetName()}avatar",
				$"{GameData.GetPlayerCharacterId().GetName()} ({GameData.GetPlayerCharacterRank()})");
		}

		public void SetMainMenu()
		{
			UpdateActivity("In menu", null, null, null);
		}

		public void SetEndMenu(bool isWin)
		{
			UpdateActivity("End screen", isWin ? "Victory" : "Defeat", null, null);
		}
		
		public void ClearActivity()
		{
			_activityManager.ClearActivity((res) => { });
		}

		public void UpdateActivity(string details, string state, string image, string imageTitle)
		{
			if (!UseDiscord)
				return;
			
			var activity = new Activity()
			{
				State = state,
				Details = details,
				Assets =
				{
					LargeImage = "icon",
					LargeText = "Requiem for a Lost World",
					SmallImage = image?.ToLower(),
					SmallText = imageTitle
				},
				Timestamps =
				{
					Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
				},
				Type = ActivityType.Playing
			};
			
			_activityManager.UpdateActivity(activity, (res) =>
			{
				if (res == global::Discord.Result.Ok)
				{
					Debug.Log("Discord activity updated!");
				}
				else
				{
					Debug.LogError($"Discord activity failed to update: {res}");
				}
			});
		}
	}
}