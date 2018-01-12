using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneController : MonoBehaviour {

    // Use this for initialization
    public ScreenOverlayController screenOverlay;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public IEnumerator transitionThenLoad(bool playTransition, string sceneName)
    {
        screenOverlay.screenAppear();
        yield return new WaitForSeconds(screenOverlay.GetComponent<GraphicColorLerp>().duration);
        GameState.playTransition = playTransition;
        SceneManager.LoadScene(sceneName);
    }
    public IEnumerator transitionThenLoad(float duration, bool playTransition, string sceneName)
    {
        screenOverlay.GetComponent<GraphicColorLerp>().duration = duration;
        screenOverlay.screenAppear();
        yield return new WaitForSeconds(duration);
        GameState.playTransition = playTransition;
        SceneManager.LoadScene(sceneName);
    }
    public IEnumerator whiteScreenThenLoad(float duration, bool playTransition, string sceneName)
    {
        screenOverlay.GetComponent<Image>().enabled = true;
        screenOverlay.GetComponent<Image>().color = Color.white;
        yield return new WaitForSeconds(duration);
        GameState.playTransition = playTransition;
        SceneManager.LoadScene(sceneName);
    }
}
