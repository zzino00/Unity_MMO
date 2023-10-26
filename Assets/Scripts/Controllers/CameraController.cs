using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
     Define.CameraMode _mode = Define.CameraMode.QuaterView;

    [SerializeField]
     Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f);

    [SerializeField]
    GameObject _player = null;
    
    void Start()
    {
        
    }

   
    void LateUpdate()// �÷��̾� update�� ������ update�� ����Ǿ�� �Ѵ�.
    {
        if(_mode == Define.CameraMode.QuaterView)
        {
            transform.position = _player.transform.position + _delta;// _delta�� �÷��̾���ġ�� �״�� ������ �÷��̾�� ��ġ�ϱ� �÷��̾ �ߺ��̴� ��ġ
            transform.LookAt(_player.transform);// �ٶ󺸴� ���� ����
        }
      
    }


    public void SetQuaterView(Vector3 delta) // ���߿� �ڵ�󿡼� ���ͺ並 �����ϰ� ������ �̿밡��
    {
        _mode = Define.CameraMode.QuaterView;
        _delta = delta;
    }
}
