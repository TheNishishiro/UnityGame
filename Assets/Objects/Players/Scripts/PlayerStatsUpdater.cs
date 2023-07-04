﻿using System;
using System.Collections.Generic;
using Interfaces;
using Objects.Characters;
using Objects.Characters.Amelia;
using Objects.Characters.Arika;
using Objects.Characters.Chitose;
using Objects.Characters.Corina_Alter;
using Objects.Characters.David;
using Objects.Characters.Maid;

namespace Objects.Players.Scripts
{
	public class PlayerStatsUpdater
	{
		private readonly Dictionary<CharactersEnum, ICharacterStrategy> _characterStrategies = new()
		{
			{ CharactersEnum.Chitose, new ChitoseStrategy() },
			{ CharactersEnum.Amelia_BoD, new AmeliaStrategy() },
			{ CharactersEnum.Arika_BoV, new ArikaStrategy() },
			{ CharactersEnum.Corina_BoB, new CorinaStrategy() },
			{ CharactersEnum.David_BoF, new DavidStrategy() },
			{ CharactersEnum.Maid, new ElizaStrategy() },
		};

		public void ApplyStrategy(CharactersEnum characterId, CharacterRank characterRank, PlayerStats playerStats)
		{
			if (_characterStrategies.TryGetValue(characterId, out var characterStrategy))
			{
				characterStrategy.Apply(playerStats, characterRank);
			}
		}
	}
}