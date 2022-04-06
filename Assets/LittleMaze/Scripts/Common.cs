using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputBlockCode
{
    //public static int BLOCK_INIT_MAZE = 1 << 0;
    public static int BLOCK_CHANGE_CONTROLLER_ANIM = 1 << 0;
    public static int BLOCK_GAME_FINISH = 1 << 1;
    public static int BLOCK_AUTO_PROXY = 1 << 2;
}

public static class CameraIndex
{
    public static int INDEX_GLOBAL = 1;
    public static int INDEX_3RD_PERSON = 2;
}