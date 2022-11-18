using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameoverScreen : MonoBehaviour
{
    public TextMeshProUGUI headerTextMeshPro;
    public static string headerText;
    // Start is called before the first frame update
    void Start()
    {
        headerTextMeshPro.text = headerText;
    }

    public void GoToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        print("exit");
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
