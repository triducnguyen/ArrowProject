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
    public Text Gold;
    public Text AttackRange;
    public Slider hpSlider;
    public Slider xpSlider;


    //public and private variables
    public float additionHitDamage;
    public float Health { get => health; set => health = value; }
    public float Xperiences { get => xp; set => xp = value; }
    public float Golds { get => golds; set => golds = value; }

    [SerializeField] private int level;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float health;
    [SerializeField] private float xp = 0f;
    [SerializeField] private float golds;
    [SerializeField] private float maxXP = 20f;
    [SerializeField] private float minAttack;
    [SerializeField] private float maxAttack;
    private float baseAttack;
    private float hitDamage;
    private bool openMenu = false;
    private bool takingDamage = false;


    private void Start()
    {
        maxHealth = 100f + (level * 0.1f * 100f);
        health = maxHealth;
        baseAttack = 2f;
        minAttack = baseAttack;
        maxAttack = minAttack * 2;
        minAttack += bow.damage + arrow.damage;
        maxAttack += bow.damage + arrow.damage;
        

        //UI action
        var activate = actionAsset.FindActionMap("XRI LeftHand").FindAction("PlayerUI");
        activate.Enable();
        activate.performed += OnMenuActivate;

        //Hit Damage Range
        hitDamage = Random.Range((int)minAttack, (int)maxAttack);
    }

    private void Update()
    {
        CheckHealth();
        HP.text = health + "/" + maxHealth;
        hpSlider.value = CalculateHealth();
        XP.text = "XP: "+ xp + "/" + maxXP;
        xpSlider.value = CalculateXP();
        levelText.text = "Level: " + level;
        Gold.text = "Golds: " + golds;
        AttackRange.text = "Min Attack - Max Attack: " + minAttack + " - " + maxAttack;
        CheckLevelUp();
    }

    public float CalculateHealth()
    {
        return health / maxHealth;
    }

    public float CalculateXP()
    {
        return xp / maxXP;
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
    }

    private IEnumerator TakingDamageBySec(float sec, float damage)
    {
        yield return new WaitForSeconds(sec);
        health -= damage;
        takingDamage = false;
    }

    private void CheckLevelUp()
    {
        if(xp >= maxXP)
        {
            level++;
            maxXP *= 2;
            baseAttack *= 2;
            UpdateStats();
        }
    }

    private void UpdateStats()
    {
        minAttack = baseAttack;
        maxAttack = minAttack * 2;
        minAttack += bow.damage + arrow.damage;
        maxAttack += bow.damage + arrow.damage;
    }

    public int DealDamage()
    {
        return Random.Range((int)minAttack, (int)maxAttack);
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 6f);
    }

}
