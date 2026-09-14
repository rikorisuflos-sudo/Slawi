using UnityEngine;

public class GameManager : SingletoMonoBehaviour<GameManager>
{
    protected override bool DestroyTargetGameObject => true;

    // 任意の処理が実行済みか
    private bool isExecuted = false;

    /// <summary>
    /// GameManager生成時に1回だけ実行
    /// </summary>
    protected override void Init()
    {
        base.Init();

        GameStart();
    }

    /// <summary>
    /// ゲーム起動時に1回だけ実行する処理
    /// </summary>
    private void GameStart()
    {
        Debug.Log("ゲーム起動時の処理");

        // ゲーム起動時に行いたい処理
    }

    /// <summary>
    /// 任意のタイミングで1回だけ実行
    /// </summary>
    public void ExecuteOnce()
    {
        if (isExecuted)
        {
            return;
        }

        isExecuted = true;

        Debug.Log("任意のタイミングで1回だけ実行");

        // 任意のタイミングで行いたい処理
    }
}