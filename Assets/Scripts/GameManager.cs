using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static UnityEngine.LowLevelPhysics2D.PhysicsLayers;

public class GameManager : SingletoMonoBehaviour<GameManager>
{
    protected override bool DestroyTargetGameObject => true;

    // 任意の処理が実行済みか
    private bool isExecuted = false;

    [SerializeField] AudioSource audioSource;

    // 操作したいカメラ
    [SerializeField] private Camera targetCamera;

    // 非表示にしたいレイヤー名
    [SerializeField] LayerMask layerName;

    float volume;
    bool indication;

    public float Volume { set { volume = value; } }
    public bool Indication {set { indication = value; } }

    [SerializeField] GameObject config;

    //[SerializeField]AudioManager audioManager;
    //[SerializeField] SystemManager systemManager;

    /// <summary>
    /// GameManager生成時に1回だけ実行
    /// </summary>
    protected override void Init()
    {
        base.Init();

        GameStart();
        //volume = audioManager.Volume;
        //indication = systemManager.IsOn;
    }
    private void Start()
    {
        config.SetActive(false);
        
    }
    /// <summary>
    /// ゲーム起動時に1回だけ実行する処理
    /// </summary>
    private void GameStart()
    {
        Debug.Log("ゲーム起動時の処理");

        // ゲーム起動時に行いたい処理
        ChangeVolume();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 任意のタイミングで1回だけ実行
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (isExecuted)
        {
            return;
        }

        isExecuted = true;

        Debug.Log("任意のタイミングで1回だけ実行");

        config.SetActive(false);

        // 任意のタイミングで行いたい処理
        audioSource = FindFirstObjectByType<AudioSource>();

        ChangeVolume();
    }

    void ChangeVolume()
    {
        audioSource.Play();
        audioSource.volume = volume;
    }
}