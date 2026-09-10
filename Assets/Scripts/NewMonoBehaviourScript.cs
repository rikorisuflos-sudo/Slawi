using UnityEngine;
using UnityEngine.Video;



public class VideoTest : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.errorReceived += OnError;

        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        Debug.Log("動画の準備完了！");
        Debug.Log("長さ：" + vp.length);
        Debug.Log("幅：" + vp.width);
        Debug.Log("高さ：" + vp.height);

        vp.Play();
    }

    private void OnError(VideoPlayer vp, string message)
    {
        Debug.LogError("動画エラー：" + message);
    }

}
