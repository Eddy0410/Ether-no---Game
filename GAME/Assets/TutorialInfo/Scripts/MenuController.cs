using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void ModoHistoria()
    {
        // Cargar la escena del modo historia
        SceneManager.LoadScene("ModoHistoria");
    }

    public void Configuracion()
    {
        // Cargar la escena del menú de configuración
        SceneManager.LoadScene("Configuracion");
    }

    public void Salir()
    {
        // Salir del juego
        Application.Quit();
#if UNITY_EDITOR
        // Si estamos en el editor de Unity, detener el juego
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}