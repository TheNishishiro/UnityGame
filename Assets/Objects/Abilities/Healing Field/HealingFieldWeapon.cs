﻿using DefaultNamespace;
using DefaultNamespace.Data;
using DefaultNamespace.Data.Achievements;
using Managers;
using Objects.Abilities.BindingField;
using UnityEngine;
using UnityEngine.Pool;
using Weapons;

namespace Objects.Abilities.Healing_Field
{
	public class HealingFieldWeapon : PoolableWeapon<HealingFieldProjectile>
	{
		[SerializeField] private GameObject healingFieldPrefab;
		[HideInInspector] public bool IsEmpowering;
		private ObjectPool<HealingField> _subProjectilePool;
		private Vector3 _subProjectilePosition;
		private WeaponStats _subProjectileStats;
		private WeaponStatsStrategyBase _subProjectileStatsStrategy;
		
		public override void Awake()
		{
			base.Awake();

			_subProjectileStats = new WeaponStats()
			{
				TimeToLive = 1f
			};
			_subProjectileStatsStrategy = new WeaponStatsStrategyBase(_subProjectileStats, ElementField);
			_subProjectilePool = new ObjectPool<HealingField>(
				() =>
				{
					var projectile = SpawnManager.instance.SpawnObject(_subProjectilePosition, healingFieldPrefab).GetComponent<HealingField>();
					projectile.Init(_subProjectilePool, projectile);
					return projectile;
				},
				projectile =>
				{
					_subProjectileStats.Scale = WeaponStatsStrategy.GetScale();
					projectile.transform.position = _subProjectilePosition;
					projectile.SetParentWeapon(this, false);
					projectile.SetStats(_subProjectileStatsStrategy);
					projectile.Setup(WeaponStatsStrategy.GetHealPerHit(true), IsEmpowering);
					projectile.gameObject.SetActive(true);
				},
				projectile => projectile.gameObject.SetActive(false),
				projectile => Destroy(projectile.gameObject),
				true, 20, 40
			);
		}
		
		public void SpawnSubProjectile(Vector3 position)
		{
			_subProjectilePosition = position;
			_subProjectilePool.Get();
		}

		public override void SetupProjectile(NetworkProjectile networkProjectile)
		{
			var pointOnSurface = Utilities.GetPointOnColliderSurface(new Vector3(transform.position.x, 0, transform.position.z), transform);
			networkProjectile.Initialize(this, pointOnSurface);
		}

		protected override void OnLevelUp()
		{
			if (LevelField == 9)
				IsEmpowering = true;
		}

		protected override int GetAttackCount()
		{
			return 1;
		}
	}
}