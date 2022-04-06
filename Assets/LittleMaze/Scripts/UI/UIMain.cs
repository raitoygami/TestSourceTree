using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoBehaviour
{
    public Text mTextAuto;
    private bool bAutoMoving = false;
    public void OnButtonClick_Auto()
    {
        bAutoMoving = !bAutoMoving;
        mTextAuto.text = bAutoMoving ? "�Զ��ƶ���" : "�Զ�";

        if (bAutoMoving)
            LittleMaze.Instance().AutoMovement();
        else
            LittleMaze.Instance().StopAutoMovement();
    }

    public void Reset()
    {
        bAutoMoving = false;
        mTextAuto.text = "�Զ�";
    }

}
