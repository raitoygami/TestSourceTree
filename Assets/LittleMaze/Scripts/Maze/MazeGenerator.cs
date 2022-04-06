using UnityEngine;

public static class MazeGenerator
{
    public static int mStepLimit;
    public static Maze Generate(int width, int height, int step)
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        mStepLimit = step;
        Maze maze = new Maze(width, height);

        Compute(ref maze, new P(1, 1), width - 2, height - 2, 1);

        return maze;
    }


    // width height  无墙区域的宽高
    // origin 无墙区域的原点
    // 一个区域可能有多个出口

    private static void Compute(ref Maze maze, P origin, int width, int height, int step = 0)
    {
        if (step >= mStepLimit)
        {
            return;
        }
        if (width < 2 || height < 2)
            return;

        bool bRow = height >= width;// Random.Range(0, 9) % 2 == 0;

        int wallIdx = bRow ? Random.Range(0, height - 1) : Random.Range(0, width - 1);


        int pathIdx = bRow ? Random.Range(0, width - 1) : Random.Range(0, height - 1);

        if (bRow)
        {
            // 水平切割区域(top bottom)
            maze.DisConnectRow(origin, wallIdx, width, height, pathIdx, step);
            
            Compute(ref maze, origin, width, wallIdx ,  step + 1);
            Compute(ref maze, new P(origin.x, origin.y + wallIdx + 1), width, height - wallIdx - 1,  step + 1);


        }
        else
        {
            // 垂直切割区域(left right)
            maze.DisConnectCol(origin, wallIdx, width, height, pathIdx, step);
            Compute(ref maze, origin, wallIdx , height, step + 1);
            Compute(ref maze, new P(origin.x + wallIdx + 1, origin.y), width - wallIdx - 1, height,  step + 1);

        }

    }
}
