using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class ThirdPersonShooterController : MonoBehaviour {

    // Cámara virtual para apuntar
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    // Sensibilidad normal
    [SerializeField] private float normalSensitivity;
    // Sensibilidad al apuntar
    [SerializeField] private float aimSensitivity;
    // Máscara de capas para el colisionador de apuntar
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    // Prefab del proyectil de bala
    [SerializeField] private Transform pfBulletProjectile;
    // Posición de generación del proyectil de bala
    private Transform spawnBulletPosition;
    // Efecto visual al impactar en el objetivo
    [SerializeField] private Transform vfxHitTarget;
    // Referencia al controlador de tercera persona
    private ThirdPersonController thirdPersonController;
    // Referencia a los inputs del jugador
    private StarterAssetsInputs starterAssetsInputs;
    // Referencia al animador
    private Animator animator;
    // Objeto que referencia la particula de disparo.
    public GameObject disparo;
    // Objeto que referencia el gestor de armas
    public GameObject weaponManager;
    // Fuente de audio para el disparo
    public AudioSource audioDisparo;
    // Referencia al script del arma
    private Arma arma;

    /// <summary>
    /// Cambia el arma actual por la nueva arma especificada.
    /// </summary>
    /// <param name="arma">La nueva arma a equipar.</param>
    public void ChangeWeapon(Arma arma)
    {
        // Obtiene el componente Arma de la nueva arma
        this.arma = arma.GetComponent<Arma>();
        // Obtiene el componente AudioSource de la nueva arma
        audioDisparo = arma.GetComponent<AudioSource>();
        // Obtiene la posición de generación del proyectil de bala de la nueva arma
        spawnBulletPosition = arma.gameObject.transform.GetChild(0).transform;
    }

    /// <summary>
    /// Configura el estado del cursor, obtiene referencias a componentes necesarios 
    /// y establece la posición de generación del proyectil de bala.
    /// </summary>
    private void Awake() {
        // Bloquea el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        // Obtiene el componente Arma del arma actualmente equipada
        arma = weaponManager.GetComponentInChildren<Arma>();
        // Obtiene el componente ThirdPersonController
        thirdPersonController = GetComponent<ThirdPersonController>();
        // Obtiene el componente StarterAssetsInputs
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        // Obtiene el componente Animator
        animator = GetComponent<Animator>();
        // Obtiene la posición de generación del proyectil de bala del arma actual
        spawnBulletPosition = arma.gameObject.transform.GetChild(0).transform;
    }

    /// <summary>
    /// Metodo encargado de gestionar el apuntado y disparo del arma
    /// </summary>
    private void Update() {

        // Si se ha pulsado el botón de disparar pero no se está apuntando,
        // cambia el valor del botón de disparar a falso.
        if (!starterAssetsInputs.aim && starterAssetsInputs.shoot) starterAssetsInputs.shoot = false;

       if (starterAssetsInputs.aim) {
            // Establece el parámetro "aimingPistol" en el animador según el tipo de arma equipada
            animator.SetBool("aimingPistol", arma.tipoArma == Arma.Tipos.Pistola);
            // Establece el parámetro "aimingRifle" en el animador según el tipo de arma equipada
            animator.SetBool("aimingRifle", arma.tipoArma == Arma.Tipos.Rifle);

            // Variable que almacena la posición en el mundo del ratón
            Vector3 mouseWorldPosition = Vector3.zero;

            // Punto central de la pantalla en coordenadas 2D
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

            // Rayo que parte desde la cámara y apunta hacia el punto central de la pantalla
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            // Transform que representa el objeto alcanzado por el rayo
            Transform hitTransform = null;

            // Realiza un lanzamiento de rayo y comprueba si colisiona con un objeto de la capa de aim
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
            {
                // La posición del ratón en el mundo se actualiza con el punto de colisión del rayo
                mouseWorldPosition = raycastHit.point;
                // El objeto alcanzado por el rayo se asigna al Transform 
                hitTransform = raycastHit.transform;
            }
            // Si el rayo no colisiona con ningún objeto, la posición del ratón en el mundo
            // se establece en un punto a una distancia de 10 unidades
            else { mouseWorldPosition = ray.GetPoint(10); }

            // Activa la cámara virtual
            aimVirtualCamera.gameObject.SetActive(true);

            // Establece la sensibilidad de rotación del controlador de tercera persona
            // al valor de sensibilidad de apuntado
            thirdPersonController.SetSensitivity(aimSensitivity);
            // Desactiva la rotación automática al moverse
            thirdPersonController.SetRotateOnMove(false);

            // Calcula la posición objetivo de apuntado en el mundo,
            // manteniendo la misma altura del objeto en el que se encuentra el script
            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;

            // Calcula la dirección de apuntado normalizada hacia el objetivo
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            // Interpola suavemente la dirección actual del objeto hacia la dirección de apuntado
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);

            // Verifica si el "calor" del arma es mayor a 0
            // (esta caliente, no puede disparar)
            if (arma.gunHeat > 0)
            {
                //Reduce el valor segun el tiempo que está pasando.
                arma.gunHeat -= Time.deltaTime;
            }

            // Verifica si se ha presionado el botón de disparo
            // y el calor del arma es menor o igual a cero
            if (starterAssetsInputs.shoot && arma.gunHeat <= 0)
            {
                // Establece el calor del arma al tiempo entre disparos configurado en el arma
                arma.gunHeat = arma.tiempoEntreDisparos;

                // Reproduce el sonido de disparo
                audioDisparo.Play();
                // Establece el tiempo de finalización programado del sonido del disparo
                audioDisparo.SetScheduledEndTime(AudioSettings.dspTime + (0.5f));

                // Calcula la dirección de apuntado normalizada hacia la posición de generación de la bala
                Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;

                // Instancia la partícula de disparo en la posición de generación de la bala
                // y con la dirección de apuntado
                var disparoparticle = 
                    Instantiate(disparo, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));

                // Destruye la partícula de disparo después de 0.1 segundos
                Destroy(disparoparticle,0.1f);

                // Instancia el proyectil de bala en la posición de generación de la bala
                // y con la dirección de apuntado
                var bullet = Instantiate(pfBulletProjectile, spawnBulletPosition.position, 
                    Quaternion.LookRotation(aimDir, Vector3.up));

                // Restablece el estado de la variable de disparo en false
                starterAssetsInputs.shoot = false;
            }
        } else {
            // Si el tipo de arma es una pistola,
            // establece el estado de animación de apuntado de pistola en falso
            if (arma.tipoArma == Arma.Tipos.Pistola) animator.SetBool("aimingPistol", false);

            // Si el tipo de arma es un rifle,
            // establece el estado de animación de apuntado de rifle en falso
            else animator.SetBool("aimingRifle", false);

            // Desactiva la cámara virtual de apuntado
            aimVirtualCamera.gameObject.SetActive(false);

            // Restablece la sensibilidad del controlador de tercera persona a la sensibilidad normal
            thirdPersonController.SetSensitivity(normalSensitivity);

            // Habilita la rotación automática del objeto cuando se mueve
            thirdPersonController.SetRotateOnMove(true);
            //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        }

       
    }

    public void SetpfBulletProjectile(Transform bullet)
    {
        this.pfBulletProjectile = bullet;
    }

}