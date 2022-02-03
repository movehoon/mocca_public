using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineSelectTest : MonoBehaviour
{


	public Canvas canvas = null;
	public GameObject parentObject = null;



	[Header("debug")]
	public Vector3 parentWorldPos = Vector3.zero;
	public Vector3 worldPos = Vector3.zero;
	public Vector3 localPos = Vector3.zero;
	public Vector3 anchoredPosition = Vector3.zero;
	public Transform _transform = null;

	[Header("debug pos")]
	public Vector3 pos_mouse = Vector3.zero;
	public Vector3 pos1_mouse_local = Vector3.zero;
	public Vector3 pos1_mouse_local_c = Vector3.zero;
	public Vector3 pos1_mouse_local_p = Vector3.zero;
	public Vector3 pos2_mouse_world = Vector3.zero;
	public Vector3 pos2_mouse_world_c = Vector3.zero;
	public Vector3 pos2_mouse_world_p = Vector3.zero;
	public Vector3 pos3_uiPos = Vector3.zero;
	public Vector3 pos4_uiPosParent = Vector3.zero;


	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{

		if( Input.GetMouseButtonDown(0) == true )
		{
			Vector3 outWorldPos = Vector3.zero;
			Vector2 outlocalPos = Vector3.zero;

			Vector3 uiPos = Vector3.zero;

			parentWorldPos = transform.parent.position;
			worldPos = transform.position;
			localPos = transform.localPosition;

			pos_mouse = Input.mousePosition;

			var rectTransform = GetComponent<RectTransform>();

			anchoredPosition = rectTransform.anchoredPosition;

			Camera cam = null;
			//Camera cam = canvas.worldCamera;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pos_mouse, cam, out outlocalPos);
			RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, pos_mouse, cam, out outWorldPos);

			pos1_mouse_local = outlocalPos;
			pos2_mouse_world = outWorldPos;


			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), pos_mouse, cam, out outlocalPos);
			RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), pos_mouse, cam, out outWorldPos);

			pos1_mouse_local_c = outlocalPos;
			pos2_mouse_world_c = outWorldPos;


			RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), pos_mouse, cam, out outlocalPos);
			RectTransformUtility.ScreenPointToWorldPointInRectangle(transform.parent.GetComponent<RectTransform>(), pos_mouse, cam, out outWorldPos);

			pos1_mouse_local_p = outlocalPos;
			pos2_mouse_world_p = outWorldPos;



			if (TestLegacy.UIUtil.ScreenPointToUIPoint(canvas, Input.mousePosition, gameObject , out uiPos))
			{
				pos3_uiPos = uiPos;
			}

			if (TestLegacy.UIUtil.ScreenPointToUIPoint(canvas, Input.mousePosition, transform.parent.gameObject, out uiPos))
			{
				pos4_uiPosParent = uiPos;
			}



			//REEL.D2EEditor.MCBezierLine.testCount = 0;

			//System.DateTime dt = System.DateTime.Now;

			//var line = REEL.D2EEditor.MCBezierLine.GetHitLine(pos1_mouse_local);


			//Debug.Log((System.DateTime.Now - dt).TotalMilliseconds);
			////Debug.Log(REEL.D2EEditor.MCBezierLine.testCount);

			//if( line != null )
			//{
			//	line.SetLineColor( new Color( Random.Range(0.5f,1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f) ) ) ;
			//}

		}

	}

}
