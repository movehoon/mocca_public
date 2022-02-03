using UnityEngine;
using System.Collections.Generic;

namespace REEL.D2EEditor
{
    public class MCDeleteMultipleCommentCommand : MCCommand
    {
        private MCComment prefab = null;
        private Transform parentTransform = null;
        private List<DeleteCommentInfo> deletedCommentInfos = new List<DeleteCommentInfo>();
        private List<MCComment> deletedComments = new List<MCComment>();

        public MCDeleteMultipleCommentCommand(List<MCComment> deletedComments, MCComment prefab, Transform parentTransform)
        {
            // Undo 시 복원을 위해 삭제할 주석의 정보를 리스트에 저장.
            foreach (MCComment comment in deletedComments)
            {
                DeleteCommentInfo info = new DeleteCommentInfo()
                {
                    commentID = comment.CommentID,
                    commentName = comment.commentText.text,
                    position = comment.transform.localPosition,
                    size = comment.GetComponent<RectTransform>().sizeDelta
                };

                deletedCommentInfos.Add(info);
            }

            this.deletedComments = deletedComments;
            this.prefab = prefab;
            this.parentTransform = parentTransform;
        }

        public void Execute()
        {
            if (deletedComments == null)
            {
                return;
            }

            // 주석 삭제.
            foreach (MCComment comment in deletedComments)
            {
                MCCommentManager.Instance.DeleteComment(comment);
            }

            deletedComments.Clear();
            deletedComments = null;
        }

        public void Undo()
        {
            if (deletedCommentInfos == null || deletedCommentInfos.Count == 0)
            {
                return;
            }

            // 삭제된 주석 복원.
            foreach (DeleteCommentInfo info in deletedCommentInfos)
            {
                MCComment comment = GameObject.Instantiate(prefab, parentTransform);
                comment.SetCommentText(info.commentName);
                comment.CommentID = info.commentID;

                RectTransform rect = comment.GetComponent<RectTransform>();
                rect.localPosition = info.position;
                rect.sizeDelta = info.size;

                MCCommentManager.Instance.AddNewComment(comment);
            }
        }
    }
}