using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGlobal : CameraTarget
{
    private void Awake()
    {
        mIndex = 1;

        if (CameraController.Instance())
        {
            CameraController.Instance().Regist(mIndex, this);
        }
    }

    public override void Tick(float deltaTime)
    {
        var rotation = Quaternion.LookRotation(mLookAtTarget.position - Position);
        Rotation = Quaternion.Lerp(Rotation, rotation, mSmoothVelocity * deltaTime * 2);
    }

}
