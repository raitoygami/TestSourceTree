using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLeaf
{
    public PLeaf(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public PLeaf(int x, int y, ref PLeaf parent)
    {
        this.x = x;
        this.y = y;
        this.parent = parent;
    }

    public void SetParent(ref PLeaf parent)
    {
        this.parent = parent;
    }
    public bool isNeighbor(PLeaf target)
    {
        return Mathf.Abs(target.x - x) <= 1 && Mathf.Abs(target.y - y) <= 1;
    }

    public static bool operator ==(PLeaf lhs, PLeaf rhs) { Debug.Log(lhs.x + "---" + rhs.y); return lhs.x == rhs.x && lhs.y == rhs.y; }
    public static bool operator !=(PLeaf lhs, PLeaf rhs) { Debug.Log(lhs.x + "-+-" + rhs.y); return lhs.x != rhs.x || lhs.y != rhs.y; }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static PLeaf operator +(PLeaf lhs, P rhs) { return new PLeaf(lhs.x + rhs.x, lhs.y + rhs.y); }

    public int x;
    public int y;
    public PLeaf parent;
}


public class PathFindingProxy : MonoBehaviour
{
    
    private static PathFindingProxy _mInstance;
    public static PathFindingProxy Instance() { return _mInstance; }

    private bool bAutoProxy = false;
    private PLeaf result = null;
    private P[] mNeighbor = new P[] { 
        new P(0,1),
        new P(-1,0),
        new P(0,-1),
        new P(1,0),
    };

    private Character mCharacter2Proxy;
    
    private void Awake()
    {
        _mInstance = this;
        bAutoProxy = false;
    }

    public bool AutoProxy(int width, int height, float offset)
    {
        if (Player.Instance())
        {
            mCharacter2Proxy = Player.Instance().Character;
            if (mCharacter2Proxy)
            {
                result = null;
                bAutoProxy = true;

                if (BFS(width, height, offset))
                {
                    Player.Instance().BlockInput(InputBlockCode.BLOCK_AUTO_PROXY);
                    Player.Instance().SetCommands(ref result, width, height, offset);
                    return true;
                }
            }
        }

        return false;
    }
    public void Stop()
    {
        bAutoProxy = false;
        Player.Instance().UnBlockInput(InputBlockCode.BLOCK_AUTO_PROXY);
    }
    void Update()
    {
        if (bAutoProxy && mCharacter2Proxy)
        {
            //print("++++");

            if (!mCharacter2Proxy.AutoMove())
                Stop();
        }    
    }

    private bool BFS(int width, int height, float offset)
    {
        var maze = LittleMaze.Instance().MazeIst;

        var position = mCharacter2Proxy.transform.position;
        
        PLeaf current = new PLeaf(Mathf.FloorToInt(position.x / offset +  width * 0.5f + 0.5f)
            , Mathf.FloorToInt(position.z / offset + 0.5f * height + 0.5f));
        current.parent = null;
        
        Queue<PLeaf> wait = new Queue<PLeaf>();
        wait.Enqueue(current);

        maze.ResetVisit();
        maze.SetVisited(current.x, current.y);
        
        bool find = false;

        while (wait.Count > 0)
        {
            //Debug.Log(wait.Count);

            var p = wait.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                var neighber = p + mNeighbor[i];
                if (maze.IsExit(neighber.x, neighber.y))
                {
                    find = true;
                    result = new PLeaf(neighber.x, neighber.y, ref p);
                    break;
                }

                if (maze.Valid(neighber.x, neighber.y))
                {
                    var cell = maze.Get(neighber.x, neighber.y);
                    if (!cell.bBlocked && !maze.Visited(neighber.x, neighber.y))
                    {
                        neighber.parent = p;
                        wait.Enqueue(neighber);
                        maze.SetVisited(neighber.x, neighber.y);
                    }
                }
            }
            //Debug.Log("++++++++" + wait.Count);
            if (find)
                break;

        }
        if (find)
            SimplifyPath();

        return find;
        
    }
    
    private void SimplifyPath()
    {
        if (!(result is null))
        {
            var temp = result;
            while (!(temp.parent is null))
            {
                if (!(temp.parent.parent is null))
                {
                    if (temp.isNeighbor(temp.parent.parent))
                    {
                        temp.parent = temp.parent.parent;
                    }
                }
                temp = temp.parent;
            }
        }
    }
    

}
