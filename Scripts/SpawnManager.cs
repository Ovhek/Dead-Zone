using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] spawnPoints; // Array de puntos de aparición de enemigos
    public GameObject[] enemies; // Array de enemigos disponibles
    public int enemigosPorRonda; // Cantidad de rondas por nivel
    public int ronda; // Ronda actual
    public int enemyType; // Tipo de enemigo a spawnear
    public bool spawning; // Indicador de si se está spawneando enemigos
    public int enemiesSpawned; // Cantidad de enemigos spawneados
    private GameManager gameManager; // Referencia al GameManager
    public bool rondaBoos; // Indicador de si es la ronda del jefe
    public int maximoRondas = 6; //Maximo de rondas

    // Start is called before the first frame update
    void Start()
    {
        enemigosPorRonda = DifficultyManager.instance.enemigosPorRonda;
        ronda = 0; // Inicializa la ronda actual
        spawning = false; // Inicializa el indicador de spawneo en falso
        enemiesSpawned = 0; // Inicializa la cantidad de enemigos spawneados en cero
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>(); // Obtiene la referencia al GameManager
        gameManager.ActualizarNumeroEnemigos(enemigosPorRonda);
        rondaBoos = false; // Inicializa el indicador de ronda del jefe en falso
    }

    // Update is called once per frame
    void Update()
    {
        // Si no se está spawneando, la cantidad de enemigos spawneados es menor o igual a la cantidad de enemigos derrotados
        // y la ronda actual es menor a 6
        if (spawning == false && enemiesSpawned <= gameManager.defeatedEnemies && ronda < maximoRondas)
        {

            StartCoroutine(SpawnWave(enemigosPorRonda)); // Inicia la corrutina para spawnear una oleada de enemigos
        }
        // Si no se está spawneando, la cantidad de enemigos spawneados es menor o igual a la cantidad de enemigos derrotados,
        // la ronda actual es igual a 6 y no es la ronda del jefe
        else if (spawning == false && enemiesSpawned <= gameManager.defeatedEnemies && ronda == maximoRondas && !rondaBoos)
        {
            SpawnBossEnemy(); // Spawnea al jefe
            rondaBoos = true; // Marca que es la ronda del jefe
        }
    }

    IEnumerator SpawnWave(int waveC)
    {
        gameManager.ActualizarRondas(ronda+1);
        gameManager.ActualizarNumeroEnemigos(enemigosPorRonda);
        spawning = true; // Marca que se está spawneando
        // Activar texto de nueva ronda
        yield return new WaitForSeconds(4);
        // Desactiva texto de nueva ronda
        for (int i = 0; i < waveC; i++)
        {
            SpawnEnemy(); // Spawnea un enemigo
            yield return new WaitForSeconds(2);
        }
        ronda += 1; // Incrementa la ronda actual
        enemigosPorRonda += 2; // Incrementa la cantidad de enemigos por ronda
        spawning = false; // Marca que se ha terminado el spawneo

        yield break;
    }

    void SpawnBossEnemy()
    {
        gameManager.ActualizarRondas(ronda + 1);
        gameManager.ActualizarNumeroEnemigos(1);
        int spawnPos = 1; // Posición de aparición del jefe
        enemyType = 4;
            Instantiate(enemies[enemyType], spawnPoints[spawnPos].transform.position, spawnPoints[spawnPos].transform.rotation);
            enemiesSpawned+=1;
    }

    void SpawnEnemy()
    {
        int spawnPos = Random.Range(0, 4); // Genera un número aleatorio entre 0 y 3 para determinar la posición de aparición
        enemyType = Random.Range(0, 3); // Genera un número aleatorio entre 0 y 2 para determinar el tipo de enemigo

        Instantiate(enemies[enemyType], spawnPoints[spawnPos].transform.position, spawnPoints[spawnPos].transform.rotation);
        enemiesSpawned += 1; // Incrementa la cantidad de enemigos generados
    }


}
