using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettle : MonoBehaviour
{
    public delegate void OnClickCallback();
    private OnClickCallback mCallback;
    public OnClickCallback Callback { set { mCallback = value; } }
    // Start is called before the first frame update
    public void OnButtonClick_Finish()
    {
        if (mCallback != null)
            mCallback();
    }
}
