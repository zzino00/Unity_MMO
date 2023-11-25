using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);// �Ź� Layer�� ��Ʈ������ �޴°� ���ŷο��� ��Ʈ�����ڸ� �̿��ؼ� �ش� ���̾ ����?

    PlayerStat _stat;
    Vector3 _destPos; // ���콺�� Ŭ���� ������ �����ϴ� ����

    [SerializeField]
    PlayerState _state = PlayerState.Idle;

  
    GameObject _lockTarget;

    void Start()
    {

        _stat = GetComponent<PlayerStat>();
        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);// UI_HPBar�� �����Ѵ��� �÷��̾����� ����
    }

    public PlayerState State// �ڵ����� �÷��̾� ���¿� �´� �ִϸ��̼��� �����Ҽ� �ֵ���
    {
        get { return _state; }
        set {
            _state = value;
        Animator anim = GetComponent<Animator>();
            switch(_state)
            {
                case PlayerState.Die:
                    break;
                case PlayerState.Idle:
                    anim.CrossFade("WAIT", 0.1f);// �ٸ��ִϸ��̼����� �Ѿ�� 0.1���� ������ ����
                    break;
                case PlayerState.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    break;
                case PlayerState.Skill:
                    anim.CrossFade("ATTACK", 0.1f,-1,0);// ������ �Ű����� normalizedTimeOffset�� 0���� �������ָ� ó������ ���ư� �ٽý����
                                                        // ������ �������ص� �ݺ��ȴٴ� ��
                    break;
            }
        }
    }
    void UpdateDie()
    {

    }

    void UpdateMoving()
    {
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            float distance = (_destPos - transform.position).magnitude; // �÷��̾�� ���ͱ����� �Ÿ�
            if (distance <= 1)
            {
                State = PlayerState.Skill;
                Debug.Log("Attack");
                return;
            }
        }

        Vector3 dir = _destPos - transform.position;// �÷��̾� ��ġ���� ������������ ���⺤��
        if (dir.magnitude < 0.1f)// �������� ��: float���� ��꿡���� 0�� ������ ����� ������ �׳� ��û �������ڷ� ����
        {
            State = PlayerState.Idle;
        }
        else
        {
            float MoveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);// �����̴� �Ÿ��� ������������ �Ÿ��� �Ѿ�� �ȵȴ�. clamp�� ���� �����̴� �Ÿ��� ������ ������

            NavMeshAgent nma = gameObject.GetOrAddComponent<NavMeshAgent>();// Util�� ������ GetOrAddComponent�Լ��� NavMeshAgent������Ʈ�� ����

            nma.Move(dir.normalized * MoveDist);// �������� �Ʒ��� �ּ�ó���Ѱ� ó�� ������ġ�� ���⺤�͸� ���ϴ� ����� �̿�������
                                                // NavMeshAgent���� �����ϴ� Move�Լ��� ���� ��ĸ�ŭ �������� �ʴٴ� ���� �˼� ����, �׷��� ���������� ���� ũ�� ��ƾ���


            //  transform.position += dir.normalized * MoveDist;// ���� ��ġ���ٰ� ���⺤�͸� ��ֶ������Ѱ�(���̰� 1�̵�)* �ӵ�);

            Debug.DrawRay(transform.position, dir.normalized, Color.green, 1.0f);
            if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block")))// Laycast�� �ؼ� �տ� ��ֹ��� ������ ���߰� ����
            {
                if (Input.GetMouseButton(0) == false) // ���콺�� Ŭ���ϰ� ������ Idle�� �Ⱥ���
                    State = PlayerState.Idle;

                return;
            }



            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);// �̵��ϴ� �������� ĳ���Ͱ� ���� ������ �Ҷ� �ܼ��� LookAt�� ����ϸ�
                                                                                                                         // �Ҷ� ����� �ȴ�. �̸� �ذ��ϱ����� Slerp�� ���
                                                                                                                         //�ִϸ��̼�

        }

    }

    void UpdateIdle()
    {

    }

    void UpdateSkill()
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
          Stat myStat = gameObject.GetComponent<PlayerStat>();
          int damage =Mathf.Max(0, myStat.Attack - targetStat.Defense); // �� ���ݷ� - Ÿ���� ���� => ������, �ٵ� ������ �ȶ������� �ּҰ� 0���μ���
          Debug.Log(damage);
            targetStat.HP -= damage;
            
        }


        Debug.Log("OnHitEvent");
        if(_stopSkill)// �ѹ������� 1�� ���� 
        {
            State = PlayerState.Idle;
        }
        else// �ڴ����� ���Ӱ���
        {
            State = PlayerState.Skill;
          //  _stopSkill = false;
        }
    }
    void Update()
    {
        switch (State)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
                UpdateIdle();
                break;
            case PlayerState.Skill:
                UpdateSkill();
                break;
        }
    }

    bool _stopSkill = false;

    void OnMouseEvent(Define.MouseEvent evt)
    {
       switch(State)
       {
            case PlayerState.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Skill://�̰� ������ ���¿��� �ٽ� ���������� 1�� ������ �����ϱ� ���ؼ�
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
        if (State == PlayerState.Die)
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
                        State = PlayerState.Moving;
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