using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Full : HealthManager
{
    public GameObject healthAsset;
    private GameObject healthObject;

    private int totalHealth;

    public float visualPaddingX;
    public float visualPaddingY;

    public override void Init(int health)
    {
        totalHealth = health;

        if(healthObject == null)
        {
            healthObject = Instantiate(healthAsset, transform);
            RectTransform healthTransform = healthObject.GetComponent<RectTransform>();
            healthTransform.anchorMin = new Vector2(visualPaddingX, visualPaddingY);
            healthTransform.anchorMax = new Vector2(1 - visualPaddingX, 1 - visualPaddingY);
        }
    }

    public override void SetHealth(int health)
    {
        healthObject.GetComponent<Image>().fillAmount = (float)health / totalHealth;
    }
}