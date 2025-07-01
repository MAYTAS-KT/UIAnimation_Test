// Optimized UIElementInfo.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIElementInfo : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image icon;
    [SerializeField] private Image desaturatedIcon;
    [SerializeField] public Button elementButton;

    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.5f;

    private float targetFill;
    private int skillUnlocked = 1;
    private float skillTreeTargetFill = 0.5f;

    private void Start()
    {
        elementButton.onClick.AddListener(OnElementClick);
    }

    public void SetElementInfo(ElementUI elementUI)
    {
        icon.sprite = elementUI.icon;
        desaturatedIcon.sprite = elementUI.desaturatedIcon;
        icon.fillAmount = elementUI.initialfillAmount;

        targetFill = elementUI.initialfillAmount;
        skillUnlocked = elementUI.skillUnlocked;
        skillTreeTargetFill = elementUI.skillTreeTargetfill;
    }

    private void OnElementClick()
    {
        UIManager.instance?.skilltreeIcon.UnlockSkill(skillUnlocked, skillTreeTargetFill);
        StopAllCoroutines();
        StartCoroutine(AnimateFill(0, targetFill));
    }

    private IEnumerator AnimateFill(float startValue, float endValue)
    {
        float elapsedTime = 0f;
        icon.fillAmount = startValue;

        while (elapsedTime < animationDuration)
        {
            icon.fillAmount = Mathf.Lerp(startValue, endValue, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        icon.fillAmount = endValue;
    }
}
