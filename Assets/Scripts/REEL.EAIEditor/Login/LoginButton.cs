using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using REEL.D2E;

namespace REEL.D2EEditor
{
    public class LoginButton : MonoBehaviour
	{
        [SerializeField] private string sceneName = "D2EEditor_Editor";
        [SerializeField] private string requestIP = "";

		[SerializeField] private Toggle keepLoginStatusToggle = null;


		[Header("TMP IputField")]
		[SerializeField] private TMPro.TMP_InputField TMP_idField = null;
		[SerializeField] private TMPro.TMP_InputField TMP_pwField = null;


		[Header("OLD IputField")]
		[SerializeField] private InputField idField = null;
		[SerializeField] private InputField pwField = null;

        public void OnLoginButtonCliced()
        {
			//Debug.Log(idField.text + " : " + pwField.text + " : " + keepLoginStatusToggle.isOn);

			string id = TMP_idField != null ? TMP_idField.text : idField.text;
			string pw = TMP_pwField != null ? TMP_pwField.text : pwField.text;

			//----------------------
			// Try Login.
			//string id = idField.text.Equals("") ? "reel" : TMP_idField.text;
			//string pw = pwField.text.Equals("") ? "1234" : TMP_idField.text;
			if (string.IsNullOrWhiteSpace(id)) id = "reel";
			if (string.IsNullOrWhiteSpace(pw)) pw = "1234";
			//----------------------


			PlayerPrefs.SetString(D2EConstants.currentUserIDKey, id);
            //Debug.Log("id: " + id + " pw: " + pw);
            WebCommunicationManager.Instance.Login(id, pw, GetLoginResult);

            // Move to work space scene.
            SceneManager.LoadScene(sceneName);
        }

        private void MoveMainScene()
        {
            SceneManager.LoadScene(sceneName);
        }

        private void GetLoginResult(string result)
        {
            LoginResult jsonResult = JsonUtility.FromJson<LoginResult>(result);
            print(jsonResult);

            if (jsonResult.success)
                MoveMainScene();
        }
	}
}