using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
public class EnemyManager : MonoBehaviour
{
    public PlayerManager player;  // Referencia al objeto del jugador
    private Animator enemyAnimator;  // Controlador de animaciones del enemigo
    private NavMeshAgent navMeshAgent;  // Agente de navegación del enemigo
    public float damage = 20f;  // Daño que inflige el enemigo
    public float health = 100f;  // Salud total del enemigo
    public float healthAcT = 100f;  // Salud actual del enemigo
    public GameManager gameManager;  // Referencia al administrador del juego
    public GameObject shieldToSpawn;  // Objeto que se spawnea al derrotar al enemigo
    public bool playerInReach;  // Indicador de si el jugador está al alcance del enemigo
    public float attackDelayTimer;  // Temporizador para retrasar los ataques del enemigo
    public float attackAnimStartDelay;  // Retraso antes de iniciar la animación de ataque
    public float delayBetweenAttacks;  // Retraso entre los ataques del enemigo
    public float attackDistance = 2f;  // Distancia a la que el enemigo ataca al jugador
    private string runAnimationName = "isRunning";  // Nombre de la animación de correr
    private string attackAnimationName = "isAttacking";  // Nombre de la animación de ataque
    private string deathAnimationName = "isDead";  // Nombre de la animación de muerte
    public float distanceToPlayer;  // Distancia entre el enemigo y el jugador
    public AudioSource audioSource;  // Fuente de audio para los sonidos del enemigo
    public AudioClip[] zombieSounds;  // Sonidos del enemigo (array de clips de audio)
    public TextMeshProUGUI puntos;  // Referencia al texto que muestra los puntos
    private bool isDead = false;  // Indicador de si el enemigo está muerto
    public AudioClip zombieAtaca;  // Sonido de ataque del enemigo
    public AudioClip zombieDamage;  // Sonido de daño recibido por el enemigo

    public float probabilidadSpawnEscudos = 0.1f; //Probabilidad de hacer spawn de un escudo
    public int puntosEnemigos = 100; //Puntos que da el enemigo

    // Start is called before the first frame update
    void Start()
    {
        if (!tag.Equals("Boss"))
        {
            health = DifficultyManager.instance.vidaZombies;
            healthAcT = DifficultyManager.instance.vidaZombies;
        }
        damage = DifficultyManager.instance.danoZombies;
        puntosEnemigos = DifficultyManager.instance.obtencionPuntos;
        puntos = GameObject.FindGameObjectsWithTag("puntos")[0].GetComponent<TextMeshProUGUI>();
        // Busca el primer objeto con la etiqueta "puntos" y obtiene el componente TextMeshProUGUI para asignarlo a la variable "puntos"

        enemyAnimator = GetComponent<Animator>();
        // Obtiene el componente Animator adjunto a este objeto y lo asigna a la variable "enemyAnimator"

        player = FindObjectOfType<PlayerManager>();
        // Encuentra el objeto que tiene el componente PlayerManager adjunto y lo asigna a la variable "player"

        gameManager = FindObjectOfType<GameManager>();
        // Encuentra el objeto que tiene el componente GameManager adjunto y lo asigna a la variable "gameManager"

        navMeshAgent = GetComponent<NavMeshAgent>();
        // Obtiene el componente NavMeshAgent adjunto a este objeto y lo asigna a la variable "navMeshAgent"

        navMeshAgent.stoppingDistance = attackDistance;
        // Establece la distancia de parada del NavMeshAgent al valor de la variable "attackDistance"
    }

    // Update is called once per frame
    void Update()
    {
        //Si está muerto, no se hace nada.
        if (isDead) return;
        // Verificar si la salud del enemigo es igual o menor a cero y si aún no está muerto
        if (healthAcT <= 0 && !isDead)
        {
            navMeshAgent.enabled = false;
            isDead = true;  // Marcar al enemigo como muerto
            enemyAnimator.SetBool(deathAnimationName, true);  // Activar la animación de muerte

            puntos.text = (int.Parse(puntos.text) + puntosEnemigos).ToString();  // Actualizar el puntaje sumando la variable puntos enemigos
            gameManager.defeatedEnemies += 1;  // Incrementar el contador de enemigos derrotados en el GameManager

            gameManager.ReducirNumeroEnemigos();

            //Si el valor devuelto por Random.value es menor o igual a la probabilidad de soltar escudos
            if (Random.value <= probabilidadSpawnEscudos)
            {
                // Instancia el objeto aquí
                Instantiate(shieldToSpawn, transform.position + Vector3.up*1.5f, transform.rotation);
            }
            if (tag.Equals("Boss")) Invoke("Win", 1f);
            Invoke("DisappearEnemy", 3f);  // Llamar a la función DisappearEnemy después de 3 segundos
        }
        else
        {
            // Si el jugador no es nulo
            if (player != null)
            {
                distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);  // Calcular la distancia entre el enemigo y el jugador

                // Si la distancia al jugador es menor o igual a la distancia de ataque
                if (distanceToPlayer <= attackDistance)
                {
                    navMeshAgent.isStopped = true;  // Detener el movimiento del enemigo

                    attackDelayTimer += Time.deltaTime;  // Incrementar el temporizador de retraso entre ataques

                    // Si el temporizador de retraso entre ataques alcanza el valor necesario
                    if (attackDelayTimer >= delayBetweenAttacks)
                    {
                        AttackPlayer();  // Realizar el ataque al jugador
                        attackDelayTimer = 0f;  // Reiniciar el temporizador de retraso entre ataques
                    }
                }
                else
                {
                    enemyAnimator.SetBool(attackAnimationName, false);  // Desactivar la animación de ataque
                    enemyAnimator.SetBool(runAnimationName, true);  // Activar la animación de correr
                    navMeshAgent.isStopped = false;  // Continuar el movimiento del enemigo
                    navMeshAgent.SetDestination(player.transform.position);  // Establecer la posición de destino del enemigo como la posición actual del jugador
                }
            }
        }
    }

    public void Win()
    {
        gameManager.isSurvived();
    }

    private void DisappearEnemy()
    {
        // Código para hacer que el enemigo desaparezca
        Destroy(gameObject);
        //gameObject.SetActive(false);
    }
    /// <summary>
    /// Inicia la animación y sonido de ataque
    /// </summary>
    void AttackPlayer()
    {
        enemyAnimator.SetBool(attackAnimationName, true);  // Activar la animación de ataque del enemigo
          
    }

    /// <summary>
    /// Función que se ejecuta al terminar la aniamción de atacar. Si el jugador esta en la distancia le hace daño
    /// </summary>
    public void HitPlayer()
    {
        // Reproducir el sonido de ataque del enemigo
        AudioSource.PlayClipAtPoint(zombieAtaca, transform.position, AudioManager.instance.GetSoundVolume());

        //Calcula la distancia entre el enemigo y el jugador
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        //Si la distancia es menor a la distancia de ataque, llama a la función de recibir daño del jugador.
        if (distanceToPlayer <= attackDistance)
        {
            player.Hit(damage);
        }

    }

    /// <summary>
    /// Función que compara si el enemigo ha recibido un disparo. Si lo ha recibido toma el valor del daño del arma y se lo resta a la vida del zombie.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        /*if (other.CompareTag("Bullet"))
        {
            healthAcT = healthAcT - GameObject.FindGameObjectWithTag("WeaponManager").transform.GetChild(0).GetComponent<Arma>().ArmaDMG ;  // Reducir la salud actual del enemigo al recibir un impacto de una bala
            AudioSource.PlayClipAtPoint(zombieDamage, transform.position, AudioManager.instance.GetSoundVolume());  // Reproducir el sonido de daño del enemigo al recibir un impacto
        }*/
    }

    public void getHit()
    {
        // Reducir la salud actual del enemigo al recibir un impacto de una bala
        healthAcT = healthAcT - GameObject.FindGameObjectWithTag("WeaponManager")
            .transform.GetChild(0).GetComponent<Arma>().ArmaDMG;
        // Reproducir el sonido de daño del enemigo al recibir un impacto
        AudioSource.PlayClipAtPoint(zombieDamage, transform.position, AudioManager.instance.GetSoundVolume());  
    }


}
