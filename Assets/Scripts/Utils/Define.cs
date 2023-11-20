using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    public enum Layer
    {
        Monster =8,
        Ground =6,
        Block =7
    }

    public enum Scene
    {
        Unknown,
        Login,
        Lobby,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,

    }

    public enum UIEvent
    {
        Click,
        Drag,

    }
    public enum MouseEvent
    {
        Press,
        Click,
    }
    public enum CameraMode
    {
        QuaterView,
    }

  
   
}
