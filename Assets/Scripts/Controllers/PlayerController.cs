using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float _speed = 10.0f;


    bool _moveToDest = false; // ���콺�� Ŭ���� �������� �̵����� ������ ���ϴ� ����
    Vector3 _destPos; // ���콺�� Ŭ���� ������ �����ϴ� ����
    void Start()
    {
        Managers.Input.KeyAction -= OnKeyboard; // Ȥ�� OnKeyboard�� �̹� �ٸ� ������ �����Ǿ������� 2�� �����Ǵ°Ŵϱ� �̸� ������Ҹ� �ѹ�����
        Managers.Input.KeyAction += OnKeyboard;
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
    }

    void OnKeyboard()
    {

        // TransfromDirection - Local���� World�� ��ǥ�� ��ȯ
        if (Input.GetKey(KeyCode.W))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.2f);
            transform.position += Vector3.forward * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.2f);
            transform.position += Vector3.back * Time.deltaTime * _speed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.2f);
            transform.position += Vector3.right * Time.deltaTime * _speed;

        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.2f);
            transform.position += Vector3.left * Time.deltaTime * _speed;

        }
        _moveToDest = false;
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if(evt != Define.MouseEvent.Click)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// ī�޶󿡼� MouseĿ�� ��ġ�� ���� ��
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);// ������ ���� �ְ� ������ ���� ��

        RaycastHit hit; // �ε�ģ ��ü�� ������ �����ϴ� ����
        if(Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))// ���� ���� �ε��� ��ü�� ��ȯ�� �ٵ� ��ü�� ���̾ Wall�϶��� ��ȯ
        {
            _destPos =hit.point;// �ε��� ������ ���Ͱ� ����
            _moveToDest =true;// �ε������� �������� ��������
        }



        
    }
    void Update()
    {
        if(_moveToDest) 
        {
            Vector3 dir = _destPos - transform.position;// �÷��̾� ��ġ���� ������������ ���⺤��
            if(dir.magnitude < 0.0001f)// �������� ��: float���� ��꿡���� 0�� ������ ����� ������ �׳� ��û �������ڷ� ����
            {
                _moveToDest = false;// ���������ϱ� false
            }
            else
            {
                float MoveDist = Mathf.Clamp(_speed * Time.deltaTime, 0 , dir.magnitude);// �����̴� �Ÿ��� ������������ �Ÿ��� �Ѿ�� �ȵȴ�. clamp�� ���� �����̴� �Ÿ��� ������ ������
                transform.position += dir.normalized * MoveDist;// ���� ��ġ���ٰ� ���⺤�͸� ��ֶ������Ѱ�(���̰� 1�̵�)* �ӵ�

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);// �̵��ϴ� �������� ĳ���Ͱ� ���� ������ �Ҷ� �ܼ��� LookAt�� ����ϸ�
                                                                                                                               // �Ҷ� ����� �ȴ�. �̸� �ذ��ϱ����� Slerp�� ���
               
            }
        }
        
    }
}