using UnityEngine;
using UnityEngine.EventSystems;

public static class RectTransformExtension
{
    public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
    {
        if (rectTransform == null) return;

        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - pivot;
        Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
        rectTransform.pivot = pivot;
        rectTransform.localPosition -= deltaPosition;
    }
}

public class ContentScaler : MonoBehaviour, IScrollHandler
{
    public static ContentScaler Instance = null;

    public float SCALE_MAX = 1.0f;
    public float SCALE_MIN = 0.33f;

    public bool useMouseWheel = true;
    public bool usePivot = false;

    public float mouseWheelUpScale = 1.25f;
    public float mouseWheelDownScale = 0.8f;

    public float targetScale = 1.0f;
    public Vector2 targetPivot = Vector2.zero;

    public float smoothTimeScale = 0.2f;
    public float smoothTimePivot = 0.2f;


    public float velocity = 0.0f;
    private Vector2 velocityPivot = Vector2.zero;
    private RectTransform rectTransform = null;
    private RectTransform contentRectTransform = null;


    //public Vector2 testPivot = Vector2.zero;

    public float changeTime = 0.0f;


    Vector2 ScalePivot
    {
        get
        {
            return contentRectTransform.pivot;
        }

        set
        {
            contentRectTransform.SetPivot(value);
            //contentRectTransform.pivot = value;
        }
    }

	private void Awake()
	{
        Instance = this;
    }

	void Start()
    {
        UnityEngine.UI.ScrollRect scrollRect = GetComponent<UnityEngine.UI.ScrollRect>();
        contentRectTransform = scrollRect.content;
        ScalePivot = contentRectTransform.pivot;

        scrollRect.scrollSensitivity = 0.0f;
        scrollRect.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;

        rectTransform = transform as RectTransform;

        //Vector2 size = Vector2.zero;

        //size.x = rectTransform.rect.width * (1.0f / SCALE_MIN) * 0.9f;
        //size.y = rectTransform.rect.height * (1.0f / SCALE_MIN) * 0.9f;

        //contentRectTransform.sizeDelta = size;

        //EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();

        //EventTrigger.Entry eventScroll = new EventTrigger.Entry();
        //eventScroll.eventID = EventTriggerType.Scroll;
        //eventScroll.callback.AddListener((data) =>
        //{
        //	CheckMouseWheel();
        //}
        //);
        //eventTrigger.triggers.Add(eventScroll);
    }

    public void OnScroll(PointerEventData eventData)
    {
        CheckMouseWheel();
    }

    void Update()
    {
        //CheckMouseWheel();
        CheckScaleLimit();

        UpdateScale();
    }

    public void ResetAll()
    {
        ResetScale();
        ResetVelocity();
        ApplyScaleAndPivot(1.0f, new Vector2(0.0f, 1.0f));
        targetPivot = new Vector2(0.0f, 1.0f);
    }

    public void ResetScale()
    {
        targetScale = 1f;
    }

    private Vector2 GetTargetPivot()
    {
        if (usePivot == false)
        {
            return new Vector2(0.0f, 1.0f);
        }

        Vector2 localpoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(contentRectTransform, Input.mousePosition, null, out localpoint) == true)
        {
            return Rect.PointToNormalized(contentRectTransform.rect, localpoint);
        }

        return Vector2.left;
    }


    private void CheckMouseWheel()
    {

        if (useMouseWheel == false)
            return;

        //if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition) == false)
        //    return;

        float y = Input.mouseScrollDelta.y;

        if (y > 0)
        {
            ScaleChange(mouseWheelUpScale);
            //usePivot = Input.GetKey(KeyCode.LeftControl);
        }

        if (y < 0)
        {
            ScaleChange(mouseWheelDownScale);
            //usePivot = Input.GetKey(KeyCode.LeftControl);
        }
    }

    public void ResetVelocity()
    {
        velocity = 0.0f;
        velocityPivot = Vector2.zero;
    }

    private void ScaleChange(float scale)
    {
        targetScale *= scale;

        targetPivot = GetTargetPivot();
    }

    public void SetScale(float scale)
    {
        targetScale = scale;
        targetPivot = GetTargetPivot();
    }

    // 작성자: 장세윤 20200924.
    // targetScale 값과 targetPivot 값을 명시적으로 설정할 때 사용.
    // 탭을 전환할 때 스케일/피벗 값을 저장해두고 로드할 때 필요해 추가함.
    public void SetScaleAndPivot(float scale, Vector2 pivot)
    {
        targetScale = scale;
        targetPivot = pivot;

        ApplyScaleAndPivot(scale, pivot);
    }

    // 작성자: 장세윤 20210210.
    // 타겟 스케일만 변경.
    // 스케일 업/다운 버튼을 추가해서 마우스 포인터 위치로 피벗은 변경하지 않고,
    // 스케일만 조정하기 위한 메소드.
    private void ScaleChangeOnly(float scale)
    {
        targetScale *= scale;
    }

    public void ScaleUpOnly()
    {
        ScaleChangeOnly(mouseWheelUpScale);
    }

    public void ScaleDownOnly()
    {
        ScaleChangeOnly(mouseWheelDownScale);
    }

    private void CheckScaleLimit()
    {
        targetScale = Mathf.Clamp(targetScale, SCALE_MIN, SCALE_MAX);
    }

    private void UpdateScale()
    {
        float currentScale = contentRectTransform.localScale.x;

        if (Mathf.Abs(currentScale - targetScale) < 0.01f)
        {
            ResetVelocity();
            return;
        }

        float newScale = Mathf.SmoothDamp(currentScale, targetScale, ref velocity, smoothTimeScale);

        Vector2 pivot = Vector2.SmoothDamp(ScalePivot, targetPivot, ref velocityPivot, smoothTimePivot);


        ApplyScaleAndPivot(newScale, pivot);
    }

    private void ApplyScaleAndPivot(float newScale, Vector2 pivot)
    {
        ScalePivot = pivot;

        contentRectTransform.localScale = Vector3.one * newScale;

    }


}