using Malee.List;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using Tutorial;
using System;
using REEL.D2EEditor;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager   Instance;

    public static void SendEvent(Tutorial.CustomEvent ev,string text = "")
    {
#if UNITY_EDITOR
        Debug.Log("TutorialManager Event : " + ev.ToString() + " text : " + text);
#endif

        if (IsPlaying == false) return;
        if(Instance == null) return;

        Instance.OnCustomEvent(ev,text);
	}


    public static void SendEvent(MCNode newNode)
    {
        if(IsPlaying == false) return;
        if(Instance == null) return;

        Instance.OnCustomEvent(newNode);
    }


	public Sequence             CurrentSequence
    {
        get;set;
	}

    public static bool          IsPlaying = false;

    public static Vector2       NodeCreatePosition = Vector2.zero;

	public bool IsBusy    
    { 
        get 
        {
            if( CurrentSequence != null )
            {
                if(CurrentSequence.isWaiting == true)
                    return true;
			}

            if(functions == null || functions.Length == 0) return false;

            foreach(var f in functions)
            {
                if(f.isWaiting == true)
                    return true;
			}

            return false;
        }
    }



	[Header("Canvas")]
    public GameObject           canvas;


    [Header("Scrollbars")]
    public RectTransform        blockRect;
    public Scrollbar            blockScrollbar;
    public Scrollbar[]          scrollbars;


    [Header("Object")]
    public GameObject           exitButton;
    public GameObject           simulatorBlock;
    public GameObject           disablePrefab;


    [Header("Video Player")]
    public TutorialMoviePlayer  videoTutorial;


    [Header("Functions")]
    public FunctionBase[]       functions;

    [Header("Test")]
    public int                  testNumber = 2;
    public bool                 testRun = false;

    [Header("Use Test Key")]
    public bool                 useTestKey = true;
    public KeyCode              testKeyCode = KeyCode.F3;


    public int                  lastTutorialIndex = 0;

	TutorialManager()
    {
        Instance = this;
    }

    void Start()
    {
    }

    void Update()
    {
        if(CurrentSequence != null && CurrentSequence.isRunning == true)
        {
            if(IsBusy == false)
            {
                if(CurrentSequence.PlayNextStep() == false)
                {
                    IsPlaying = false;
                    ExitTutorial(true);
                }
            }
        }

        //if(useTestKey && Input.GetKeyDown(KeyCode.F2))
        //{
        //    if(videoTutorial.gameObject.activeInHierarchy == false)
        //    {
        //        videoTutorial.gameObject.SetActive(true);
        //    }
        //}

#if UNITY_EDITOR

        if(useTestKey && Input.GetKeyDown(testKeyCode))
        {
            if(IsPlaying)
            {
                SkipSequenceStep();
            }
            else
            {
                PlaySequence(testNumber);
            }
        }

        if(testRun == true)
        {
            testRun = false;

            PlaySequence(testNumber);
        }
#endif
    }


    public void Test(FunctionBase.ObjectFlag flg)
    {

	}

    public Sequence[] GetSequences()
    {
        return gameObject.GetComponentsInChildren<Sequence>();
	}

    public VideoSequence[] GetVideoSequences()
    {
        return FindObjectsOfType<VideoSequence>();
	}

    public bool PlaySequence(string tutorialName)
    {
        var sequences = GetSequences();

        if(sequences == null) return false;

        var seq = sequences.FirstOrDefault(x=> tutorialName == x.title);

        if(seq == null) return false;

        return PlaySequence(seq.numbering);
    }

    public bool PlaySequence(int numbering)
    {
        if(FirebaseManager.CheckLogin() == false)
        {
            LoginCheck.Instance.OpenLoginWindow();
            return false;
		}

        foreach(var func in functions)
        func.OnClear();

        if(CurrentSequence != null && CurrentSequence.isRunning == true )
        {
            CurrentSequence.Stop();
            CurrentSequence = null;
        }

        var sequences = GetSequences();

        if(sequences == null) return false;


        lastTutorialIndex = numbering;
        ResetEditor();

        var seq = sequences.FirstOrDefault(x=> numbering == x.numbering );

        if(seq == null) return false;

        CurrentSequence = seq;
        functions = gameObject.GetComponents<FunctionBase>();
        NodeCreatePosition = Vector2.zero;

        if( CurrentSequence.PlayStart() == true )
        {
            IsPlaying = true;
            exitButton.gameObject.SetActive(true);
            simulatorBlock.gameObject.SetActive(true);
        }

        return true;
    }

    public void PlayVideo(int numbering)
    {
        var sequences = GetVideoSequences();

        if(sequences == null) return;

        var seq = sequences.FirstOrDefault(x=> numbering == x.numbering );

        if(seq == null) return;

        if(videoTutorial.gameObject.activeInHierarchy == false)
        {
            videoTutorial.Play(seq);
        }
    }

    public void ResetEditor()
	{
        ResetContentScaler();
        ResetScrollbars();

        ResetBlockList();
    }

	private void ResetBlockList()
	{
        UnCollapseBlockCategory();  //좌측 블록카테고리 전부 열기
	}

    private void UnCollapseBlockCategory()
    {
        var objs = FindObjectsOfType<BlockCategory>();

        foreach(var obj in objs)
        {
            obj.Open();
        }
    }

    public void ResetContentScaler()
	{
        if(ContentScaler.Instance == null) return;

        ContentScaler.Instance.ResetAll();
    }

	public void ResetScrollbars()
	{
        foreach(var s in scrollbars)
        {
            if( s.direction == Scrollbar.Direction.BottomToTop )
            {
                s.value = 1.0f;
			}
            else 
            {
                s.value = 0.0f;
			}
		}
	}

	public void EnableWindowArea(Tutorial.WindowAreaFlg areas)
	{
        GetComponent<WindowArea>().Enable(areas);
	}

    public void SetBlockScrollbar(float yPosition)
    {
        yPosition = Math.Abs(yPosition);

        float yP = yPosition / blockRect.rect.height;

        blockScrollbar.value = 1.0f - yP;
	}


    private void OnCustomEvent(CustomEvent ev,string text)
    {
        if(IsPlaying == false) return;
        if(CurrentSequence == null) return;

        Array.ForEach(functions, x => x.OnCustomEvent(ev, text));

        CurrentSequence.OnCustomEvent(ev, text);
    }

    private void OnCustomEvent(MCNode newNode)
    {
        if(IsPlaying == false) return;
        if(CurrentSequence == null) return;

        if(NodeCreatePosition == Vector2.zero) return;

        newNode.transform.position = NodeCreatePosition;

        NodeCreatePosition = Vector2.zero;

        newNode.DisableUserControl();
    }


    public void SkipSequenceStep()
    {
        if(IsPlaying == false) return;
        if(CurrentSequence == null) return;

        Array.ForEach(functions, x => x.OnSkip());

        CurrentSequence.OnSkip();

    }


    public void ShowMessageBox(string msg)
    {
        msg = LocalizationManager.ConvertText(msg, "tutorial", "common");
        MessageBox.Show(msg);
    }

    public void NextTutorial()
    {
        MessageBox.ShowYesNo(LocalizationManager.ConvertText("[ID_NEXT_TUTORIAL]다음 튜토리얼로 진행할까요?", "tutorial"), (res) =>
        {
            if( res == true)
            {
                PlaySequence(lastTutorialIndex + 1);
			}
        });
	}


    public void OnExitClicked()
    {
        MessageBox.ShowYesNo("튜토리얼을 종료 하시겠습니까?" , (res)=>
        {
            MessageBox.Close();

            if( res == true )
            {
                ExitTutorial(false);
            }
        });
    }



    public void EnableBlockList(bool enabled)
    {
        foreach(var obj in FindObjectsOfType<NodeListItemComponent>() )
        {
            obj.enabled = enabled;
		}
	}


	private void ExitTutorial(bool completed)
	{
        IsPlaying = false;
        exitButton.gameObject.SetActive(false);
        simulatorBlock.gameObject.SetActive(false);
        NodeCreatePosition = Vector2.zero;

        if(CurrentSequence != null && CurrentSequence.isRunning == true)
        {
            CurrentSequence.Stop();
        }

        Array.ForEach(functions, x => x.OnEndSequence(completed) );

        if(REEL.D2EEditor.MCWorkspaceManager.Instance != null )
        {
            //if(REEL.D2EEditor.MCWorkspaceManager.Instance.IsSimulation == true)
            if (REEL.D2EEditor.MCPlayStateManager.Instance.IsSimulation == true)
            {
                REEL.D2EEditor.MCWorkspaceManager.Instance.StopProject();
                REEL.D2EEditor.MCPlayStateManager.Instance.IsSimulation = false;
            }

            REEL.D2EEditor.MCWorkspaceManager.Instance.CloseCurrentProject();
        }

        EnableBlockList(true);
    }
}