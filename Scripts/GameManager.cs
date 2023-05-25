using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using System.Linq;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    //Número de enemigos derrotados
    public int defeatedEnemies = 0;
    //Ememigos vivos
    public int enemiesAlive = 0;
    //Número de ronda
    public int round = 0;
    //Menú de pausa
    public GameObject pauseMenu;
    //Hud
    public GameObject hud;
    //Texto de las rondas
    public TextMeshProUGUI roundNum;
    //Texto de los enemigos
    public TextMeshProUGUI numEnemigos;
    //Menu de victoria
    public GameObject winScreen;
    //Menu de derrota
    public GameObject loseScreen;
    //Panel de recibir daño
    public GameObject hurtPanel;
    //imagen de fondo
    public GameObject fondo;
    //camara que persigue al jugador
    public GameObject followCam;
    //camara que se encarga del apuntado
    public GameObject aimCam;
    //jugador
    public PlayerManager player;
    //indica si el juego está en pausa
    public bool isPause = false;
    //indica si se ha ganado
    bool win = false;
    //indica si se ha perdido
    bool lose = false;
    // Start is called before the first frame update
    void Start()
    {
        //Asigna la escala de tiempo a 1
        Time.timeScale = 1.0f;
        //Instancia los enemigos derrotados a 0
		defeatedEnemies= 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Si el juego está en pausa, si se ha ganado o perdido --> No se bloquea el ratón
        if(isPause || win || lose) Cursor.lockState = CursorLockMode.None;

        //si el número de enemigos vivos es 0
        if (enemiesAlive == 0)
        {
            //aumenta el número de ronda
            round++;
        }

        //Si se pulsa el escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Si no se está en pausa se llama a la función Pause
            if(!isPause)Pause();
        }

    }

    /// <summary>
    /// Función encargada de pausar el juego
    /// </summary>
    public void Pause()
    {
        //indica que se ha pausado
        isPause = true;
        //se deshabilita el hud
        hud.SetActive(false);
        //se activa el menu de pausa
        pauseMenu.SetActive(true);
        //se desactiva la camara de apuntado
        aimCam.SetActive(false);
        //se para el timepo
        Time.timeScale = 0;
        //desbloquea el cursor
        Cursor.lockState = CursorLockMode.None;
        //cambia el volumen a 0
        AudioListener.volume = 0;
    }

    /// <summary>
    /// Función encargada de despaurar el juego
    /// </summary>
    public void UnPause()
    {
        //indica que ya no está en pausa
        isPause = false;
        //desactiva el menú de pausa
        pauseMenu.SetActive(false);
        //activa el hud
        hud.SetActive(true);
        //activa la camara de apuntado
        aimCam.SetActive(true);
        //restaura el tiempo
        Time.timeScale = 1;
        //bloquea el cursor
        Cursor.lockState = CursorLockMode.Locked;
        //cambia el volumen a 1
        AudioListener.volume = 1;
    }

    /// <summary>
    /// Si el jugador pierde
    /// </summary>
    public void isDeath()
    {
        //Pausa el tiempo
        Time.timeScale = 0;
        //indica que ha perdido
        lose = true;
        //desbloquea el ratón
        Cursor.lockState = CursorLockMode.None;
        //deshabilita el raton
        hud.SetActive(false);
        //deshabilita el menu de recibir daño
        hurtPanel.SetActive(false);
        //deshabilita el menu de victoria
        winScreen.SetActive(false);
        //habilita el fondo
        fondo.SetActive(true);
        //habilita el menú de victoria
        loseScreen.SetActive(true);
        //obtiene el componente text del objeto numero de rondas y cambia su contenido al numero de rondas que ha sobrevivido.
        loseScreen.transform.Find("NumeroRondas").GetComponent<TextMeshProUGUI>().text = 
            $"Has sobrevivido {int.Parse(roundNum.text)} rondas.";
        //obtiene el componente text del objeto que muestra la dificultad y cambia su texto al que nos devuelve el singleton de dificultad
        loseScreen.transform.Find("Dificult").GetComponent<TextMeshProUGUI>().text = 
            $"Modo: {DifficultyManager.instance.toString(DifficultyManager.instance.dificultadActual)}";
    }

    /// <summary>
    /// Función que se ejecuta cuando el jugador gana
    /// </summary>
    public void isSurvived()
    {
        //pausa el tiempo
        Time.timeScale = 0;
        //indica que ha ganado
        win = true;
        //desbloquea el cursor
        Cursor.lockState = CursorLockMode.None;
        //desactiva el cursor
        hud.SetActive(false);
        //desactiva el panel de recibir daño
        hurtPanel.SetActive(false);
        //activa el fondo
        fondo.SetActive(true);
        //activa el menu de victoria
        winScreen.SetActive(true);
    }

    /// <summary>
    /// Actualiza el numero de enemigos del hud
    /// </summary>
    /// <param name="enemigos">número de enemigos</param>
    public void ActualizarNumeroEnemigos(int enemigos)
    {
        numEnemigos.text = enemigos.ToString();
    }

    /// <summary>
    /// Actualiza el numero de rondas del hud
    /// </summary>
    /// <param name="rondas">número de rondas</param>
    public void ActualizarRondas(int rondas)
    {
        roundNum.text = rondas.ToString();
    }

    /// <summary>
    /// Reduce el numero de enemigos del Hud
    /// </summary>
    public void ReducirNumeroEnemigos()
    {
        //numero de enemigos obtenido de la interfaz
        int enemigos = (int.Parse(numEnemigos.text) - 1);

        //si el numero de enemigos es menor a 0 lo pone en 0
        if (enemigos <= 0) enemigos = 0;

        //actualiza el numero de enemigos
        numEnemigos.text = enemigos.ToString();
    }
}
