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


    //ó�� ȭ���� Ŭ�������� ���͸� Ŭ���ߴٸ� ���콺�� �������� ���¿��� �������� ���͸� ���� �÷��̾ �����̵���
    // ó�� ȭ���� Ŭ�������� ���͸� Ŭ�����ߴٸ� ���콺�� �������� ���¿��� �����̸� Ŀ���� ���� �����̵��� 
    //���� ��Ĵ�� �����̷��� Ŭ���� �������� ���� ���콺�� ó�� ���������� ���콺�� ���� ���� ���°� �ʿ��ϴ�.
    public enum MouseEvent
    {
        Press,
        PointerDown, // ó�� Ŭ���� ����
        PointerUp, // ���콺�� �� ����?
        Click,
    }
    public enum CameraMode
    {
        QuaterView,
    }

  
   
}
