using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject background;
    float step = 0.1f;
    public float border = 150f; 
    public float speed = 1f;

    public void Update()
    {
        AnimateBackground();
    }

    void AnimateBackground()
    {

        background.transform.position += new Vector3(step * speed, 0, 0);
        if (Mathf.Abs(background.transform.position.x - transform.position.x) >= Mathf.Abs(border)) { step *= -1; }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
