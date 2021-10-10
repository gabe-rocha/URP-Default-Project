using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateAttackingClosestEnemy : IState {
    Player player;
    float attackStartTime, weaponAttacksPerSecond, weaponDamage, attack1AnimationLength;

    private bool isAttackAnimationRunning = false;

    public PlayerStateAttackingClosestEnemy (Player player) {
        this.player = player;

        foreach (var animationClip in player.AnimPlayer.runtimeAnimatorController.animationClips) {
            switch (animationClip.name) {
                case "HeroKnight_Attack1":
                    attack1AnimationLength = animationClip.length;
                    break;
                default:
                    break;
            }
        }
    }

    public void OnEnter () {
        attackStartTime = float.NegativeInfinity;
        weaponAttacksPerSecond = player.WeaponAttacksPerSecond;
        weaponDamage = player.WeaponDamage;
    }

    public void OnExit () { }

    public IState Tick () {

        TargetClosestEnemy ();
        AttackTarget ();

        if (player.CurrentEnemyTarget != null) {
            return this;
        } else {
            if (isAttackAnimationRunning) {
                return this;
            } else {
                return player.StateWalking;
            }
        }
    }

    private void AttackTarget () {
        if (player.CurrentEnemyTarget != null) {
            if (Time.time > attackStartTime + 1f / weaponAttacksPerSecond) { //1f is not a magical number, it just means 1 second
                attackStartTime = Time.time;

                player.AnimPlayer.Play ("Player Attack1");
                isAttackAnimationRunning = true;
                Debug.Log ("Player Attacked");

                var origin = player.Position;
                var range = player.WeaponRange;
                var direction = player.CurrentEnemyTarget.transform.position - player.Position;
                var enemyMask = player.WhatIsEnemy;
                Collider2D collider = Physics2D.CircleCast (origin, range, direction, range, enemyMask).collider;
                if (collider != null) {
                    player.CurrentEnemyTarget.ApplyDamage (weaponDamage);
                }
            }
        }

        if (Time.time > attackStartTime + attack1AnimationLength) {
            isAttackAnimationRunning = false;
            player.AnimPlayer.Play ("Player Idle");
        }
    }
    private void TargetClosestEnemy () {
        var listOfEnemies = Physics2D.OverlapCircleAll (player.Position, player.WeaponRange, player.WhatIsEnemy);
        if (listOfEnemies.Length == 0) {
            player.CurrentEnemyTarget = null;
            return;
        }

        var closestEnemyDistance = float.PositiveInfinity;
        for (int i = 0; i < listOfEnemies.Length; i++) {
            Enemy enemy = listOfEnemies[i].gameObject.GetComponent<Enemy> ();

            var distance = Vector2.Distance (player.Position, enemy.transform.position);
            if (distance < closestEnemyDistance) {
                closestEnemyDistance = distance;
                player.CurrentEnemyTarget = enemy;
            }
        }
    }
}