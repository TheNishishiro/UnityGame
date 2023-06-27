using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using Interfaces;
using Objects.Abilities;
using Objects.Items;
using Objects.Players.Scripts;
using Objects.Stage;
using UI.Labels.InGame.LevelUpScreen;
using UnityEngine;
using UnityEngine.Events;
using Weapons;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private List<WeaponBase> availableWeapons;
    [SerializeField] private List<ItemBase> availableItems;
    [SerializeField] private Transform weaponContainer;
    private List<WeaponBase> _unlockedWeapons;
    private List<ItemBase> _unlockedItems;
    private PlayerStatsComponent _playerStatsComponent;
    public int maxWeaponCount = 6;
    public int maxItemCount = 6;
    private int _weaponsUpgraded;
    private int _itemsUpgraded;
    [SerializeField] private UnityEvent<WeaponBase, int> onWeaponAdded;
    [SerializeField] private UnityEvent<WeaponBase, int> onWeaponUpgraded;
    [SerializeField] private UnityEvent<ItemBase, int> onItemAdded;
    [SerializeField] private UnityEvent<ItemBase, int> onItemUpgraded;
    private SaveFile _saveFile;
    
    private void Start()
    {
        _saveFile = FindObjectOfType<SaveFile>();
        _playerStatsComponent = FindObjectOfType<PlayerStatsComponent>();
        _unlockedWeapons = new List<WeaponBase>();
        _unlockedItems = new List<ItemBase>();

        var characterStartingWeapon = GameData.GetPlayerCharacterStartingWeapon() ?? availableWeapons.FirstOrDefault();
        AddWeapon(characterStartingWeapon);
    }

    public void AddWeapon(WeaponBase weapon)
    {
        var weaponGameObject = Instantiate(weapon, weaponContainer);
        availableWeapons.Remove(weapon);
        _unlockedWeapons.Add(weaponGameObject);
        onWeaponAdded?.Invoke(weapon, _unlockedWeapons.Count);
    }

    public void AddItem(ItemBase item)
    {
        availableItems.Remove(item);
        _playerStatsComponent.Apply(item.ItemStats);
        _unlockedItems.Add(item);
        onItemAdded?.Invoke(item, _unlockedItems.Count);
    }

    public void UpgradeWeapon(WeaponBase weapon, UpgradeData upgradeData)
    {
        weapon.Upgrade(upgradeData);
        onWeaponUpgraded?.Invoke(weapon, ++_weaponsUpgraded);
    }

    public void UpgradeItem(ItemBase itemBase, ItemUpgrade itemUpgrade)
    {
        itemBase.RemoveUpgrade(itemUpgrade);
        _playerStatsComponent.Apply(itemUpgrade.ItemStats);
        onItemUpgraded?.Invoke(itemBase, ++_itemsUpgraded);
    }
    
    public List<IPlayerItem> GetUnlockedWeaponsAsInterface()
    {
        return _unlockedWeapons.Cast<IPlayerItem>().ToList();
    }
    
    public List<IPlayerItem> GetUnlockedItemsAsInterface()
    {
        return _unlockedItems.Cast<IPlayerItem>().ToList();
    }

    public IEnumerable<UpgradeEntry> GetUpgrades()
    {
        var upgrades = new List<UpgradeEntry>();
        upgrades.AddRange(GetWeaponUnlocks());
        upgrades.AddRange(GetItemUnlocks());
        upgrades.AddRange(GetWeaponUpgrades());
        upgrades.AddRange(GetItemUpgrades());
        return upgrades;
    }
    
    public IEnumerable<UpgradeEntry> GetWeaponUpgrades()
    {
        return _unlockedWeapons.Select(unlockedWeapon => new UpgradeEntry()
        {
            Weapon = unlockedWeapon,
            Upgrade = unlockedWeapon.GetAvailableUpgrades().FirstOrDefault()
        }).Where(x => x.Upgrade != null);
    }
    
    public IEnumerable<UpgradeEntry> GetItemUpgrades()
    {
        return _unlockedItems.Select(unlockedItem =>
        {
            var nextUpgrade = unlockedItem.GetAvailableUpgrades().FirstOrDefault();
            if (nextUpgrade == null)
                return null;

            return new UpgradeEntry()
            {
                ItemUpgrade = nextUpgrade,
                Item = unlockedItem
            };
        }).Where(x => x != null);
    }
    
    private IEnumerable<UpgradeEntry> GetWeaponUnlocks()
    {
        if (_unlockedWeapons.Count >= maxWeaponCount)
            return new List<UpgradeEntry>();
        
        return availableWeapons.Where(x => x.IsUnlocked(_saveFile)).Select(availableWeapon => new UpgradeEntry()
        {
            Weapon = availableWeapon
        });
    }
    
    private IEnumerable<UpgradeEntry> GetItemUnlocks()
    {
        if (_unlockedItems.Count >= maxItemCount)
            return new List<UpgradeEntry>();
        
        return availableItems.Where(x => x.IsUnlocked(_saveFile)).Select(availableItem => new UpgradeEntry()
        {
            Item = availableItem
        });
    }
    
    public void ReduceWeaponCooldowns(float reductionPercentage)
    {
        foreach (var weapon in _unlockedWeapons)
        {
            weapon.ReduceCooldown(reductionPercentage);
        }
    }
}
