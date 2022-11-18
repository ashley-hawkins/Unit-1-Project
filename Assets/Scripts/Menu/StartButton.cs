using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    UnityEngine.UI.Button b;
    public GameObject[] objectsToDisable;
    public GameObject[] objectsToEnable;

    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<UnityEngine.UI.Button>();
    }

    public void LoadTheMainGame()
    {
        foreach (GameObject obj in objectsToDisable)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
