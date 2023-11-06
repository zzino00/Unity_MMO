using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float _speed = 10.0f;
    Vector3 _destPos; // ���콺�� Ŭ���� ������ �����ϴ� ����
    void Start()
    {
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

    
    }

   

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// ī�޶󿡼� MouseĿ�� ��ġ�� ���� ��
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);// ������ ���� �ְ� ������ ���� ��

        RaycastHit hit; // �ε�ģ ��ü�� ������ �����ϴ� ����
        if(Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))// ���� ���� �ε��� ��ü�� ��ȯ�� �ٵ� ��ü�� ���̾ Wall�϶��� ��ȯ
        {
            _destPos =hit.point;// �ε��� ������ ���Ͱ� ����
            _state =PlayerState.Moving;
        }



        
    }// ���콺�� ����

  
    public enum PlayerState
    {
        Die,
        Moving,
        Idle,
        Channeling,
        Jumping,
        Falling,
    }

    PlayerState _state = PlayerState.Idle;

    void UpdateDie()
    {

    }

    void UpdateMoving()
    {
     
        
            Vector3 dir = _destPos - transform.position;// �÷��̾� ��ġ���� ������������ ���⺤��
            if (dir.magnitude < 0.0001f)// �������� ��: float���� ��꿡���� 0�� ������ ����� ������ �׳� ��û �������ڷ� ����
            {
            _state = PlayerState.Idle;
            }
            else
            {
                float MoveDist = Mathf.Clamp(_speed * Time.deltaTime, 0, dir.magnitude);// �����̴� �Ÿ��� ������������ �Ÿ��� �Ѿ�� �ȵȴ�. clamp�� ���� �����̴� �Ÿ��� ������ ������
                transform.position += dir.normalized * MoveDist;// ���� ��ġ���ٰ� ���⺤�͸� ��ֶ������Ѱ�(���̰� 1�̵�)* �ӵ�

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);// �̵��ϴ� �������� ĳ���Ͱ� ���� ������ �Ҷ� �ܼ��� LookAt�� ����ϸ�
                                                                                                                             // �Ҷ� ����� �ȴ�. �̸� �ذ��ϱ����� Slerp�� ���
                                                                                                                             //�ִϸ��̼�
           
              }


        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", _speed);


    }

    void UpdateIdle()
    {

        Animator anim = GetComponent<Animator>();
        anim.SetFloat("speed", 0.0f);

    }
    void Update()
    {
        

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