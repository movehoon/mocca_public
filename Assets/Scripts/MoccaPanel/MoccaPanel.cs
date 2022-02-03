using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DynamicPanels;

namespace Mocca
{
    public class MoccaPanel : Panel
    {
        protected Panel refPanel;
        protected PanelHeader panelHeader;

        protected virtual void Start()
        {
            refPanel = GetComponent<Panel>();
            panelHeader = refPanel.GetComponentInChildren<PanelHeader>();

            IMoccaPanel moccaPanel = gameObject.GetComponentInChildren<IMoccaPanel>();
            if (moccaPanel != null)
            {
                moccaPanel.Initialize(this);
            }
            //else
            //{
            //    Debug.Log("moccaPanel null: " + (refPanel == null ? "refPanel null" : (refPanel[0] == null ? ("refPanel[0] null " + refPanel.gameObject) : refPanel[0].gameObject.ToString())));
            //}
        }
    }
}