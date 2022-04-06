using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

public class LittleMaze : MonoBehaviour
{
    private Maze mMaze;
    public Maze MazeIst { get { return mMaze; } }
    private static LittleMaze _mInstance;
    public static LittleMaze Instance() { return _mInstance; }

    public int mComputeStep = 100;

    private Transform mWallParent;
    public GameObject mWallPrefab;
    public GameObject mEntryPrefab;
    public GameObject mExitPrefab;

    public GameObject mCharacterPrefab;
    public float mCellOffset = 1f;
    [Range(10, 30)]
    public int mWidth = 15;
    [Range(10, 30)]
    public int mHeight = 15;
    
    private GameObject mCharacter = null;

    private void Awake()
    {
        _mInstance = this;
        mWallParent = GameObject.Find("LayerWalls").transform;

    }

    public void GameStart(int width, int height)
    {
        ReleaseMaze();

        mWidth = width;
        mHeight = height;

        UIMgr.Instance().uiLittleMaze.gameObject.SetActive(false);
        
        GenerateMaze();
        
    }

    public void GameFinish()
    {
        Player.Instance().BlockInput(InputBlockCode.BLOCK_GAME_FINISH);

        if (CameraController.Instance().IsActive(CameraIndex.INDEX_GLOBAL))
            StartCoroutine(Settle());
        else
            CameraController.Instance().Active(CameraIndex.INDEX_GLOBAL, 1.0f, DG.Tweening.Ease.Linear, () =>
            {
                StartCoroutine(Settle());
            });
    }

    public void AutoMovement()
    {
        if (PathFindingProxy.Instance())
        {
            PathFindingProxy.Instance().AutoProxy(mWidth, mHeight, mCellOffset);
        }
    }
    public void StopAutoMovement()
    {
        if (PathFindingProxy.Instance())
        {
            PathFindingProxy.Instance().Stop();
        }
    }

    private IEnumerator Settle()
    {
        yield return new WaitForSeconds(1.0f);

        UIMgr.Instance().uiSettle.gameObject.SetActive(true);
        UIMgr.Instance().uiSettle.Callback = () =>{

            UIMgr.Instance().uiSettle.gameObject.SetActive(false);
            UIMgr.Instance().uiMain.gameObject.SetActive(false);
            UIMgr.Instance().uiLittleMaze.gameObject.SetActive(true);

            Player.Instance().UnBlockInput(InputBlockCode.BLOCK_GAME_FINISH);
        };

        yield return 0;
    }

    private void GenerateMaze()
    {

        Debug.Log("----------   Generate    ----------");
        mMaze = MazeGenerator.Generate(mWidth, mHeight, mComputeStep);

        CameraController.Instance().FocusGlobal(mWidth, mHeight, ()=> {
            StartCoroutine(RenderMaze());
        });
    }

    private void ReleaseMaze()
    {
        if (mCharacter)
        {
            Object.DestroyImmediate(mCharacter.gameObject);
            mCharacter = null;
            Player.Instance().UnbindCharacter();
        }

        if (mMaze != null)
        {
            mMaze.Release();
            mMaze = null;
        }
        //print("++++++");
        System.GC.Collect();
    }

    IEnumerator RenderMaze()
    {

        yield return StartCoroutine(mMaze.RenderEntryAndExit(mEntryPrefab, mExitPrefab, mWallParent, mCellOffset));

        for (int i = 0; i < mMaze.MaxStep; i++)
        {
            yield return StartCoroutine(mMaze.Render(mWallPrefab, mWallParent, mCellOffset, i));
        }

        Debug.Log("----------    Finish     ----------");
        yield return new WaitForSeconds(1);
        if (mCharacterPrefab)
        {
            mCharacter = Player.Instance().BindPlayer(mCharacterPrefab);
            mCharacter.transform.position = mMaze.GetStartLocation(mCellOffset);
            mCharacter.GetComponentInChildren<Character>().transform.rotation = mMaze.GetStartRotation();
        }

        yield return new WaitForSeconds(2);

        if (CameraController.Instance() != null)
            CameraController.Instance().Active(2, 1.0f, DG.Tweening.Ease.InQuad, ()=> {
                UIMgr.Instance().uiMain.gameObject.SetActive(true);
                UIMgr.Instance().uiMain.Reset();
            });

        yield return 0;

    }

    // ≤‚ ‘
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    if (CameraController.Instance() != null)
        //        CameraController.Instance().Active(2, 1.0f, DG.Tweening.Ease.Linear);
        //}

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    if (CameraController.Instance() != null)
        //        CameraController.Instance().Active(1, 1.0f, DG.Tweening.Ease.Linear);
        //}
    }
    private void OnDestroy()
    {
        _mInstance = null;

        if (mMaze != null)
        {
            mMaze.Release();
        }
        mMaze = null;
        
    }
}
