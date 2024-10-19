using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupScene : MonoBehaviour
{
    public List<AudioClip> audioClips = new List<AudioClip>();

    public float cameraSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OnStart());
    }

    private void Update()
    {
        Camera.main.orthographicSize += cameraSpeed * Time.timeScale;
    }

    IEnumerator OnStart()
    {
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(0.5f);

        Time.timeScale = 1;

        foreach (var audioClip in audioClips)
        {
            GetComponent<AudioSource>().PlayOneShot(audioClip);
            yield return new WaitForSecondsRealtime(audioClip.length);
            yield return new WaitForSecondsRealtime(0.5f);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        OnLoadNextScene();

        yield return null;
    }

    public void OnLoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
