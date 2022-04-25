using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Estados para los capitanes
enum estadosCapitanes { mover, perseguir, atacar, morir }

/// <summary>
/// Clase que controla el comportamiento de los capitanes 
/// </summary>
/// Version 1.0
/// Fecha de creacion 13/04/22
/// Creador Isaac Librado
public class Capitanes : MonoBehaviour {


    // Atributos del personaje
    public float vel;
    public int escudo;
    public int fuerza;
    public int salud;

    // Para la maquina de estados
    public int estado;

    // Para controlar el movimiento en el grafo
    public Grafo mapa;
    private int nodoActual = 0;
    private bool enObjetivo = false;
    Vector3 nodoObjetivo;

    // Para controlar la deteccion de obstaculos
    public float distanciaDeteccion;

    // Para lanzar rayos de deteccion de obstaculos
    private RaycastHit hitD;
    private RaycastHit hitI;

    //Vectores para la deteccion de obstaculos
    private Vector3 ladoDerecho;
    private Vector3 ladoIzquierdo;
    private Vector3 Objetivo;
    private Vector3 direccion;
    private Vector3 puntoI;
    private Vector3 normal;

    private RaycastHit hit;

    public float escala;

    //Referencia al enemigo que se debe atacar
    public Transform enemigo;

    //Para calcular el tiempo entre ataques
    float tiempoAtaque;

    //Game object de textos
    public TextMesh vida;

    public TextMesh ataque;

    //Para el ataque combo nuevo
    int escudoE;
    int ataqueComb = 0;

    // Use this for initialization
    void Start()
    {
        //Inicializamos las variables
        tiempoAtaque = 0;
        estado =(int)estadosCapitanes.mover;

        salud = 343;
        vel = 5;
        escudo = 5;
        fuerza = 6;

        distanciaDeteccion = 4f;
        escala = 1f;

        vida = gameObject.GetComponentsInChildren<TextMesh>()[0];
        ataque = gameObject.GetComponentsInChildren<TextMesh>()[1];

        ataque.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ValidarSalud();

        if (estado == (int)estadosCapitanes.morir)
        {
            Morir();
        }
        if (estado==(int)estadosCapitanes.mover)
        {
            Movimiento();
        }
        if (estado == (int)estadosCapitanes.perseguir)
        {
            Perseguir();
        }
        if (estado == (int)estadosCapitanes.atacar)
        {
            Atacar();
        }

    }

    /// <summary>
    /// Metodo para indicarle al personaje que debe ir a un nuevo punto
    /// </summary>
    /// <param name="pNuevoFin">El nuevo nodo al que debe ir</param>
    /// Version 1.0
    /// Fecha de creacion 13/04/22
    /// Creador Isaac Librado
    public void NuevoPunto(int pNuevoFin)
    {
        //Si estamos en el objetivo indicamos que el inicio va a ser el final
        if (enObjetivo == true)
        {
            mapa.inicio = mapa.final;
        }
 
        //colocamos el final como el nodo inidcado
        mapa.final = pNuevoFin;

        //Calculamos la nueva ruta
        mapa.CalculaRuta();
        
        //Indicamos que el nodo actual es el 0 y que no estamos en el objetivo
        nodoActual = 0;
        enObjetivo = false;

    }

    /// <summary>
    /// Metodo que se ejecuta cuando el estado es morir
    /// </summary>
    public void Morir()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Metodo para validar la salud actual del personaje
    /// </summary>
    public void ValidarSalud()
    {
        //Ponemos el texto de vida
        vida.text = salud.ToString();

        if (salud < 50)
            vida.color = Color.yellow;

        if (salud < 10)
            vida.color = Color.red;

        //Cambiamos de estado a muerto si ya no hay salud
        if (salud <= 0)
            estado = (int)estadosCapitanes.morir;
    }

    /// <summary>
    /// Metodo que se ejecuta cuando el jugador se mueve
    /// </summary>
    public void Movimiento()
    {
        ataque.gameObject.SetActive(false);
        //Si no estamos en el objetivo realizamos el movimiento
        if (enObjetivo == false)
        {
            // Verificamos si hemos llegado al nodo
            nodoObjetivo = new Vector3(mapa.nodosCoords[mapa.ruta[nodoActual]].x, 0.5f, mapa.nodosCoords[mapa.ruta[nodoActual]].z);


            if (Vector3.Magnitude(transform.position - nodoObjetivo) < 1f)
            {
                nodoActual++;
                if (nodoActual == mapa.ruta.Count)
                    enObjetivo = true;
            }

            // Vemos al objetivo
            transform.LookAt(nodoObjetivo);
            transform.Translate(Vector3.forward * Time.deltaTime * vel);

            //Esquivamos
            Esquivar();
        }
    }

    /// <summary>
    /// Metodo para esquivar obstaculos
    /// </summary>
    public void Esquivar()
    {
        //El objetivo sera diferente dependiendo del estado del jugador
        if(estado==(int)estadosCapitanes.mover)
            Objetivo = nodoObjetivo;
        if (estado == (int)estadosCapitanes.perseguir)
            Objetivo = enemigo.transform.position;

        //Hacemos la deteccion de obstaculos
        ladoDerecho = transform.forward + (transform.right * 0.7f);
        ladoIzquierdo = transform.forward - (transform.right * 0.7f);

        if (Physics.Raycast(transform.position, ladoDerecho, out hitD, distanciaDeteccion))
        {
            if (hitD.transform.gameObject.CompareTag("Obstaculo"))
            {
                Objetivo -= transform.right * (distanciaDeteccion / hitD.distance);
            }
        }

        if (Physics.Raycast(transform.position, ladoIzquierdo, out hitI, distanciaDeteccion))
        {
            if (hitI.transform.gameObject.CompareTag("Obstaculo"))
            {
                Objetivo += transform.right * (distanciaDeteccion / hitI.distance);
            }
        }


        if (estado == (int)estadosCapitanes.mover)
            direccion = nodoObjetivo - transform.position;
        if (estado == (int)estadosCapitanes.perseguir)
            direccion = enemigo.transform.position - transform.position;


        if (Physics.Raycast(transform.position, direccion, out hit, 4f))
        {
            if (hit.transform.gameObject.CompareTag("Obstaculo"))
            {
                puntoI = hit.point;
                normal = hit.normal;

                escala = 4f / (puntoI - transform.position).magnitude;

                Objetivo = puntoI + (normal * escala);
                Objetivo.y = transform.position.y;

            }
        }
        
        transform.LookAt(Objetivo);
        transform.Translate(Vector3.forward * vel * Time.deltaTime);
    }

    /// <summary>
    /// Metodo para cuando el enemigo entra al collider del jugador
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        //si el jugador se esta movimiendo
        if(estado==(int)estadosCapitanes.mover)
        {
            //si entra el enemigo
            if (other.gameObject.CompareTag("Enemigo"))
            {
                //indicamos al enemigo y cambiamos de estado
                enemigo = other.gameObject.transform;
                estado = (int)estadosCapitanes.perseguir;
            }
        }
        
    }

    /// <summary>
    /// Metodo para perseguir al enemigo cuando se detecta
    /// </summary>
    void Perseguir()
    {
        //Si el enemigo no es null
        if (enemigo != null)
        {
            //Obtenemos la ubicacion del enemigo como el objetivo y nos dirigimos hacia el
            Objetivo = enemigo.transform.position;
            transform.LookAt(Objetivo);
            transform.Translate(Vector3.forward * vel * Time.deltaTime);

            //Esquivamos los obstaculos
            Esquivar();

            //Obtenemos la distancia entre enemigo y jugador
            Vector3 distancia = enemigo.position - transform.position;

            //Si el jugador se aleja lo quitamos de referencia y cambiamos de estado
            if (distancia.magnitude > 22)
            {
                enemigo = null;
                estado = (int)estadosCapitanes.mover;
            }
            else if (distancia.magnitude < 1.5f)
                estado = (int)estadosCapitanes.atacar;

            //Si el enemigo desaparece lo hacemos nulo
            if (!enemigo.gameObject.activeInHierarchy)
                enemigo = null;
        }
        else
            estado = (int)estadosCapitanes.mover;
    }

    /// <summary>
    /// Metodo para atacar al enemigo
    /// </summary>
    void Atacar()
    {
        //Si el enemigo esta referenciado
        if (enemigo != null)
        {

            //Si el tiempo de ataque es mayor o igual a su fuera
            if (tiempoAtaque >= fuerza)
            {
                ataqueComb = 0;

                //activamos el text de ataque
                ataque.gameObject.SetActive(true);
                ataque.characterSize = 0;

                //Obtenemos la cantidad de escudo del enemigo y le restamos la fuerza del jugador
                escudoE = enemigo.gameObject.GetComponent<Enemigo>().escudo;
                ataqueComb = escudoE - fuerza;

                //Si el ataque combinado es negativo se lo quitamos a la salud del enemigo
                if (ataqueComb < 0)
                    enemigo.gameObject.GetComponent<Enemigo>().salud += ataqueComb;

                ataque.text = ataqueComb.ToString();

                //Reiniciamos el tiempo de ataque
                tiempoAtaque = 0;
            }


            //Aumentamos el tiempo de ataque cada segundo
            tiempoAtaque += 1 * Time.deltaTime;

            //Animacion para el texto
            if (ataque.characterSize >= 0 && ataqueComb < 0)
            {
                if (tiempoAtaque > 2f)
                    ataque.characterSize -= Time.deltaTime * 0.5f;
                else
                    ataque.characterSize += Time.deltaTime * 0.5f;
            }

            //Si el enemigo desaparece lo hacemos nulo
            if (!enemigo.gameObject.activeInHierarchy)
                enemigo = null;
        }
        else
            estado = (int)estadosCapitanes.mover;
    }

}
