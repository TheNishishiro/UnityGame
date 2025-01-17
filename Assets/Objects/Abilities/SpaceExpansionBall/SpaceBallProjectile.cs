﻿using System.Collections;
using Interfaces;
using UnityEngine;
using Weapons;

namespace Objects.Abilities.SpaceExpansionBall
{
	public class SpaceBallProjectile : PoolableProjectile<SpaceBallProjectile>
	{
		private int enemiesHit = 0;
		private Vector3 direction;
		private enum State
		{
			Traveling,
			Exploding,
			Developed
		}
		private State state = State.Traveling;
		private SpaceBallWeapon SpaceBallWeapon => ParentWeapon as SpaceBallWeapon;

		public override void SetStats(IWeaponStatsStrategy weaponStatsStrategy)
		{
			base.SetStats(weaponStatsStrategy);
			state = State.Traveling;
			enemiesHit = 0;
		}

		public void SetDirection(float dirX, float dirY, float dirZ)
		{
			direction = (new Vector3(dirX, dirY, dirZ) - transformCache.position).normalized;
			direction.y = 0;
		}

		protected override void CustomUpdate()
		{
			if (state == State.Traveling)
				transformCache.position += direction * (WeaponStatsStrategy.GetSpeed() * Time.deltaTime);
			
			if ((CurrentTimeToLive <= WeaponStatsStrategy.GetTotalTimeToLive() / 2 || enemiesHit > WeaponStatsStrategy.GetPassThroughCount()) && state == State.Traveling)
			{
				state = State.Exploding;
				transformCache.localScale *= WeaponStatsStrategy.GetScale();
				ProjectileDamageIncreasePercentage = 0.5f;
				StartCoroutine(Enlarge());
			}
		}

		protected override void OnStateChanged(ProjectileState state)
		{
			if (state == ProjectileState.Dissipating)
			{
				StartCoroutine(Collapse());
			}
			base.OnStateChanged(state);
		}

		private IEnumerator Enlarge()
		{
			var increaseTimes = 0;
			while (state == State.Exploding)
			{
				if (increaseTimes >= 10)
					state = State.Developed;
				
				increaseTimes++;
				transformCache.localScale *= 1.1f;
				transformCache.localPosition.Scale(new Vector3(0,1.1f,0));
				yield return new WaitForSeconds(0.1f);
			}
		}

		private IEnumerator Collapse()
		{
			// Collapse object on itself
			var scale = transformCache.localScale;
			while (scale.x > 0)
			{
				scale.x -= 0.2f;
				scale.y -= 0.2f;
				scale.z -= 0.2f;
				transformCache.localScale = scale;
				yield return new WaitForSeconds(0.01f);
			}
			
			// Spawn explosion
			if (SpaceBallWeapon.IsGallacticCollapse)
			{
				SpaceBallWeapon.SpawnSubProjectile(transformCache.position);
				yield return new WaitForSeconds(1f);
			}

			base.Destroy();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Enemy"))
				enemiesHit++;
			
			if (state == State.Traveling)
				SimpleDamage(other, false);
			else
				DamageArea(other, out _);
			
		}
	}
}