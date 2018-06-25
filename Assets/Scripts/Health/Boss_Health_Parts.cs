using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Health_Parts : HealthManager
{
    public GameObject healthPartAsset;
    private List<GameObject> healthParts = new List<GameObject>();

    public int healthPerPart;

    public float visualPaddingX;
    public float visualPaddingY;

    public override void Init(int startHealth)
    {
        foreach (GameObject iPart in healthParts)
        {
            Destroy(iPart);
        }
        healthParts.Clear();

        int numParts = startHealth / healthPerPart; // Be sure to make this even out with boss health
        for(int iPart = 0; iPart < numParts; ++iPart)
        {
            GameObject newPart = Instantiate(healthPartAsset, transform);
            RectTransform partTransform = newPart.GetComponent<RectTransform>();
            partTransform.anchorMin = new Vector2(visualPaddingX + (float)iPart / numParts, visualPaddingY);
            partTransform.anchorMax = new Vector2((float)(iPart + 1) / numParts - visualPaddingX, 1 - visualPaddingY);
            healthParts.Add(newPart);
        }
    }

    public override void UpdateHealth(int currentHealth, int totalHealth, int entityIndex)
    {
        float healthFraction = (float)currentHealth / totalHealth;
        for (int iPart = healthParts.Count - 1; iPart >= 0; --iPart)
        {
            float healthFractionBegin = (float)iPart / healthParts.Count;
            float healthFractionEnd = (float)(iPart + 1) / healthParts.Count;
            if (healthFraction <= healthFractionBegin)
            {
                healthParts[iPart].GetComponent<Image>().fillAmount = 0.0f;
            }
            else if(healthFraction >= healthFractionEnd)
            {
                healthParts[iPart].GetComponent<Image>().fillAmount = 1.0f;
            }
            else
            {
                healthParts[iPart].GetComponent<Image>().fillAmount = (healthFraction - healthFractionBegin) / (healthFractionEnd - healthFractionBegin);
            }
        }
    }
}