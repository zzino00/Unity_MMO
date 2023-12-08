using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);// �Ź� Layer�� ��Ʈ������ �޴°� ���ŷο��� ��Ʈ�����ڸ� �̿��ؼ� �ش� ���̾ ����?
   
    PlayerStat _stat;
    bool _stopSkill = false;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Player;

        _stat = GetComponent<PlayerStat>();
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);// UI_HPBar�� �����Ѵ��� �÷��̾����� ����

    }


    protected override void UpdateMoving()
    {
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude; // �÷��̾�� ���ͱ����� �Ÿ�
            if (distance <= 1)
            {
                State = Define.State.Skill;
                Debug.Log("Attack");
                return;
            }
        }

        Vector3 dir = _destPos - transform.position;// �÷��̾� ��ġ���� ������������ ���⺤��
        if (dir.magnitude < 0.1f)// �������� ��: float���� ��꿡���� 0�� ������ ����� ������ �׳� ��û �������ڷ� ����
        {
            State = Define.State.Idle;
        }
        else
        {
        
            Debug.DrawRay(transform.position, dir.normalized, Color.green, 1.0f);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))// Laycast�� �ؼ� �տ� ��ֹ��� ������ ���߰� ����
            {
                if (Input.GetMouseButton(0) == false) // ���콺�� Ŭ���ϰ� ������ Idle�� �Ⱥ���
                    State = Define.State.Idle;

                return;
            }

            float MoveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);// �����̴� �Ÿ��� ������������ �Ÿ��� �Ѿ�� �ȵȴ�. clamp�� ���� �����̴� �Ÿ��� ������ ������

            transform.position += dir.normalized * MoveDist; // Ÿ�ٿ��� �̵��ϵ���
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);// �̵��ϴ� �������� ĳ���Ͱ� ���� ������ �Ҷ� �ܼ��� LookAt�� ����ϸ�
                                                                                                                         // �Ҷ� ����� �ȴ�. �̸� �ذ��ϱ����� Slerp�� ���
                                                                                                                         //�ִϸ��̼�

        }

    }

    protected override void UpdateSkill()
    {
       if(_lockTarget !=null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;// �����ؾ��ϴ� ����=> Ÿ����ġ���� ����ġ ����
            Quaternion quat = Quaternion.LookRotation(dir);//LookRotation�Լ��� ���� quat���� ����
            transform.rotation =Quaternion.Lerp(transform.rotation, quat, 20 * Time.deltaTime);// Lerp�Լ��� ���� �ε巴�� �����̰�
        }
    }

    void OnHitEvent()
    {
        if(_lockTarget!=null)
        {
          Stat targetStat = _lockTarget.GetComponent<Stat>();
          targetStat.OnAttacked(_stat);

        }


        Debug.Log("OnHitEvent");
        if(_stopSkill)// �ѹ������� 1�� ���� 
        {
            State = Define.State.Idle;
        }
        else// �ڴ����� ���Ӱ���
        {
            State = Define.State.Skill;
          //  _stopSkill = false;
        }
    }
  

    void OnMouseEvent(Define.MouseEvent evt)
    {
       switch(State)
       {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill://�̰� ������ ���¿��� �ٽ� ���������� 1�� ������ �����ϱ� ���ؼ�
                {
                    if (evt == Define.MouseEvent.PointerUp)// ���ݻ��¿��� ���콺�� �ö󰬴ٸ�
                    {
                        _stopSkill = true;
                    }
                }
                break;
            
       }
       
        
        
    }// ���콺�� ����

  
    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        if (State == Define.State.Die)
            return;

        RaycastHit hit; // �ε�ģ ��ü�� ������ �����ϴ� ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// ī�޶󿡼� MouseĿ�� ��ġ�� ���� ��
        bool raycasthit = Physics.Raycast(ray, out hit, 100.0f, _mask);
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);// ������ ���� �ְ� ������ ���� ��
        switch (evt)
        {
            case Define.MouseEvent.PointerDown:
                {
                    if (raycasthit)
                    {
                        _destPos = hit.point;// �ε��� ������ ���Ͱ� ����
                        State = Define.State.Moving;
                        _stopSkill = false;// Ŭ���� �� �������� �ϴ� ������ ������� ������ �𸣴ϱ� false

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                            _lockTarget = hit.collider.gameObject;
                        else
                            _lockTarget = null;
                    }
                }
                break;

            case Define.MouseEvent.Press:
                {
                 if (_lockTarget==null &&raycasthit == true)
                        _destPos = hit.point;
                }
                break;

            case Define.MouseEvent PointerUp: // �̰� Idle�̳� Run���¿��� ������ ���������� 1�������ϴ� ������ ���ؼ�
                {
                    _stopSkill = true;// ���콺�� �ö�����(Ŭ���� Ǯ������) �ϴ� �ѹ��� �����ϴ� �Ŵϱ� true
                }
                break;
        }

    }

    
}