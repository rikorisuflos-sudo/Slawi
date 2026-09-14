using UnityEngine;

public class SingletoMonoBehaviour<T> : MonoBehaviour where T : SingletoMonoBehaviour<T>
{
    protected virtual bool DestroyTargetGameObject => false;

    public static T I { get; private set; } = null;

    /// <summary>
    /// Singletonが有効か
    /// </summary>
    public static bool IsValid() => I != null;

    private void Awake()
    {
        if (I == null)
        {
            // シーンを移動しても残す
            DontDestroyOnLoad(gameObject);
            I = this as T;
            I.Init();
            return;
        }
        if (DestroyTargetGameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// 派生クラス用のAwake
    /// </summary>
    protected virtual void Init()
    {
    }

    private void OnDestroy()
    {
        if (I == this)
        {
            I = null;
        }
        OnRelease();
    }

    /// <summary>
    /// 派生クラス用のOnDestroy
    /// </summary>
    protected virtual void OnRelease()
    {
    }
}