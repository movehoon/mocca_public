using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotOvrLipSyncContext : OVRLipSyncContextBase
{

	[Tooltip("Play input audio back through audio output.")]
	public bool audioLoopback = false;


	[Tooltip("Skip data from the Audio Source. Use if you intend to pass audio data in manually.")]
	public bool skipAudioSource = false;


	[Tooltip("Adjust the linear audio gain multiplier before processing lipsync")]
	public float gain = 1.0f;


	// Manually assign the material
	public Material material = null;


	public SpriteRenderer mouthSprite = null;


	[Tooltip("The texture used for each viseme.")]
	[OVRNamedArray(new string[] { "sil", "PP", "FF", "TH", "DD", "kk", "CH",
		"SS", "nn", "RR", "aa", "E", "ih", "oh", "ou" })]
	public Texture[] Textures = new Texture[OVRLipSync.VisemeCount];


	[Tooltip("The texture used for each viseme.")]
	[OVRNamedArray(new string[] { "sil", "PP", "FF", "TH", "DD", "kk", "CH",
		"SS", "nn", "RR", "aa", "E", "ih", "oh", "ou" })]
	public Sprite[] sprite = new Sprite[OVRLipSync.VisemeCount];




	[OVRNamedArray(new string[] { "sil", "PP", "FF", "TH", "DD", "kk", "CH",
		"SS", "nn", "RR", "aa", "E", "ih", "oh", "ou" })]
	public float[] vieme = new float[OVRLipSync.VisemeCount];


	// smoothing amount
	[Range(1, 100)]
	[Tooltip("Smoothing of 1 will yield only the current predicted viseme," +
		"100 will yield an extremely smooth viseme response.")]
	public int smoothAmount = 70;



	string[] vimeName = { "sil", "PP", "FF", "TH", "DD", "kk", "CH",
		"SS", "nn", "RR", "aa", "E", "ih", "oh", "ou" };


	// PRIVATE

	// Look for a Phoneme Context (should be set at the same level as this component)
	private OVRLipSyncContextBase lipsyncContext = null;

	// Capture the old viseme frame (we will write back into this one)
	private OVRLipSync.Frame oldFrame = new OVRLipSync.Frame();



	public Text uiText = null;
	static public string VismeString = "";

	void Start()
	{
		// make sure there is a phoneme context assigned to this object
		lipsyncContext = GetComponent<OVRLipSyncContextBase>();
		if (lipsyncContext == null)
		{
			Debug.LogWarning("LipSyncContextTextureFlip.Start WARNING:" +
				" No lip sync context component set to object");
		}
		else
		{
			// Send smoothing amount to context
			lipsyncContext.Smoothing = smoothAmount;
		}

		if (material == null)
		{
			Debug.LogWarning("LipSyncContextTextureFlip.Start WARNING:" +
				" Lip sync context texture flip has no material target to control!");
		}

		//mouthSprite = FindObjectOfType<MouthSaySprite>();

	}

	void Update()
	{
		if ((lipsyncContext != null) && (material != null))
		{
			OVRLipSync.Frame frame = lipsyncContext.GetCurrentPhonemeFrame();
			if (frame != null)
			{
				if (lipsyncContext.provider == OVRLipSync.ContextProviders.Original)
				{
					for (int i = 0; i < frame.Visemes.Length; i++)
					{
						float smoothing = ((smoothAmount - 1) / 100.0f);
						oldFrame.Visemes[i] =
							oldFrame.Visemes[i] * smoothing +
							frame.Visemes[i] * (1.0f - smoothing);
					}
				}
				else
				{
					oldFrame.Visemes = frame.Visemes;
				}

				SetVisemeToTexture();
			}
		}

		// Update smoothing value in context
		if (smoothAmount != lipsyncContext.Smoothing)
		{
			lipsyncContext.Smoothing = smoothAmount;
		}
	}


	public void ProcessAudioSamples(float[] data, int channels)
	{
		// Do not process if we are not initialized, or if there is no
		// audio source attached to game object
		if ((OVRLipSync.IsInitialized() != OVRLipSync.Result.Success) || audioSource == null)
			return;

		// Increase the gain of the input
		for (int i = 0; i < data.Length; ++i)
			data[i] = data[i] * gain;

		// Send data into Phoneme context for processing (if context is not 0)
		lock (this)
		{
			if (Context != 0)
			{

				OVRLipSync.Frame frame = this.Frame;
				OVRLipSync.ProcessFrame(Context, data, frame, channels == 2);
			}
		}

		// Turn off output (so that we don't get feedback from mics too close to speakers)
		if (!audioLoopback)
		{
			for (int i = 0; i < data.Length; ++i)
				data[i] = data[i] * 0.0f;
		}
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
		if (!skipAudioSource)
		{
			ProcessAudioSamples(data, channels);
		}
	}






	void SetVisemeToTexture()
	{
		int gV = -1;
		float gA = 0.0f;
		string name = "none";

		for (int i = 0; i < oldFrame.Visemes.Length; i++)
		{
			if (oldFrame.Visemes[i] > gA)
			{
				gV = i;
				gA = oldFrame.Visemes[i];
			}

			vieme[i] = oldFrame.Visemes[i];
		}

		if ((gV != -1) && (gV < Textures.Length))
		{
			Texture t = Textures[gV];

			if (t != null)
			{
				material.SetTexture("_MainTex", t);
			}

			if (mouthSprite != null && (gV < sprite.Length))
			{
				mouthSprite.sprite = sprite[gV];

				name = vimeName[gV];
			}
		}


		if (name != "sil")
		{
			VismeString += " " + name;

			if (uiText != null)
			{
				uiText.text = VismeString;
			}
		}
		else
		{
			VismeString = "";
		}

	}

}
