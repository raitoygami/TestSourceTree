using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CameraController : MonoBehaviour
{
    private static CameraController _mInstance;
    public static CameraController Instance() { return _mInstance; }

    public Dictionary<int, CameraTarget> mCameraTargets;

    private CameraTarget mCameraTarget = null;

    // Start is called before the first frame update
    void Awake()
    {
        _mInstance = this;

        mCameraTargets = new Dictionary<int, CameraTarget>();
    }

    // Update is called once per frame
    public void Tick(float deltaTime)
    {
        if (mCameraTarget)
        {
            mCameraTarget.Tick(deltaTime);
        }
    }

    public void FocusGlobal(int width, int height, ActiveCallback callback = null)
    {
        var size = Mathf.Max(width, height * 3f);
        var global = Get(CameraIndex.INDEX_GLOBAL);

        if (Player.Instance())
            Player.Instance().BlockInput(InputBlockCode.BLOCK_CHANGE_CONTROLLER_ANIM);
        
        var t = global.transform.DOMove(new Vector3(0.0f, size, 0.0f), 1.0f).SetEase(Ease.Flash);

        t.OnComplete(() =>
        {
            if (callback != null)
                callback();

            if (Player.Instance())
                Player.Instance().UnBlockInput(InputBlockCode.BLOCK_CHANGE_CONTROLLER_ANIM);
        });
        mCameraTarget = global;

    }

    //public void FocusGlobal(int width, int height)
    //{
    //    var size = Mathf.Max(width, height * 2);
    //    var global = Get(CameraIndex.INDEX_GLOBAL);

    //    Player.Instance().BlockInput(InputBlockCode.BLOCK_CHANGE_CONTROLLER_ANIM);
    //    var t = global.transform.DOMove(new Vector3(0.0f, size, 0.0f), 1.0f).SetEase(Ease.Flash);

    //    t.OnComplete(() =>
    //    {
    //        Player.Instance().UnBlockInput(InputBlockCode.BLOCK_CHANGE_CONTROLLER_ANIM);
    //    });
    //    mCameraTarget = global;
    //}

    public CameraTarget Get(int key)
    {
        CameraTarget ret;
        if (mCameraTargets.TryGetValue(key, out ret))
        {
            return ret;
        }
        return null;
    }

    public void Regist(int key, CameraTarget target)
    {
        UnRegist(key);

        mCameraTargets.Add(key, target);
    }

    public void UnRegist(int key)
    {
        if (mCameraTargets.TryGetValue(key, out CameraTarget temp))
        {
            mCameraTargets.Remove(key);
        }
    }

    public delegate void ActiveCallback();
    public void Active(int key, float duration = 0.0f, Ease ease = Ease.Unset, ActiveCallback callback = null)
    {
        if (Player.Instance() == null)
            return;

        if (Player.Instance().isBlock(InputBlockCode.BLOCK_CHANGE_CONTROLLER_ANIM))
            return;

        if (mCameraTarget != null && mCameraTarget.Index == key)
            return;
        
        if (mCameraTargets.TryGetValue(key, out mCameraTarget))
        {
            var camera = Camera.main.transform;
            mCameraTarget.Active();

            var position = mCameraTarget.transform.position;
            var rotation = mCameraTarget.transform.rotation;
            mCameraTarget.Position = camera.position;
            mCameraTarget.Rotation = camera.rotation;


            camera.SetParent(mCameraTarget.transform);
            camera.transform.localRotation = Quaternion.Euler(0, 0, 0);

            Player.Instance().BlockInput(InputBlockCode.BLOCK_CHANGE_CONTROLLER_ANIM);
            Sequence s = DOTween.Sequence();
            
            //for (int i = 0; i < posArray.Length; ++i)
            //{
            //    Vector3 toPos = posArray[i];
            //    Vector3 toRot = rotArray[i];
            //    s.Append(myTransform.DOMove(toPos, duration));
            //    s.Join(myTransform.DORotate(toRot, duration));
            //}
            var t = mCameraTarget.transform.DOMove(position, duration).SetEase(ease);
            s.Append(t);
            s.Join(mCameraTarget.transform.DORotate(rotation.eulerAngles, duration).SetEase(ease));

            s.OnComplete(() =>
            {
                if (callback!= null)
                    callback();

                Player.Instance().UnBlockInput(InputBlockCode.BLOCK_CHANGE_CONTROLLER_ANIM);
            });
        }
    }

    public bool IsActive(int key)
    {
        return mCameraTarget && mCameraTarget.Index == key;
    }
    private void OnDestroy()
    {
        _mInstance = null;
    }

}
