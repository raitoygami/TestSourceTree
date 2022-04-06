using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera3rdPerson : CameraTarget
{
    // 摄像机偏移
    private Vector3 mOffsetInit = Vector3.zero;
    private Vector3 mOffsetCurrent = Vector3.zero;
    private float mCameraRadius = 0.0f;
    private float mMouseX = 0.0f;
    private bool bMouseButtonDown = false;

    private void Start()
    {
        mIndex = 2;

        if (CameraController.Instance())
        {
            CameraController.Instance().Regist(mIndex, this);
        }

        if (mLookAtTarget)
        {
            // 初始位置在角色后方
            var lookForward = mLookAtTarget.forward * -1;

            mOffsetInit = Position - mLookAtTarget.position;

            lookForward.y = 0;
            mOffsetInit.y = 0;
            var rad = Vector3.Dot(lookForward.normalized, mOffsetInit.normalized);

            var angle = Mathf.Acos(rad) * Mathf.Rad2Deg;
            transform.RotateAround(mLookAtTarget.position, Vector3.up, angle);

            mOffsetInit = Position - mLookAtTarget.position;
            mOffsetCurrent = mOffsetInit;

            mCameraRadius = mOffsetCurrent.magnitude;
        }

    }

    public override void Active()
    {
        //mOffsetCurrent = mOffsetInit;
        transform.position = mLookAtTarget.position + mOffsetCurrent;
    }

    public override void Tick(float deltaTime)
    {

        UpdateInput(deltaTime);

        UpdateLocomotion(deltaTime);
    }

    // 水平移动 Camera
    private void UpdateInput(float deltaTime)
    {

        if (Input.GetMouseButtonDown(0))
        {
            mMouseX = Input.mousePosition.x;
            bMouseButtonDown = true;
        }

        if (bMouseButtonDown)
        {
            float offset = Input.mousePosition.x - mMouseX;
            mMouseX = Input.mousePosition.x;

            transform.RotateAround(mLookAtTarget.position, Vector3.up, offset * 0.25f);

            var angle = Quaternion.Euler(0, offset * 0.25f, 0);
            mOffsetCurrent = angle * mOffsetCurrent;

        }

        if (Input.GetMouseButtonUp(0))
        {
            bMouseButtonDown = false;
        }
        
        var zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0)
        {
            mCameraRadius = mCameraRadius - zoom * 10.0f;
            mCameraRadius = Mathf.Clamp(mCameraRadius, 5.0f, 1000.0f);
            mOffsetCurrent = mOffsetCurrent.normalized * mCameraRadius;
        }
        
    }

    // 跟随
    private void UpdateLocomotion(float deltaTime)
    {
        if (mLookAtTarget)
        {
            Vector3 basePosition = mLookAtTarget.position + mOffsetCurrent;
            Vector3 abovePosition = mLookAtTarget.position + Vector3.up * (mCameraRadius + 10.0f);

            Vector3[] checkPoint = new Vector3[5];
            checkPoint[0] = basePosition;

            checkPoint[1] = Vector3.Lerp(basePosition, abovePosition, 0.25f);
            checkPoint[2] = Vector3.Lerp(basePosition, abovePosition, 0.5f);
            checkPoint[3] = Vector3.Lerp(basePosition, abovePosition, 0.75f);

            checkPoint[4] = abovePosition;

            Vector3 result = checkPoint[4];

            for (int i = 0; i < 5; i++)
            {
                if (Raycast(checkPoint[i], mLookAtTarget.position))
                {
                    result = checkPoint[i];
                    break;
                }
            }

            Position = Vector3.Lerp(Position, result, mSmoothVelocity * deltaTime);

            var rotation = Quaternion.LookRotation(mLookAtTarget.position - Position);
            Rotation = Quaternion.LerpUnclamped(Rotation, rotation, mSmoothVelocity * deltaTime);
        }
    }

    private void OnDestroy()
    {
        if (CameraController.Instance())
        {
            CameraController.Instance().UnRegist(mIndex);
        }
    }

    private bool Raycast(Vector3 lhs, Vector3 rhs)
    {
        RaycastHit hit;

        if (Physics.Raycast(lhs, rhs - lhs, out hit, 10f))
        {
            //Debug.Log(hit);
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

}
