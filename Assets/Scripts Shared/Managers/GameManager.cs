﻿using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Data;
using Managers.StageEvents;
using Objects.Characters;
using Objects.Players.PermUpgrades;
using Objects.Players.Scripts;
using Objects.Stage;
using UnityEngine;

namespace Managers
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] private PlayerStatsComponent playerStatsComponent;

		private void Start()
		{
			AudioListener.volume = 0;
			var saveFile = FindObjectOfType<SaveFile>();
			playerStatsComponent.Set(GameData.GetPlayerStartingStats());
            
			var permUpgrades = GameData.GetPermUpgrades().ToList();
			foreach (var permUpgradesSaveData in saveFile.PermUpgradeSaveData ?? new Dictionary<PermUpgradeType, int>())
			{
				var permUpgrade = permUpgrades.FirstOrDefault(x => x.type == permUpgradesSaveData.Key);
				if (permUpgrade != null)
				{
					playerStatsComponent.ApplyPermanent(permUpgrade, permUpgradesSaveData.Value);
				}
			}
			
			FindObjectOfType<DiscordManager>().SetInGame();
		}
	}
}