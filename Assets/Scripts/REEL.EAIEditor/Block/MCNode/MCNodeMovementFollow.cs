using REEL.D2EEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCNodeMovementFollow : MonoBehaviour
{
	protected RectTransform refRT;
	protected MCNode refNode;


	public RectTransform		parentObject = null;
	public MCNodeMovementFollow parentFollowObj = null;
	public Vector3 offsetPosition = Vector3.zero;
	public Vector2 velocity = Vector2.zero;
	public float smoothTime = 0.035f;


	public Vector2 lastTargetPos = Vector2.zero;
	public bool released = true;


	private void OnEnable()
	{
		refRT = GetComponent<RectTransform>();
		if (refNode == null)
		{
			refNode = GetComponent<MCNode>();
		}
	}

	void Update()
	{
		if (parentObject == null)
			return;

		if(released == false && KeyInputManager.Instance.isShiftPressed == false )
		{
			released = true;
		}

		UpdateMove();
	}


	public void SetParent(MCNode node)
	{
		if( node == null )
		{
			parentObject = null;
			released = true;
			return;
		}

		parentObject = node.GetComponent<RectTransform>();
		parentFollowObj = node.GetComponent<MCNodeMovementFollow>();

		if (parentObject != null )
		{
			offsetPosition = refRT.localPosition - parentObject.localPosition;
			released = false;
		}
	}


	Vector2 GetPosition()
	{
		return transform.localPosition;
	}

	Vector2 GetSize()
	{
		return refRT.sizeDelta;
	}

	Vector2 GetSizeHalf()
	{
		return GetSize() * 0.5f;
	}

	void SetPosition(Vector2 pos)
	{
		GetComponent<MCNodeDrag>().SetPosition(pos);
	}


	public void SetReleased()
	{
		released = true;
	}

	public bool CheckMoveCompleted()
	{
		return Vector2.Distance(GetPosition(), lastTargetPos) < 0.5f;
	}


	private void UpdateMove()
	{
		lastTargetPos = parentObject.localPosition + offsetPosition;

		lastTargetPos = ClippingCheckEditorArea(lastTargetPos);

		var pos = Vector2.SmoothDamp(GetPosition(), lastTargetPos , ref velocity, smoothTime);

		SetPosition(pos);

		if( released == true )
		{
			if (CheckMoveCompleted() /*&& parentFollowObj.CheckMoveCompleted()*/ )
			{
				SetPosition(pos);

				velocity = Vector2.zero;
				parentObject = null;
			}
		}
	}


	public Vector2 ClippingCheckEditorArea(Vector2 pos)
	{
		if (enabled == false) return pos;

		var size = GetSizeHalf() * 1.2f; //1.2f 는 어느정도 간격을 띄워 주려고 곱해줌

		//left, top 비교
		if (pos.x < Utils.EDITOR_AREA_LEFT + size.x ||
			 pos.x > Utils.EDITOR_AREA_RIGHT - size.x ||
			 pos.y > Utils.EDITOR_AREA_TOP - size.y ||
			 pos.y < Utils.EDITOR_AREA_BOTTOM + size.y
			 )
		{
			// left right 비교
			pos.x = Mathf.Clamp(pos.x, Utils.EDITOR_AREA_LEFT + size.x, Utils.EDITOR_AREA_RIGHT - size.x);

			// top 비교
			if (pos.y > Utils.EDITOR_AREA_TOP - size.y) pos.y = Utils.EDITOR_AREA_TOP - size.y;

			// bottom 비교
			if (pos.y < Utils.EDITOR_AREA_BOTTOM + size.y) pos.y = Utils.EDITOR_AREA_BOTTOM + size.y;
		}

		return pos;
	}


	//Vector2 ClippingEditorArea(Vector2 targetPosition)
	//{
	//	if (enabled == false) return;

	//	var pos = targetPosition;
	//	var size = GetSizeHalf() * 1.2f; //1.2f 는 어느정도 간격을 띄워 주려고 곱해줌

	//	//left, top 비교
	//	if (pos.x < EDITOR_AREA_LEFT + size.x ||
	//		 pos.x > EDITOR_AREA_RIGHT - size.x ||
	//		 pos.y > EDITOR_AREA_TOP - size.y ||
	//		 pos.y < EDITOR_AREA_BOTTOM + size.y
	//		 )
	//	{
	//		SetMoveInside();
	//	}
	//}



}
