#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using UnityEngine;

namespace REEL.D2EEditor
{
    public class GetSetFacialBlock : MonoBehaviour
    {
#if USINGTMPPRO
        public TMP_Text parameterName;
        public TMP_Text text;
#else
        public Text parameterName;
        public Text text;
#endif
    }
}