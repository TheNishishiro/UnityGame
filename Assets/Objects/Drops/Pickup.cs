﻿using DefaultNamespace;
using Interfaces;
using Managers;
using Objects.Drops;
using Objects.Drops.ExpDrop;
using Objects.Players.Scripts;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Pickup : PickupBase
{
	[SerializeField] private PickupObject pickUpObject;
	public PickupEnum PickupType => PickupEnum;
	public bool canBeAttracted;
	
	private void Start()
	{
		Init();
	}

	private void Update()
	{
		FollowPlayerWhenClose();
	}

	private void OnTriggerEnter(Collider col)
	{
		OnCollision(col);
	}

	public void SetIsFollowingPlayer(bool isFollowingPlayer)
	{
		IsFollowingPlayer = canBeAttracted && isFollowingPlayer;
	}

	public void SetAmount(int amount)
	{
		if (pickUpObject != null)
			pickUpObject.SetAmount(amount);
	}

	protected override void Destroy()
	{
		PickupManager.instance.DestroyPickup(this);
	}
}