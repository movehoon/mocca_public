using UnityEngine;

namespace REEL.D2EEditor
{
    public class MCAddCommentCommand : MCCommand
    {
        private string commentName = string.Empty;
        private Vector2 position = Vector2.zero;

        DeleteCommentInfo info;

        private MCComment prefab = null;
        private Transform parentTransform = null;
        private int refCommentID = -1;

        public MCAddCommentCommand(string commentName, Vector2 position, MCComment prefab, Transform parentTransform)
        {
            this.commentName = commentName;
            this.position = position;
            this.prefab = prefab;
            this.parentTransform = parentTransform;
        }

        public void Execute()
        {
            MCComment comment = GameObject.Instantiate(prefab, parentTransform);
            comment.SetCommentText(commentName);
            comment.CommentID = Utils.NewGUID;
            refCommentID = comment.CommentID;
            RectTransform rect = comment.GetComponent<RectTransform>();

            MCCommentManager.Instance.AddNewComment(comment);

            rect.position = position;
        }

        public void Undo()
        {
            MCComment targetComment = MCCommentManager.Instance.FindCommentWithID(refCommentID);
            MCCommentManager.Instance.DeleteComment(targetComment);
        }
    }
}
