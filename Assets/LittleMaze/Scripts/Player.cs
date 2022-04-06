using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Range(1.0f, 20.0f)]
    public float mMoveVelocity = 5.0f;
    [Range(2f, 10f)]
    public float mRotateVelocity = 5f;


    public static Player _mInstance = null;
    public static Player Instance() { return _mInstance; }

    private Animator mAnimator;
    private Character mCharacter;
    public Character Character { get { return mCharacter; } }

    private int mBlockCode = 0;
    public int BlockCode { get { return mBlockCode; } }

    public GameObject BindPlayer(GameObject prefab)
    {
        var go = GameObject.Instantiate(prefab);

        mCharacter = go.GetComponentInChildren<Character>();
        mCharacter.SetVelocity(mMoveVelocity, mRotateVelocity);
        mCharacter.TriggerExit = () =>{LittleMaze.Instance().GameFinish();};

        mAnimator = gameObject.GetComponentInChildren<Animator>();

        return go;
    }

    public void UnbindCharacter()
    {
        mCharacter = null;
        mAnimator = null;
    }

    private void Awake()
    {
        if (_mInstance != null)
        {
            Debug.LogError("Duplicate Player has been Created" + gameObject.name);
        }
        _mInstance = this;
    }

    private void Update()
    {
        if (mBlockCode == 0 || isBlock(InputBlockCode.BLOCK_CHANGE_CONTROLLER_ANIM))
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector2 input = new Vector2(horizontal, vertical);

            TickInput(input);
        }
        else
            TickInput(Vector2.zero);

        if (CameraController.Instance())
        {
            CameraController.Instance().Tick(Time.deltaTime);
        }

    }

    // “∆∂Ø ‰»Î
    private void TickInput(Vector2 input)
    {
        if (mCharacter && !isBlock(InputBlockCode.BLOCK_AUTO_PROXY))
        {
            mCharacter.UpdateMove(input.sqrMagnitude);

            var camera = Camera.main;
            if (camera)
            {
                var forward = camera.transform.TransformDirection(Vector3.forward);
                forward.y = 0;
                forward.Normalize();

                if (forward.sqrMagnitude <= 0.0f)
                {
                    forward = Vector3.forward;
                }

                var right = new Vector3(forward.z, 0, -forward.x);

                var disertDirection = input.x * right + input.y * forward;
                if (disertDirection.magnitude > 1)
                {
                    disertDirection.Normalize();
                }

                mCharacter.UpdateRotation(disertDirection);
            }
        }
    }

    public void BlockInput(int blockCode)
    {
        mBlockCode |= blockCode;
    }

    public void UnBlockInput(int blockCode)
    {
        mBlockCode -= mBlockCode & blockCode;
    }

    public bool isBlock(int blockCode)
    {
        return blockCode == (mBlockCode & blockCode);
    }

    private void OnDestroy()
    {
        _mInstance = null;
        mCharacter = null;
        mAnimator = null;
        print("Destroy");
    }


    public void SetCommands(ref PLeaf exit, int width, int height, float offset)
    {
        mCharacter.SetCommands(ref exit, width, height, offset);
    }
}
