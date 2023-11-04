﻿using System;
using System.Collections.Generic;
using DefaultNamespace;
using Events.Scripts;
using Interfaces;
using Managers;
using Objects.Drops;
using Objects.Drops.ChestDrop;
using Objects.Players.Scripts;
using Objects.Stage;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Objects.Enemies
{
	public class Enemy : MonoBehaviour
	{
		[SerializeField] private ChaseComponent chaseComponent;
		[SerializeField] private GameResultData gameResultData;
		[SerializeField] private Damageable damageableComponent;
		[SerializeField] private SpriteRenderer spriteRenderer;
		[SerializeField] private CapsuleCollider capsuleCollider;
		[SerializeField] private DropOnDestroy dropOnDestroyComponent;
		[SerializeField] private Pickup chestDrop;
		[SerializeField] private Pickup expDrop;
		[SerializeField] private Pickup goldDrop;
		[SerializeField] private Pickup gemDrop;
		[SerializeField] private GameObject grandOctiBoss;
		public Transform TargetPoint => damageableComponent.targetPoint.transform;
		private EnemyManager _enemyManager;
		private EnemyStats stats;
		private GameObject targetGameObject;
		private Player playerTarget;
		private float _damageCooldown = 0.5f;
		private float _currentDamageCooldown = 0;
		private float _timeAlive = 0;
		private bool _isBossEnemy = false;
		private float _removeCollisionsTimer;
		private bool _isRemoveCollisions;
		private float _damageReduction;
		private List<Collision> _ignoredEnemyColliders = new ();

		private ChanceDrop chestDropChance;
		private ChanceDrop expDropChance;
		private ChanceDrop goldDropChance;
		private ChanceDrop gemDropChance;
		
		public void Setup(EnemyData newStats, Player target, EnemyManager enemyManager, PlayerStatsComponent playerStats, float healthMultiplier, Sprite sprite)
		{
			_enemyManager = enemyManager;
			damageableComponent.Clear();
			chaseComponent.Clear();
			
			_timeAlive = 0;
			_currentDamageCooldown = 0;
			_damageCooldown = 0.5f;
			_removeCollisionsTimer = 0;
			_damageReduction = 0;
			_isRemoveCollisions = false;
			RevertIgnoredCollisions();
			_ignoredEnemyColliders.Clear();
			
			playerTarget = target;
			spriteRenderer.transform.localPosition = new Vector3(0, newStats.groundOffset, 0);
			spriteRenderer.sprite = sprite;
			grandOctiBoss.SetActive(false);
			stats ??= new EnemyStats();
			stats.Copy(newStats.stats);
			dropOnDestroyComponent.ClearDrop();

			if (newStats.ExpDrop > 0)
			{
				expDropChance ??= new ChanceDrop()
				{
					pickupObject = expDrop,
				};
				expDropChance.amount = newStats.ExpDrop;
				expDropChance.chance = 0.75f;
				dropOnDestroyComponent.AddDrop(expDropChance);
			}

			if (newStats.isBossEnemy)
			{
				_isBossEnemy = true;
				stats.hp *= playerTarget?.GetLevel() ?? 1;
				
				dropOnDestroyComponent.AddDrop(chestDropChance ??= new ChanceDrop()
				{
					chance = 1,
					pickupObject = chestDrop,
				});
				dropOnDestroyComponent.AddDrop(expDropChance);
			}

			goldDropChance ??= new ChanceDrop()
			{
				pickupObject = goldDrop,
			};
			goldDropChance.chance = (playerTarget?.playerStatsComponent?.GetLuck() ?? 0) / 16;
			goldDropChance.amount = Random.Range(1, 25);
			dropOnDestroyComponent.AddDrop(goldDropChance);
			
			gemDropChance ??= new ChanceDrop()
			{
				pickupObject = gemDrop,
			};
			gemDropChance.chance = (playerTarget?.playerStatsComponent?.GetLuck() ?? 0) / 100;
			gemDropChance.amount = Random.Range(1, 50);
			dropOnDestroyComponent.AddDrop(gemDropChance);
			
			stats.hp = (int)(stats.hp * healthMultiplier * playerStats.GetEnemyHealthIncrease());
			stats.speed *= playerStats.GetEnemySpeedIncrease();
			chaseComponent.FollowYAxis = newStats.allowFlying;
			SetChaseTarget(target.gameObject);
		}

		public void SetupBoss()
		{
			grandOctiBoss.SetActive(true);
		}

		private void SetChaseTarget(GameObject target)
		{
			targetGameObject = target;
			chaseComponent.SetTarget(target);
			chaseComponent.SetSpeed(stats.speed);
			damageableComponent.SetHealth(stats.hp);
			damageableComponent.SetResistances(stats.elementStats);
		}

		private void Update()
		{
			chaseComponent.SetMovementState(_enemyManager.IsTimeStop());
			_timeAlive += Time.deltaTime;
			
			if (_currentDamageCooldown > 0)
				_currentDamageCooldown -= Time.deltaTime;
			if (_timeAlive > 90 && !_isBossEnemy)
			{
				_enemyManager.Despawn(this);
			}
			
			if (damageableComponent.IsDestroyed())
				Die();

			if (_removeCollisionsTimer > 0)
			{
				_isRemoveCollisions = true;
				_removeCollisionsTimer -= Time.deltaTime;
			}
			else if (_removeCollisionsTimer <= 0 && _isRemoveCollisions)
			{
				_isRemoveCollisions = false;
				RevertIgnoredCollisions();
			}
		}

		private void Die()
		{
			gameResultData.MonstersKilled++;
			EnemyDiedEvent.Invoke();
			dropOnDestroyComponent.CheckDrop();
			_enemyManager.Despawn(this);
			AchievementManager.instance.OnEnemyKilled(this);
		}

		private void OnCollisionStay(Collision collisionInfo)
		{
			if (collisionInfo.gameObject.CompareTag("Player"))
			{
				var playerComponent = GameManager.instance.playerComponent;
				Attack(playerComponent);
			}
			else if (collisionInfo.gameObject.CompareTag("Enemy") && _isRemoveCollisions)
			{
				_ignoredEnemyColliders.Add(collisionInfo);
				Physics.IgnoreCollision(collisionInfo.collider, GetComponent<CapsuleCollider>(), true);
			}
		}
		
		private void RevertIgnoredCollisions()
		{
			var collider = capsuleCollider;
			for (var index = 0; index < _ignoredEnemyColliders.Count; index++)
			{
				var ignoredEnemyCollider = _ignoredEnemyColliders[index];
				if (ignoredEnemyCollider?.collider != null && collider != null)
					Physics.IgnoreCollision(ignoredEnemyCollider.collider, collider, false);
				
				_ignoredEnemyColliders.RemoveAt(index--);
			}

			_ignoredEnemyColliders.Clear();
		}

		private void Attack(Player player)
		{
			if (_currentDamageCooldown > 0 || _isRemoveCollisions)
				return;

			_currentDamageCooldown = _damageCooldown;
			player.TakeDamage(stats.damage * (1 + Math.Max(_damageReduction, -1)));
		}

		public void SetNoCollisions(float timer)
		{
			if (_removeCollisionsTimer < timer)
				_removeCollisionsTimer = timer;
		}

		public void AddDamageReduction(float amount)
		{
			_damageReduction -= amount;
		}

		public Damageable GetDamagableComponent()
		{
			return damageableComponent;
		}

		public ChaseComponent GetChaseComponent()
		{
			return chaseComponent;
		}
	}
}