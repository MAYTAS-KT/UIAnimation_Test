using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Element UI")]
    [SerializeField] private ElementUIData elementUIData;
    [SerializeField] private UIElementInfo elementUIPrefab;
    [SerializeField] private Transform elementParent;
    [SerializeField] private TextMeshProUGUI elementName;

    [Header("Skill Tree")]
    [SerializeField] public SkillTree skilltreeIcon;

    [Header("UI Elements")]
    [SerializeField] private Image skillFillImage;
    [SerializeField] private Image dimImage;
    [SerializeField] private Image baseImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform mainBtn;
    [SerializeField] private TextMeshProUGUI elementText;

    [Header("Layout")]
    [SerializeField] private RadialLayout radialLayout;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        InstantiateElementUI();
    }

    private void InstantiateElementUI()
    {
        foreach (var elementData in elementUIData.UIelements)
        {
            UIElementInfo element = Instantiate(elementUIPrefab, elementParent);
            element.SetElementInfo(elementData);

            string name = elementData.ElementType.ToString();
            element.elementButton.onClick.AddListener(() => elementName.text = name);
        }
    }

    public void OpenSkillTree() => PlaySkillTreeTransition(true);

    public void CloseSkillTree() => PlaySkillTreeTransition(false);

    private void PlaySkillTreeTransition(bool isOpening)
    {
        float targetAlpha = isOpening ? 0.5f : 0f;
        float canvasAlpha = isOpening ? 1f : 0f;
        Vector2 anchorMin = isOpening ? new Vector2(1, 0.5f) : new Vector2(1, 0);
        Vector2 anchorMax = isOpening ? new Vector2(1, 0.5f) : new Vector2(1, 0);
        Vector2 position = isOpening ? new Vector2(-134, -23) : new Vector2(152, 21);
        Vector2 size = isOpening ? Vector2.one * 410 : Vector2.one * 350;
        float radialChildSize = isOpening ? 125 : 150;
        float radius = isOpening ? 190 : 156;
        float offsetAngle = isOpening ? 105 : 91;

        skillFillImage.gameObject.SetActive(isOpening);

        StartCoroutine(FadeImage(dimImage, dimImage.color.a, targetAlpha));
        StartCoroutine(FadeImage(baseImage, baseImage.color.a, targetAlpha));
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, canvasAlpha));

        StartCoroutine(AnimateRect(mainBtn, anchorMin, anchorMax, position, size, 0.5f,
            radialChildSize, radius, offsetAngle, isOpening));
    }

    private IEnumerator FadeImage(Image img, float from, float to, float duration = 0.5f)
    {
        float elapsed = 0f;
        Color color = img.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(from, to, elapsed / duration);
            img.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = to;
        img.color = color;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration = 0.5f)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        group.alpha = to;
    }

    private IEnumerator AnimateRect(
        RectTransform rect,
        Vector2 targetAnchorMin,
        Vector2 targetAnchorMax,
        Vector2 targetPosition,
        Vector2 targetSize,
        float duration,
        float targetRadialChildSize,
        float targetRadius,
        float targetOffsetAngle,
        bool showElementText)
    {
        Vector2 startAnchorMin = rect.anchorMin;
        Vector2 startAnchorMax = rect.anchorMax;
        Vector2 startPosition = rect.anchoredPosition;
        Vector2 startSize = rect.sizeDelta;
        Vector2 startChildSize = radialLayout.childSize;
        float startRadius = radialLayout.radius;
        float startOffset = radialLayout.offsetAngle;

        float elapsed = 0f;
        elementText.gameObject.SetActive(showElementText);

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            rect.anchorMin = Vector2.Lerp(startAnchorMin, targetAnchorMin, t);
            rect.anchorMax = Vector2.Lerp(startAnchorMax, targetAnchorMax, t);
            rect.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            rect.sizeDelta = Vector2.Lerp(startSize, targetSize, t);

            radialLayout.childSize = Vector2.Lerp(startChildSize, Vector2.one * targetRadialChildSize, t);
            radialLayout.radius = Mathf.Lerp(startRadius, targetRadius, t);
            radialLayout.offsetAngle = Mathf.Lerp(startOffset, targetOffsetAngle, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rect.anchorMin = targetAnchorMin;
        rect.anchorMax = targetAnchorMax;
        rect.anchoredPosition = targetPosition;
        rect.sizeDelta = targetSize;

        radialLayout.childSize = Vector2.one * targetRadialChildSize;
        radialLayout.radius = targetRadius;
        radialLayout.offsetAngle = targetOffsetAngle;
    }
} 
