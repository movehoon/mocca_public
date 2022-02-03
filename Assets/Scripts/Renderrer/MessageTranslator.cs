using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace REEL.Animation
{
    public class MessageTranslator : MonoBehaviour
    {
        public TextAsset riveScriptFile;
        public RobotFacialRenderer robotFacialRenderer;
        public Text debugText;

        string rsFile = "/begin.txt";

        //RiveScript.RiveScript riveScript = new RiveScript.RiveScript(utf8: true, debug: true);

        public void Awake()
        {
            Init();
        }

        void Init()
        {
            //string[] lines = riveScriptFile.text.Split(new char[] { '\n', '\r' });
            //if (!riveScript.parse(rsFile, lines))
            //    ShowDebugTest("RS File Not Loaded");

            //riveScript.sortReplies();
        }

        void ShowDebugTest(string text)
        {
            debugText.text = text;
        }

        public void SetMessage(string input)
        {   
            //string reply = riveScript.reply("default", input);
            ////Debug.Log(reply);
            //Process(reply);
        }

        void Process(string reply)
        {
            Regex rx = new Regex("(<[^>]+>)");
            MatchCollection matches = rx.Matches(reply);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    GroupCollection groupCollection = match.Groups;
                    String command = groupCollection[1].ToString();
                    reply = reply.Replace(command, "");
                    command = command.Substring(1).Substring(0, command.Length - 2);

                    int index = command.IndexOf("=");
                    if (index > 0)
                    {
                        String tag = command.Substring(0, index);
                        command = command.Substring(index + 1);

                        switch (tag)
                        {
                            case "sm":
                                String[] detail = Regex.Split(command, ":");
                                if (detail.Length > 0)
                                {
                                    switch (detail[0])
                                    {
                                        case "facial":
                                            ShowDebugTest("Sub command facial with " + detail[1]);
                                            //Debug.Log("Sub command facial with " + detail[1]);
                                            robotFacialRenderer.Play(detail[1]);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                break;
                            default:
                                ShowDebugTest("No matched command with " + tag);
                                //Debug.Log("No matched command with " + tag);
                                break;
                        }
                    }
                }
            }
        }
    }
}