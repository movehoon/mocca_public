using DynamicPanels;
using REEL.D2EEditor;
using UnityEngine;

namespace Mocca
{
    public class MoccaSenarioPanel : MoccaPanelBase
    {
        private PanelTab refTab;
        private bool hasInitialized = false;

        public override void Initialize(Panel panel)
        {
            if (hasInitialized == true)
            {
                return;
            }

            base.Initialize(panel);
            panel.name = "Senario";
            refTab = panel.GetTab(refRT);

            SetActive(false);

            hasInitialized = true;
        }

        public void SetName(string name)
        {
            refTab.Label = name;
            SetActive(string.IsNullOrEmpty(name) ? false : true);
        }

        public void SetActive(bool isActive)
        {
            //refTab.Internal.SetActive(isActive);
            refTab.gameObject.SetActive(isActive);
        }

        //private void Update()
        //{
        //    // Add new tab test code.
        //    if (Input.GetKeyDown(KeyCode.P))
        //    {
        //        GameObject newGO = new GameObject("New Tab");
        //        newGO.transform.SetParent(canvas.RectTransform);
        //        newGO.transform.localPosition = Vector3.zero;
        //        newGO.transform.localScale = Vector3.one;

        //        UnityEngine.UI.Image newImage = newGO.AddComponent<UnityEngine.UI.Image>();
        //        newGO.AddComponent<MoccaFunctionPanel>();
        //        newGO.AddComponent<MCFunctionTab>();
        //        newImage.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        //        PanelTab newTab = panel.AddTab(newGO.GetComponent<RectTransform>());
        //        int index = panel.GetTabIndex(newTab);
        //        newTab.Panel[index].Icon = null;
        //        newTab.Panel[index].Label = "Function";
        //    }
        //}
    }
}