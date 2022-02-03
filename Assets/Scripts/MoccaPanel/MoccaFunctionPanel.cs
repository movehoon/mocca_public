using DynamicPanels;
using System.Security.Permissions;
using UnityEngine;

namespace Mocca
{
    public class MoccaFunctionPanel : MoccaPanelBase
    {
        //public ContentScaler contentScaler;
        //public RectTransform blockPane;
        //public RectTransform linePane;

        public override void Initialize(Panel panel)
        {
            base.Initialize(panel);
            panel.name = "Function";
        }

        public void AddFunction()
        {

        }
    }
}
