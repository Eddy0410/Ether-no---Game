using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuInicial : MonoBehaviour
{
    public void jugar(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+ 1);
       
    }
    public void Salir(){
        Debug.Log("Salir...");
        Application.Quit();

    }
}
