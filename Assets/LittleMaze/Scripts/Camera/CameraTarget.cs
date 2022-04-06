using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    protected int mIndex = 0;
    public int Index { get { return mIndex; } }
    public Transform mLookAtTarget = null;

    public Vector3 Position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public Quaternion Rotation
    {
        get { return transform.rotation; }
        set { transform.rotation = value; }
    }

    protected float mSmoothVelocity = 1f;

    public virtual void Tick(float deltaTime) { }
    public virtual void Active() { }

}
