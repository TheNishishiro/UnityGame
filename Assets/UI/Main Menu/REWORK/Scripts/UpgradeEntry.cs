﻿using DefaultNamespace.Data;
using Objects.Players.PermUpgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Main_Menu.REWORK.Scripts
{
    public class UpgradeEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI labelTitle;
        [SerializeField] private TextMeshProUGUI labelDescription;
        [SerializeField] private TextMeshProUGUI labelRating;
        [SerializeField] private TextMeshProUGUI labelPrice;
        [SerializeField] private TextMeshProUGUI labelUpgrade;
        [SerializeField] private Image imageIcon;
        private PermUpgrade _permUpgrade;

        public void Setup(PermUpgrade permUpgrade)
        {
            _permUpgrade = permUpgrade;
            Refresh();
        }

        public void Refresh()
        {
            SaveFile.Instance.PermUpgradeSaveData.TryGetValue(_permUpgrade.type, out var savedUpgradeLevel);
            var upgradeMaxLevel = _permUpgrade.maxLevel;
            var cost = _permUpgrade.basePrice + _permUpgrade.costPerLevel * savedUpgradeLevel;
            
            labelTitle.text = _permUpgrade.name;
            labelDescription.text = _permUpgrade.description;
            imageIcon.sprite = _permUpgrade.icon;
            var isMaxLevel = savedUpgradeLevel >= upgradeMaxLevel;
            labelPrice.text = isMaxLevel ? "---" : cost.ToString("0");
            labelUpgrade.text = isMaxLevel ? "Maxed" : "Upgrade";
                
            
            var filledStars = new string('\u25c8', savedUpgradeLevel);
            var emptyStars = new string('\u25c7', upgradeMaxLevel - savedUpgradeLevel);
            labelRating.text = filledStars + emptyStars;
        }

        public void Upgrade()
        {
            SaveFile.Instance.PermUpgradeSaveData.TryGetValue(_permUpgrade.type, out var savedUpgradeLevel);
            var cost = _permUpgrade.basePrice + _permUpgrade.costPerLevel * savedUpgradeLevel;
            var isMaxLevel = savedUpgradeLevel >= _permUpgrade.maxLevel;
            
            if (SaveFile.Instance.Gold >= cost && !isMaxLevel)
            {
                SaveFile.Instance.Gold -= (ulong)cost;
                SaveFile.Instance.AddUpgradeLevel(_permUpgrade.type);
                Refresh();
            }
        }
    }
}