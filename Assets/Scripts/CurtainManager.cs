using UnityEngine;
using UnityEngine.Video;

public class CurtainManager : MonoBehaviour
{
    [SerializeField] VideoClip OpenCurtains;
    [SerializeField] VideoClip CloseCurtains;

    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if(videoPlayer == null) { Debug.LogError("[Curtain Manager] Component Not Found: VideoPlayer"); }
    }

    /// <summary>
    /// Plays the clip arg
    /// </summary>
    /// <param name="arg"></param>
    public void Play(string arg)
    {
        if (videoPlayer == null) return;
        switch (arg.ToLower())
        {
            case "open":
                videoPlayer.clip = OpenCurtains;
                break;
            case "close":
                videoPlayer.clip = CloseCurtains;
                break;
            default:
                Debug.LogWarning("[Curtain Manager] Unknown Clip");
                return;
        }

        //Commented out stop since it bugs the screen for a second
        //videoPlayer.Stop();
        videoPlayer.Play();

        Debug.Log($"[Curtain Manager] Now Playing: {arg}");
    }
}
