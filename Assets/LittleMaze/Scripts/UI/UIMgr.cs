using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    public static UIMgr _mInstance;
    public Transform mUIRoot;

    public UILittleMaze uiLittleMaze;
    public UISettle uiSettle;
    public UIMain uiMain;
    public static UIMgr Instance() { return _mInstance; }
    private void Awake()
    {
        _mInstance = this;
        uiLittleMaze.transform.SetParent(transform, false);

        uiMain.transform.SetParent(transform, false);
        uiMain.transform.localScale = Vector3.one;
        uiMain.transform.localPosition = Vector3.zero;

        uiSettle.transform.SetParent(transform, false);
        uiSettle.transform.localScale = Vector3.one;
        uiSettle.transform.localPosition = Vector3.zero;
    }

}
