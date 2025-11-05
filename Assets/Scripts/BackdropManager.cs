using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

public class BackdropManager : MonoBehaviour
{
    [Header("Backdrop Sequence")]
    [SerializeField] List<Material> Backdrops;

    [Header("Curtain Management")]
    [SerializeField] CurtainManager Curtain;
    private float curtainClipLength = 6f;

    [Header("Timeline Management")]
    [SerializeField] private PlayableDirector Director;

    private MeshRenderer _renderer;
    private int index = 0;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        if (Backdrops.Count > 0) { _renderer.sharedMaterial = Backdrops[index]; }

        if (Curtain == null) { Debug.LogError("[BackdropManager] Associated CurtainManager is not assigned!"); }
        
        if (Director == null) { Debug.LogError("[BackdropManager] Associated PlayableDIrector is not assigned!"); }
    }

    /// <summary>
    /// Transitions the backdrop to the next one on the list with a curtain flourish
    /// </summary>
    public void TransitionBackdrop()
    {
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        if(Curtain!=null) { Curtain.Play("close"); }

        yield return new WaitForSeconds(curtainClipLength);

        if (Director != null) { Director.Pause(); }

        Debug.Log("[BackdropManager] Press SPACE to reveal the next backdrop.");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        if (Director != null) { Director.Resume(); }

        index++;
        if (index < Backdrops.Count)
        {
            _renderer.sharedMaterial = Backdrops[index];
        }

        if(Curtain!=null) { Curtain.Play("open"); }
    }
}
