using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Character
{
    private float currentMovementSpeed;

    public float attackSpeed;
    private float currentAttackSpeed;

    public float attackDistance;
    public int attackDamage;

    private bool attacking;
    public GameObject attackAsset;

    private bool defending;

    public float defendReflectTime;
    private float currentDefendReflectTime;
    public float defendMovementSpeedModifier;

    public int maxDamageStored;
    private float currentDamageStored;
    public float minSizeStored;
    public float maxSizeStored;
    public float timeUntilDissipation;
    private float currentTimeUntilDissipation;
    public float dissipationSpeed;

    public GameObject projectileAsset;
    public ParticleSystem energyStoredObject;
    private float originalRate;
    private ParticleSystem.EmissionModule energyStoredObjectEmission;

    public GameObject shieldObject;

    public bool reflect;

    protected override void Awake()
    {
        base.Awake();

        energyStoredObjectEmission = energyStoredObject.emission;

        originalRate = energyStoredObjectEmission.rateOverTimeMultiplier;
        energyStoredObjectEmission.rateOverTimeMultiplier = 0.0f;
    }

    private void BeforeUpdate()
    {
        if(defending)
            currentMovementSpeed = movementSpeed * defendMovementSpeedModifier;
    }

    protected override void Update()
    {
        BeforeUpdate();

        base.Update();

        if (!incapacitated)
        {
            UpdateDefend();
        }

        AfterUpdate();
    }

    private void AfterUpdate()
    {
        currentMovementSpeed = movementSpeed;
    }

    private void UpdateDefend()
    {
        float horizontalAim = player.GetAxis("Horizontal Aim");
        float verticalAim = player.GetAxis("Vertical Aim");
        if (player.GetPositiveAxis("Right Bumper", 0.5f) && (!Mathf.Approximately(horizontalAim, 0.0f) || !Mathf.Approximately(verticalAim, 0.0f)))
        {
            defending = true;

            currentDefendReflectTime += Time.deltaTime;

            shieldObject.SetActive(true);

            Vector3 direction = new Vector3(horizontalAim, verticalAim, 0.0f).normalized;

            shieldObject.transform.rotation = Quaternion.identity;
            shieldObject.transform.Rotate(0.0f, 0.0f, Vector3.SignedAngle(Vector3.up, direction, Vector3.forward));

            DissipateShieldEnergyStorage();
        }
        else
        {
            if(currentDamageStored > 0.0f)
            {
                GameObject projectile = Instantiate(projectileAsset, shieldObject.transform.position, shieldObject.transform.rotation);
                projectile.transform.localScale = Vector3.Lerp(Vector3.one * minSizeStored, Vector3.one * maxSizeStored, currentDamageStored / maxDamageStored);
                projectile.GetComponent<Damage>().damage = Mathf.CeilToInt(currentDamageStored);

                currentDamageStored = 0;

                DissipateShieldEnergyStorage();
            }

            defending = false;
            currentDefendReflectTime = 0.0f;

            shieldObject.SetActive(false);
        }
    }

    private void DissipateShieldEnergyStorage()
    {
        currentTimeUntilDissipation += Time.deltaTime;
        if(currentTimeUntilDissipation >= timeUntilDissipation)
        {
            currentDamageStored = Mathf.Max(0.0f, currentDamageStored - dissipationSpeed * Time.deltaTime);
        }

        energyStoredObjectEmission.rateOverTimeMultiplier = (currentDamageStored / maxDamageStored) * originalRate;
    }

    protected override void UpdateAttack()
    {
        /*currentAttackSpeed += Time.deltaTime;

        if(player.GetPositiveAxis("Right Bumper", 0.5f))
        {
            if(currentAttackSpeed >= attackSpeed)
            {
                currentAttackSpeed = 0.0f;
                attacking = false;

                GameObject attackEffect = Instantiate(attackAsset, new Vector3(transform.position.x + lastMovementDirection * attackDistance, transform.position.y + Random.Range(-attackDistance, attackDistance), 0.0f), Quaternion.identity) as GameObject;
                attackEffect.transform.Rotate(0.0f, 0.0f, Random.Range(-60.0f, 60.0f));

                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackDistance, attackLayer);
                if (colliders.Length > 0)
                {
                    foreach(Collider2D iCollider in colliders)
                    {
                        if((lastMovementDirection > 0 && Vector3.Dot((iCollider.transform.position - transform.position).normalized, Vector3.right) > 0.0f)
                            || (lastMovementDirection < 0 && Vector3.Dot((iCollider.transform.position - transform.position).normalized, Vector3.left) > 0.0f))
                        {
                            attacking = true;

                            Boss boss = iCollider.gameObject.GetComponent<Boss>();
                            if (boss)
                            {
                                boss.OnHit(attackDamage);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            attacking = false;
        }*/
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        Damage damage = collider.gameObject.GetComponent<Damage>();
        if (damage != null)
        {
            OnHit(damage.damage);
        }
    }

    public override void OnHit(int damage)
    {
        if (defending)
        {
            // instead of doing this, put it on the shield to handle all shield specific collisions?
            // reflect or store attack points to unleash on next attack?
            if (reflect)
            {
                if (currentDefendReflectTime <= defendReflectTime)
                {
                    // reflect projectiles and the like
                }
            }
            else
            {
                currentDamageStored = Mathf.Min(currentDamageStored + damage, maxDamageStored);

                currentTimeUntilDissipation = 0.0f;
            }
        }
        else
        {
            base.OnHit(damage);
        }
    }
}