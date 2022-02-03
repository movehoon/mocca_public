using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace REEL.PoseAnimation
{
    //public class RobotTransformController : Singleton<RobotTransformController>
    public class RobotTransformController : MonoBehaviour
    {
        public JointSet[] jointInfo;
        public bool breath = true;

        public string currentGesture;

        string _gesture;
        float duration = -1;

        bool _breathe_enable = true;

        private float rotSpeed = 360f;
        private float rotSpeedPercentage = 0.8f;

        readonly static string MOTION_DIR = "/MotionData/";

        // Y, 	Z, 	  Z,   Y, 	X, Z
        float[] zeroAngle = new float[8] { 0f, -90f, 90f, 90f, 0f, 0f, 0f, 0f };
        float[] baseAngle = new float[8] { 45f, -45f, -45f, -45f, 45f, 45f, 0f, 0f };
        float[] DIRECTION = new float[8] { 1f, -1f, -1f, -1f, -1f, -1f, -1f, -1f };

        float[] OFFSET = new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

        #region 모션 데이터 백업
        //     float[][] hiList = {
        //				// Time,	Left Arm, 			Right Arm,			Head
        //new float[9] {  1.5f,        45, -45, -45,       60, 60, 38,         0, 20      },
        //         new float[9] {  0.7f,        45, -45, -45,       60, 80, 34,         0, 20     },
        //         new float[9] {  0.7f,        45, -45, -45,       60, 60, 38,         0, 20     },
        //         new float[9] {  0.7f,        45, -45, -45,       60, 80, 34,         0, 20     },
        //         new float[9] {  1.5f,        45, -45, -45,       -45, 45, 45,        0, 20    }
        //     };
        //     float[][] helloList = {
        //				// Time,	Left Arm, 			Right Arm,		    Head
        //new float[9] {  1.5f,       -60f, -60f, -38f,   60f, 60f, 38f,      0f, 20f   },
        //         new float[9] {  0.7f,       -60f, -80f, -34f,   60f, 80f, 34f,      0f, 20f   },
        //         new float[9] {  0.7f,       -60f, -60f, -38f,   60f, 60f, 38f,      0f, 20f   },
        //         new float[9] {  0.7f,       -60f, -80f, -34f,   60f, 80f, 34f,      0f, 20f   },
        //         new float[9] {  1.5f,        45f, -45f, -45f,   -45f, 45f, 45f,     0f, 20f   }
        //     };
        //     float[][] angryList = {
        //				// Time,	Left Arm, 		    Right Arm,			Head
        //new float[9] {  0.8f,        -20, -70, -24,      20, 70, 24,         0, -2      },
        //         new float[9] {  0.8f,        45, -70, -24,       -45, 70, 24,        0, -2      },
        //         new float[9] {  0.8f,        -20, -70, -24,      20, 70, 24,         0, -2      },
        //         new float[9] {  0.8f,        45, -45, -45,       -45, 45, 45,        0, 20      }
        //     };
        //     float[][] sadList = {
        //             			// Time,    Left Arm,           Right Arm,          Head
        //   new float[9] {  1.2f,       -24, -80, -67,      42, 80, 67,         0, 5   },
        //         new float[9] {  0.5f,       -42, -80, -67,      24, 80, 67,         0, 5   },
        //         new float[9] {  0.5f,       -24, -80, -67,      42, 80, 67,         0, 5   },
        //         new float[9] {  0.5f,       -42, -80, -67,      24, 80, 67,         0, 5   },
        //         new float[9] {  1.2f,        45, -45, -45,      -45, 45, 45,        0, 20  }
        //     };

        //     float[][] okList = {
        //             			// Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0, -10    },
        //         new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0, 20     },
        //         new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0,  -10   },
        //         new float[9] {  0.6f,       45, -45, -45,       -45, 45, 45,        0,  20    }
        //     };
        //     float[][] noList = {
        //             			// Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.7f,       45, -45, -45,       -45, 45, 45,        -20, 10 },
        //         new float[9] {  0.7f,       45, -45, -45,       -45, 45, 45,         20, 10 },
        //         new float[9] {  0.7f,       45, -45, -45,       -45, 45, 45,        -20, 10 },
        //         new float[9] {  0.7f,       45, -45, -45,       -45, 45, 45,         0,  20 }
        //     };

        //     float[][] wrongList =
        //     {
        //         new float[9] {  1.1f,       -20, -45, -45,    -60, 45, 45,         0, 10  },
        //         new float[9] {  0.9f,        60, -45, -45,     20, 45, 45,         0, 10  },
        //         new float[9] {  0.9f,       -20, -45, -45,    -60, 45, 45,         0, 10  },
        //         new float[9] {  0.9f,        45, -45, -45,    -45, 45, 45,         0, 10  },
        //     };

        //     float[][] happyList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.6f,       -30, -95, -57,      30, 95, 57,          0, -15 },
        //         new float[9] {  0.8f,       -30, -95, -57,      30, 95, 57,         -14, -2 },
        //         new float[9] {  0.8f,       -30, -95, -57,      30, 95, 57,          0, -15 },
        //         new float[9] {  1.6f,        45, -45, -45,      -45, 45, 45,         0, 20  }
        //     };
        //     float[][] refuseList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.9f,       64, -41, -16,       -64, 41, 16,         25, -13   },
        //         new float[9] {  0.9f,       48, -53, -50,       -52, 48, 50,        -25, -13   },
        //         new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45,          0, 20    }
        //     };
        //     float[][] smileList = new float[5][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  1.5f,       -31,-94,-40,        31,94,40,           0,-10   },
        //         new float[9] {  0.5f,       -28,-101,-62,       28,101,62,          0,-10   },
        //         new float[9] {  0.5f,       -31,-94,-40,        31,94,40,           0,-10   },
        //         new float[9] {  0.5f,       -28,-101,-62,       28,101,62,          0,-10   },
        //         new float[9] {  1.5f,        45, -45, -45,      -45, 45, 45,        0, 20   }
        //     };
        //     float[][] takenabackList = new float[3][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.7f,        32, -28, -5,       -32, 23, 5,         0,-25    },
        //         new float[9] {  1.2f,        32, -28, -5,       -32, 23, 5,         0,-25   },
        //         new float[9] {  0.8f,        45, -45, -45,      -45, 45, 45,        0, 20  }
        //     };
        //     float[][] tellList = new float[5][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.9f,       4,-59,-50,          -4,59,50 ,           0,-12      },
        //         new float[9] {  0.5f,       4,-77,-70,          -4,77,70,            0,-12      },
        //         new float[9] {  0.5f,       4,-58,-50,          -4,58,50,            0,-12      },
        //         new float[9] {  0.5f,       4,-77,-70,          -4,77,70,            0,-12      },
        //         new float[9] {  0.9f,       45, -45, -45,       -45, 45, 45,         0, 20      }
        //     };
        //     float[][] sleepyList = new float[4][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.4f,       50,-71,-11,         -50,71,11,           0,-20      },
        //         new float[9] {  1.4f,       50,-71,-11,         -50,71,11,           0,20       },
        //         new float[9] {  0.4f,       50,-71,-11,         -50,71,11,           0,-20      },
        //         new float[9] {  1.4f,       45, 45, -45,        -45, 45, 45,         0, 20      }
        //     };
        //     float[][] thinkingList = new float[6][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.5f,       45,-82,-16,         30,85,60,           -17,-13     },
        //         new float[9] {  0.8f,       45,-82,-16,         30,85, 60,          -29,8       },
        //         new float[9] {  0.8f,       45,-82,-16,         30,85,60 ,           1,17       },
        //         new float[9] {  0.8f,       45,-82,-16,         30,85,60   ,        -10,-16     },
        //         new float[9] {  0.8f,       45,-82,-16,         15,85,60   ,         1,-2       },
        //         new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45,         0, 20      }
        //     };
        //     float[][] excellentList = new float[5][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.2f,       -68, 29, 17,        68, -29, -17,      0, 5 },
        //         new float[9] {  0.9f,       -60, 78, 27,        60, -78, -27,      0, 5 },
        //         new float[9] {  0.9f,       -68, 29, 17,        68, -29, -17,      0, 5 },
        //         new float[9] {  0.9f,       -60, 78, 27,        60, -78, -27,      0, 5 },
        //         new float[9] {  1.2f,        45, -45, -45,      -45, 45, 45,       0, 20  }
        //     };
        //     float[][] shakeList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       45,-45,-45,        10,45,45,            0,10  },
        //         new float[9] {  0.5f,       45,-45,-45,        -30,45,40,           0,10  },
        //         new float[9] {  0.5f,       45,-45,-45,        10,45,45,            0,10  },
        //         new float[9] {  0.5f,       45,-45,-45,        -30,45,40,           0,10   },
        //         new float[9] {  0.5f,       45,-45,-45,        -45,45,45,           0,10  },
        //     };
        //     float[][] stretchList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  2.2f,       0, 0, 0,            0, 0, 0,            0, -13  },
        //         new float[9] {  1.2f,       45, -45, -45,      -45, 45, 45,        0, 20   }
        //     };
        //     float[][] hugList = new float[3][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.5f,       20, 74, 50,         -20, -74, -50 ,     0,20},
        //         new float[9] {  1.1f,       20, 74, 50,         -20, -74, -50 ,     0,20},
        //         new float[9] {  0.8f,       45, 45, 45,         -45, -45, -45,      0, 20 }
        //     };
        //     float[][] fearList = new float[3][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       121, -30, -30,      -21, 30, 30 ,        20, 0 },
        //         new float[9] {  0.6f,       71 , -30, -30,      -71, 30, 30 ,        20, 0 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 10 }
        //     };
        //     float[][] bowList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.5f,       60, -41, -67,       -62, 42, 73,         0, -19 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] attentionList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.5f,       45, -77, -28,       -45, 77, 28,         0, 0 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] bestList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.9f,       12,-72,-57,         12,72,57,           0,10  },
        //         new float[9] {  0.5f,       -12,-72,-57,        -12,72,57,          0,10},
        //         new float[9] {  0.5f,       12,-72,-57,         12,72,57,           0,20   },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] leftNeckList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45 ,       40, 40 },
        //         new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45 ,       0, 10  }
        //     };
        //     float[][] rightNeckList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45 ,       -40, 40 },
        //         new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45 ,       0, 10   }
        //     };
        //     float[][] forwardList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45,        0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] coachList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.5f,       45, -45, -45,       -45, 45, 45,        0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] requireList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45,        0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] resetList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.5f,       45, -45, -45,       -45, 45, 45,        0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] exchangedList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45,        0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] headTiltList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45,        31, -12 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] headBackList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.2f,       45, -45, -45,       -45, 45, 45,        19, 71 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] headUpList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45,        0, 30 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] headRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45, -26, 7 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] headLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       45,-45,-45,         -45,45,45,          26,7 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] headDownList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       45, -45, -45,       -45, 45, 45,        0, -10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] headList = new float[3][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       45,-45,-45,         -45,45,45,          0,-18  },
        //         new float[9] {  1.6f,       45,-45,-45,         -45,45,45,          0,18   },
        //         new float[9] {  0.8f,       45,-45,-45,         -45,45,45,          0,-18   }
        //     };

        //     float[][] armsUpList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       -40, -45, -45,      40, 45, 45,         0, 10 },
        //         new float[9] {  1f,          45, -45, -45,      -45, 45, 45,        0, 20 }
        //     };
        //     float[][] armsUpRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       45, -45, -45,       40, 45, 45,         0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45,        0, 20 }
        //     };
        //     float[][] armsUpLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       -40, -45, -45,      -45, 45, 45,        0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,       0, 20 }
        //     };
        //     float[][] armsDownList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       60, -40, -45,       -60, 45, 40,         0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsDownRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       45, -45, -45,       -60, 45, 40,         0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsDownLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       60, -40, -45,       -45, 45, 45,         0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsUpdownList = new float[4][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       -40,-45,-45,        40,45,45,           0,10  },
        //         new float[9] {  0.8f,       60,-40,-45,         -60,45,40,           0,10  },
        //         new float[9] {  0.8f,       -40,-45,-45,        40,45,45,           0,10  },
        //         new float[9] {  0.8f,       60,-40,-45,         -60,45,40,           0,10   }
        //     };
        //     float[][] armsUpdownRightList = new float[4][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       45,-45,-45,         40,45,45,          0,10  },
        //         new float[9] {  0.8f,       45,-45,-45,         -60,45,40,           0,10  },
        //         new float[9] {  0.8f,       45,-45,-45,         40,45,45,          0,10  },
        //         new float[9] {  0.8f,       45,-45,-45,         -60,45,40,           0,10   }
        //     };
        //     float[][] armsUpdownLeftList = new float[4][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       -40,-45,-45,        -45, 45, 45,          0,10  },
        //         new float[9] {  0.8f,       60,-40,-45,         -45, 45, 45,           0,10  },
        //         new float[9] {  0.8f,       -40,-45,-45,        -45, 45, 45,          0,10  },
        //         new float[9] {  0.8f,       60,-40,-45,         -45, 45, 45,           0,10   }
        //     };
        //     float[][] armsInList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       25, -77, -76,       -25, 77, 76,         0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsInRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       45, -45, -45,       -25, 77, 76,         0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsInLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       25, -77, -76,       -45, 45, 45,         0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] spreadoutList = new float[4][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.7f,       45,-41,-89,         -45,41,89,          0,10  },
        //         new float[9] {  0.7f,       45,-41,-89,         -45,41,89,          0,10  },
        //         new float[9] {  0.7f,       45,-41,-89,         -45,41,89,          0,10  },
        //         new float[9] {  0.7f,       45,-41,-89,         -45,41,89,          0,10   }
        //     };
        //     float[][] spreadoutRightList = new float[4][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.7f,       45,-45,-45,         -45,41,89,          0,10  },
        //         new float[9] {  0.7f,       45,-45,-45,         -45,41,89,          0,10  },
        //         new float[9] {  0.7f,       45,-45,-45,         -45,41,89,          0,10  },
        //         new float[9] {  0.7f,       45,-45,-45,         -45,41,89,          0,10   }
        //     };
        //     float[][] spreadoutLeftList = new float[4][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.7f,       45,-41,-89,         -45,45,45,          0,10  },
        //         new float[9] {  0.7f,       45,-41,-89,         -45,45,45,          0,10  },
        //         new float[9] {  0.7f,       45,-41,-89,         -45,45,45,          0,10  },
        //         new float[9] {  0.7f,       45,-41,-89,         -45,45,45,          0,10   }
        //     };
        //     float[][] armsForwardList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.0f,       0, -90, 0,          0, 90, 0,           0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsForwardRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  1.0f,       45, -45, -45,       0, 90, 0,           0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsForwardLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  1.0f,       0, -90, 0,          -45, 45, 45,           0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsFrontList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       -40, -45, -45,      40, 45, 45,        0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsFrontRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  0.8f,       45, -45, -45,       40, 45, 45,        0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsFrontLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.8f,       -40, -45, -45,      -45, 45, 45,        0, 10},
        //         new float[9] {  1f,       45, -45, -45,         -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsSwingList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.0f,       0, -98, -31,        0, 52, 16,          0, 10},
        //         new float[9] {  1.0f,       0, -52, -16,        0, 98, 31,          0, 10 },
        //         new float[9] {  1.0f,       0, -98, -31,        0, 52, 16,          0, 10},
        //         new float[9] {  1.0f,       0, -52, -16,        0, 98, 31,          0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsSwingRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.0f,       45, -45, -45,        0, 52, 16,          0, 10},
        //         new float[9] {  1.0f,       45, -45, -45,        0, 98, 31,          0, 10 },
        //         new float[9] {  1.0f,       45, -45, -45,        0, 52, 16,          0, 10},
        //         new float[9] {  1.0f,       45, -45, -45,        0, 98, 31,          0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsSwingLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.0f,       0, -98, -31,        -45, 45, 45,          0, 10},
        //         new float[9] {  1.0f,       0, -52, -16,        -45, 45, 45,          0, 10 },
        //         new float[9] {  1.0f,       0, -98, -31,        -45, 45, 45,          0, 10},
        //         new float[9] {  1.0f,       0, -52, -16,        -45, 45, 45,          0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.0f,       0, -98, -31,        0, 52, 16,          0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsRightRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  1.0f,       45, -45, -45,        0, 52, 16,          0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsRightLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  1.0f,       0, -98, -31,        -45, 45, 45,          0, 10},
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  1.0f,       0, -52, -16,        0, 98, 31,          0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsLeftRightList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  1.0f,       45, -45, -45,        0, 98, 31,          0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] armsLeftLeftList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //new float[9] {  1.0f,       0, -52, -16,        -45, 45, 45,          0, 10 },
        //         new float[9] {  1f,         45, -45, -45,       -45, 45, 45 ,        0, 20 }
        //     };
        //     float[][] clapList = new float[8][] {
        //				// Time,	Left Arm, 			Right Arm,			Head
        //new float[9] {  0.9f,       -20f, -45f, -45f,   20f, 45f, 45f,     0f, 25f    },
        //         new float[9] {  0.3f,       -20f, -85f, -75f,   20f, 85f, 75f,     0f, 25f   },
        //         new float[9] {  0.3f,       -20f, -45f, -45f,   20f, 45f, 45f,     0f, 25f   },
        //         new float[9] {  0.3f,       -20f, -85f, -75f,   20f, 85f, 75f,     0f, 25f   },
        //         new float[9] {  0.3f,       -20f, -45f, -45f,   20f, 45f, 45f,     0f, 25f   },
        //         new float[9] {  0.3f,       -20f, -85f, -75f,   20f, 85f, 75f,     0f, 25f   },
        //         new float[9] {  0.3f,       -20f, -45f, -45f,   20f, 45f, 45f,     0f, 25f   },
        //         new float[9] {  0.9f,       45f, -45f, -45f,    -45f, 45f, 45f,     0f, 25f    }
        //     };
        //     float[][] nodList = {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.2f,       45f, -45f, -45f,    -45f, 45f, 45f,     -10f, -10f    },
        //         new float[9] {  0.2f,       45f, -45f, -45f,    -45f, 45f, 45f,     10f, -10f    },
        //         new float[9] {  0.2f,       45f, -45f, -45f,    -45f, 45f, 45f,     -10f, -10f    },
        //         new float[9] {  0.2f,       45f, -45f, -45f,    -45f, 45f, 45f,     10f, -10f    },
        //         new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     0f, 10f   }
        //     };
        //     float[][] nodRightList = new float[2][] {
        //                         // Time,    Left Arm,           Right Arm,          Head
        //         new float[9] {  0.2f,       45f, -45f, -45f,    -45f, 45f, 45f,     -10f, -10f    },
        //         new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     0f, 10f   }
        //     };
        //     float[][] nodLeftList = new float[2][] {
        //				// Time,	Left Arm, 			Right Arm,			Head
        //         new float[9] {  0.2f,       45f, -45f, -45f,    -45f, 45f, 45f,     10f, -10f    },
        //         new float[9] {  0.4f,       45f, -45f, -45f,    -45f, 45f, 45f,     0f, 10f   }
        //     };

        //     float[][] breathing_active = new float[][] {
        //                         // Time,	Left Arm, 			Right Arm,			Head
        //         //new float[9] {  0.0f,       45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f },
        //         new float[9] {  1.2f,       35f, -35f, -35f,    -35f, 35f, 35f,     0f, 5f },
        //         new float[9] {  1.2f,       45f, -45f, -45f,    -45f, 45f, 45f,     0f, 0f },
        //     };

        //     float[][] breathing_inactive = new float[][] {
        //                      // Time,	Left Arm, 			    Right Arm,			Head
        //         //new float[9] {  0.0f,       45f, -45f, -45f, 45f, 45f, 45f, 0f, 10f },
        //         //new float[9] {  1.2f,       40f, -40f, -40f,    -40f, 40f, 40f,    0f, -2f },
        //         //new float[9] {  1.2f,       45f, -45f, -45f,    -45f, 45f, 45f,    0f, 4f },
        //         new float[9] {  1.2f,       40f, -40f, -40f,    -40f, 40f, 40f,    0f, 0f },
        //         new float[9] {  1.2f,       45f, -45f, -45f,    -45f, 45f, 45f,    0f, 0f },
        //     };

        //     float[][] hurrayList = {
        //         new float[9] { 0.6f,     -72.9f, -73.3f, -16.7f,    64.4f, 61.5f, 27.2f,   -0.2f, 21.3f },
        //         new float[9] { 0.6f,     49.9f, -62.9f, -15.9f,     -52.7f, 58.5f, 15.8f,   -0.2f, -0.9f },
        //         new float[9] { 0.6f,     -72.9f, -73.3f, -16.7f,     64.4f, 61.5f, 27.2f,   -0.2f, 21.3f },
        //         new float[9] { 0.6f,     49.9f, -62.9f, -15.9f,     -52.7f, 58.5f, 15.8f,   -0.2f, -0.9f },
        //         new float[9] { 0.6f,     -72.9f, -73.3f, -16.7f,    64.4f, 61.5f, 27.2f,    -0.2f, 21.3f },
        //         new float[9] { 0.6f,     49.9f, -62.9f, -15.9f,     -52.7f, 58.5f, 15.8f,   -0.2f, -0.9f },
        //     };

        //     float[][] hi_v2List = {
        //         new float[9] { 0.6f,    -67.4f, -48.5f, -19.2f,     -51.6f, 51.6f, 34.7f,   -0.9f, 12.9f },
        //         new float[9] { 0.6f,    -55.9f, -77f, -55.3f,       -55.1f, 55.2f, 32.5f,   1.3f, 0f },
        //         new float[9] { 0.6f,    -67.4f, -48.5f, -19.2f,     -51.6f, 51.6f, 34.7f,   -0.9f, 12.9f },
        //         new float[9] { 0.6f,    -55.9f, -77f, -55.3f,       -55.1f, 55.2f, 32.5f,   1.3f, 0f },
        //         new float[9] { 0.6f,    -67.4f, -48.5f, -19.2f,     -51.6f, 51.6f, 34.7f,   -0.9f, 12.9f },
        //     };

        //     float[][] handShakingList = {
        //         new float[9] { 0.6f,    58.2f, -57.3f, -3.4f,    14.2f, 73.4f, 20.7f,    -0.8f, 12.1f },
        //         new float[9] { 0.6f,    58.2f, -57.3f, -3.4f,    -15.6f, 75f, 24.7f,     -2.4f, 3.4f },
        //         new float[9] { 0.6f,    58.2f, -57.3f, -3.4f,    14.2f, 73.4f, 20.7f,    -0.8f, 12.1f },
        //         new float[9] { 0.6f,    58.2f, -57.3f, -3.4f,    -15.6f, 75f, 24.7f,     -2.4f, 3.4f },
        //         new float[9] { 0.6f,    58.2f, -57.3f, -3.4f,    14.2f, 73.4f, 20.7f,    -0.8f, 12.1f },
        //         new float[9] { 0.6f,    58.2f, -57.3f, -3.4f,    -15.6f, 75f, 24.7f,     -2.4f, 3.4f },
        //     };

        //     float[][] excitingList = {
        //         new float[9] { 0.6f,    -47.6f, -38.1f, -90f,   53.1f, 7.6f, 12.5f,     -11.3f, 6.3f },
        //         new float[9] { 0.6f,    -29.8f, -38.1f, 0f,     64.7f, 36.7f, 88.1f,    12.8f, 4f },
        //         new float[9] { 0.6f,    -47.6f, -38.1f, -90f,   53.1f, 7.6f, 12.5f,     -11.3f, 6.3f },
        //         new float[9] { 0.6f,    -29.8f, -38.1f, 0f,     64.7f, 36.7f, 88.1f,    12.8f, 4f },
        //         new float[9] { 0.6f,    -47.6f, -38.1f, -90f,   53.1f, 7.6f, 12.5f,     -11.3f, 6.3f },
        //         new float[9] { 0.6f,    -29.8f, -38.1f, 0f,     64.7f, 36.7f, 88.1f,    12.8f, 4f },
        //         new float[9] { 0.6f,    -47.6f, -38.1f, -90f,   53.1f, 7.6f, 12.5f,     -11.3f, 6.3f },
        //         new float[9] { 0.6f,    -29.8f, -38.1f, 0f,     64.7f, 36.7f, 88.1f,    12.8f, 4f },
        //     };

        //     float[][] banggaBanggaList = {
        //         new float[9] { 0.6f,    -51.4f, -55.7f, -3.4f,  49.7f, 49.7f, 23.4f,    -3.1f, 4.9f },
        //         new float[9] { 0.6f,    -45.3f, -69f, -54.7f,   49.4f, 67f, 59.3f,      -1f, 1.8f },
        //         new float[9] { 0.6f,    -51.4f, -55.7f, -3.4f,  49.7f, 49.7f, 23.4f,    -3.1f, 4.9f },
        //         new float[9] { 0.6f,    -45.3f, -69f, -54.7f,   49.4f, 67f, 59.3f,      -1f, 1.8f },
        //         new float[9] { 0.6f,    -51.4f, -55.7f, -3.4f,  49.7f, 49.7f, 23.4f,    -3.1f, 4.9f },
        //         new float[9] { 0.6f,    -45.3f, -69f, -54.7f,   49.4f, 67f, 59.3f,      -1f, 1.8f },
        // };

        //     float[][] sleepy_v2List = {
        //         new float[9] { 1.0f,    53f, -59.6f, -18.6f,    -49.5f, 54.5f, 20.2f,   17.7f, -15.0f },
        //         new float[9] { 0.5f,    42.7f, -63.8f, -15.3f,  -49.6f, 64.7f, 17f,     18.4f, 11f },
        //         new float[9] { 1.0f,    53f, -59.6f, -18.6f,    -49.5f, 54.5f, 20.2f,   17.7f, -15.0f },
        //         new float[9] { 0.5f,    42.7f, -63.8f, -15.3f,  -49.6f, 64.7f, 17f,     18.4f, 11f },
        //     };

        //     float[][] sad_v2List = {
        //         new float[9] { 0.5f,    -52.4f, -90f, -65f,     50.5f, 84.1f, 68.9f,    0.5f, -12.9f },
        //         new float[9] { 0.5f,    -52.5f, -90f, -48.2f,   54.6f, 90f, 49.8f,      -0.5f, 18.3f },
        //         new float[9] { 0.5f,    -52.4f, -90f, -65f,     50.5f, 84.1f, 68.9f,    0.5f, -12.9f },
        //         new float[9] { 0.5f,    -52.5f, -90f, -48.2f,   54.6f, 90f, 49.8f,      -0.5f, 18.3f },
        //         new float[9] { 0.5f,    -52.4f, -90f, -65f,     50.5f, 84.1f, 68.9f,    0.5f, -12.9f },
        //         new float[9] { 0.5f,    -52.5f, -90f, -48.2f,   54.6f, 90f, 49.8f,      -0.5f, 18.3f },
        //         new float[9] { 0.5f,    -52.4f, -90f, -65f,     50.5f, 84.1f, 68.9f,    0.5f, -12.9f },
        //         new float[9] { 0.5f,    -52.5f, -90f, -48.2f,   54.6f, 90f, 49.8f,      -0.5f, 18.3f },
        //         new float[9] { 0.5f,    -52.4f, -90f, -65f,     50.5f, 84.1f, 68.9f,    0.5f, -12.9f },
        //     };

        //     float[][] rightPointList = {
        //         new float[9] { 0.6f,    51.9f, -52f, -29.7f,    54.4f, 53.4f, 20.9f,    -25.4f, 8f },
        //         new float[9] { 0.5f,    51.9f, -52f, -29.7f,    65.7f, 40.7f, 26.8f,    -25.3f, 11.9f },
        //         new float[9] { 0.6f,    51.9f, -52f, -29.7f,    54.4f, 53.4f, 20.9f,    -25.4f, 8f },
        //         new float[9] { 0.5f,    51.9f, -52f, -29.7f,    65.7f, 40.7f, 26.8f,    -25.3f, 11.9f },
        //         new float[9] { 0.6f,    51.9f, -52f, -29.7f,    54.4f, 53.4f, 20.9f,    -25.4f, 8f },
        //     };

        //     float[][] politeGreetingList = {
        //         new float[9] { 0.7f,    43.5f, -42.4f, -90f,    -35.3f, 46.4f, 90f,     -1.9f, 4.7f },
        //         new float[9] { 0.9f,    43.5f, -50.9f, -90f,    -33.1f, 59.6f, 90f,     -1.7f, -15.0f },
        //         new float[9] { 0.7f,    43.5f, -42.4f, -90f,    -35.3f, 46.4f, 90f,     -1.9f, 4.7f },
        //     };

        //     float[][] ok_v2List = {
        //         new float[9] { 0.6f,    51.6f, -55.5f, -20.8f,  -58f, 58.3f, 14.7f,     -3.5f, 24.5f },
        //         new float[9] { 0.6f,    51.6f, -55.5f, -20.8f,  -58f, 58.3f, 14.7f,     -1.5f, -15.0f },
        //         new float[9] { 0.6f,    51.6f, -55.5f, -20.8f,  -58f, 58.3f, 14.7f,     -3.5f, 24.5f },
        //         new float[9] { 0.6f,    51.6f, -55.5f, -20.8f,  -58f, 58.3f, 14.7f,     -1.5f, -15.0f },
        //     };

        //     float[][] no_v2List = {
        //         new float[9] { 0.6f,    42.7f, -64.3f, -13.6f,  -34.7f, 58.4f, 23f,     25.9f, 11.2f },
        //         new float[9] { 0.6f,    42.7f, -64.3f, -13.6f,  -34.7f, 58.4f, 23f,     -30f, 12.4f },
        //         new float[9] { 0.6f,    42.7f, -64.3f, -13.6f,  -34.7f, 58.4f, 23f,     25.9f, 11.2f },
        //         new float[9] { 0.6f,    42.7f, -64.3f, -13.6f,  -34.7f, 58.4f, 23f,     -30f, 12.4f },
        //     };

        //     float[][] leftPointList = {
        //         new float[9] { 0.6f,    -63.2f, -63.2f, -6.4f,  -45.9f, 56f, 20.9f,     20.4f, -2.1f },
        //         new float[9] { 0.5f,    -56.4f, -65f, -14.6f,   -45.9f, 56f, 20.9f,     20.8f, -1.6f },
        //         new float[9] { 0.6f,    -63.2f, -63.2f, -6.4f,  -45.9f, 56f, 20.9f,     20.4f, -2.1f },
        //         new float[9] { 0.5f,    -56.4f, -65f, -14.6f,   -45.9f, 56f, 20.9f,     20.8f, -1.6f },
        //         new float[9] { 0.6f,    -63.2f, -63.2f, -6.4f,  -45.9f, 56f, 20.9f,     20.4f, -2.1f },
        //     };

        //     float[][] surprisedList = {
        //         new float[9] { 0.5f,    -68f, -68.3f, -29.4f,   61.1f, 63.5f, 28.6f,    0.2f, 21.2f },
        //     };


        //        Dictionary<string, float[][]> motionTable;
        #endregion

        IEnumerator currentAnimation = null;
        private bool isPlaying = false;
        private bool isBreathActive = true;

        private readonly float playMotionDelayTime = 1f;

        Queue<MotionAnimInfo> animationQueue = new Queue<MotionAnimInfo>();

        private MotionFileManager refMotionFileManager;

        private IEnumerator Start()
        {
            //InitMotionTable();

            if (refMotionFileManager == null)
            {
                refMotionFileManager = FindObjectOfType<MotionFileManager>();
            }

            yield return StartCoroutine(SetBasePos());

            if (isBreathActive == true)
            {
                PlayMotion("breathing_active");
            }
        }

        public void SetBreathActiveState(string message)
        {
            isBreathActive = Convert.ToBoolean(message);
        }

        public MotionData GetMotionDataFromName(string name)
        {
            if (refMotionFileManager == null)
            {
                refMotionFileManager = FindObjectOfType<MotionFileManager>();
            }

            //Utils.LogRed($"[GetMotionDataFromName] motion: {name}");

            if (refMotionFileManager.GetMotionData(name, out MotionData motionData) == true)
            {
                return motionData;
            }
            return null;
        }

        public void PlayMotion(string motion)
        {
            if (refMotionFileManager == null)
            {
                refMotionFileManager = FindObjectOfType<MotionFileManager>();
            }

            //Utils.LogRed($"[PlayMotion] motion: {motion}");

            if (refMotionFileManager.GetMotionData(motion, out MotionData motionData) == true)
            {
                if (!isBreathActive && !motion.Contains("breathing"))
                {
                    StartCoroutine("DelayPlayMotion", motion);
                    return;
                }
                else
                {
                    if (isPlaying)
                    {
                        StopCoroutine("PlayMotionCoroutine");
                        StopCoroutine("DelayPlayMotion");
                        isPlaying = false;
                    }

                    //Debug.Log("Play Motion: " + motion);
                    StartCoroutine("PlayMotionCoroutine", motionData);
                    return;
                }
            }
            else
            {
                Debug.LogWarning("Motion " + motion + " file not exist!");
            }

            //string uri = Application.dataPath + MOTION_DIR + motion + ".json";
            //if (File.Exists(uri))
            //{
            //    string text = System.IO.File.ReadAllText(uri);
            //    //Debug.Log(text);
            //    MotionData motionData = JsonUtility.FromJson<MotionData>(text);
            //    //Debug.Log(motionData.Length);
            //    //// 흘끗보기 동작은 우선적으로 재생.
            //    //if (motion.Contains("nod"))
            //    //{
            //    //    if (isPlaying)
            //    //    {
            //    //        StopAllCoroutines();
            //    //        isPlaying = false;
            //    //    }

            //    //    StartCoroutine(PlayMotionCoroutine(motion));
            //    //    return;
            //    //}

            //    if (!isBreathActive && !motion.Contains("breathing"))
            //    {
            //        StartCoroutine("DelayPlayMotion", motion);
            //        return;
            //    }
            //    else
            //    {
            //        if (isPlaying)
            //        {
            //            StopCoroutine("PlayMotionCoroutine");
            //            StopCoroutine("DelayPlayMotion");
            //            isPlaying = false;
            //        }

            //        //Debug.Log("Play Motion: " + motion);
            //        StartCoroutine(PlayMotionCoroutine(motionData));
            //        return;

            //    }

            //    //animationQueue.Enqueue(new MotionAnimInfo(motion, PlayMotionCoroutine(motion)));
            //    //if (!isPlaying && animationQueue.Count > 0)
            //    //{
            //    //    StartCoroutine(animationQueue.Dequeue().motionCoroutine);
            //    //}
            //}
            //else
            //{
            //    Debug.LogWarning("Motion " + motion + " file not exist!");
            //}
        }

        IEnumerator DelayPlayMotion(string motion)
        {
            yield return new WaitForSeconds(playMotionDelayTime);
            PlayMotion(motion);
        }

        //IEnumerator TestAllMotion()
        //{
        //    foreach (KeyValuePair<string, float[][]> motion in motionTable)
        //    {
        //        Debug.Log(motion.Key);
        //        yield return PlayMotionCoroutine(motion.Key);
        //    }
        //}

        public void SetZeroPos()
        {
            for (int i = 0; i < 6; i++)
            {
                SetAngle(i, zeroAngle[i]);
            }
        }

        public IEnumerator SetBasePos()
        {
            float basePoseTime = 1f;

            for (int ix = 0; ix < jointInfo.Length; ++ix)
            {
                //jointInfo[ix].SetBaseAngle();
                //SetAngle(ix, baseAngle[ix]);
                SetAngleLerp(ix, baseAngle[ix], basePoseTime);
            }

            yield return new WaitForSeconds(basePoseTime);
        }

        public IEnumerator PlayMotionCoroutine(MotionData motionData)
        {
            isPlaying = true;

            for (int ix = 0; ix < motionData.Length; ++ix)
            {
                // 첫 번째를 제외한 나머지 각도 설정인 경우, 각도 데이터에 있는 시간 값 사용.
                float rotDuration = motionData[ix].GetTime();

                // 첫 번째 각도 설정인 경우, 최대 회전 각도를 기반으로 회전 시간 계산.
                if (ix == 0)
                {
                    float maxDegree = GetDurationToFirstAngle(motionData[0]);
                    rotDuration = maxDegree / (rotSpeed * rotSpeedPercentage);
                    //Debug.Log("Max Degree: " + maxDegree + " , Rot Duration: " + rotDuration);
                }

                for (int jx = 0; jx < jointInfo.Length; ++jx)
                {
                    float jointAngle = DIRECTION[jx] * motionData[ix][jx];
                    //Debug.Log("Joint[" + ix + ":" + jx + "]" + jointAngle);
                    StartCoroutine(jointInfo[jx].SetAngleLerp(jointAngle, rotDuration));
                }

                yield return new WaitForSeconds(rotDuration);
            }

            yield return StartCoroutine(SetBasePos());

            isPlaying = false;

            //Debug.Log("Motion Finished, queue count: " + animationQueue.Count);
            if (animationQueue.Count > 0)
            {
                MotionAnimInfo info = animationQueue.Dequeue();
                //Debug.Log("Play Next motion: " + info.motion);
                StartCoroutine(info.motionCoroutine);
            }
            else if (animationQueue.Count == 0)
            {
                if (breath)
                {
                    //bool isActiveMode = WebSurvey.Instance.GetBehaviorMode == WebSurvey.Mode.Active;
                    string brethingName = isBreathActive ? "breathing_active" : "breathing_inactive";
                    PlayMotion(brethingName);
                }
            }

            yield return null;
        }

        //void InitMotionTable()
        //{
        //    motionTable = new Dictionary<string, float[][]>();
        //    motionTable.Add("hi", hiList);
        //    motionTable.Add("hello", helloList);
        //    motionTable.Add("angry", angryList);
        //    motionTable.Add("sad", sadList);
        //    motionTable.Add("ok", okList);
        //    motionTable.Add("clap", clapList);
        //    motionTable.Add("no", noList);
        //    motionTable.Add("wrong", wrongList);
        //    motionTable.Add("happy", happyList);
        //    motionTable.Add("nod", nodList);
        //    motionTable.Add("nodRight", nodRightList);
        //    motionTable.Add("nodLeft", nodLeftList);
        //    motionTable.Add("breathing_active", breathing_active);
        //    motionTable.Add("breathing_inactive", breathing_inactive);
        //    motionTable.Add("refuse", refuseList);
        //    motionTable.Add("smile", smileList);
        //    motionTable.Add("takenaback", takenabackList);
        //    motionTable.Add("tell", tellList);
        //    motionTable.Add("sleep", sleepyList);
        //    motionTable.Add("thinking", thinkingList);
        //    motionTable.Add("excellent", excellentList);
        //    motionTable.Add("stretch", stretchList);
        //    motionTable.Add("hug", hugList);
        //    motionTable.Add("fear", fearList);
        //    motionTable.Add("bow", bowList);
        //    motionTable.Add("attention", attentionList);
        //    motionTable.Add("best", bestList);
        //    motionTable.Add("leftNeck", leftNeckList);
        //    motionTable.Add("rightNeck", rightNeckList);
        //    motionTable.Add("forward", forwardList);
        //    motionTable.Add("coach", coachList);
        //    motionTable.Add("require", requireList);
        //    motionTable.Add("reset", resetList);
        //    motionTable.Add("exchanged", exchangedList);
        //    motionTable.Add("shake", shakeList);

        //    motionTable.Add("hurray", hurrayList);
        //    motionTable.Add("hi_v2", hi_v2List);
        //    motionTable.Add("handshaking", handShakingList);
        //    motionTable.Add("exciting", excitingList);
        //    motionTable.Add("bangga_bangga", banggaBanggaList);
        //    motionTable.Add("sleepy_v2", sleepy_v2List);
        //    motionTable.Add("sad_v2", sad_v2List);
        //    motionTable.Add("right_point", rightPointList);
        //    motionTable.Add("polite_greeting", politeGreetingList);
        //    motionTable.Add("ok_v2", ok_v2List);
        //    motionTable.Add("no_v2", no_v2List);
        //    motionTable.Add("left_point", leftPointList);

        //    motionTable.Add("headTilt", headTiltList);
        //    motionTable.Add("headBack", headBackList);
        //    motionTable.Add("headUp", headUpList);
        //    motionTable.Add("headRight", headRightList);
        //    motionTable.Add("headLeft", headLeftList);
        //    motionTable.Add("headDown", headDownList);
        //    motionTable.Add("head", headList);

        //    motionTable.Add("armsUp", armsUpList);
        //    motionTable.Add("armsUpRight", armsUpRightList);
        //    motionTable.Add("armsUpLeft", armsUpLeftList);
        //    motionTable.Add("armsDown", armsDownList);
        //    motionTable.Add("armsDownRight", armsDownRightList);
        //    motionTable.Add("armsDownLeft", armsDownLeftList);
        //    motionTable.Add("armsUpdown", armsUpdownList);
        //    motionTable.Add("armsUpdownRight", armsUpdownRightList);
        //    motionTable.Add("armsUpdownLeft", armsUpdownLeftList);
        //    motionTable.Add("armsIn", armsInList);
        //    motionTable.Add("armsInRight", armsInRightList);
        //    motionTable.Add("armsInLeft", armsInLeftList);
        //    motionTable.Add("spreadout", spreadoutList);
        //    motionTable.Add("spreadoutRight", spreadoutRightList);
        //    motionTable.Add("spreadoutLeft", spreadoutLeftList);
        //    motionTable.Add("armsForward", armsForwardList);
        //    motionTable.Add("armsForwardRight", armsForwardRightList);
        //    motionTable.Add("armsForwardLeft", armsForwardLeftList);
        //    motionTable.Add("armsFront", armsFrontList);
        //    motionTable.Add("armsFrontRight", armsFrontRightList);
        //    motionTable.Add("armsFrontLeft", armsFrontLeftList);
        //    motionTable.Add("armsSwing", armsSwingList);
        //    motionTable.Add("armsSwingRight", armsSwingRightList);
        //    motionTable.Add("armsSwingLeft", armsSwingLeftList);
        //    motionTable.Add("armsRight", armsRightList);
        //    motionTable.Add("armsRightRight", armsRightRightList);
        //    motionTable.Add("armsRightLeft", armsRightLeftList);
        //    motionTable.Add("armsLeft", armsLeftList);
        //    motionTable.Add("armsLeftRight", armsLeftRightList);
        //    motionTable.Add("armsLeftLeft", armsLeftLeftList);
        //}

        float GetDurationToFirstAngle(DoubleArray doubleArray)
        {
            float maxDegree = 0f;
            for (int ix = 0; ix < jointInfo.Length; ++ix)
            {
                float degree = jointInfo[ix].GetTargetAngleAxis(doubleArray[ix]);
                maxDegree = Mathf.Max(maxDegree, degree);
            }

            return maxDegree;
        }

        //IEnumerator GestureProcess(float[][] motionInfo)
        //{
        //    isPlaying = true;

        //    for (int ix = 0; ix < motionInfo.Length; ++ix)
        //    {
        //        // 첫 번째를 제외한 나머지 각도 설정인 경우, 각도 데이터에 있는 시간 값 사용.
        //        float rotDuration = motionInfo[ix][0];
        //        //Debug.Log(motionInfo[ix][8]);

        //        // 첫 번째 각도 설정인 경우, 최대 회전 각도를 기반으로 회전 시간 계산.
        //        if (ix == 0)
        //        {
        //            float maxDegree = GetDurationToFirstAngle(motionInfo[0]);
        //            rotDuration = maxDegree / (rotSpeed * rotSpeedPercentage);
        //            //Debug.Log("Max Degree: " + maxDegree + " , Rot Duration: " + rotDuration);
        //        }

        //        for (int jx = 0; jx < jointInfo.Length; ++jx)
        //        {
        //            // For Debug.
        //            //if (jx == 0)
        //            //{
        //            //    StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], motionInfo[ix][0], true));
        //            //    continue;
        //            //}

        //            if (jx == 7)
        //            {
        //                StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], rotDuration));
        //                continue;
        //            }

        //            StartCoroutine(jointInfo[jx].SetAngleLerp(motionInfo[ix][jx + 1], rotDuration));
        //        }

        //        //float waitTime = motionInfo[ix][0];
        //        yield return new WaitForSeconds(rotDuration);
        //    }

        //    yield return StartCoroutine(SetBasePos());

        //    isPlaying = false;

        //    //Debug.Log("Motion Finished, queue count: " + animationQueue.Count);
        //    if (animationQueue.Count > 0)
        //    {
        //        MotionAnimInfo info = animationQueue.Dequeue();
        //        //Debug.Log("Play Next motion: " + info.motion);
        //        StartCoroutine(info.motionCoroutine);
        //    }
        //    else if (animationQueue.Count == 0)
        //    {
        //        if (breath)
        //        {
        //            //bool isActiveMode = WebSurvey.Instance.GetBehaviorMode == WebSurvey.Mode.Active;
        //            string brethingName = isBreathActive ? "breathing_active" : "breathing_inactive";
        //            PlayMotion(brethingName);
        //        }
        //    }

        //    //breatheEnable = true;
        //}

        IEnumerator WaitUntilNextCoroutine(float time)
        {
            float elapsedTime = 0f;
            while (elapsedTime <= time)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator TestExecutor()
        {
            for (int ix = 0; ix < 5; ++ix)
            {
                yield return StartCoroutine(Test());
            }
        }

        float waitTime = 0.1f;
        IEnumerator Test()
        {
            for (int ix = 1; ix < 11; ++ix)
            {
                //SetAngle(6, -ix * 10f);
                SetAngle(7, ix * 10f);

                yield return new WaitForSeconds(waitTime);
            }

            yield return new WaitForSeconds(.5f);
            SetBasePos();
        }

        public void SetAngle(int jointId, float angle)
        {
            jointInfo[jointId].SetAngle(angle);
        }

        public void SetAngleLerp(int jointId, float angle, float duration)
        {
            StartCoroutine(jointInfo[jointId].SetAngleLerp(angle, duration));
        }

        public float GetAngle(int JointID)
        {
            return jointInfo[JointID].GetAngle();
        }

        class MotionAnimInfo
        {
            public string motion;
            public IEnumerator motionCoroutine;

            public MotionAnimInfo() { }
            public MotionAnimInfo(string motion, IEnumerator coroutine)
            {
                this.motion = motion;
                motionCoroutine = coroutine;
            }
        }
    }
}