﻿using System;
using System.Linq;
using Data.Elements;
using Objects.Abilities.Reality_Crack;
using UnityEngine;

namespace Objects.Characters.Alice.Skill
{
	public class AliceSkill : CharacterSkillBase
	{
		private Vector3 _weaponCenter;
		private EnemyManager _enemyManager;
		private ElementalWeapon _elementalWeapon;
		private bool exploded;
		
		private void Start()
		{
			_enemyManager = FindObjectsByType<EnemyManager>(FindObjectsSortMode.None).FirstOrDefault();
			_weaponCenter = FindObjectsByType<Player>(FindObjectsSortMode.None).First().transform.position;
			_elementalWeapon = new ElementalWeapon(Element.Lightning);
		}
		
		public void Update()
		{
			TickLifeTime();
			if (LifeTime < 0.5f)
			{
				Explode();
			}
		}
		
		private void Explode()
		{
			if (exploded)
				return;
			
			exploded = true;
			foreach (Transform child in transform)
			{
				if (child.TryGetComponent<Rigidbody>(out var childRigidBody))
				{
					childRigidBody.useGravity = true;
					childRigidBody.AddExplosionForce(100f, _weaponCenter, 10f);
				}
			}

			_enemyManager.GlobalDamage(25, _elementalWeapon);
		}
	}
}