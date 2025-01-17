﻿using System.Collections.Generic;
using Data.Difficulty;
using DefaultNamespace.Data;
using Managers;
using Objects.Characters;
using Objects.Players;
using Objects.Players.PermUpgrades;
using Objects.Runes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Weapons;

namespace Objects.Stage
{
	public class GameData
	{
		private static DifficultyData _currentDifficultyData;
		private static StageDefinition _currentStage;

		public static void SetCurrentDifficultyData(DifficultyData difficultyData)
		{
			_currentDifficultyData = difficultyData;
		}
		
		public static void SetCurrentStage(StageDefinition stage)
		{
			_currentStage = stage;
		}
		
		public static StageDefinition GetCurrentStage()
		{
			return _currentStage;
		}
		
		public static Sprite GetPlayerAbilityIcon()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.AbilityIcon;
		}

		public static Sprite GetPlayerCharacterAvatar()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.Avatar;
		}
		
		public static Sprite GetCharacterAvatar(CharactersEnum characterEnum)
		{
			return CharacterListManager.instance?.GetCharacter(characterEnum)?.Avatar;
		}
		
		public static Color? GetPlayerColorTheme()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.ColorTheme;
		}
		
		public static CharacterData GetPlayerCharacterData()
		{
			return CharacterListManager.instance?.GetActiveCharacter();
		}
		
		public static CharacterSaveData GetPlayerCharacterSaveData()
		{
			return CharacterListManager.instance?.GetActiveCharacterData();
		}
		
		public static WeaponBase GetPlayerCharacterStartingWeapon()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.StartingWeapon;
		}
		
		public static CharactersEnum GetPlayerCharacterId()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.Id ?? CharactersEnum.Maid;
		}
		
		public static CharacterRank GetPlayerCharacterRank()
		{
			return CharacterListManager.instance?.GetActiveRankCharacter() ?? CharacterRank.E0;
		}
		
		public static float GetCharacterSkillCooldown()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.AbilityCooldown ?? 1f;
		}
		
		public static CharacterSkillBase GetSkillPrefab()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.AbilityPrefab;
		}
		
		public static CharacterSkillBase GetSpecialPrefab()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.SpecialPrefab;
		}
		
		public static CharacterAchievementListener GetAchievementListenerPrefab()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.AchievementListenerPrefab;
		}

		public static Sprite GetPlayerSprite()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.CharacterSprite;
		}
		
		public static Sprite GetCharacterSprite(CharactersEnum characterEnum)
		{
			return CharacterListManager.instance?.GetCharacter(characterEnum)?.CharacterSprite;
		}

		public static PlayerStats GetPlayerStartingStats()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.Stats ?? new PlayerStats();
		}

		public static List<int> GetUnlockedSkillTreeNodeIds()
		{
			return GetPlayerCharacterSaveData()?.unlockedSkillPoints ?? new List<int>();
		}

		public static IEnumerable<PermUpgrade> GetPermUpgrades()
		{
			return PermUpgradeListManager.instance?.GetUpgrades() ?? new List<PermUpgrade>();
		}

		public static List<SkillNode> GetCharacterSkillNodes()
		{
			return CharacterListManager.instance?.GetActiveCharacter()?.skillNodes ?? new List<SkillNode>();
		}

		public static List<RuneData> GetRunesNodes()
		{
			return RuneListManager.instance?.GetRunes() ?? new List<RuneData>();
		}

		public static float ScaleRune(RuneSaveData runeSaveData)
		{
			return RuneListManager.instance?.GetScaledValue(runeSaveData) ?? 0;
		}

		public static DifficultyData GetCurrentDifficulty()
		{
			return _currentDifficultyData;
		}

		public static bool IsCharacterWithRank(CharactersEnum characterId, CharacterRank rank)
		{
			return GetPlayerCharacterId() == characterId && GetPlayerCharacterRank() >= rank;
		}

		public static bool IsCharacterRank(CharacterRank rank)
		{
			return GetPlayerCharacterRank() >= rank;
		}

		public static bool IsCharacter(CharactersEnum characterId)
		{
			return GetPlayerCharacterId() == characterId;
		}
	}
}