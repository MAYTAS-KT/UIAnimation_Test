// Optimized UICircularLayoutGroup.cs
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class UICircularLayoutGroup : LayoutGroup
{
    [Header("Circular Layout Settings")]
    [SerializeField] private float childRadius = 30f;
    [SerializeField] private float containerPadding = 1f;
    [SerializeField] public float startAngle = 360f;

    private enum Axis { X = 0, Y = 1 }

    private float angle;
    private float containerRadius;
    private float paddedRadius;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        CalculateLayout(Axis.X);
    }

    public override void CalculateLayoutInputVertical()
    {
        CalculateLayout(Axis.Y);
    }

    public override void SetLayoutHorizontal()
    {
        SetLayout(Axis.X);
    }

    public override void SetLayoutVertical()
    {
        SetLayout(Axis.Y);
    }

    private void CalculateLayout(Axis axis)
    {
        if (rectChildren.Count == 0) return;

        angle = (startAngle / rectChildren.Count) * Mathf.Deg2Rad;

        float containerSize = axis == Axis.X ? rectTransform.rect.width : rectTransform.rect.height;
        containerRadius = containerSize / 2f;
        paddedRadius = containerRadius - containerPadding - childRadius;
    }

    private void SetLayout(Axis axis)
    {
        for (int i = 0; i < rectChildren.Count; i++)
        {
            RectTransform child = rectChildren[i];

            float angularPosition = axis == Axis.X
                ? Mathf.Cos(i * angle - Mathf.PI / 2f)
                : Mathf.Sin(i * angle - Mathf.PI / 2f);

            float paddedPosition = paddedRadius * angularPosition + containerRadius - childRadius;
            SetChildAlongAxis(child, (int)axis, paddedPosition, childRadius * 2f);
        }
    }

    public void UpdateGrid()
    {
        CalculateLayoutInputHorizontal();
        CalculateLayoutInputVertical();
        SetLayoutHorizontal();
        SetLayoutVertical();
    }
}
