// Optimized RadialLayout.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RadialLayout : MonoBehaviour
{
    [Header("Layout Settings")]
    public float radius = 200f;
    public bool halfCircle = false;
    public float offsetAngle = 0f;
    public Vector2 childSize = new Vector2(100f, 100f);
    public float middleChildScale = 1.2f;

    [Header("Animation Settings")]
    public float animationDuration = 0.5f;
    public float childChangeAnimationDuration = 0.5f;

    private bool isAnimating;
    private bool isChildAnimating;
    private float animationProgress;
    private float childAnimationProgress;
    private bool targetHalfCircleState;

    private Transform rotatingChild;
    private int rotatingChildIndex;
    private float startAngle;
    private float endAngle;

    private readonly Dictionary<Button, UnityEngine.Events.UnityAction> buttonListeners = new();

    private void Start()
    {
        AddListenersToButtons();
    }

    private void AddListenersToButtons()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Button button = transform.GetChild(i).GetComponent<Button>();
            if (button == null) continue;

            if (buttonListeners.TryGetValue(button, out var oldListener))
                button.onClick.RemoveListener(oldListener);

            int index = i;
            UnityEngine.Events.UnityAction action = () => CheckMoveType(index);
            buttonListeners[button] = action;
            button.onClick.AddListener(action);
        }
    }

    private void Update()
    {
        if (isAnimating) AnimateLayout();
        else if (isChildAnimating) AnimateChildLayout();
        else ArrangeChildren();
    }

    private void ArrangeChildren()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        float angleStep = (halfCircle ? 180f : 360f) / childCount;
        float currentAngle = offsetAngle;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            if (child == null) continue;

            child.sizeDelta = childSize;

            float angle = currentAngle * Mathf.Deg2Rad;
            Vector2 position = new(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            child.anchoredPosition = position;
            child.localRotation = Quaternion.Euler(0f, 0f, halfCircle ? currentAngle - 90f : currentAngle);
            child.localScale = (halfCircle && i == childCount / 2) ? Vector3.one * middleChildScale : Vector3.one;

            currentAngle += angleStep;
            child.rotation = Quaternion.identity;
        }
    }

    private void AnimateLayout()
    {
        animationProgress += Time.deltaTime / animationDuration;
        if (animationProgress >= 1f)
        {
            animationProgress = 1f;
            isAnimating = false;
            halfCircle = targetHalfCircleState;
        }

        AnimateChildren(interpolated: true);
    }

    private void AnimateChildren(bool interpolated)
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        float startAngleStep = (halfCircle ? 180f : 360f) / childCount;
        float targetAngleStep = (targetHalfCircleState ? 180f : 360f) / childCount;
        float angleStep = Mathf.Lerp(startAngleStep, targetAngleStep, animationProgress);
        float currentAngle = offsetAngle;

        for (int i = 0; i < childCount; i++)
        {
            if (rotatingChild != null && rotatingChildIndex == i) continue;

            RectTransform child = transform.GetChild(i) as RectTransform;
            if (child == null) continue;

            child.sizeDelta = childSize;

            float angle = currentAngle * Mathf.Deg2Rad;
            Vector2 targetPosition = new(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            child.anchoredPosition = Vector2.Lerp(child.anchoredPosition, targetPosition, animationProgress);

            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetHalfCircleState ? currentAngle - 90f : currentAngle);
            child.localRotation = Quaternion.Lerp(child.localRotation, targetRotation, animationProgress);

            child.localScale = (targetHalfCircleState && i == childCount / 2)
                ? Vector3.Lerp(child.localScale, Vector3.one * middleChildScale, animationProgress)
                : Vector3.Lerp(child.localScale, Vector3.one, animationProgress);

            currentAngle += angleStep;
            child.rotation = Quaternion.identity;
        }
    }

    private void AnimateChildLayout()
    {
        childAnimationProgress += Time.deltaTime / childChangeAnimationDuration;
        if (childAnimationProgress >= 1f)
        {
            childAnimationProgress = 1f;

            if (rotatingChild != null)
            {
                if (startAngle < endAngle) rotatingChild.SetAsLastSibling();
                else rotatingChild.SetAsFirstSibling();
            }

            ArrangeChildren();
            AddListenersToButtons();
            rotatingChild = null;
            isChildAnimating = false;
            return;
        }

        float angle = Mathf.Lerp(startAngle, endAngle, childAnimationProgress) * Mathf.Deg2Rad;
        Vector2 position = new(-Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        rotatingChild.GetComponent<RectTransform>().anchoredPosition = position;

        AnimateLayout();
    }

    public void ToggleCircleState()
    {
        if (isAnimating) return;

        if (!halfCircle)
        {
            UIManager.instance?.OpenSkillTree();
            transform.GetChild(transform.childCount / 2).GetComponent<Button>()?.onClick.Invoke();
        }
        else
        {
            UIManager.instance?.CloseSkillTree();
        }

        targetHalfCircleState = !halfCircle;
        isAnimating = true;
        animationProgress = 0f;
        childAnimationProgress = 0f;
    }

    public void DelayToggleCircleState() => Invoke(nameof(ToggleCircleState), 0.25f);

    public void RotateChildren(bool moveFirstToLast)
    {
        if (!halfCircle) return;

        int childCount = transform.childCount;
        if (childCount == 0) return;

        rotatingChild = moveFirstToLast ? transform.GetChild(0) : transform.GetChild(childCount - 1);
        rotatingChildIndex = moveFirstToLast ? 0 : childCount - 1;

        float angleStep = 180f / childCount;
        startAngle = moveFirstToLast ? offsetAngle : offsetAngle + (childCount - 1) * angleStep;
        endAngle = moveFirstToLast ? startAngle + 180f : startAngle - 180f;

        isChildAnimating = true;
        animationProgress = 0f;
        childAnimationProgress = 0f;
    }

    private void CheckMoveType(int index)
    {
        if (isChildAnimating || transform.childCount / 2 == index) return;

        int middleIndex = transform.childCount / 2;
        int diff = Mathf.Abs(index - middleIndex);

        if (diff == 1)
        {
            RotateChildren(index > middleIndex);
        }
        else
        {
            StartCoroutine(MoveChildMultipleTimes(index, diff));
        }
    }

    private IEnumerator MoveChildMultipleTimes(int index, int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            RotateChildren(index > transform.childCount / 2);
            yield return new WaitUntil(() => !isChildAnimating);
        }
    }
}