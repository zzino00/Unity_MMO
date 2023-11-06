using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T: Object
    {
        return Resources.Load<T>(path);
    }
  
    public GameObject Instantiate(string path, Transform parent = null)// = null�� �Ǿ������� �⺻���� null �־��ָ� �־��� ���� ���
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if(prefab == null)
        {
            Debug.Log($"Failed to Load Prefab:{path}");
            return null;
        }

        GameObject go = Object.Instantiate(prefab, parent);
        int index = go.name.IndexOf("(Clone)");// Clone�̶�� �̸��� �������ִ� ���ӿ�����Ʈ�� �ε����� ����
        if(index>0 )
        {
        go.name = go.name.Substring(0, index);// Substring(a,b) a������ b�������� ���ڿ��� �߶����
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
