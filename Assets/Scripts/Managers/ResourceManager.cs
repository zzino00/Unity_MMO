using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T: Object
    {
        return Resources.Load<T>(path);
    }
  
    public GameObject Instantiate(string path, Transform parent = null)// = null이 되어있으면 기본값은 null 넣어주면 넣어준 값을 사용
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Debug.Log($"Failed to Load Prefab:{path}");
            return null;
        }

        GameObject go = Object.Instantiate(prefab, parent);
        int index = go.name.IndexOf("(Clone)");// Clone이라는 이름을 가지고있는 게임오즈젝트의 인덱스를 구함
        if(index>0 )
        {
        go.name = go.name.Substring(0, index);// Substring(a,b) a번부터 b번까지로 문자열을 잘라버림
        }
        return go;
    }

    public void Destroy(GameObject go) 
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }

}
