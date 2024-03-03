using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlashingText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    public float flickerSpeed = 10;
    private Color baseColor;

    void Start()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp)
        {
            baseColor = tmp.color;
        }
        else
        {
            Debug.LogError("Couldn't get flashing text text component");
        }
    }

    void Update()
    {
        if (tmp)
        {
            float alpha = 0.5f + Mathf.Sin(Time.time * flickerSpeed) / 2.0f;
            Color newColor = baseColor;
            newColor.a = alpha;
            tmp.color = newColor;
        }
    }
}
