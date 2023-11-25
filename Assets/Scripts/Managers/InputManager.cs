using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager 
{

    public Action KeyAction = null; // delegate�� ���� Listener����(�����ý���)�� ����ϱ� ���� ����
    public Action<Define.MouseEvent> MouseAction = null; // Define.MouseEvent�� ���ڷ� �޴� delegate
    bool _pressed = false;// Ŭ���� �����ϱ�����  bool����
    float _pressedTime = 0;
    public void OnUpdate()
    {

        if (EventSystem.current.IsPointerOverGameObject())// UI��ư�� Ŭ�������� �ƴ���
            return;

        if(Input.anyKey&&KeyAction != null )
            KeyAction.Invoke();//�ش�׼��� �Էµƴٰ� �����Ǿ��ִ� �Լ��鿡�� ��ȣ�� ����

        if(MouseAction != null )
        {
            if(Input.GetMouseButton(0))// ��Ŭ����ư�� �������ֶ�
            {
                if (!_pressed)// ���콺�� ������ ó������ ��������
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);// MouseAction�� �����Ǿ� �ִ� �Լ��鿡�� Press��ȣ�� ����
                _pressed = true; //���� ��Ŭ����ư�� �������ִ�
            }
            else//��Ŭ����ư�� �Էµǰ� ���� �ʴٸ�
            {
                if(_pressed)// �׷��� �ѹ� �������� �±�� �ϴٸ�(���ȴ� ���ٸ�=> Ŭ���� �ƴٸ�)
                {
                    if(Time.time <_pressedTime+0.2f)
                    {
                        MouseAction.Invoke(Define.MouseEvent.Click);//MouseAction�� �����Ǿ� �ִ� �Լ��鿡�� Click��ȣ�� ����
                    }
                   
                        MouseAction.Invoke(Define.MouseEvent.PointerUp);

                    _pressed = false;// ���� ��Ŭ����ư�� ���������� �ʴ�
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
