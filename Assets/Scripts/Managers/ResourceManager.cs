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
            int index = name.LastIndexOf('/'); //Load�� ��θ� ���ι޴µ� ���������� ã�� �Լ��� �̸��� �ޱ⶧���� 
                                               // ���/ ������ ���������
            if (index>= 0) 
            {
                name = name.Substring(index + 1);// /���� �����ֱ�
            }

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null) // ã������(���� ��������� ������) �ٷ� ��ȯ
            {
                return go as T;
            }

        }


        return Resources.Load<T>(path); // �װ� �ƴϸ� ��θ� ���� ã�ƾ���
    }
  

    public GameObject Instantiate(string path, Transform parent = null)// = null�� �Ǿ������� �⺻���� null �־��ָ� �־��� ���� ���
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if(original == null)
        {
            Debug.Log($"Failed to Load Prefab:{path}");
            return null;
        }

        // ������Ʈ�� Instantiate�ϱ����� Ǯ���� ������Ʈ�� �ִ� �� Ȯ��
        if(original.GetComponent<Poolable>() != null)// �׷��� ��� Ǯ���� ������Ʈ�� Poolable�� ������ �����ϱ� �̰ͺ��� Ȯ��
        {
            return Managers.Pool.Pop(original, parent).gameObject; 
        }

        //Poolable�� ���� ������Ʈ�̸� �׳� ����
        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }

    public void Destroy(GameObject go) 
    {
        if (go == null)
            return;

        //�ı��� ������Ʈ�� Poolable�� �ִ��� Ȯ��
        Poolable poolable = go.GetComponent<Poolable>();

        //poolable�� �ִ� ������Ʈ�̸� �ٽ� ���ÿ� �־���
        if(poolable != null) 
        {
        Managers.Pool.Push(poolable);
        return;
        }

        //�ƴϸ� �׳� ����
        Object.Destroy(go);
    }

}
