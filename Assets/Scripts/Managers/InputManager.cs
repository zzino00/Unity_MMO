using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager 
{

    public Action KeyAction = null; // delegate의 일종 Listener패턴(구독시스템)을 사용하기 위해 선언
    public Action<Define.MouseEvent> MouseAction = null; // Define.MouseEvent를 인자로 받는 delegate
    bool _pressed = false;// 클릭을 구현하기위한  bool변수
    float _pressedTime = 0;
    public void OnUpdate()
    {

        if (EventSystem.current.IsPointerOverGameObject())// UI버튼이 클릭중인지 아닌지
            return;

        if(Input.anyKey&&KeyAction != null )
            KeyAction.Invoke();//해당액션이 입력됐다고 구독되어있는 함수들에게 신호를 보냄

        if(MouseAction != null )
        {
            if(Input.GetMouseButton(0))// 좌클릭버튼이 눌러져있때
            {
                if (!_pressed)// 마우스를 땐다음 처음으로 눌렀을때
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);// MouseAction이 구독되어 있는 함수들에게 Press신호를 보냄
                _pressed = true; //아직 좌클릭버튼이 눌러져있다
            }
            else//좌클릭버튼이 입력되고 있지 않다면
            {
                if(_pressed)// 그런데 한번 눌려진게 맞기는 하다면(눌렸다 땠다면=> 클릭이 됐다면)
                {
                    if(Time.time <_pressedTime+0.2f)
                    {
                        MouseAction.Invoke(Define.MouseEvent.Click);//MouseAction이 구독되어 있는 함수들에게 Click신호를 보냄
                    }
                   
                        MouseAction.Invoke(Define.MouseEvent.PointerUp);

                    _pressed = false;// 이제 좌클릭버튼은 눌러져있지 않다
                    _pressedTime = 0.0f;

                }
            }
        }
    }


    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
