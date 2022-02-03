using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Next = REEL.PROJECT.Next;

namespace REEL.D2EEditor
{
	public class MCNodeNext : MCNodeSocket
	{
        public Next next;

        protected override void OnEnable()
        {
            if (hasInitialized)
            {
                return;
            }

            base.OnEnable();

            //if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT)
            //{
            //    next = new Next()
            //    {
            //        next = -1,
            //        value = "NEXT"
            //    };
            //}
            //else if (MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.FUNCTION)
            //{
            //    next = new Next()
            //    {
            //        next = -2,
            //        value = "NEXT"
            //    };
            //}

            next = new Next()
            {
                next = -1,
                value = "NEXT"
            };

            socketPosition = SocketPosition.Right;
            socketType = SocketType.EPRight;
        }

        public override void SetLine(MCBezierLine line)
        {
            base.SetLine(line);
            next.next = line.right.Node.NodeID;
        }

        public override void RemoveLine(int lineID = -1)
        {
            base.RemoveLine(lineID);
            next.next = MCWorkspaceManager.Instance.CurrentTabOwnerGroup == Constants.OwnerGroup.PROJECT ? -1 : -2;
        }

        public override bool CheckTargetSocketType(SocketType targetType, MCNodeSocket targetSocket = null)
        {
            if (targetSocket == null)
            {
                return false;
            }

            //if (targetType == SocketType.EPLeft && !targetSocket.HasLine)
            if (targetType == SocketType.EPLeft)
            {
                return true;
            }

            return false;
        }

		internal void HighlightOn()
		{
			if(line != null )
			{
				line.HighlightOn();
			}
		}
	}
}