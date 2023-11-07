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
                MouseAction.Invoke(Define.MouseEvent.Press);// MouseAction�� �����Ǿ� �ִ� �Լ��鿡�� Press��ȣ�� ����
                _pressed = true; //���� ��Ŭ����ư�� �������ִ�
            }
            else//��Ŭ����ư�� �Էµǰ� ���� �ʴٸ�
            {
                if(_pressed)// �׷��� �ѹ� �������� �±�� �ϴٸ�(���ȴ� ���ٸ�=> Ŭ���� �ƴٸ�)
                {
                    MouseAction.Invoke(Define.MouseEvent.Click);//MouseAction�� �����Ǿ� �ִ� �Լ��鿡�� Click��ȣ�� ����
                    _pressed = false;// ���� ��Ŭ����ư�� ���������� �ʴ�
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
