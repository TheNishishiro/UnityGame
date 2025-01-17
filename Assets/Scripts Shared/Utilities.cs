﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DefaultNamespace.Data.Achievements;
using Objects.Enemies;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
	public static class Utilities
	{
		public static string GetShortNumberFormatted(float number)
		{
			var suffixes = new[] {"", "K", "M", "G", "T", "P", "E", "Z", "Y", "R", "Q"};
			var suffixIndex = 0;
			var damage = number;
			while (damage > 1000)
			{
				damage /= 1000;
				suffixIndex++;
			}
			return $"{damage:0.##} {suffixes[suffixIndex]}";
		}

		public static string StatToString(float number, float rarityFactor = 0, bool isPercent = false, bool isInvertFactor = false, bool allowNegatives = false)
		{
			number = allowNegatives ? number : Math.Abs(number);
			string value;
			if (rarityFactor == 0 || Math.Abs(rarityFactor - 1) < 0.0001)
			{
				value = (isPercent ? number * 100 : number).ToString("0.00");
			}
			else if (!isInvertFactor)
			{
				value = (isPercent ? (number * rarityFactor) * 100 : number * rarityFactor).ToString("0.00");
			}
			else
			{
				value = (isPercent ? (number * (2-rarityFactor)) * 100 : number * (2-rarityFactor)).ToString("0.00");
			}
			
			return value.Replace(",", ".").TrimEnd('0').TrimEnd('.');
		}

		public static float AdjustForRarity(float value, float rarityFactor)
		{
			return value > 0 ? value * (2 - rarityFactor) : value * rarityFactor;
		}
		
		public static float RandomDoubleRange(float min1, float max1, float min2, float max2)
		{
			return Random.value < 0.5f ? Random.Range(min1, max1) : Random.Range(min2, max2);
		}
		
		public static IEnumerable<T> EnumToEnumerable<T> ()
		{
			return Enum.GetValues (typeof (T)).Cast<T>();
		}
		
		public static T RandomEnumValue<T> ()
		{
			var v = Enum.GetValues (typeof (T));
			return (T) v.GetValue (Random.Range(0,v.Length));
		}
		
		public static List<T> RandomEnumValues<T> (int takeAmount)
		{
			return Enum.GetValues(typeof(T)).Cast<T>().OrderBy(_ => Random.value).Take(takeAmount).ToList();
		}
		
		public static Color HexToColor(string hexColor)
		{
			// Remove the '#' character from the start if it exists
			if (hexColor.IndexOf('#') != -1)
				hexColor = hexColor.Replace("#", "");

			var r = byte.Parse(hexColor.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
			var g = byte.Parse(hexColor.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
			var b = byte.Parse(hexColor.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

			return new Color32(r, g, b, 255); // Alpha = 255 (opaque)
		}

		public static string FloatToTimeString(float time)
		{
			var timeSpan = GetTimeSpan(time);
			return $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
		}
		
		public static TimeSpan GetTimeSpan(float time)
		{
			var minutes = (int)time / 60;
			var seconds = (int)time % 60;
			return new TimeSpan(0, 0, minutes, seconds);
		}
		
		public static Texture2D CovertSteamAvatar(this Image image )
		{
			var avatar = new Texture2D( (int)image.Width, (int)image.Height, TextureFormat.ARGB32, false )
				{
					filterMode = FilterMode.Trilinear
				};

			// Flip image
			for ( var x = 0; x < image.Width; x++ )
			{
				for ( var y = 0; y < image.Height; y++ )
				{
					var p = image.GetPixel( x, y );
					avatar.SetPixel( x, (int)image.Height - y, new Color( p.r / 255.0f, p.g / 255.0f, p.b / 255.0f, p.a / 255.0f ) );
				}
			}
	
			avatar.Apply();
			return avatar;
		}
		
		public static Vector3 GetRandomInArea(Vector3 position, float size)
		{
			return new Vector3(
				Random.Range(position.x - size, position.x + size),
				Random.Range(position.y - size, position.y + size),
				Random.Range(position.z - size, position.z + size));
		}

		public static Vector3 GetRandomInAreaFreezeParameter(Vector3 position, float size, bool isFreezeX = false, bool isFreezeY = false, bool isFreezeZ = false)
		{
			return new Vector3(
				isFreezeX ? position.x : Random.Range(position.x - size, position.x + size),
				isFreezeY ? position.y : Random.Range(position.y - size, position.y + size),
				isFreezeZ ? position.z : Random.Range(position.z - size, position.z + size));
		}

		public static Vector3 GetPointOnColliderSurface(Vector3 point, Transform transform, float heightOffset = 0)
		{
			var isFound = GetPointOnColliderSurface(point, 1000f, transform, out var pointSurface);
			pointSurface.y += heightOffset;
			return isFound ? pointSurface : point;
		}

		public static Vector3 GetRandomPointOnCollider(Collider collider)
		{
			if (collider == null)
				return Vector3.zero;
			
			var colliderBounds = collider.bounds;
			var minBounds = new Vector3(colliderBounds.min.x, colliderBounds.min.y, colliderBounds.min.z);
			var maxBounds = new Vector3(colliderBounds.max.x, colliderBounds.max.y, colliderBounds.max.z);

			return new Vector3(Random.Range(minBounds.x, maxBounds.x), Random.Range(minBounds.y, maxBounds.y), Random.Range(minBounds.z, maxBounds.z));
		}
		
		public static bool GetPointOnColliderSurface(Vector3 point, float surfaceHigh, Transform transform, out Vector3 pointSurface)
		{
			var pointOnSurface = Vector3.zero;
			var pointFound = false;
			if (Physics.Raycast(point - surfaceHigh * transform.up, transform.up, out var hit, Mathf.Infinity, LayerMask.GetMask("FloorLayer")))
			{
				pointOnSurface = hit.point;
				pointFound = true;
			}
			else
			{
				if (Physics.Raycast(point + surfaceHigh * transform.up, -transform.up, out hit, Mathf.Infinity, LayerMask.GetMask("FloorLayer")))
				{
					pointOnSurface = hit.point;
					pointFound = true;
				}
			}
 
			pointSurface = pointOnSurface;
			return pointFound;
		}

		public static Enemy FindClosestEnemy(Vector3 position, IEnumerable<Enemy> enemies, out float distanceToClosest)
		{
			distanceToClosest = Mathf.Infinity;
			Enemy closest = null;

			foreach (var enemy in enemies)
			{
				var distance = Vector3.Distance(position, enemy.transform.position);
				if (distance < distanceToClosest)
				{
					distanceToClosest = distance;
					closest = enemy;
				}
			}

			return closest;
		}
		
		public static IEnumerable<Damageable> GetEnemiesInArea(Vector3 position, float radius, IEnumerable<Damageable> enemies)
		{
			var enemiesInArea = new List<Damageable>();
			foreach (var enemy in enemies)
			{
				var distance = Vector3.Distance(position, enemy.transform.position);
				if (distance < radius)
				{
					enemiesInArea.Add(enemy);
				}
			}

			return enemiesInArea;
		}

		private static readonly Collider[] EnemiesDetected = new Collider[30];
		public static IEnumerable<Enemy> GetEnemiesInAreaNonAlloc(Vector3 position, float radius)
		{
			var layer = LayerMask.GetMask("EnemyLayer");
			var size = Physics.OverlapSphereNonAlloc(position, radius, EnemiesDetected, layer);
			return EnemiesDetected.Take(size).Select(x => x.GetComponent<Enemy>()).ToList();
		}

		public static Damageable FindClosestUniqueDamageable(Vector3 position, IEnumerable<Damageable> enemies, List<Damageable> foundDamagables, out float distanceToClosest)
		{
			distanceToClosest = Mathf.Infinity;
			Damageable closest = null;

			foreach (var enemy in enemies)
			{
				var distance = Vector3.Distance(position, enemy.transform.position);
				if (distance < distanceToClosest && !foundDamagables.Contains(enemy))
				{
					distanceToClosest = distance;
					closest = enemy;
				}
			}

			return closest;
		}

		public static Vector3 GenerateRandomPositionOnEdge(Vector2 spawnArea)
		{
			var position = new Vector3();

			var f = Random.value > 0.5f ? -1f : 1f;
			if (Random.value > 0.5f)
			{
				position.x = Random.Range(-spawnArea.x, spawnArea.x);
				position.z = spawnArea.y * f;
			}
			else
			{
				position.z = Random.Range(-spawnArea.y, spawnArea.y);
				position.x = spawnArea.x * f;
			}

			return position;
		}

		public static bool IsPositionOccupied(Vector3 position, float radius = 1f, string layer = "Default")
		{
			var positionOccupied = Physics.CheckSphere(position, radius, LayerMask.GetMask(layer));
			return positionOccupied;
		}
		
		public static bool IsWithinCameraView(Camera camera, Bounds bounds)
		{
			var planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, bounds);
		}
		
		public static bool IsWithinCameraView(Camera camera, Bounds bounds, Vector3 cameraPosition, Vector3 objectPosition, float maxDistance)
		{
			if (!IsWithinCameraView(camera, bounds)) 
				return false;
			
			var distanceToPlayer = Vector3.Distance(cameraPosition, objectPosition);
			return distanceToPlayer <= maxDistance;
		}
		
		public static Vector3 GetWorldPositionFromUI(Vector3 uiPosition)
		{
			return Camera.current.ScreenToWorldPoint(uiPosition);
		}

		public static List<Vector3> GetPositionsOnSurfaceWithMinDistance(int amount, Vector3 center, float spawnRange, float minDistance, Transform transform, int maxAttempts)
		{
			var availablePositions = new List<Vector3>();
			for (int i = 0; i < amount; i++)
			{
				Vector3 position;
				bool isValidPosition = false;
				int attempts = 0;
				do
				{
					position = Utilities.GetRandomInAreaFreezeParameter(center, spawnRange, isFreezeY: true);
					position = Utilities.GetPointOnColliderSurface(position, transform);

					// Calculate the distance between new position and existing positions
					if (availablePositions.Count == 0 || availablePositions.All(spawnedPos => Vector3.Distance(spawnedPos, position) >= minDistance))
					{
						isValidPosition = true;
					}
					else
					{
						attempts++;
					}
				} while (!isValidPosition && attempts < maxAttempts); //Max attempts to try 

				if (isValidPosition)
				{
					availablePositions.Add(position);
				}
			}

			return availablePositions;
		}

		public static Color RarityToColor(Rarity rarity)
		{
			return rarity switch
			{
				Rarity.None => Color.white,
				Rarity.Common => Color.green,
				Rarity.Rare => Color.cyan,
				Rarity.Legendary => Color.yellow,
				Rarity.Mythic => Color.red,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public static bool IsServer(this NetworkBehaviour networkBehaviour)
		{
			return networkBehaviour.IsHost || !networkBehaviour.IsSpawned;
		}
	}
}