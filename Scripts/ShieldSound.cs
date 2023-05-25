using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSound : MonoBehaviour
{
    //Audio de recoger el escudo
    public AudioClip recogerEscudo;

    //Valor de la rotación
    public Vector3 valorRotacion = new Vector3(50,50,50);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Rota el objeto según el vector multiplicado por el valor de deltatime.
        transform.Rotate(valorRotacion * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Si el objeto colisiona con otro objeto, reproduce el sonido, destruye el objeto y llama a la función del jugador de obtener un escudo.
        if(other.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(recogerEscudo, transform.position, AudioManager.instance.GetSoundVolume());
            Destroy(gameObject);

            FindObjectOfType<PlayerManager>().obtenerEscudo();
        }
        
    }
}
