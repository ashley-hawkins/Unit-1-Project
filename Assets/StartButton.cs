using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    UnityEngine.UI.Button b;
    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<UnityEngine.UI.Button>();
    }

    public void LoadTheMainGame()
    {
        print("a");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
