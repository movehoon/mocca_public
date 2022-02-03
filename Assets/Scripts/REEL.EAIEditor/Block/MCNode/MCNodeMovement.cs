using REEL.D2EEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MCNodeMovement : MonoBehaviour, IBeginDragHandler, IPointerUpHandler
{
    //const float EDITOR_AREA_LEFT = 0;
    //const float EDITOR_AREA_TOP = 0;
    //const float EDITOR_AREA_RIGHT = 3000;		//임시
    //const float EDITOR_AREA_BOTTOM = -1500;		//임시

    protected RectTransform refRT;
    private MCNode refNode;


    protected bool isTargetMoving = false;


    protected Vector2 targetPosition = Vector2.zero;
    protected Vector2 velocity = Vector2.zero;
    protected float smoothTime = 0.125f;


    //GameObject followObject = null;
    //Vector2 relativePosition = Vector2.zero;


    private void OnEnable()
    {
        if (refRT == null)
        {
            refRT = GetComponent<RectTransform>();
        }

        if (refNode == null)
        {
            refNode = GetComponent<MCNode>();
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

    MCNode[] GetLeftNodes()
    {
        return null;
    }

    MCNode[] GetRightNodes()
    {
        return null;
    }

    void Start()
    {
        InitCreateEffect();
    }

    private void InitCreateEffect()
    {
        var effect = gameObject.AddComponent<UIEffect>();
        effect.PlayScale(0.0f, 1.0f, 1.0f);
    }

    //void Update ()
    void LateUpdate()
    {
        if (isTargetMoving)
        {
            UpdateMove();
        }
    }


    private void UpdateMove()
    {
        var pos = Vector2.SmoothDamp(GetPosition(), targetPosition, ref velocity, smoothTime);

        SetPosition(pos);

        //이동 종료 체크
        if (Vector2.Distance(pos, targetPosition) < 1.5f)
        {
            SetPosition(pos);

            isTargetMoving = false;
            velocity = Vector2.zero;
        }
    }

    public void SetMoveTarget(Vector2 targetPos)
    {
        isTargetMoving = true;

        var pos = targetPos;
        var size = GetSizeHalf() * 1.2f;    //1.2f 는 어느정도 간격을 띄워 주려고 곱해줌

        // left right 비교
        pos.x = Mathf.Clamp(pos.x, Utils.EDITOR_AREA_LEFT + size.x, Utils.EDITOR_AREA_RIGHT - size.x);

        // top 비교
        if (pos.y > Utils.EDITOR_AREA_TOP - size.y) pos.y = Utils.EDITOR_AREA_TOP - size.y;

        // bottom 비교
        if (pos.y < Utils.EDITOR_AREA_BOTTOM + size.y) pos.y = Utils.EDITOR_AREA_BOTTOM + size.y;

        targetPosition = pos;
    }

    public void StopMove()
    {
        isTargetMoving = false;
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        StopMove();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var movements = FindObjectsOfType<MCNodeMovement>();

        Array.ForEach(movements, x => x.CheckOutEditorArea());
    }

    public void CheckOutEditorArea()
    {
        if (enabled == false) return;

        var pos = GetPosition();
        var size = GetSizeHalf() * 1.2f; //1.2f 는 어느정도 간격을 띄워 주려고 곱해줌

        //left, top 비교
        if (pos.x < Utils.EDITOR_AREA_LEFT + size.x ||
             pos.x > Utils.EDITOR_AREA_RIGHT - size.x ||
             pos.y > Utils.EDITOR_AREA_TOP - size.y ||
             pos.y < Utils.EDITOR_AREA_BOTTOM + size.y
             )
        {
            SetMoveInside();
        }
    }

    private void SetMoveInside()
    {
        var pos = GetPosition();

        SetMoveTarget(pos);
    }


}
