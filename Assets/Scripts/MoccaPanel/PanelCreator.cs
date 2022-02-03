using UnityEngine;
using DynamicPanels;

public class PanelCreator : MonoBehaviour
{
    public DynamicPanelsCanvas canvas;

    public RectTransform toolbarContent, blockContent, variableContent, senarioContent, logContent, robotContent, camviewContent, propertyContent;
    public string toolbarLabel, blockLabel, variableLabel, senarioLabel, logLabel, robotLabel, camviewLabel, propertyLabel;
    public Sprite toolbarIcon, blockIcon, variableIcon, senarioIcon, logIcon, robotIcon, camviewIcon, propertyIcon;

    void Start()
    {
        // Create 3 panels
        Panel toolbarPanel = PanelUtils.CreatePanelFor(toolbarContent, canvas);

        Panel blockVariablePanel = PanelUtils.CreatePanelFor(blockContent, canvas);
        Panel senarioPanel = PanelUtils.CreatePanelFor(senarioContent, canvas);
        Panel logPanel = PanelUtils.CreatePanelFor(logContent, canvas);
        Panel robotCamviewPanel = PanelUtils.CreatePanelFor(robotContent, canvas);
        Panel propertyPanel = PanelUtils.CreatePanelFor(propertyContent, canvas);

        // Add a second tab to the first panel
        blockVariablePanel.AddTab(variableContent);
        //senarioPanel.AddTab(logContent);
        robotCamviewPanel.AddTab(camviewContent);

        // Set the labels and the (optional) icons of the tabs
        toolbarPanel[0].Icon = toolbarIcon;
        toolbarPanel[0].Label = toolbarLabel;

        blockVariablePanel[0].Icon = blockIcon; // first tab
        blockVariablePanel[0].Label = blockLabel;
        blockVariablePanel[1].Icon = variableIcon; // second tab
        blockVariablePanel[1].Label = variableLabel;

        senarioPanel[0].Icon = senarioIcon;
        senarioPanel[0].Label = senarioLabel;
        
        logPanel[0].Icon = logIcon;
        logPanel[0].Label = logLabel;

        robotCamviewPanel[0].Icon = robotIcon;  // first tab.
        robotCamviewPanel[0].Label = robotLabel;
        robotCamviewPanel[1].Icon = camviewIcon;  // second tab.
        robotCamviewPanel[1].Label = camviewLabel;

        propertyPanel[0].Icon = propertyIcon;
        propertyPanel[0].Label = propertyLabel;

        // Set the minimum sizes of the contents associated with the tabs
        blockVariablePanel[0].MinSize = new Vector2(150f, 150f); // first tab
        blockVariablePanel[1].MinSize = new Vector2(150f, 150f); // second tab

        senarioPanel[0].MinSize = new Vector2(150f, 150f); // first tab

        robotCamviewPanel[0].MinSize = new Vector2(150f, 150f); // first tab
        robotCamviewPanel[1].MinSize = new Vector2(150f, 150f); // second tab

        propertyPanel[0].MinSize = new Vector2(150f, 150f);

        // Create a vertical panel group
        PanelGroup groupCenter = new PanelGroup(canvas, Direction.Top);
        groupCenter.AddElement(logPanel);
        groupCenter.AddElement(senarioPanel);

        PanelGroup groupLeftVertical = new PanelGroup(canvas, Direction.Top); // elements are always arranged from bottom to top
        groupLeftVertical.AddElement(blockVariablePanel); // bottom panel

        PanelGroup groupRightVertical = new PanelGroup(canvas, Direction.Top);
        groupRightVertical.AddElement(propertyPanel);   // bottom panel
        groupRightVertical.AddElement(robotCamviewPanel); // top panel

        // Dock the elements to the Dynamic Panels Canvas (the order is important)
        toolbarPanel.DockToRoot(Direction.Top);
        //senarioPanel.DockToPanel(toolbarPanel, Direction.Bottom);
        //logPanel.DockToPanel(senarioPanel, Direction.Bottom);
        groupLeftVertical.DockToPanel(toolbarPanel, Direction.Left);
        groupRightVertical.DockToPanel(toolbarPanel, Direction.Right);
        groupCenter.DockToPanel(toolbarPanel, Direction.Bottom);

        // Rebuild the layout before attempting to resize elements or read their correct sizes/minimum sizes
        canvas.ForceRebuildLayoutImmediate();

        // It is recommended to manually resize layout elements that are created by code and docked.
        // Otherwise, their sizes will not be deterministic. In this case, we are resizing them to their minimum size
        toolbarPanel.ResizeTo(new Vector2(toolbarPanel.Size.x, toolbarPanel.Size.y));

        groupCenter.ResizeTo(new Vector2(groupCenter.MinSize.x, groupCenter.Size.y));
        //senarioPanel.ResizeTo(new Vector2(senarioPanel.Size.x, senarioPanel.Size.y));
        //logPanel.ResizeTo(new Vector2(logPanel.Size.x, logPanel.Size.y));

        groupLeftVertical.ResizeTo(new Vector2(groupLeftVertical.MinSize.x, groupLeftVertical.MinSize.y));
        groupRightVertical.ResizeTo(new Vector2(groupRightVertical.MinSize.x, groupRightVertical.MinSize.y));
    }
}