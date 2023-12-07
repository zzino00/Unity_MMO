using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField]
     Define.CameraMode _mode = Define.CameraMode.QuaterView;

    [SerializeField]
     Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f);

    [SerializeField]
    GameObject _player = null;
    
    public void SetPlayer(GameObject player)
    {
        _player = player;
    }
    void Start()
    {
        
    }

   
    void LateUpdate()// �÷��̾� update�� ������ update�� ����Ǿ�� �Ѵ�.
    {
        if(_mode == Define.CameraMode.QuaterView)
        {
            if (!_player.IsValid())
            {
                return;
            }
            RaycastHit hit;
            if(Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Block")))// �÷��̾� ��ġ���� ī�޶� ���� ���� ��µ� �߰��� ���� �������� ��ȯ
            {
                float dist = (hit.point - _player.transform.position+ new Vector3(0,2.5f,0)).magnitude*0.8f;// �÷��̾���� ���� ���� ���� �κб����� ����� ũ��, 0.8�� �������ν� ������ ��¦ �������� �ȴ�.
                transform.position = _player.transform.position + _delta.normalized*dist;
            }
            else
            {
                transform.position = _player.transform.position + _delta;// _delta�� �÷��̾���ġ�� �״�� ������ �÷��̾�� ��ġ�ϱ� �÷��̾ �ߺ��̴� ��ġ
                transform.LookAt(_player.transform);// �ٶ󺸴� ���� ����
            }
        }
           
      
    }


    public void SetQuaterView(Vector3 delta) // ���߿� �ڵ�󿡼� ���ͺ並 �����ϰ� ������ �̿밡��
    {
        _mode = Define.CameraMode.QuaterView;
        _delta = delta;
    }
}
