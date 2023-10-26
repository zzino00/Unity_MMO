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

   
    void LateUpdate()// 플레이어 update가 끝나고 update가 실행되어야 한다.
    {
        if(_mode == Define.CameraMode.QuaterView)
        {
            transform.position = _player.transform.position + _delta;// _delta는 플레이어위치를 그대로 받으면 플레이어랑 겹치니까 플레이어가 잘보이는 위치
            transform.LookAt(_player.transform);// 바라보는 방향 설정
        }
      
    }


    public void SetQuaterView(Vector3 delta) // 나중에 코드상에서 쿼터뷰를 설정하고 싶을때 이용가능
    {
        _mode = Define.CameraMode.QuaterView;
        _delta = delta;
    }
}
