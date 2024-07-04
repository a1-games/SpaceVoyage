using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipCycle : MonoBehaviour
{
    [SerializeField] private Image tooltipImage;
    [SerializeField] private Sprite[] tooltipSprites;

    private int currentTooltipIndex;

    public void SetTooltip(Sprite image)
    {
        tooltipImage.sprite = image;
        tooltipImage.SetNativeSize();
    }
    public void NextTooltip()
    {
        currentTooltipIndex++;
        if (currentTooltipIndex >= tooltipSprites.Length) currentTooltipIndex = 0;

        SetTooltip(tooltipSprites[currentTooltipIndex]);
    }
    public void PreviousTooltip()
    {
        currentTooltipIndex--;
        if (currentTooltipIndex < 0) currentTooltipIndex = tooltipSprites.Length - 1;

        SetTooltip(tooltipSprites[currentTooltipIndex]);
    }
}
