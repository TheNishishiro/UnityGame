using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DefaultNamespace.Data;
using DefaultNamespace.Data.Locale;
using Managers;
using NaughtyAttributes;
using Objects.Characters;
using Objects.Players;
using Objects.Stage;
using TMPro;
using UI.Main_Menu.REWORK.Scripts;
using UI.UI_Elements;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class MainCharacterCard : MonoBehaviour
{
    [BoxGroup("Managers")] [SerializeField] private AudioManager audioManager;
    [Space]
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelName;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelTitle;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelStarRating;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelFinishedDifficulty;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelLevel;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelWeaponDisplayType;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelWeaponName;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelWeaponDescription;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelPassiveDescription;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelSkillLinesAbility;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelSkillLinesPassive;
    [BoxGroup("Labels")] [SerializeField] private TextMeshProUGUI labelSkillLinesStats;
    [Space]
    [BoxGroup("Bars")] [SerializeField] private SliderBarComponent experienceSlider;
    [Space]
    [BoxGroup("Images")] [SerializeField] private Image characterCardImage;
    [BoxGroup("Images")] [SerializeField] private Image experienceSliderImage;
    [BoxGroup("Images")] [SerializeField] private Image imageBackGlow;
    [BoxGroup("Images")] [SerializeField] private Image imageLeftGlowStrip;
    [BoxGroup("Images")] [SerializeField] private Image imageRightGlowStrip;
    [BoxGroup("Images")] [SerializeField] private Image imageUltimateIcon;
    [BoxGroup("Images")] [SerializeField] private Image imageWeaponIcon;
    [BoxGroup("Images")] [SerializeField] private Image imageStatsBorder;
    [BoxGroup("Images")] [SerializeField] private Image imagePassiveBorder;
    [BoxGroup("Images")] [SerializeField] private Image imageWeaponBorder;
    [BoxGroup("Images")] [SerializeField] private Image imageLock;
    [BoxGroup("Images")] [SerializeField] private Image skillLines;
    [BoxGroup("Images")] [SerializeField] private SVGImage imageRuneIcon;
    [Space]
    [BoxGroup("Materials")] [SerializeField] private Material weaponIconHighlightMaterial;
    [BoxGroup("Materials")] [SerializeField] private Material grayScaleMaterial;
    [Space]
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statEntryHealth;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statEntryDamage;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statEntryCooldown;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statEntryCrit;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statsEntryAttackCount;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statsEntryResPen;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statEntryDot;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statEntryArmor;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statEntryRegen;
    [BoxGroup("Stats")] [SerializeField] private CharacterStatsEntry statEntryLuck;
    [Space]
    [BoxGroup("Menus")] [SerializeField] private ShardScreenManager shardScreenManager;
    [BoxGroup("Menus")] [SerializeField] private RuneScreenManager runeScreenManager;
    [BoxGroup("Menus")] [SerializeField] private GameSettingsScreenManager gameSettingsScreenManager;
    [BoxGroup("Menus")] [SerializeField] private ServerScreenManager serverScreenManager;
    [Space]
    [BoxGroup("Navigation")] [SerializeField] private TextMeshProUGUI arrowPreviousCharacter;
    [BoxGroup("Navigation")] [SerializeField] private TextMeshProUGUI arrowNextCharacter;

    private CharacterData _currentCharacterData;
    private CharacterSaveData _currentCharacterSaveData;
    
    public void Setup(CharacterData character, CharacterSaveData characterSaveData)
    {
        _currentCharacterData = character;
        _currentCharacterSaveData = characterSaveData;
        
        characterCardImage.sprite = character.CharacterCard;
        characterCardImage.material = null;
        labelName.text = character.Name;
        labelTitle.text = character.Title.Translate();
        var fullStars = new string ('\u2726', characterSaveData.RankUpLevel);
        var missingStars = new string ('\u2727', 5 - characterSaveData.RankUpLevel);
        labelStarRating.text = fullStars + missingStars;
        
        imageLock.gameObject.SetActive(!characterSaveData.IsUnlocked);
        
        fullStars = new string ('\u25c8', (int)characterSaveData.GetFinishedDifficulty(GameData.GetCurrentStage()?.id ?? StageEnum.CapitalOutskirts));
        missingStars = new string ('\u25c7', 4 - (int)characterSaveData.GetFinishedDifficulty(GameData.GetCurrentStage()?.id ?? StageEnum.CapitalOutskirts));
        labelFinishedDifficulty.text = fullStars + missingStars;
        labelLevel.text = "lv. ".Translate() + characterSaveData.Level;
        experienceSlider.SetValue(characterSaveData.Experience, characterSaveData.ExperienceNeeded);

        SetDescription(true);
        labelPassiveDescription.text = _currentCharacterData.GetPassiveDescription(_currentCharacterSaveData.RankUpLevel);
        
        imageUltimateIcon.sprite = character.AbilityIcon;
        imageWeaponIcon.sprite = character.StartingWeapon.Icon;
        
        imageBackGlow.color = character.ColorTheme;
        imageLeftGlowStrip.color = character.ColorTheme;
        imageRightGlowStrip.color = character.ColorTheme;
        imageStatsBorder.color = character.ColorTheme;
        imagePassiveBorder.color = character.ColorTheme;
        imageWeaponBorder.color = character.ColorTheme;
        labelFinishedDifficulty.color = character.ColorTheme;
        labelStarRating.color = character.ColorTheme;
        experienceSliderImage.color = character.ColorTheme;
        skillLines.color = character.ColorTheme;
        labelSkillLinesAbility.color = character.ColorTheme;
        labelSkillLinesPassive.color = character.ColorTheme;
        labelSkillLinesStats.color = character.ColorTheme;
        arrowPreviousCharacter.color = character.ColorTheme;
        arrowNextCharacter.color = character.ColorTheme;
        imageRuneIcon.color = character.ColorTheme;
        
        if (!characterSaveData.IsUnlocked)
        {
            labelName.text = "Locked".Translate();
            labelTitle.text = _currentCharacterData.IsPullable ? 
                "Fragments".Translate() + $": {_currentCharacterSaveData.Fragments}/50" : 
                "Coming soon".Translate();
            labelStarRating.text = "";
            labelFinishedDifficulty.text = "";
            labelLevel.text = "";
            experienceSlider.SetValue(0, 1);
            imageBackGlow.color = Color.gray;
            imageLeftGlowStrip.color = Color.gray;
            imageRightGlowStrip.color = Color.gray;
            imageStatsBorder.color = Color.gray;
            imagePassiveBorder.color = Color.gray;
            imageWeaponBorder.color = Color.gray;
            labelFinishedDifficulty.color = Color.gray;
            labelStarRating.color = Color.gray;
            experienceSliderImage.color = Color.gray;
            skillLines.color = Color.gray;
            labelSkillLinesAbility.color = Color.gray;
            labelSkillLinesPassive.color = Color.gray;
            labelSkillLinesStats.color = Color.gray;
            imageRuneIcon.color = Color.gray;
            characterCardImage.material = grayScaleMaterial;
            statEntryHealth.Set("???");
            statEntryArmor.Set("???");
            statEntryDamage.Set("???");
            statsEntryResPen.Set("???");
            statEntryCooldown.Set("???");
            statEntryCrit.Set("???");
            statEntryDot.Set("???");
            statEntryLuck.Set("???");
            statEntryRegen.Set("???");

            return;
        }

        SaveManager.instance.GetSaveFile().SelectedCharacterId = _currentCharacterData.Id;

        var stats = new PlayerStats(character.Stats);
        statEntryHealth.Set(stats.HealthMax.ToString(CultureInfo.InvariantCulture));
        statEntryArmor.Set($"{stats.Armor*100:N0}%");
        statEntryDamage.Set($"{stats.Damage} | {stats.DamagePercentageIncrease*100:N0}%");
        statEntryCooldown.Set($"{stats.CooldownReduction}s | {stats.CooldownReductionPercentage*100:N0}%");
        statEntryCrit.Set($"\u2684 {stats.CritRate*100:N0}% | \u2694 {stats.CritDamage*100:N0}%");
        statEntryDot.Set($"\u2694 {stats.DamageOverTime:N0} | \u23f2 {stats.DamageOverTimeDurationIncreasePercentage*100:N0}% | \u23f6 {stats.DamageOverTimeFrequencyReductionPercentage*100:N0}%");
        statEntryLuck.Set($"{stats.Luck*100:N0}%");
        statEntryRegen.Set($"{stats.HealthRegen} /s");
        statsEntryAttackCount.Set($"{stats.AttackCount}");
        statsEntryResPen.Set($"{stats.ResPen*100:N0}%");
        
        audioManager.PlaySound(character.nameAnnouncer, 2.5f);
    }

    public void SetDescription(bool isUltimate)
    {
        if (isUltimate)
        {
            labelWeaponDisplayType.text = "Ultimate".Translate();
            labelWeaponDescription.text = _currentCharacterData.GetAbilityDescription(_currentCharacterSaveData.RankUpLevel);
            labelWeaponName.text = _currentCharacterData.AbilityName;
            imageUltimateIcon.material = weaponIconHighlightMaterial;
            imageWeaponIcon.material = null;
        }
        else
        {
            labelWeaponDisplayType.text = "Weapon".Translate();
            labelWeaponDescription.text = _currentCharacterData.GetWeaponDescription(_currentCharacterSaveData.RankUpLevel);
            labelWeaponName.text = _currentCharacterData.StartingWeapon.Name;
            imageUltimateIcon.material = null;
            imageWeaponIcon.material = weaponIconHighlightMaterial;
        }
    }

    public void OpenShardsMenu()
    {
        if (!_currentCharacterSaveData.IsUnlocked) return;
        
        shardScreenManager.Open(_currentCharacterData, _currentCharacterSaveData.RankUpLevel);
    }

    public void OpenRuneMenu()
    {
        if (!_currentCharacterSaveData.IsUnlocked) return;
        
        runeScreenManager.Open(_currentCharacterData, SaveFile.Instance.GetCharacterSaveData(_currentCharacterData.Id));
    }

    public void OpenGameSettingsMenu()
    {
        if (!_currentCharacterSaveData.IsUnlocked) return;
        
        gameSettingsScreenManager.Open();
    }

    public void JoinGameScreen()
    {
        if (!_currentCharacterSaveData.IsUnlocked) return;
        
        serverScreenManager.Open();
    }
}
