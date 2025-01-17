﻿using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Data.Weapons;
using Managers;
using Objects.Abilities.SpaceExpansionBall;
using UnityEngine;
using UnityEngine.Pool;
using Weapons;

namespace Objects.Abilities.Bouncer
{
	public class BouncerWeapon : PoolableWeapon<BouncerProjectile>
	{
		public float ElectroDefenceShred;
		public bool Thunderstorm;

		private ObjectPool<BouncerSubProjectile> _subProjectilePool;
		private Vector3 _subProjectilePosition;
		private WeaponStats _subProjectileStats;
		private WeaponStatsStrategyBase _subProjectileStatsStrategy;
		[SerializeField] private GameObject subProjectilePrefab;

		public override void Awake()
		{
			base.Awake();

			_subProjectileStats = new WeaponStats()
			{
				TimeToLive = 0.5f,
				Scale = 0.5f,
				Speed = 1,
				PassThroughCount = 1
			};
			_subProjectileStatsStrategy = new WeaponStatsStrategyBase(_subProjectileStats, ElementField);
			
			_subProjectilePool = new ObjectPool<BouncerSubProjectile>(
				() =>
				{
					var projectile = SpawnManager.instance.SpawnObject(_subProjectilePosition, subProjectilePrefab).GetComponent<BouncerSubProjectile>();
					projectile.Init(_subProjectilePool, projectile);
					return projectile;
				},
				projectile =>
				{
					_subProjectileStats.Damage = WeaponStatsStrategy.GetDamage() * 0.5f;
					projectile.transform.position = _subProjectilePosition;
					projectile.gameObject.SetActive(true);
					projectile.SetParentWeapon(this, false);
					projectile.SetStats(_subProjectileStatsStrategy);
					projectile.FindNextTarget();
				},
				projectile => projectile.gameObject.SetActive(false),
				projectile => Destroy(projectile.gameObject),
				true, 150, 200
			);
		}
		
		public void SpawnSubProjectile(Vector3 position)
		{
			_subProjectilePosition = position;
			_subProjectilePool.Get();
		}
		
		public override void SetupProjectile(NetworkProjectile networkProjectile, WeaponPoolEnum weaponPoolEnum)
		{
			var currentPosition = transform.position;
			var target = EnemyManager.instance.GetRandomEnemy().GetDamagableComponent();
			if (target is null)
			{
				networkProjectile.Despawn(WeaponId);
				return;
			}

			networkProjectile.Initialize(this, currentPosition);
			networkProjectile.GetProjectile<BouncerProjectile>().SetTarget(target);
		}

		protected override void OnLevelUp()
		{
			switch (LevelField)
			{
				case 4:
					ElectroDefenceShred += 0.2f;
					break;
				case 8:
					ElectroDefenceShred += 0.1f;
					break;
				case 11:
					Thunderstorm = true;
					break;
			}
		}
	}
}