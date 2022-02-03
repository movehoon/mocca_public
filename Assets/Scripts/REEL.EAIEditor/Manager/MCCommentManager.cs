using REEL.PROJECT;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace REEL.D2EEditor
{
    public class MCCommentManager : Singleton<MCCommentManager>
    {
        [Header("For creating comment")]
        public RectTransform commentParentTransform;
        public MCComment commentPrefab;

        [Header("References")]
        public List<MCComment> locatedComments = new List<MCComment>();
        public MCNodeDragManager dragManager;

        [Header("Cursor Images")]
        public Texture2D horizontal;
        public Texture2D vertical;
        public Texture2D topLeft;
        public Texture2D topRight;

        private KeyInputManager inputManager;

        public bool IsCommentSelected
        {
            get
            {
                if (SelectedList.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        private void OnEnable()
        {
            if (inputManager == null)
            {
                inputManager = KeyInputManager.Instance;
            }
        }

        public void SetOneSelected(MCComment comment)
        {
            SetAllUnSelected();
            comment.OnSelect(true);
        }

        // 사용자가 키보드 C 키를 눌렀을 때 주석 추가하는 함수.
        public void AddNewComment()
        {
            AddNewComment("코멘트", UnityEngine.Input.mousePosition);
        }

        public void AddNewComment(MCComment comment)
        {
            locatedComments.Add(comment);
        }

        public void AddNewComment(string comment, Vector2 position)
        {
            MCAddCommentCommand command = new MCAddCommentCommand(comment, position, commentPrefab, commentParentTransform);
            MCUndoRedoManager.Instance.AddCommand(command);

            //MCComment newComment = Instantiate(commentPrefab, commentParentTransform);
            //newComment.SetCommentText(comment);
            //newComment.CommentID = Utils.NewGUID;
            //RectTransform rect = newComment.GetComponent<RectTransform>();

            //locatedComments.Add(newComment);

            //rect.position = position;
        }

        public void AddNewComment(string comment, Vector2 position, Vector2 size)
        {
            MCComment newComment = Instantiate(commentPrefab, commentParentTransform);
            newComment.SetCommentText(comment);
            newComment.CommentID = Utils.NewGUID;
            RectTransform rect = newComment.GetComponent<RectTransform>();

            locatedComments.Add(newComment);

            rect.localPosition = position;
            rect.sizeDelta = size;
        }

        public void SetCommentSelectionWithDragArea(DragInfo dragInfo)
        {
            dragManager.SetCommentSelectionWithDragArea(dragInfo);
        }

        public void AddToSelection(MCComment comment)
        {
            if (KeyInputManager.Instance.shouldMultiSelect)
            {
                comment.OnSelect(true);
            }
            else
            {
                SetOneSelected(comment);
            }
        }

        // 노드가 클릭됐을 때 Ctrl/Shift키가 눌리지 않았으면 선택 해제를 위해 호출.
        public void OnNodePointDown()
        {
            if (!inputManager.shouldMultiSelect)
            {
                SetAllUnSelected();
            }
        }

        public void SetAllUnSelected()
        {
            if (MCProjectManager.ProjectDescription == null)
            {
                return;
            }

            foreach (MCComment comment in locatedComments)
            {
                comment.OnSelect(false);
            }
        }

        public void SetCommentSelected(MCComment comment)
        {
            comment.OnSelect(true);
        }

        public void SetCommentUnSelected(MCComment comment)
        {
            comment.OnSelect(false);
        }

        public void SetDragOffset(Vector3 pointerPosition)
        {
            List<MCComment> selectedComments = SelectedList;
            foreach (var selected in selectedComments)
            {
                selected.SetDragOffset(pointerPosition);
            }
        }

        public void DragComments(PointerEventData eventData, MCComment comment = null)
        {
            List<MCComment> selectedComments = SelectedList;
            //if (comment != null && selectedComments.Count == 1)
            //{
            //    if (selectedComments[0].gameObject.GetInstanceID().Equals(comment.gameObject.GetInstanceID()) == false)
            //    {
            //        selectedComments[0].IsSelected = false;
            //        comment.IsSelected = true;

            //        comment.SetDragOffset(eventData.position);

            //        selectedComments[0] = comment;
            //    }
            //}

            foreach (var selected in selectedComments)
            {
                selected.SetChangedPosition(eventData);
            }
        }

        public MCComment FindCommentWithID(int commentID)
        {
            if (commentID == -1)
            {
                Utils.LogRed("[MCCommentManager Error] commentID is -1");
                return null;
            }

            foreach (MCComment comment in locatedComments)
            {
                if (comment.CommentID.Equals(commentID))
                {
                    return comment;
                }
            }

            Utils.LogRed($"[MCCommentManager Error] Can't find comment with commentID is {commentID}");
            return null;
        }

        public Comment[] ProjectCommentInfo
        {
            get
            {
                Comment[] comments = new Comment[locatedComments.Count];
                for (int ix = 0; ix < locatedComments.Count; ++ix)
                {
                    comments[ix] = new Comment();
                    comments[ix].title = locatedComments[ix].commentText.text;
                    comments[ix].position = locatedComments[ix].GetComponent<RectTransform>().localPosition;
                    comments[ix].size = locatedComments[ix].GetComponent<RectTransform>().sizeDelta;
                }

                return comments;
            }
        }

        public void DeleteAll()
        {
            for (int ix = locatedComments.Count - 1; ix >= 0; --ix)
            {
                MCComment comment = locatedComments[ix];
                DeleteComment(comment);
            }
        }

        public void DeleteComment(MCComment comment)
        {
            locatedComments.Remove(comment);
            comment.DeleteComment();
        }

        public void DeleteSelected()
        {
            if (!IsCommentSelected)
            {
                return;
            }

            MCDeleteMultipleCommentCommand command = new MCDeleteMultipleCommentCommand(SelectedList, commentPrefab, commentParentTransform);
            MCUndoRedoManager.Instance.AddCommand(command);

            //List<MCComment> selected = SelectedList;
            //foreach (MCComment comment in selected)
            //{
            //    locatedComments.Remove(comment);
            //    comment.DeleteComment();
            //}

            //selected = null;
        }

        // Depth checking.
        public void UpdateCommentsDepth()
        {
            //Utils.LogRed("UpdateCommentsDepth");

            locatedComments.Sort();
            for (int ix = 0; ix < locatedComments.Count; ++ix)
            {
                locatedComments[ix].transform.SetSiblingIndex(ix);
                //locatedComments[ix].transform.SetAsLastSibling();
            }
        }

        private List<MCComment> SelectedList
        {
            get
            {
                List<MCComment> selected = new List<MCComment>();
                foreach (MCComment comment in locatedComments)
                {
                    if (comment.IsSelected)
                    {
                        selected.Add(comment);
                    }
                }

                return selected;
            }
        }

        public int CurrentSelectedCommentCount
        {
            get
            { 
                return SelectedList.Count; 
            }
        }
    }
}