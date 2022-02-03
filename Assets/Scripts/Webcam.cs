using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using REEL.D2EEditor;
using Newtonsoft.Json.Serialization;
using System.Threading;
using REEL.PROJECT;
using TMPro;
using System;

namespace REEL.D2EEditor
{
    public class Coroutine<T>
    {
        public T returnValue;
        public Coroutine coroutine;

        public IEnumerator InternalRoutine(IEnumerator coroutine)
        {
            while (true)
            {
                if (!coroutine.MoveNext())
                {
                    yield break;
                }
                object yielded = coroutine.Current;

                if (yielded != null && yielded.GetType() == typeof(T))
                {
                    returnValue = (T)yielded;
                    yield break;
                }
                else
                {
                    yield return coroutine.Current;
                }
            }
        }
    }

    public static class CoroutineExt
    {
        public static Coroutine<T> StartCoroutine<T>(this MonoBehaviour obj, IEnumerator coroutine)
        {
            Coroutine<T> coroutineObject = new Coroutine<T>();
            coroutineObject.coroutine = obj.StartCoroutine(coroutineObject.InternalRoutine(coroutine));
            return coroutineObject;
        }
    }
}

public class Webcam : Singleton<Webcam>
{
    [Header("WebCam View 관련 속성")]
    public RawImage[] camViewImages;
    //public RawImage camViewImage = null;
    //public RawImage camIiewPopupImage = null;
    public MCWebcamProcessMessageWindow[] processMessageWindows;
    //public GameObject processMessageWindow = null;
    //public TMPro.TextMeshProUGUI messageText = null;
    public Color inactiveColor;
    public Color activeColor = Color.white;
    public TMP_Dropdown dropdown;

    private WebCamTexture webCamTexture = null;
    private WebCamTexture frontCamTexture = null;

    private readonly int IMAGE_WIDTH = 320;
    private readonly int IMAGE_HEIGHT = 240;

    private bool isOn = false;
    private bool shouldDelay = false;
    private float delayTime = 2f;
    private float targetTime = 0f;

    public WebCamTexture FrontCamTexture
    {
        get
        {
            if (frontCamTexture == null)
            {
                WebCamDevice[] webCamDevice = WebCamTexture.devices;
                if (webCamDevice.Length > 1 && frontCamTexture == null)
                {
                    frontCamTexture = new WebCamTexture(webCamDevice[1].name, IMAGE_WIDTH, IMAGE_HEIGHT);
                    frontCamTexture.Play();
                }
            }

            return frontCamTexture;
        }
    }

    public bool IsAvailable
    {
        get
        {
            return (webCamTexture != null);
        }
    }

    // Use this for initialization
    void OnEnable()
    {
        Debug.Log("Webcam:Start with " + Utils.WebcamName);
        this.RefreshWebcamListOnDropdown();
        if (Utils.WebcamName.Length == 0 && this.dropdown.options.Count > 0)
        {
            Utils.WebcamName = this.dropdown.options[0].text;
            Debug.Log("WebCam set initial value to " + Utils.WebcamName);
        }
        this.SelectDropdownToInput(this.dropdown, Utils.WebcamName);
        Debug.Log("WebcamAvailable: " + this.IsAvailable.ToString());
        if (this.dropdown != null)
        {
            if (this.dropdown.options.Count > 0 && !this.IsAvailable)
            {
                SwitchWebcam();
            }
        }

        //if (MCWorkspaceManager.Instance != null)
        //    MCWorkspaceManager.Instance.SubscribeSimulationStateChanged(OnSimulationStateChanged);
    }

    private void RefreshWebcamListOnDropdown()
    {
        if (dropdown != null)
        {
            dropdown.gameObject.SetActive(true);
            dropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

            if (WebCamTexture.devices.Length == 0)
            {
                dropdown.gameObject.SetActive(false);
                Utils.WebcamName = "";
                return;
            }

            foreach (WebCamDevice wcd in WebCamTexture.devices)
            {
                if (wcd.name.Length > 0)
                {
                    TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData(wcd.name);
                    options.Add(data);
                }
            }
            dropdown.AddOptions(options);

            //SelectDropdownToInput(dropdown, Utils.WebcamName);
        }
        else
        {
            WebCamDevice[] webCamDevice = WebCamTexture.devices;
            Debug.Log("Webcam count: " + webCamDevice.Length);
            if (webCamDevice != null && webCamDevice.Length > 0 && webCamTexture == null)
            {
#if UNITY_EDITOR
                webCamTexture = new WebCamTexture(webCamDevice[0].name, IMAGE_WIDTH, IMAGE_HEIGHT);
#elif UNITY_ANDROID || UNITY_IOS
                webCamTexture = new WebCamTexture(webCamDevice[1].name, IMAGE_WIDTH, IMAGE_HEIGHT);
#else
                webCamTexture = new WebCamTexture(webCamDevice[0].name, IMAGE_WIDTH, IMAGE_HEIGHT);
#endif
                webCamTexture.Play();
            }
        }
    }

    private bool SelectDropdownToInput(TMP_Dropdown dd, string input)
    {
        if (dd != null)
        {
            if (input.Length > 0)
            {
                for (int i = 0; i < dd.options.Count; i++)
                {
                    if (dd.options[i].text.Equals(input))
                    {
                        dd.value = i;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public IEnumerator GetImage(int inputSize, System.Action<Color32[]> callback)
    {
        yield return StartCoroutine(TextureTools.CropSquare(webCamTexture,
            TextureTools.RectOptions.Center, snap =>
            {
                var scaled = Scale(snap, inputSize);
                callback(scaled.GetPixels32());
            }));
    }
    public IEnumerator GetTexture(int inputSize, System.Action<Texture2D> callback)
    {
        yield return StartCoroutine(TextureTools.CropSquare(webCamTexture,
            TextureTools.RectOptions.Center, snap =>
            {
                var scaled = Scale(snap, inputSize);
                callback(scaled);

                shouldDelay = true;
                targetTime = Time.time + delayTime;
                SetTexture2DToWebcamView(scaled);
                SetProcessMessage("[ID_PROCESSING]");
            }));
    }

    private Texture2D Scale(Texture2D texture, int imageSize)
    {
        var scaled = TextureTools.scaled(texture, imageSize, imageSize, FilterMode.Bilinear);

        return scaled;
    }


    private Color32[] Rotate(Color32[] pixels, int width, int height)
    {
        return TextureTools.RotateImageMatrix(
                pixels, width, height, -90);
    }

    //private void Update()
    //{
    //    if (isOn == true)
    //    {
    //        UpdateCamView();
    //    }
    //    else
    //    {
    //        if (shouldDelay == true && Time.time > targetTime)
    //        {
    //            isOn = true;
    //            shouldDelay = false;
    //        }
    //    }
    //}

    private void LateUpdate()
    {
        //Utils.LogRed($"WebCamDeviceCount: {WebCamTexture.devices.Length}");

        if (shouldDelay == true)
        {
            if (Time.time > targetTime)
            {
                shouldDelay = false;
                isOn = true;
                return;
            }

            return;
        }

        if (isOn == true)
        {
            UpdateCamView();
        }
    }

    public void SwitchWebcam()
    {
        string webcamName = this.dropdown.options[dropdown.value].text;
        Debug.Log("Webcam:SwitchWebcam " + webcamName);
        Utils.WebcamName = webcamName;
        if (this.webCamTexture != null && this.webCamTexture.isPlaying)
        {
            this.webCamTexture.Stop();
            //this.webCamTexture = null;
        }
        this.webCamTexture = new WebCamTexture(webcamName, IMAGE_WIDTH, IMAGE_HEIGHT);
        if (this.webCamTexture.isReadable && webCamTexture.isPlaying == false)
        {
            webCamTexture.requestedFPS = 30f;
            foreach (var camImage in camViewImages)
            {
                camImage.texture = webCamTexture;
            }

            //this.camViewImage.texture = this.webCamTexture;
            try
            {
                this.webCamTexture.Play();
                TurnOnCamView();
            }
            catch (UnityException ex)
            {
                Utils.LogRed($"Can't use the webcam");
                MessageBox.Show(LocalizationManager.ConvertText("[ID_ERROR_CANNOT_USE]"));
                TurnOffCamView();
                SimulationWindow window = FindObjectOfType<SimulationWindow>();
                window.useCameraToggle.isOn = false;
            }
            //finally
            //{
            //    Utils.LogRed($"Can't use the webcam");
            //    MessageBox.Show(LocalizationManager.ConvertText("[ID_ERROR_CANNOT_USE]"));
            //    TurnOffCamView();
            //    SimulationWindow window = FindObjectOfType<SimulationWindow>();
            //    window.useCameraToggle.isOn = false;
            //}

            //TurnOnCamView();
        }
    }

    public void StopWebcam()
    {
        if (dropdown.gameObject.activeSelf == false)
        {
            return;
        }

        string webcamName = this.dropdown.options[dropdown.value].text;
        Utils.WebcamName = webcamName;
        if (this.webCamTexture != null && this.webCamTexture.isPlaying)
        {
            this.webCamTexture.Stop();
            //this.webCamTexture = null;
        }

        TurnOffCamView();
    }

    public void TurnOnCamView()
    {
        //if (webCamTexture == null)
        //{
        //    return;
        //}

        //if (webCamTexture.isPlaying == false)
        //{
        //    webCamTexture.Play();
        //}

        //if (camViewImage != null)
        //{
        //    camViewImage.texture = texture2D;
        //    camViewImage.color = activeColor;
        //}
        if (camViewImages != null && camViewImages.Length > 0)
        {
            foreach (var camView in camViewImages)
            {
                if (camView != null)
                {
                    camView.texture = texture2D;
                    camView.color = activeColor;
                }
            }
        }

        isOn = true;
    }

    public void TurnOffCamView()
    {
        Utils.LogRed("TurnOffCamView");

        isOn = false;
        shouldDelay = false;
        HideProcessMessageWindow();
        ClearCamView();

        //if (webCamTexture.isPlaying == true)
        //{
        //    webCamTexture.Stop();
        //}
    }

    private void OnSimulationStateChanged(bool isSimulation)
    {
        if (isSimulation == true)
        {
            TurnOnCamView();
        }
        else
        {
            TurnOffWithDelay(2f);
        }
    }

    public void TurnOffWithDelay(float delay)
    {
        shouldDelay = false;
        Invoke("TurnOffCamView", delay);
    }

    private void UpdateCamView()
    {
        foreach (var messageWindow in processMessageWindows)
        {
            if (messageWindow.gameObject.activeSelf == true)
            {
                messageWindow.Hide();
            }
        }

        //if (processMessageWindow.activeSelf == true)
        //{
        //    HideProcessMessageWindow();
        //}

        //if (webCamTexture == null)
        //{
        //    return;
        //}

        //if (webCamTexture.isPlaying == false)
        //{
        //    webCamTexture.Play();
        //}

        UpdateWebcamTexture();
    }

    private void SetTexture2DToWebcamView(Texture2D texture)
    {
        if (webCamTexture == null)
        {
            return;
        }

        if (texture2D == null || texture2D.width != texture.width || texture2D.height != texture.height)
        {
            texture2D = new Texture2D(texture.width, texture.height);

            if (camViewImages != null && camViewImages.Length > 0)
            {
                foreach (var camView in camViewImages)
                {
                    if (camView != null)
                    {
                        camView.texture = texture2D;
                        camView.color = activeColor;
                    }
                }
            }

            //if (camViewImage != null)
            //{
            //    camViewImage.texture = texture2D;
            //    camViewImage.color = activeColor;
            //}

            colors = new Color32[texture.width * texture.height];
        }

        texture2D.SetPixels32(texture.GetPixels32());
        texture2D.Apply();
    }

    Texture2D texture2D = null;
    Color32[] colors;
    private Texture2D UpdateWebcamTexture()
    {
        //Texture2D texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);
        if (this.webCamTexture == null)
            return texture2D;

        if (texture2D == null)
        {
            texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);

            if (camViewImages != null && camViewImages.Length > 0)
            {
                foreach (var camView in camViewImages)
                {
                    if (camView != null)
                    {
                        camView.texture = texture2D;
                        camView.color = activeColor;
                    }
                }
            }

            //if (camViewImage != null)
            //{
            //    camViewImage.texture = texture2D;
            //    camViewImage.color = activeColor;
            //}

            colors = new Color32[webCamTexture.width * webCamTexture.height];

            //Debug.Log("Webcam Hello");
        }

        if (webCamTexture.isPlaying)
        {
            if (colors.Length != webCamTexture.width * webCamTexture.height)
            {
                colors = new Color32[webCamTexture.width * webCamTexture.height];
            }

            webCamTexture.GetPixels32(colors);
        }

        //texture2D.SetPixels(webCamTexture.GetPixels());

        // 작성: 장세윤 (20210414).
        // 앞에서 생성한 texture2D의 너비와 높이가 webCamTexture와 맞지 않는 경우가 생겨 예외처리 추가.
        if (texture2D.width != webCamTexture.width || texture2D.height != webCamTexture.height)
        {
            texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);

            if (camViewImages != null && camViewImages.Length > 0)
            {
                foreach (var camView in camViewImages)
                {
                    if (camView != null)
                    {
                        camView.texture = texture2D;
                        camView.color = activeColor;
                    }
                }
            }

            //camViewImage.texture = texture2D;
            //camViewImage.color = activeColor;
        }

        texture2D.SetPixels32(colors);
        texture2D.Apply();
        //if (camViewImage != null)
        //{
        //    camViewImage.texture = texture2D;
        //    camViewImage.color = activeColor;
        //}

        return texture2D;
    }

    public IEnumerator PostPicture(string uri, string name = null)  //52.78.62.151:5000/")
    {
        //if (webCamTexture == null)
        //{
        //    return;
        //}

        //if (webCamTexture.isPlaying == false)
        //{
        //    webCamTexture.Play();
        //}

        // Test.
        //TurnOnCamView();
        shouldDelay = true;
        targetTime = Time.time + delayTime;
        isOn = false;
        SetProcessMessage("[ID_PROCESSING]");

        _analysisResult = "";

        yield return StartCoroutine(Upload(uri, null, name));
    }

    #region void PostPicture 백업
    //public void PostPicture(string uri, string name = null)  //52.78.62.151:5000/")
    //{
    //    //if (webCamTexture == null)
    //    //{
    //    //    return;
    //    //}

    //    //if (webCamTexture.isPlaying == false)
    //    //{
    //    //    webCamTexture.Play();
    //    //}

    //    // Test.
    //    //TurnOnCamView();
    //    shouldDelay = true;
    //    targetTime = Time.time + delayTime;
    //    isOn = false;
    //    SetProcessMessage("[ID_DETECTING_FRAME]");

    //    _analysisResult = "";

    //    StartCoroutine(Upload(uri, null, name));
    //}
    #endregion

    private WaitForSeconds poseWait = new WaitForSeconds(0.028f);
    private WaitForEndOfFrame waitEndFrame = new WaitForEndOfFrame();
    private int poseFrameCount = 44;
    private readonly string activityString = "activity";
    private readonly string yetString = "yet";

    class PoseReturnJson
    {
        public string activity;
    }

    public IEnumerator PostPose(string uri)
    {
        float waitTime = Time.time + 20f;

        while (true)
        {
            shouldDelay = true;
            targetTime = Time.time + delayTime;
            isOn = false;
            SetProcessMessage("[ID_PROCESSING]");

            _analysisResult = "";

            //Utils.LogBlue("here 0");

            //Coroutine<string> routine = this.StartCoroutine<string>(Upload(uri, null));
            Coroutine<string> routine = this.StartCoroutine<string>(Upload(uri, null, FirebaseManager.CurrentUserDesc.uuid));


            // 타임아웃.
            //Utils.LogRed($"Time.time: {Time.time} / waitTime: {waitTime}");
            if (Time.time > waitTime)
            {
                //Utils.LogBlue("pose rec timeout");
                yield return false;
                yield break;
            }

            yield return routine.coroutine;

            //Utils.LogBlue("here 1");

            if (routine.returnValue.Length > 0)
            {
                //Utils.LogBlue($"here 2: {routine.returnValue}");

                PoseReturnJson result = JsonUtility.FromJson<PoseReturnJson>(routine.returnValue);
                if (result != null && result.activity.Equals(yetString) == false)
                {
                    //Utils.LogBlue("here 3");
                    yield return true;
                    yield break;
                }
            }
        }

        //for (int ix = 0; ix < poseFrameCount; ix++)
        //{
        //    shouldDelay = true;
        //    targetTime = Time.time + delayTime;
        //    isOn = false;
        //    SetProcessMessage("[ID_DETECTING_FRAME]");

        //    _analysisResult = "";

        //    yield return StartCoroutine(Upload(uri, null));

        //    //yield return StartCoroutine(PostPicture(uri));
        //}

        //yield return StartCoroutine(PostPicture(uri));
    }

    public void sendPicture(string uri, string image, string name = null)  //52.78.62.151:5000/")
    {
        _analysisResult = "";
        Debug.Log("image string : " + image);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(image);
        Debug.Log("image bytes : " + bytes);
        //File.WriteAllBytes("image.png", bytes);
        Debug.Log(uri);
        StartCoroutine(Upload(uri, bytes, name));
    }

    private string _analysisResult;
    public string Result
    {
        get { return _analysisResult; }
    }

    public void SetProcessMessage(string message)
    {
        foreach (var messageWindow in processMessageWindows)
        {
            messageWindow.SetMessage(message);
        }

        //processMessageWindow.SetActive(true);
        //messageText.text = message;
        //LocalizationManager.Localize(messageText);
    }

    public void HideProcessMessageWindow()
    {
        foreach (var messageWindow in processMessageWindows)
        {
            if (messageWindow.gameObject.activeSelf == true)
            {
                messageWindow.Hide();
            }
        }

        //processMessageWindow.SetActive(false);
        //messageText.text = string.Empty;
    }

    public void ClearCamView()
    {
        if (camViewImages != null && camViewImages.Length > 0)
        {
            foreach (var camView in camViewImages)
            {
                if (camView != null)
                {
                    camView.texture = null;
                    camView.color = inactiveColor;
                }
            }
        }

        //camViewImage.texture = null;
        //camViewImage.color = inactiveColor;
    }

    public WebCamTexture returnTexture()
    {
        return webCamTexture;
    }

    IEnumerator Upload(string uri, byte[] image, string name = null)
    {
        #region 코드 백업
        //Utils.LogRed("UploadStart: " + Time.realtimeSinceStartup);

        // 작성자: 장세윤 (2020.10.08).
        // 대기시간 없이 바로 사진찍어 보내도록 로직 변경.
        //Texture2D texture2D = null;
        //for (int i = 0; i < 20; i++)
        //{
        //    texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);
        //    texture2D.SetPixels(webCamTexture.GetPixels());
        //    texture2D.Apply();
        //    if (camViewImage != null)
        //    {
        //        camViewImage.texture = texture2D;
        //    }

        //    yield return new WaitForSeconds(0.1f);
        //}

        //Texture2D texture2D = new Texture2D(webCamTexture.width, webCamTexture.height);
        //texture2D.SetPixels(webCamTexture.GetPixels());
        //texture2D.Apply();
        //if (camViewImage != null)
        //{
        //    camViewImage.texture = texture2D;
        //    camViewImage.color = activeColor;
        //}
        //yield return new WaitForSeconds(0.1f);
        #endregion
        UpdateWebcamTexture();

        //if (image == null)
        //{
        //    //Utils.LogGreen("Image Set");
        //    if (texture2D != null)
        //    {
        //        image = texture2D.EncodeToPNG();
        //    }
        //}

        if (texture2D != null)
        {
            image = texture2D.EncodeToPNG();
        }

        //Utils.LogRed("UploadMiddle: " + Time.realtimeSinceStartup);
        WWWForm form = new WWWForm();
        form.AddBinaryData("image", image);
        if (name != null)
        {
            form.AddField("name", name);
        }

        Debug.Log("Webcam:Upload to " + uri.ToString());
        using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
        {
            //www = UnityWebRequest.Post(uri, form);
            yield return www.SendWebRequest();

            //Utils.LogRed("UploadEnd: " + Time.realtimeSinceStartup);

            if (www.isNetworkError || www.isHttpError)
            {
                _analysisResult = "_ERROR_";
                Debug.Log(www.error);

                yield return _analysisResult;
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                _analysisResult = www.downloadHandler.text;

                yield return _analysisResult;
            }
        }
    }
}