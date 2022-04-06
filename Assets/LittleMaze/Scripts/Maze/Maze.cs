using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    private int mWidth;
    private int mHeight;
    private int mMaxStep = 0;
    public int MaxStep { get { return mMaxStep; } }


    private Cell[,] mMazeCells;
    private List<GameObject> mWallObjs;
    private P mEntry;
    private P mExit;
    public P Entry { get { return mEntry; } }
    public P Exit { get { return mExit; } }
    private bool bRowFirst;

    public static int S_RENDER_IM = -1;
    //public static int S_RENDER_STEP_BY_STEP = 0;

    public Maze(int width, int height)
    {
        mWidth = width; mHeight = height;
        mWallObjs = new List<GameObject>();
        mMaxStep = 0;
        Reset();
    }

    public void Reset()
    {
        foreach (var wall in mWallObjs)
        {
            Object.Destroy(wall);
        }
        mWallObjs.Clear();

        mMazeCells = new Cell[mWidth, mHeight];
        bRowFirst = Random.Range(0, Mathf.Max(mWidth, mHeight)) % 2 == 0;

        // 水平
        mEntry = bRowFirst ? new P(Random.Range(1, mWidth - 1), 0) : new P(0, Random.Range(1, mHeight - 1));
        mExit = bRowFirst ? new P(Random.Range(1, mWidth - 1), mHeight - 1) : new P(mWidth - 1, Random.Range(1, mHeight - 1));

        for (int i = 0; i < mWidth; i++)
        {
            for (int j = 0; j < mHeight; j++)
            {
                mMazeCells[i, j] = new Cell { x = i, y = j, bBlocked = false, isPass = false, step = 0 };
                // 边缘墙壁&&出入口
                if (i == 0 || i == mWidth - 1 || j == 0 || j == mHeight - 1)
                {
                    var p = new P(i, j);
                    if (p != mEntry && p != mExit)
                    {
                        mMazeCells[i, j].bBlocked = true;
                    }
                    else
                    {
                        mMazeCells[i, j].isPass = true;
                    }
                }
            }
        }

    }

    private void DisConnect(int x, int y, int step)
    {
        //Debug.Log(x+ "--------" +y);
        var cell = mMazeCells[x, y];
        cell.bBlocked = true;
        cell.step = step;
        mMaxStep = Mathf.Max(mMaxStep, step);
        mMazeCells[x, y] = cell;
    }

    private void PassTag(int x, int y)
    {
        var cell = mMazeCells[x, y];
        cell.isPass = true;
        mMazeCells[x, y] = cell;
    }

    // 水平切
    public void DisConnectRow(P origin, int wallIdx, int width, int height, int passIdx, int step)
    {
        if (origin.y + wallIdx >= mHeight - 1 || origin.x >= mWidth)
            return;
        List<P> targets = GetPassOfZone(origin, width, height);
        
        for (int i = 0; i < width; i++)
        {

            if (i != passIdx && !isNeighbor(origin.x + i, origin.y + wallIdx, targets))
            {
                DisConnect(origin.x + i, origin.y + wallIdx, step);
            }
            else
            {
                PassTag(origin.x + i, origin.y + wallIdx);
            }
        }
    }


    //垂直切
    public void DisConnectCol(P origin, int wallIdx, int width, int height, int passIdx, int step)
    {
        if (origin.x + wallIdx >= mWidth - 1 || origin.y >= mHeight)
            return;
        List<P> targets = GetPassOfZone(origin, width, height);
        for (int i = 0; i < height; i++)
        {
            if (i != passIdx && !isNeighbor(origin.x + wallIdx, origin.y + i, targets))
            {
                DisConnect(origin.x + wallIdx, origin.y + i, step);
            }
            else
            {
                PassTag(origin.x + wallIdx, origin.y + i);
            }
        }
    }
    // origin.x -1, origin.y - 1  ---->  origin.x + width + 1, origin.y + height + 1
    private List<P> GetPassOfZone(P origin, int width, int height)
    {
        List<P> ret = new List<P>();

        for (int i = origin.x - 1; i <= origin.x + width; i++)
        {
            //if (i < 0 || i > mWidth)
            //    continue;
            if (mMazeCells[i, origin.y - 1].isPass == true)
            {
                ret.Add(new P(i, origin.y - 1));
            }
            if (mMazeCells[i, origin.y + height].isPass == true)
            {
                ret.Add(new P(i, origin.y + height));
            }
        }

        for (int i = origin.y - 1; i <= origin.y + height; i++)
        {
            if (mMazeCells[origin.x - 1, i].isPass == true)
            {
                ret.Add(new P(origin.x - 1, i));
            }
            if (mMazeCells[origin.x + width, i].isPass == true)
            {
                ret.Add(new P(origin.x + width, i));
            }
        }
        //Debug.Log("(" + origin.x + "," + origin.y +")" + "--" + width + ":" + height + "Cnt++++++++" + ret.Count);
        //foreach (var item in ret)
        //{
        //    Debug.Log(item.x + ":" + item.y);
        //}
        return ret;
    }

    private bool isNeighbor(int x, int y, List<P> targets)
    {
        foreach (var target in targets)
        {
            if (Mathf.Abs(target.x - x) + Mathf.Abs(target.y - y) == 1)
            {
                return true;
            }
        }
        //return true;
        return false;
    }

    //public void Render(GameObject prefab, Transform parent, float offset, int step)
    //{
    //    for (int i = 0; i < mWidth; i++)
    //    {
    //        for (int j = 0; j < mHeight; j++)
    //        {
    //            var cell = mMazeCells[i, j];
    //            if (cell.bBlocked && (cell.step == step || step == S_RENDER_IM))
    //            {
    //                var wall = GameObject.Instantiate(prefab, parent);
    //                wall.transform.localPosition = new Vector3(i * offset - mWidth * 0.5f * offset, 0, j * offset - mHeight * 0.5f * offset);
    //                //wall.transform.localScale = new Vector3(wall.transform.localScale.x, 1 + cell.step, wall.transform.localScale.z);
    //                mWallObjs.Add(wall);
    //            }
    //        }
    //    }
    //}

    //public IEnumerator RenderStart
    public IEnumerator RenderEntryAndExit(GameObject entryPrefab, GameObject exitPrefab, Transform parent, float offset)
    {
        //yield return new WaitForSeconds(2);
        var entry = GameObject.Instantiate(entryPrefab, parent);
        entry.transform.localPosition = new Vector3(mEntry.x * offset - mWidth * 0.5f * offset, 0, mEntry.y * offset - mHeight * 0.5f * offset);
        entry.transform.localScale = new Vector3(offset, entry.transform.localScale.y, offset);
        mWallObjs.Add(entry);
        
        var exit = GameObject.Instantiate(exitPrefab, parent);
        exit.transform.localPosition = new Vector3(mExit.x * offset - mWidth * 0.5f * offset, 0, mExit.y * offset - mHeight * 0.5f * offset);
        exit.transform.localScale = new Vector3(offset, exit.transform.localScale.y, offset);
        mWallObjs.Add(exit);

        yield return new WaitForSeconds(1);

    }
    public IEnumerator Render(GameObject prefab, Transform parent, float offset, int step)
    {
        for (int i = 0; i < mWidth; i++)
        {
            for (int j = 0; j < mHeight; j++)
            {
                var cell = mMazeCells[i, j];
                if (cell.bBlocked && (cell.step == step || step == S_RENDER_IM))
                {
                    var wall = GameObject.Instantiate(prefab, parent);
                    wall.transform.localPosition = new Vector3(i * offset - mWidth * 0.5f * offset, 0, j * offset - mHeight * 0.5f * offset);
                    wall.transform.localScale = new Vector3(offset, wall.transform.localScale.y, offset);
                    mWallObjs.Add(wall);

                    yield return new WaitForSeconds(Random.Range(0.0f, 0.02f));
                }
            }
        }

        yield return 0;
    }

    //private List<P> RandomList<P>(List<P> list)
    //{
        
    //    List<P> ret = new List<P>();
    //    foreach (var item in list)
    //    {
    //        ret.Insert(Random.Range(0, ret.Count + 1), item);
    //    }
    //    return ret;
    //}

    public Vector3 GetStartLocation(float offset)
    {
        return new Vector3((mEntry.x - mWidth * 0.5f) * offset, 0, (mEntry.y - mHeight * 0.5f) * offset);
    }

    public Quaternion GetStartRotation()
    {
        if (mEntry.x == 0 || mEntry.x == mWidth - 1)
        {
            return Quaternion.Euler(0, 90, 0);
        }
        return Quaternion.Euler(0, 0, 0);
    }

    public bool IsExit(P p)
    {
        return p == mExit;
    }

    public bool IsExit(int x, int y)
    {
        return mExit.x == x && mExit.y == y;
    }

    public bool Valid(P p)
    {
        return p.x >= 0 && p.x < mWidth && p.y >= 0 && p.y < mHeight;
    }

    public bool Valid(int x, int y)
    {
        return x >= 0 && x < mWidth && y >= 0 && y < mHeight;
    }

    public bool Visited(int x, int y)
    {
        return mMazeCells[x, y].bVisited;
    }

    public void SetVisited(int x, int y)
    {
        mMazeCells[x, y].bVisited = true;
    }

    public void ResetVisit()
    {
        for (int i = 0; i < mWidth; i++)
        {
            for (int j = 0; j < mHeight; j++)
            {
                mMazeCells[i, j].bVisited = false;
            }
        }
    }

    public Cell Get(P p)
    {
        return mMazeCells[p.x, p.y];
    }

    public Cell Get(int x, int y)
    {
        return mMazeCells[x, y];
    }



    public void Release()
    {
        foreach (var wall in mWallObjs)
        {
            Object.DestroyImmediate(wall);
        }
        mWallObjs.Clear();
        mWallObjs = null;
        mMazeCells = null;
    }

}
