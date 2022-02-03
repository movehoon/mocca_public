using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPanelContainer : MonoBehaviour , IDragHandler , IBeginDragHandler , IEndDragHandler
{
    [Header("Object")]
    public RectTransform    rectTransform;


    [Header("Option")]
    public bool    checkOutside = true;
    public float   moveSmoothDamp = 0.05f;


    //[Header("for Debug")]
    Vector3[] corners = new Vector3[4];

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

    void Start()
    {
        
    }

    void Update()
    {
        if( checkOutside == true )
        {
            UpdateCheckOutside();
		}
    }

	private void UpdateCheckOutside()
	{
        rectTransform.GetWorldCorners(corners);
        
        if(isDragging == true) return;
		if(rectTransform == null) return;

        if(IsInside(corners) == true)   //화면 밖으로 나갔는지 체크 하고
        {
            velocity = Vector2.zero;
            return;
        }

        Vector2 leftBottom = corners[0];
        Vector2 rightTop = corners[2];
        Vector2 target = rectTransform.localPosition;

        //나갔으면 나간만큼 화면 안쪽으로 밀어준다
        if(leftBottom.x < 0.0f) target.x += -leftBottom.x + 1.0f;
        if(leftBottom.y < 0.0f) target.y += -leftBottom.y + 1.0f;
		if(rightTop.x > Screen.width)   target.x -= (rightTop.x - Screen.width) + 1.0f;
		if(rightTop.y > Screen.height)  target.y -= (rightTop.y - Screen.height )+ 1.0f;

		rectTransform.localPosition = Vector2.SmoothDamp(rectTransform.localPosition, target, ref velocity, moveSmoothDamp);
    }

	private bool IsInside(Vector3[] corners)
	{
        Rect rect = new Rect(0,0,Screen.width, Screen.height);
        foreach(Vector3 corner in corners)
        {
            if(rect.Contains(corner) == false)
            {
                return false;
            }
        }

        return true;
	}
}
