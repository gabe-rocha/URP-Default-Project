using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWalking : IState
{
    Player player;
    public PlayerStateWalking(Player player)
    {
        this.player = player;
    }

    public void OnEnter()
    {
        player.AnimPlayer.Play("Player Run");
    }

    public void OnExit()
    {
        player.Rb.velocity = Vector2.zero;
    }

    public IState Tick()
    {
        Walk();

        if(IsEnemyInRange()){
            return player.StateAttackingClosestEnemy;
        }
        else{
            return this;
        }
    }

    private void Walk()
    {
        if(player.CurrentEnemyTarget != null){
            if(player.CurrentEnemyTarget.transform.position.x < player.Position.x){
                //walk to the left
                player.Rb.velocity = new Vector2( -player.DefaultWalkSpeed, player.Rb.velocity.y);
                player.PlayerModel.localScale.Set(-player.PlayerModel.localScale.x,player.PlayerModel.localScale.y,player.PlayerModel.localScale.z);
            }else{
                //Walk to the right
                player.Rb.velocity = new Vector2(player.DefaultWalkSpeed, player.Rb.velocity.y);    
                player.PlayerModel.localScale.Set(player.PlayerModel.localScale.x,player.PlayerModel.localScale.y,player.PlayerModel.localScale.z);
            }
        }
        else{
            //Walk to the right
            player.Rb.velocity = new Vector2(player.DefaultWalkSpeed, player.Rb.velocity.y);
            player.PlayerModel.localScale.Set(player.PlayerModel.localScale.x,player.PlayerModel.localScale.y,player.PlayerModel.localScale.z);
        }
    }
    private bool IsEnemyInRange()
    {
        return Physics2D.OverlapCircleAll(player.Position, player.WeaponRange, player.WhatIsEnemy).Length > 0;
    }
     
}