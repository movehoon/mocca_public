using System.Collections.Generic;
using UnityEngine;

namespace REEL.D2EEditor
{
	public class MCProjectManager : MonoBehaviour
	{
        public static ProjectDesc ProjectDescription { get; set; }

        private static ProjectBuilder projectBuilder = null;
		public static ProjectBuilder ProjectBuilder
        {
            get
            {
                if (projectBuilder == null)
                {
                    projectBuilder = FindObjectOfType<ProjectBuilder>();
                }

                return projectBuilder;
            }
        }

        public static bool IsDirty { get; set; }

        public static string Build(List<MCNode> locatedNodes,
            List<LeftMenuVariableItem> locatedVariables,
            List<LeftMenuFunctionItem> locatedFunctions,
            LocalizationManager.Language language = LocalizationManager.Language.DEFAULT)
        {
            return ProjectBuilder.BuildToJson(
                locatedNodes, 
                locatedVariables, 
                locatedFunctions, 
                language
            );
        }
	}
}