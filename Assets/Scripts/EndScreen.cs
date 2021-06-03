using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    private Button myButton;
    private Event buttonClickEvent;
    // Start is called before the first frame update
    void Start()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(Respawn);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void Respawn()
    {
        SceneManager.LoadScene(0);
    }
}
