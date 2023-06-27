﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Managers;
using Objects.Enemies;
using Objects.Players.Scripts;
using UnityEngine;
using Weapons;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] private GameObject enemyGameObject;
	[SerializeField] private List<EnemyData> defaultSpawns;
	[SerializeField] private Vector2 spawnArea;
	[SerializeField] private float spawnTimer;
	[SerializeField] private Player player;
	[SerializeField] private PlayerStatsComponent _playerStatsComponent;
	private int currentEnemyCount;
	private int enemyMinCount;
	private int enemyMaxCount = 300;
	private float _timer;
	private bool _isTimeStop;

	private void Start()
	{
		currentEnemyCount = 0;
		defaultSpawns = new List<EnemyData>();
	}

	private void Update()
	{
		_timer -= Time.deltaTime;

		if (_timer <= 0 || currentEnemyCount < enemyMinCount)
			SpawnEnemy();
	}

	private void SpawnEnemy()
	{
		if (!defaultSpawns.Any())
			return;
		
		_timer = spawnTimer * _playerStatsComponent.GetEnemySpawnRateIncrease();
		
		var randomSpawn = defaultSpawns[Random.Range(0, defaultSpawns.Count)];
		SpawnEnemy(randomSpawn);
	}

	public void SpawnEnemy(EnemyData enemyToSpawn)
	{
		var maxEnemyCount = enemyMaxCount * _playerStatsComponent.GetEnemyCountIncrease();
		if (currentEnemyCount >= maxEnemyCount && !enemyToSpawn.isBossEnemy)
			return;
		
		var position = player.transform.position - Utilities.GenerateRandomPositionOnEdge(spawnArea);
		var pointFound = Utilities.GetPointOnColliderSurface(position, 100f, player.transform, out var pointOnSurface);
		if (!pointFound)
			return;
		
		position = pointOnSurface;
		position.y += enemyToSpawn.animatedPrefab.GetComponent<BoxCollider>().size.y/2;
		var newEnemy = Instantiate(enemyGameObject);
		newEnemy.transform.position = position;
		var enemy = newEnemy.GetComponent<Enemy>();
		newEnemy.transform.parent = transform;

		var enemySprite = Instantiate(enemyToSpawn.animatedPrefab);
		enemySprite.transform.parent = newEnemy.transform;
		enemySprite.transform.localPosition = Vector3.zero;
		
		enemy.Setup(enemyToSpawn, player, this, _playerStatsComponent);
		currentEnemyCount++;
	}

	public void ChangeDefaultSpawn(List<EnemyData> enemyData)
	{
		defaultSpawns = enemyData;
	}

	public void ChangeSpawnRate(float timer)
	{
		spawnTimer = timer;
	}

	public void BurstSpawn(List<EnemyData> stageEventEnemies, float stageEventCount)
	{
		StartCoroutine(BurtSpawnCoroutine(stageEventEnemies, stageEventCount));
	}
	
	private IEnumerator BurtSpawnCoroutine(List<EnemyData> stageEventEnemies, float stageEventCount)
	{
		for (var i = 0; i < stageEventCount; i++)
		{
			var randomEnemy = stageEventEnemies[Random.Range(0, stageEventEnemies.Count)];
			SpawnEnemy(randomEnemy);
		}

		yield break;
	}

	public void ChangeMinimumEnemyCount(float stageEventMinCount)
	{
		enemyMinCount = (int)stageEventMinCount;
	}

	public void EnemyDespawn()
	{
		currentEnemyCount--;
	}

	public void EraseAllEnemies()
	{
		var enemies = GetComponentsInChildren<Enemy>();
		foreach (var enemy in enemies)
		{
			Destroy(enemy.gameObject);
		}
		currentEnemyCount = 0;
	}

	public void SetTimeStop(bool isTimeStop)
	{
		_isTimeStop = isTimeStop;
	}

	public bool IsTimeStop()
	{
		return _isTimeStop;
	}

	public void GlobalDamage(int damage, WeaponBase weapon)
	{
		var enemies = FindObjectsOfType<Damageable>();
		foreach (var enemy in enemies)
		{
			enemy.TakeDamage(damage, weapon);
		}
	}
}