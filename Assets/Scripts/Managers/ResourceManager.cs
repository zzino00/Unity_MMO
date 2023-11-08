using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.UI.Image;

public class ResourceManager
{
    public T Load<T>(string path) where T: Object
    {
        if(typeof(T)== typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/'); //Load는 경로를 전부받는데 오리지널을 찾는 함수는 이름만 받기때문에 
                                               // 경로/ 까지를 날려줘야함
            if (index>= 0) 
            {
                name = name.Substring(index + 1);// /까지 날려주기
            }

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null) // 찾았으면(전에 사용한적이 있으면) 바로 반환
            {
                return go as T;
            }

        }


        return Resources.Load<T>(path); // 그게 아니면 경로를 통해 찾아야함
    }
  

    public GameObject Instantiate(string path, Transform parent = null)// = null이 되어있으면 기본값은 null 넣어주면 넣어준 값을 사용
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if(original == null)
        {
            Debug.Log($"Failed to Load Prefab:{path}");
            return null;
        }

        // 오브젝트를 Instantiate하기전에 풀링된 오브젝트가 있는 지 확인
        if(original.GetComponent<Poolable>() != null)// 그런데 모든 풀링된 오브젝트는 Poolable을 가지고 있으니까 이것부터 확인
        {
            return Managers.Pool.Pop(original, parent).gameObject; 
        }

        //Poolable이 없는 오브젝트이면 그냥 생성
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go) 
    {
        if (go == null)
            return;

        //파괴할 오브젝트에 Poolable이 있는지 확인
        Poolable poolable = go.GetComponent<Poolable>();

        //poolable이 있는 오브젝트이면 다시 스택에 넣어줌
        if(poolable != null) 
        {
        Managers.Pool.Push(poolable);
        return;
        }

        //아니면 그냥 삭제
        Object.Destroy(go);
    }

}
