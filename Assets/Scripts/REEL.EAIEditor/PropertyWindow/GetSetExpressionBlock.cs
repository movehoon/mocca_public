#define USINGTMPPRO

#if USINGTMPPRO
using TMPro;
#endif

using UnityEngine;

namespace REEL.D2EEditor
{
    public class GetSetExpressionBlock : MonoBehaviour
    {
#if USINGTMPPRO
        public TMP_Text parameterName;
#else
        public Text parameterName;
#endif
    }
}