using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationOnEnable : MonoBehaviour
{
	public bool applyOnEnable = true;
	public bool applyChild = true;

	private void OnEnable()
	{
		if (applyOnEnable)
		{
			Localize();
		}
	}

	void Localize()
	{
		if (applyChild)
		{
			LocalizationManager.LocalizeComponentsInChildren(gameObject);
		}
	}
}
