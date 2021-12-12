using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public enum States { Alive,Die}
    //Require Components
    [SerializeField] private InputActionAsset actionAsset;
    public Bow bow;
    public Arrow arrow;
    public States playerState;
    public BodyCollision playerCollider;
    public LayerMask enemyHitBoxes;

    //Require UI Components
    public GameObject playerUI;
    public Text HP;
    public Text levelText;
    public Text XP;
    public Slider hpSlider;
    public Slider xpSlider;


    //public and private variables
    public float additionHitDamage;
    public float Health { get => health; set => health = value; }

    [SerializeField] private int level;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health;
    [SerializeField] private float xp = 0f;
    [SerializeField] private float maxXP = 100f;
    [SerializeField] private float baseHitDamage;
    [SerializeField] private float hitDamage;
    private bool openMenu = false;
    private bool takingDamage = false;


    private void Start()
    {
        maxHealth = 100f + (level * 0.1f * 100f);
        health = maxHealth;
        baseHitDamage = 2f;
        additionHitDamage = bow.damage + arrow.damage;

        //UI action
        var activate = actionAsset.FindActionMap("XRI LeftHand").FindAction("PlayerUI");
        activate.Enable();
        activate.performed += OnMenuActivate;

        //Hit Damage Range
        hitDamage = Random.Range(baseHitDamage + (additionHitDamage/2), baseHitDamage + additionHitDamage);
    }

    private void Update()
    {
        CheckHealth();
        HP.text = health + "/" + maxHealth;
        hpSlider.value = CalculateHealth();
        levelText.text = "Level: " + level;

    }

    public float CalculateHealth()
    {
        return health / maxHealth;
    }

    //Check Health of Player
    public void CheckHealth()
    {
        playerState = (health <= 0) ?  States.Die : States.Alive;      
    }

    private void OnMenuActivate(InputAction.CallbackContext context)
    {
        Debug.Log("hitting");
        if (!openMenu)
        {
            playerUI.SetActive(true);
            openMenu = true;
        }
        else
        {
            playerUI.SetActive(false);
            openMenu = false;
        }
    }

    private void TakeDamage(float sec,float damage)
    {
        takingDamage = true;
        StartCoroutine(TakingDamageBySec(sec, damage));
        Debug.Log("Attacking");
    }

    private IEnumerator TakingDamageBySec(float sec, float damage)
    {
        yield return new WaitForSeconds(sec);
        health -= damage;
        takingDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 6f);
    }

}
