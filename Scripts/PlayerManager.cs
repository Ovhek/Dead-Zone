using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;
using System.Drawing;
using UnityEngine.UI;
using System.Linq.Expressions;
using Unity.VisualScripting;

public class PlayerManager : MonoBehaviour
{
    /// Valor de salud al inicio del juego.
    public float initialHealth = 100;
    /// Salud restante del jugador.
    public float remainingHealth;
    /// Número de escudos del jugador.
    public int escudos = 3;
    /// Sonido al recibir daño.
    public AudioClip recibirDano;
    /// Indica si el jugador ha muerto.
    public bool death = false;
    /// Barra de escudos para mostrar en la interfaz.
    public Slider escudoBar;
    /// Barra de salud para mostrar en la interfaz.
    public Slider healthBar;
    /// Script encargado del manejo del juego.
    public GameManager gameManager;
    /// Cámara del jugador.
    public GameObject playerCamera;
    /// Panel de daño para efecto visual.
    public CanvasGroup hurtPanel;
    /// Tiempo de sacudida de la camara
    private float shakeTime;
    /// Duración de la sacudida de la camara
    private float shakeDuration;
    /// Referencia al script que se encarga de los inputs del jugador.
    public StarterAssetsInputs starterAssetsInputs;
    /// Rotación original de la camara antes del efecto de sacudida
    private Quaternion playerCameraOriginalRotation;
    /// GameObject de la caja de armas.
    public GameObject box;
    /// Distancia minima para interactuar con la caja
    public float minDistanceToBox = 2f;
    // indica si el jugador está seleccionando un arma nueva.
    public bool pickingWeapon = false;

    /// <summary>
    /// Método encargado de inicializar las variables.
    /// </summary>
    void Start()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        remainingHealth = initialHealth;
        playerCameraOriginalRotation = playerCamera.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // si la opacidad del panel de recibir daño es mayor a 0 se va descontando el tiempo que pasa.
        if (hurtPanel.alpha > 0)
        {
            hurtPanel.alpha -= Time.deltaTime;
        }

        //Si el tiempo de sacudida es menor a la duración de la sacudida
        //se suma al tiempo de sacudida el tiempo que está pasando y se llama a la función para sacudir la camara.
        if (shakeTime < shakeDuration)
        {
            shakeTime += Time.deltaTime;
            CameraShake();
        }

        //Obtenemos el script de la caja del gameobject de la caja.
        MysteryBoxScript boxScript = box.GetComponent<MysteryBoxScript>();

        //si el usuario ha pulsado el botón de seleccionar el arma, la distancia a la caja es menor a la distancia minima,
        //la caja tiene un arma seleccionada y no se está seleccionando un arma por parte del jugaador.
        if (starterAssetsInputs.pickweapon 
            && Vector3.Distance(transform.position,box.transform.position) <= minDistanceToBox 
            && boxScript.weaponIsSelected && !pickingWeapon)
        {
            //Marcamos que el jugador está seleccionando un arma
            pickingWeapon = true;
            //Indicamos que el botón de seleccionar arma ya no está pulsado.
            starterAssetsInputs.pickweapon = false;

            //Obtenemos el objeto con el tah WeaponManager
            GameObject parentObj = GameObject.FindGameObjectWithTag("WeaponManager");

            // Si el objeto no es nulo
            if (parentObj != null)
            {
                // Eliminamos el primer hijo del WeaponManager
                var weapon = parentObj.transform.GetChild(0).gameObject;

                Destroy(weapon);

                //Obtenemos el nuevo arma e instanciamos una copia de la misma.
                var newWeapon = boxScript.guns[boxScript.selectedWeapon];
                GameObject newChild = Instantiate(newWeapon, parentObj.transform);

                //Asignamos a la copia los valores de posición y rotación local iguales a los del arma anterior.
                newChild.transform.localPosition = weapon.transform.localPosition;
                newChild.transform.localRotation = weapon.transform.localRotation;
                newChild.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                //Indicamos que el jugador ya no está seleccionando arma.
                pickingWeapon = false;

                //Llamamos a la función de cambiar arma del script ThirsPersonShooterController
                GetComponent<ThirdPersonShooterController>().ChangeWeapon(newChild.GetComponent<Arma>());
            }

        }
    }

    /// <summary>
    /// Recibe un daño y reduce la salud o escudos del jugador.
    /// </summary>
    /// <param name="daño">Cantidad de daño recibido.</param>
    public void Hit(float damage)
    {
        //Si el jugador no está muerto, reproduce sonido de recibir daño.
        if(!death) AudioSource.PlayClipAtPoint(recibirDano, transform.position);

        //Si los escudos son mayores a 0, reduce los escudos y actualiza el HUD. No reduce la vida.
        if (escudos > 0)
        {
            escudos--;
            UpdateEscudosUI(escudos);
            return;
        }
        
        //reduce la vida del jugador
        healthBar.value -= (healthBar.maxValue / initialHealth) * damage;
        remainingHealth -= damage;

        //si la vida del jugador es menor o igual a 0 y no está muerto. Lo mata
        if (remainingHealth <= 0 && !death)
        {
            //indica que está muerto
            death = true;
            //Llama a la función Die del ThirdPersonController
            GetComponent<ThirdPersonController>().Die();
            //Invoca el menu de derrota a los 3 segundos
            Invoke("callLoseMenu", 3f);
            
        }
        //Si la vida del jugador es mayor a 0 cambia las variables de sacudida para que se produzca una sacudida en la pantalla.
        else
        {
            shakeTime = 0;
            shakeDuration = .2f;
            hurtPanel.alpha = .7f;
        }
    }

    /// <summary>
    /// Llama a la función isDeath del gamemanager que se encarga de mostrar el menú de derrota.
    /// </summary>
    public void callLoseMenu()
    {
        gameManager.isDeath();
    }

    /// <summary>
    /// Actualiza el slider de los escudos
    /// </summary>
    /// <param name="escudos"></param>
    private void UpdateEscudosUI(int escudos)
    {
        //_ --> Valor que no se usará (lo descarta el compilador)
        // Dependiendo del valor del escudo se le asigna un valor al slider.
        _ = escudos switch
        {
            0 => escudoBar.value = 0f,
            1 => escudoBar.value = 0.3f,
            2 => escudoBar.value = 0.7f,
            3 => escudoBar.value = 1f,
            _ => escudoBar.value = 0
        };
    }

    /// <summary>
    /// Sacude la camara.
    /// </summary>
    public void CameraShake()
    {
        playerCamera.transform.localRotation = Quaternion.Euler(Random.Range(-2, 2), 0, 0);
    }

    /// <summary>
    /// Función que se ejecuta al obtener un escudo. 
    /// Si los escudos son 3 no hace nada si no, aumenta los escudos y actualiza la interfaz.
    /// </summary>
    public void obtenerEscudo()
    {
        if (escudos == 3) return;

        if (escudos < 3) escudos++;
        UpdateEscudosUI(escudos);
    }

}
