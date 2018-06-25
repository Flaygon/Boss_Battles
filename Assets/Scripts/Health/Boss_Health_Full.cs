using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Health_Full : HealthManager
{
    public GameObject healthAsset;
    private GameObject healthObject;

    public float visualPaddingX;
    public float visualPaddingY;

    public override void Init(int startHealth)
    {
        if(healthObject == null)
        {
            healthObject = Instantiate(healthAsset, transform);
            RectTransform healthTransform = healthObject.GetComponent<RectTransform>();
            healthTransform.anchorMin = new Vector2(visualPaddingX, visualPaddingY);
            healthTransform.anchorMax = new Vector2(1 - visualPaddingX, 1 - visualPaddingY);
        }

        UpdateHealth(startHealth, startHealth, 0);
    }

    public override void UpdateHealth(int currentHealth, int totalHealth, int entityIndex)
    {
        healthObject.GetComponent<Image>().fillAmount = (float)currentHealth / totalHealth;
    }
}