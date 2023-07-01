﻿using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Labels.InGame.LevelUpScreen
{
	public class UpgradePanel : MonoBehaviour
	{ 
		private UpgradePanelManager _upgradePanelManager;
		private WeaponManager _weaponManager;
		private UpgradeEntry _upgradeEntry;
		[SerializeField] private Image icon;
		[SerializeField] private TextMeshProUGUI upgradeName;
		[SerializeField] private TextMeshProUGUI upgradeDescription;
		[SerializeField] private Image panelBackground;
		
		private void Awake()
		{
			_upgradePanelManager = FindObjectOfType<UpgradePanelManager>();
			_weaponManager = FindObjectOfType<WeaponManager>();
		}

		public void Set(UpgradeEntry upgradeEntry)
		{
			_upgradeEntry = upgradeEntry;

			icon.color = Color.white;
			icon.sprite = _upgradeEntry.GetUnlockIcon();

			panelBackground.color = _upgradeEntry.GetUpgradeColor();

			upgradeName.text = $"{_upgradeEntry.GetUnlockName()} +{_upgradeEntry.Rarity - 1}";
			upgradeDescription.text = _upgradeEntry.GetUnlockDescription();
		}

		public void Clean()
		{
			
		}

		public void SelectUpgrade()
		{
			_upgradeEntry.LevelUp(_weaponManager);
			_upgradePanelManager.ClosePanel();
		}
	}
}