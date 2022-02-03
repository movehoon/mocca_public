using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMovieImage : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler , IScrollHandler
{
    [Header("Object")]
    public RectTransform    rectTransform;
    public RectTransform    rectTransformBackground;


    [Header("Option")]
    public bool    checkInside = true;
    public float   moveSmoothDamp = 0.05f;


    [Header("Scale")]
    public bool     useScale = true;
    public float    SCALE_MAX = 2.0f;
    public float    SCALE_MIN = 0.33f;

    public float mouseWheelUpScale = 1.25f;
    public float mouseWheelDownScale = 0.8f;

    public float    targetScale = 1.0f;
    public float    smoothTimeScale = 0.2f;
    public float    velocityScale = 0.0f;


    [Header("for Debug")]
    public Vector3[] corners = new Vector3[4];
    public Vector3[] cornersBackground = new Vector3[4];


    private Vector2 velocity = Vector2.zero;
    private bool    isDragging = false;
    private Vector2 lastMousePosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin Drag");
        lastMousePosition = eventData.position;
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - lastMousePosition;
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 newPosition = rect.position +  new Vector3(diff.x, diff.y, transform.position.z);
        rect.position = newPosition;

        lastMousePosition = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }


    void Update()
    {
        if(useScale)
        {
            UpdateScale();
        }

        if(checkInside)
        {
            UpdateCheckInside();
        }
    }

    private void UpdateScale()
    {
        float currentScale = rectTransform.localScale.x;

        if(Mathf.Abs(currentScale - targetScale) < 0.01f) return;

        float newScale = Mathf.SmoothDamp(currentScale, targetScale, ref velocityScale, smoothTimeScale);

        rectTransform.localScale = Vector3.one * newScale;
    }


    private void UpdateCheckInside()
    {
        rectTransform.GetWorldCorners(corners);
        rectTransformBackground.GetWorldCorners(cornersBackground);

        if(isDragging == true) return;
        if(rectTransform == null) return;

        if(IsInside(corners, cornersBackground) == false)
        {
            velocity = Vector2.zero;
            return;
        }

		Vector2 leftBottom = corners[0];
		Vector2 rightTop = corners[2];
		Vector2 target = rectTransform.localPosition;

		if(leftBottom.x > cornersBackground[0].x ) target.x -= leftBottom.x - cornersBackground[0].x + 1.0f;
		if(leftBottom.y > cornersBackground[0].y ) target.y -= leftBottom.y - cornersBackground[0].y + 1.0f;
		if(rightTop.x < cornersBackground[2].x) target.x += (cornersBackground[2].x - rightTop.x ) + 1.0f;
		if(rightTop.y < cornersBackground[2].y) target.y += (cornersBackground[2].y - rightTop.y ) + 1.0f;

		rectTransform.localPosition = Vector2.SmoothDamp(rectTransform.localPosition, target, ref velocity, moveSmoothDamp);
	}

    private bool IsInside(Vector3[] corners, Vector3[] insideCorners)
    {
        if(corners[0].x > insideCorners[0].x) return true;
        if(corners[0].y > insideCorners[0].y) return true;

        if(corners[2].x < insideCorners[2].x) return true;
        if(corners[2].y < insideCorners[2].y) return true;

        return false;
    }

	public void OnScroll(PointerEventData eventData)
	{

        float y = Input.mouseScrollDelta.y;

        if(y > 0)
        {
            ScaleChange(mouseWheelUpScale);
        }

        if(y < 0)
        {
            ScaleChange(mouseWheelDownScale);
        }

    }

    private void ScaleChange(float scale)
    {
        targetScale *= scale;
        targetScale = Mathf.Clamp(targetScale, SCALE_MIN, SCALE_MAX);
    }

}
