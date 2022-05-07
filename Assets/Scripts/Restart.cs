using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    private void OnMouseDown()
    {
        string game = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(game);
    }
}
