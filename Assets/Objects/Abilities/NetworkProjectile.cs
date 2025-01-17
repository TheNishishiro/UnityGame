﻿using DefaultNamespace.Data.Weapons;
using Interfaces;
using Managers;
using Objects.Abilities.Katana;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Weapons;

namespace Objects.Abilities
{
    [RequireComponent(typeof(ClientNetworkTransform))]
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkProjectile : NetworkBehaviour
    {
        [SerializeField] public StagableProjectile projectile;
        [SerializeField] public NetworkObject networkObject;
        [SerializeField] private NetworkTransform networkTransport;
        public WeaponPoolEnum DesignedPoolId => projectile.projectileTypeId;
        private NetworkVariable<ProjectileState> networkProjectileState = new (ProjectileState.Unspecified, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            projectile.StopAllStages();
            if (GameManager.instance.saveFile.ConfigurationFile.RenderCoopProjectiles)
                projectile.ToggleStage(networkProjectileState.Value);
            networkProjectileState.OnValueChanged += OnValueChanged;
            base.OnNetworkSpawn();
        }

        public IWeapon GetParentWeapon()
        {
            return projectile.GetParentWeapon();
        }

        public override void OnNetworkDespawn()
        {
            networkProjectileState.OnValueChanged -= OnValueChanged;
            base.OnNetworkDespawn();
        }

        private void OnValueChanged(ProjectileState prevState, ProjectileState newState)
        {
            if (prevState != newState && !IsOwner && GameManager.instance.saveFile.ConfigurationFile.RenderCoopProjectiles)
            {
                projectile.StopAllStages();
                projectile.ToggleStage(newState);
            }
        }

        public void Despawn(WeaponEnum weaponId)
        {
            RpcManager.instance.DespawnProjectileRpc(networkObject, weaponId, DesignedPoolId);
        }

        public void Initialize(IWeapon weapon, Vector3 position, bool activateLast = true)
        {
            transform.position = position;
            projectile.Init();
            
            if (!activateLast)
                projectile.gameObject.SetActive(true);
            
            projectile.SetParentWeapon(weapon);
            
            if (activateLast)
                projectile.gameObject.SetActive(true);
        }

        public void ParentToPlayer()
        {
            RpcManager.instance.ParentObjectRpc(this, NetworkManager.Singleton.LocalClientId);
        }

        public void Activate()
        {
            projectile.gameObject.SetActive(true);
        }

        public T GetProjectile<T>() where T : StagableProjectile
        {
            return (T)projectile;
        }

        public void SetNewState(ProjectileState projectileState)
        {
            if (IsOwner)
                networkProjectileState.Value = projectileState;
        }
    }
}