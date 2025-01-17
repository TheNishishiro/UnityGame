﻿using System;
using Interfaces;
using Managers;
using NaughtyAttributes;
using Objects.Drops.ExpDrop;
using Objects.Players.Scripts;
using Objects.Stage;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Objects.Drops
{
	public class PickupBase : NetworkBehaviour
	{
		[SerializeField] protected PickupObject pickUpObject;
		[SerializeField] protected PickupEnum PickupEnum;
		[SerializeField] public Collider boxCollider;
		private Transform _cachedTransform;
		protected bool IsFollowingPlayer;
		private const float Speed = 20f;
		[SerializeField] private bool isStationary;
		[SerializeField] protected bool canExpire;
		[ShowIf("canExpire")]
		[SerializeField] private float lifeTime;
		protected float _currentLifeTime;
		protected int updateEveryFrameCount;

		protected void Init()
		{
			_cachedTransform = transform;
			updateEveryFrameCount = Random.Range(2, 15);
		}

		public void Reset()
		{
			IsFollowingPlayer = false;
			_currentLifeTime = lifeTime;
		}

		protected void FollowPlayerWhenClose()
		{
			if (isStationary)
				return;
			
			if (!IsFollowingPlayer)
			{
				if (Time.frameCount % updateEveryFrameCount != 0)
					return;
				var distance = Vector3.Distance(GameManager.instance.PlayerTransform.position, _cachedTransform.position);
				if (distance < PlayerStatsScaler.GetScaler().GetMagnetSize() * GameData.GetCurrentDifficulty().ItemAttractionModifier)
					IsFollowingPlayer = true;
				return;
			}
		
			_cachedTransform.position = Vector3.MoveTowards(_cachedTransform.position, GameManager.instance.PlayerTransform.position, Speed * Time.deltaTime);
		}
		
		protected void OnCollision(Collider col)
		{
			if (col.gameObject.CompareTag("Player") && col.gameObject.GetComponent<NetworkObject>()?.IsOwner == true)
			{
				pickUpObject?.OnPickUp(GameManager.instance.playerComponent);
				AchievementManager.instance.OnPickupCollected(PickupEnum);
				Destroy();
			}
		}

		protected virtual void Destroy()
		{
			Destroy(gameObject);
		}
	}
}