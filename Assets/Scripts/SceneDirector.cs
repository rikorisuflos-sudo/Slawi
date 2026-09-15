//using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDirector : MonoBehaviour
{
//    int sceneIndex =  -1;

//#if UNITY_EDITOR
//    [SerializeField] SceneAsset sceneAsset;//シーン
//    private void OnValidate()
//    {
//        if(sceneAsset != null)
//        {
//            //シーンへのアクセスパス取得
//            string scenePath = AssetDatabase.GetAssetPath(sceneAsset);

//            //アクセスパスからインデックス番号を取得
//            sceneIndex = SceneUtility.GetBuildIndexByScenePath(scenePath);
//        }
//    }
//#endif
//    public void OnClick()
//    {
//        if (sceneAsset != null)
//        {
//            Time.timeScale = 1;
//            SceneManager.LoadScene(sceneIndex);//シーン遷移
//        }
//    }

    public void Title()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");

    }

    public void Retry()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("GameScene");
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
