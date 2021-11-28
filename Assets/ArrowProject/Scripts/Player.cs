using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum States { Alive,Die}
    //Require Components
    public Bow bow;
    public Arrow arrow;
    public States playerState;

    public float additionHitDamage;
    private float health;
    private float baseHitDamage;
    private float hitDamage;

    private void Start()
    {
        health = 100f;
        baseHitDamage = 2f;
        additionHitDamage = bow.damage + arrow.damage;
        
        //Hit Damage Range
        hitDamage = Random.Range(baseHitDamage + (additionHitDamage/2), baseHitDamage + additionHitDamage);
    }

    private void FixedUpdate()
    {
        CheckHealth();

    }

    //Check Health of Player
    public void CheckHealth()
    {
        playerState = (health <= 0) ?  States.Die : States.Alive;      
    }


}
