// Optimized SkillTree.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    [Header("Skill Icons")]
    [SerializeField] private Image[] skillIcons;
    [SerializeField] private Sprite unlockedSkillSprite;
    [SerializeField] private Sprite lockedSkillSprite;
    [SerializeField] private Sprite darkSkillSprite;

    [Header("Fill Animation")]
    [SerializeField] private Image fillImage;
    [SerializeField] private float fillDuration = 0.5f;

    public void UnlockSkill(int skillUnlocked, float targetFill)
    {
        for (int i = 0; i < skillIcons.Length; i++)
        {
            skillIcons[i].sprite = i < skillUnlocked ? unlockedSkillSprite :
                                  (i == skillUnlocked ? darkSkillSprite : lockedSkillSprite);
        }

        StopAllCoroutines();
        StartCoroutine(AnimateFill(targetFill));
    }

    private IEnumerator AnimateFill(float endValue)
    {
        float startValue = 0f;
        float elapsedTime = 0f;
        fillImage.fillAmount = startValue;

        while (elapsedTime < fillDuration)
        {
            fillImage.fillAmount = Mathf.Lerp(startValue, endValue, elapsedTime / fillDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fillImage.fillAmount = endValue;
    }
}
