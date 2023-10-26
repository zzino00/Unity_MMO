using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float _speed = 10.0f;


    bool _moveToDest = false; // 마우스를 클릭한 지점으로 이동할지 말지를 정하는 변수
    Vector3 _destPos; // 마우스를 클릭한 지점을 저장하는 변수
    void Start()
    {
        Managers.Input.KeyAction -= OnKeyboard; // 혹시 OnKeyboard가 이미 다른 곳에서 구독되어있으면 2번 구독되는거니까 미리 구독취소를 한번해줌
        Managers.Input.KeyAction += OnKeyboard;
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;
    }

    void OnKeyboard()
    {

        // TransfromDirection - Local에서 World로 좌표를 변환
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

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);// 카메라에서 Mouse커서 위치로 빛을 쏨
        Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);// 눈으로 볼수 있게 디버깅용 빛을 쏨

        RaycastHit hit; // 부딪친 물체의 정보를 저장하는 변수
        if(Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall")))// 빛을 쏴서 부딪힌 물체를 반환함 근데 물체의 레이어가 Wall일때만 반환
        {
            _destPos =hit.point;// 부딪힌 지점의 백터값 저장
            _moveToDest =true;// 부딪혔으면 그쪽으로 움직여라
        }



        
    }
    void Update()
    {
        if(_moveToDest) 
        {
            Vector3 dir = _destPos - transform.position;// 플레이어 위치에서 목적지까지의 방향벡터
            if(dir.magnitude < 0.0001f)// 도착했을 때: float간의 계산에서는 0이 나오기 힘들기 때문에 그냥 엄청 작은숫자로 설정
            {
                _moveToDest = false;// 도착했으니까 false
            }
            else
            {
                float MoveDist = Mathf.Clamp(_speed * Time.deltaTime, 0 , dir.magnitude);// 움직이는 거리가 목적지까지의 거리를 넘어가면 안된다. clamp를 통해 움직이는 거리에 범위를 정해줌
                transform.position += dir.normalized * MoveDist;// 현재 위치에다가 방향벡터를 노멀라이즈한거(길이가 1이됨)* 속도

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);// 이동하는 방향으로 캐릭터가 몸을 돌리게 할때 단순히 LookAt을 사용하면
                                                                                                                               // 뚝뚝 끊기게 된다. 이를 해결하기위해 Slerp를 사용
               
            }
        }
        
    }
}