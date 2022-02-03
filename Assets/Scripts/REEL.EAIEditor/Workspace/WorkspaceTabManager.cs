using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
    public class WorkspaceTabManager : Singleton<WorkspaceTabManager>
    {
        public List<Workspace> workspaces;

        public void AddWorkspace(Workspace workspace)
        {
            workspaces.Add(workspace);
        }

        public void RemoveWorkspace(Workspace workspace)
        {
            workspaces.Remove(workspace);
        }
    }
}