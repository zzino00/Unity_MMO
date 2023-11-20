using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    PlayerStat _stat;
    Vector3 _destPos; // ���콺�� Ŭ���� ������ �����ϴ� ����
    Texture2D _attackIcon;
    Texture2D _handIcon;

    enum CursorType
    {
        None,
        Attack,
        Hand,
    }

    CursorType _cursorType = CursorType.None;
    void Start()
    {
        _attackIcon = Managers.Resource.Load<Texture2D>("Textures/Cursor/Attack");
        _handIcon =Managers.Resource.Load<Texture2D>("Textures/Cursor/Hand");
        _stat = GetComponent<PlayerStat>();
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

    
    }


    void UpdateMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// ī�޶󿡼� MouseĿ�� ��ġ�� ���� ��

        RaycastHit hit; // �ε�ģ ��ü�� ������ �����ϴ� ����
        if (Physics.Raycast(ray, out hit, 100.0f, _mask))// ���� ���� �ε��� ��ü�� ��ȯ�� �ٵ� ��ü�� ���̾ Wall�϶��� ��ȯ
        {

            if (hit.collider.gameObject.layer != (int)Define.Layer.Monster)
            {
              if(_cursorType != CursorType.Attack)
                {
                    Cursor.SetCursor(_attackIcon, new Vector2(_attackIcon.width / 5, 0), CursorMode.Auto);// 1��° ���ڴ� ����� �̹���, 2��°�� �̹��� �ȿ��� Ŭ���� �Ǵ� ��ǥ����, �������� �ϴ� Auto�� ����
                    _cursorType = CursorType.Attack;
                }
               
            }
            else
            {
                if (_cursorType != CursorType.Hand)
                {
                    Cursor.SetCursor(_handIcon, new Vector2(_handIcon.width / 3, 0), CursorMode.Auto);
                    _cursorType = CursorType.Hand;
                }
            }
        }
    }


    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);// �Ź� Layer�� ��Ʈ������ �޴°� ���ŷο��� ��Ʈ�����ڸ� �̿��ؼ� �ش� ���̾ ����?
    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// ī�޶󿡼� MouseĿ�� ��ġ�� ���� ��
        //Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);// ������ ���� �ְ� ������ ���� ��

        RaycastHit hit; // �ε�ģ ��ü�� ������ �����ϴ� ����
        if(Physics.Raycast(ray, out hit, 100.0f, _mask))// ���� ���� �ε��� ��ü�� ��ȯ�� �ٵ� ��ü�� ���̾ Wall�϶��� ��ȯ
        {
            _destPos =hit.point;// �ε��� ������ ���Ͱ� ����
            _state =PlayerState.Moving;

            if(hit.collider.gameObject.layer==(int)Define.Layer.Monster)
            {
                Debug.Log("Monster Clicked");
            }
            else
            {
                Debug.Log("Ground Clicked");
            }
        }
        
        
    }// ���콺�� ����

  
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Skill,
    }

    PlayerState _state = PlayerState.Idle;

    void UpdateDie()
    {

    }

    void UpdateMoving()
    {
     
        
            Vector3 dir = _destPos - transform.position;// �÷��̾� ��ġ���� ������������ ���⺤��
            if (dir.magnitude < 0.1f)// �������� ��: float���� ��꿡���� 0�� ������ ����� ������ �׳� ��û �������ڷ� ����
            {
            _state = PlayerState.Idle;
            }
            else
            {
            float MoveDist = Mathf.Clamp(_stat.MoveSpeed * Time.deltaTime, 0, dir.magnitude);// �����̴� �Ÿ��� ������������ �Ÿ��� �Ѿ�� �ȵȴ�. clamp�� ���� �����̴� �Ÿ��� ������ ������

            NavMeshAgent nma =  gameObject.GetOrAddComponent<NavMeshAgent>();// Util�� ������ GetOrAddComponent�Լ��� NavMeshAgent������Ʈ�� ����

               nma.Move(dir.normalized * MoveDist);// �������� �Ʒ��� �ּ�ó���Ѱ� ó�� ������ġ�� ���⺤�͸� ���ϴ� ����� �̿�������
            // NavMeshAgent���� �����ϴ� Move�Լ��� ���� ��ĸ�ŭ �������� �ʴٴ� ���� �˼� ����, �׷��� ���������� ���� ũ�� ��ƾ���


            //  transform.position += dir.normalized * MoveDist;// ���� ��ġ���ٰ� ���⺤�͸� ��ֶ������Ѱ�(���̰� 1�̵�)* �ӵ�);

            Debug.DrawRay(transform.position, dir.normalized, Color.green, 1.0f);
            if (Physics.Raycast(transform.position+Vector3.up*0.5f, dir, 1.0f, LayerMask.GetMask("Block")))// Laycast�� �ؼ� �տ� ��ֹ��� ������ ���߰� ����
            {
                _state = PlayerState.Idle;
                return;
            }
                


            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);// �̵��ϴ� �������� ĳ���Ͱ� ���� ������ �Ҷ� �ܼ��� LookAt�� ����ϸ�
                                                                                                                             // �Ҷ� ����� �ȴ�. �̸� �ذ��ϱ����� Slerp�� ���
                                                                                                                             //�ִϸ��̼�
           
            }


        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _stat.MoveSpeed);


    }

    void UpdateIdle()
    {

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0.0f);

    }
    void Update()
    {
        UpdateMouseCursor();

        switch(_state)
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
        }
        
        

        
    }
}