using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<T>();
                if(instance == null)
                {
                    Debug.LogError(typeof(T) + " 該当オブジェクトが存在しない");
                    return null;
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance != null)
        {
            Destroy(this); // オブジェクト確認のためスクリプトのみDelete
            Debug.LogError("シングルトンオブジェクトが複数存在している");
            return;
        }
        instance = this as T;
    }
}
