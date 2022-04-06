using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Character : MonoBehaviour
{
    private Animator mAnimator;
    private CharacterController mController;
    public Animator animator { get { return mAnimator; } }
    private Queue<Vector3> mCommandList;
    private Vector3 mCurrentCmd;
    private Vector3 mLastDirection;
    private float mMoveVelocity = 5.0f;

    private float mRotateVelocity = 10.0f;

    private float mAutoMoveVelocity = 10.0f;

    void Awake()
    {
        mCommandList = new Queue<Vector3>();
        mAnimator = GetComponent<Animator>();
        mController = GetComponent<CharacterController>();
    }

    public void SetVelocity(float move, float rotate)
    {
        mMoveVelocity = move;
        mRotateVelocity = rotate;
    }

    private void Start()
    {
        transform.localPosition = Vector3.zero;
        mController.enabled = true;
        mLastDirection = Vector3.zero;
    }

    private void OnAnimatorMove()
    {
        var velocty = mAnimator.deltaPosition;

        if (mController && mController.enabled)
        {
            mController.Move(velocty);
        }
    }

    public void UpdateMove(float velocity)
    {
        if (mAnimator)
        {
            mAnimator.SetFloat("MoveVelocity", velocity * mMoveVelocity);
        }
    }

    public bool AutoMove()
    {
        if (mCommandList.Count > 0)
        {
            var cmd = mCommandList.Peek();
            if (!isReachCMD(cmd))
            {
                //var lastDirection = mLastDirection;
                UpdateMove(mAutoMoveVelocity);

                mLastDirection = cmd - transform.position;
                mLastDirection.y = 0.0f;
                UpdateRotation(mLastDirection.normalized);
            }

            else
                mCommandList.Dequeue();
            return true;
        }
        return false;
    }

    public void SetCommands(ref PLeaf exit, int width, int height, float offset)
    {
        mCommandList.Clear();
        while (!(exit is null))
        {
            Vector3 cmd = new Vector3((exit.x - width * 0.5f) * offset, 0, (exit.y - height * 0.5f) * offset);
            mCommandList.Enqueue(cmd);

            exit = exit.parent;
        }

        mCommandList = new Queue<Vector3>(mCommandList.Reverse());
        //Debug.Log(mCommandList.Count);
    }

    //public void SmoothCMD(PLeaf parent, PLeaf CMD, PLeaf child)
    //{
    //    if (find && !(result is null))
    //    {
    //        var temp = result;
    //        while (!(temp.parent is null))
    //        {
    //            if (!(temp.parent.parent is null))
    //            {
    //                if (temp.isNeighbor(temp.parent.parent))
    //                {
    //                    temp.parent = temp.parent.parent;
    //                }
    //            }
    //            temp = temp.parent;
    //        }
    //    }

    //}

    public void UpdateRotation(Vector3 disertDirection)
    {
        if (disertDirection.sqrMagnitude > 0.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(disertDirection.normalized), mRotateVelocity * Time.deltaTime);
        }
    }

    private bool isReachCMD(Vector3 cmd)
    {
        var near = Vector3.Distance(transform.position, cmd) <= mAutoMoveVelocity * 0.05f;
        //var pass = Vector3.Dot(mLastDirection, (transform.position - cmd).normalized) > 0.5f;
        var pass = false;
        return near || pass;
    }

    public delegate void TriggerExitDelegate();
    private TriggerExitDelegate mTriggerExit;
    public TriggerExitDelegate TriggerExit { set { mTriggerExit = value; } }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exit"))
        {
            if (mTriggerExit != null)
            {
                mTriggerExit();
            }
            
        }
    }

}
