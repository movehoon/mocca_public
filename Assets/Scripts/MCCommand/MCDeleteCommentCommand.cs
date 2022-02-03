using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCDeleteCommentCommand : MCCommand
    {
        private DeleteCommentInfo deleteCommentInfo;
        private MCComment prefab = null;
        private Transform parentTransform = null;
        private MCComment refComment;

        public MCDeleteCommentCommand(string commentName, Vector2 position, Vector2 size, MCComment prefab, Transform parentTransform, MCComment comment)
        {
            deleteCommentInfo = new DeleteCommentInfo()
            {
                commentName = commentName,
                position = position,
                size = size,
                commentID = comment.CommentID
            };

            this.prefab = prefab;
            this.parentTransform = parentTransform;
            refComment = comment;
        }

        public void Execute()
        {
            MCCommentManager.Instance.DeleteComment(refComment);
        }

        public void Undo()
        {
            refComment = GameObject.Instantiate(prefab, parentTransform);
            refComment.SetCommentText(deleteCommentInfo.commentName);
            refComment.CommentID = deleteCommentInfo.commentID;

            RectTransform rect = refComment.GetComponent<RectTransform>();
            rect.position = deleteCommentInfo.position;
            rect.sizeDelta = deleteCommentInfo.size;

            MCCommentManager.Instance.AddNewComment(refComment);
        }
    }
}