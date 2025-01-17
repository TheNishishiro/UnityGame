﻿using System;
using System.Collections.Generic;
using Data.Difficulty;
using DefaultNamespace.Extensions;
using Objects.Characters;
using Objects.Stage;
using UnityEngine;

namespace DefaultNamespace.Data
{
	[Serializable]
	public class CharacterSaveData
	{
		public int Experience;
		public int Level;
		public bool IsUnlocked;
		public int RankUpLevel;
		public ulong KillCount;
		public int HighestInGameLevel;
		public int skillPoints;
		public int Fragments;
		public ulong TotalPlayTime;
		public ulong LongestGameTime;
		public List<int> unlockedSkillPoints;
		public List<RuneSaveData> characterRunes;
		public Dictionary<StageEnum, DifficultyEnum> FinishedDifficulty = new ();
		public int ExperienceNeeded => (int)(Level * 75 * 1.5f);
		public DateTime LastBannerDate;

		public CharacterSaveData()
		{
			Level = 1;
		}

		public List<int> GetUnlockedSkillPoints()
		{
			unlockedSkillPoints ??= new List<int>();
			return unlockedSkillPoints;
		}

		public void EquipRune(RuneSaveData runeSaveData)
		{
			characterRunes ??= new List<RuneSaveData>();
			characterRunes.Add(runeSaveData);
		}

		public void UnEquipRune(RuneSaveData runeSaveData)
		{
			characterRunes ??= new List<RuneSaveData>();
			characterRunes.Remove(runeSaveData);
		}

		public List<RuneSaveData> GetCharacterRunes()
		{
			characterRunes ??= new List<RuneSaveData>();
			return characterRunes;
		}

		public DifficultyEnum GetFinishedDifficulty(StageEnum stage)
		{
			FinishedDifficulty ??= new Dictionary<StageEnum, DifficultyEnum>();
			FinishedDifficulty.TryAdd(stage, DifficultyEnum.None);
			return FinishedDifficulty[stage];
		}

		public CharacterRank GetRankEnum()
		{
			return (CharacterRank)RankUpLevel;
		}
		
		public void Unlock()
		{
			if (!IsUnlocked)
				IsUnlocked = true;
			else if (RankUpLevel < (int)CharacterRank.E5)
				RankUpLevel++;
			else
				AddExperience(500);
		}

		public void AddFragments(CharactersEnum characterId, int fragmentReward)
		{
			Fragments += fragmentReward;
			if (Fragments >= 50)
			{
				SaveFile.Instance.UnlockCharacter(characterId);
				Fragments = 0;
			}
		}
		
		public void AddGameResultStats()
		{
			AddExperience(GameResultData.CharacterExp);
			KillCount += (ulong)GameResultData.MonstersKilled;
			if (GameResultData.Level > HighestInGameLevel)
				HighestInGameLevel = GameResultData.Level;

			if (GameResultData.IsWin && GetFinishedDifficulty(GameData.GetCurrentStage().id) < GameResultData.Difficulty)
				FinishedDifficulty[GameData.GetCurrentStage().id] = GameResultData.Difficulty;

			var timePlayed = (ulong)GameResultData.Time.ToMinutes();
			TotalPlayTime += timePlayed;
			if (LongestGameTime < timePlayed)
				LongestGameTime = timePlayed;
		}
		
		public void AddExperience(int experience)
		{
			Experience += experience;
			while (Experience >= ExperienceNeeded)
			{
				Experience -= ExperienceNeeded;
				Level++;
				skillPoints++;
			}
		}
	}
}