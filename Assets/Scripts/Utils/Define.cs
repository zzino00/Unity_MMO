using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define 
{
    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
    }
    public enum State
    {
        Die,
        Moving,
        Idle,
        Skill,
    }
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


    //처음 화면을 클릭했을때 몬스터를 클릭했다면 마우스를 때지않은 상태에서 움직여도 몬스터를 향해 플레이어가 움직이도록
    // 처음 화면을 클릭했을때 몬스터를 클릭안했다면 마우스를 때지않은 상태에서 움직이면 커서를 따라 움직이도록 
    //위의 방식대로 움직이려면 클릭과 프래스를 말고도 마우스를 처음 눌렀을때와 마우스를 땠을 때의 상태가 필요하다.
    public enum MouseEvent
    {
        Press,
        PointerDown, // 처음 클릭한 시점
        PointerUp, // 마우스를 땐 시점?
        Click,
    }
    public enum CameraMode
    {
        QuaterView,
    }

  
   
}
