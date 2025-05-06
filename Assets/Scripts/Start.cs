using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Este método puede vincularse al botón desde el Inspector de Unity
    public void LoadGameScene()
    {
        SceneManager.LoadScene("ChainTrivia"); // Reemplaza "Intro" por el nombre exacto de tu escena
    }
}
