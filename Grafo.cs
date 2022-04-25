using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estructura para los nodos y sus datos internos
/// </summary>
/// Version 1.0
/// Fecha de creacion 13/04/22
/// Creador Isaac Librado
public struct NodoGrafo
{
    public float x;
    public float z;

    public NodoGrafo(float pX, float pZ)
    {
        x = pX;
        z = pZ;
    }
}

/// <summary>
/// Clase que controla al grafo por el cual los personajes se pueden mover
/// </summary>
/// Version 1.0
/// Fecha de creacion 13/04/22
/// Creador Isaac Librado
public class Grafo : MonoBehaviour
{
    //puntos de inicio y final
    public int inicio = 0;
    public int final = 0;

    //ruta para el movimiento
    public List<int> ruta;

    //referencia al personaje para colocarlo en el inicio
    public GameObject personaje;

    //atributos para la generacion de ruta
    private int[,] mAdyacencia;
    private int[] indegree;
    private int cantNodos;
    public NodoGrafo[] nodosCoords;
    private bool inicializado = false;
    
    //todos los nodos que conforman el grafo
    public GameObject[] nodos;

    // Use this for initialization
    void Start()
    {
        //obtenemos la cantidad de nodos
        cantNodos = nodos.Length;

        // Instanciamos matriz de adyacencia
        mAdyacencia = new int[cantNodos, cantNodos];

        // Instanciamos arreglo de indegree
        indegree = new int[cantNodos];

        //  Instanciamos el arreglo con las coordenadas de nodos
        nodosCoords = new NodoGrafo[cantNodos];

        // Instanciamos la ruta
        ruta = new List<int>();

        // Agregamos todas las aristas del grafo
        AdicionaArista(0, 1);
        AdicionaArista(0, 2);
        AdicionaArista(1, 3);
        AdicionaArista(2, 3);
        AdicionaArista(3, 4);
        AdicionaArista(3, 6);
        AdicionaArista(4, 5);
        AdicionaArista(5, 6);
        AdicionaArista(5, 26);
        AdicionaArista(6, 7);
        AdicionaArista(7, 8);
        AdicionaArista(8, 9);
        AdicionaArista(8, 10);
        AdicionaArista(9, 11);
        AdicionaArista(10, 11);
        AdicionaArista(10, 12);
        AdicionaArista(12, 13);
        AdicionaArista(13, 14);
        AdicionaArista(13, 16);
        AdicionaArista(14, 15);
        AdicionaArista(14, 17);
        AdicionaArista(15, 16);
        AdicionaArista(17, 18);
        AdicionaArista(18, 19);
        AdicionaArista(18, 21);
        AdicionaArista(19, 20);
        AdicionaArista(19, 25);
        AdicionaArista(20, 21);
        AdicionaArista(21, 22);
        AdicionaArista(21, 23);
        AdicionaArista(22, 24);
        AdicionaArista(23, 24);
        AdicionaArista(25, 29);
        AdicionaArista(26, 27);
        AdicionaArista(27, 28);
        AdicionaArista(27, 30);
        AdicionaArista(28, 29);
        AdicionaArista(28, 31);
        AdicionaArista(29, 30);
        AdicionaArista(31, 35);
        AdicionaArista(32, 33);
        AdicionaArista(32, 35);
        AdicionaArista(33, 34);
        AdicionaArista(34, 35);

        //Adicionamos las coordenadas de todos los nodos
        foreach (GameObject nodo in nodos)
        {
            AdicionaCoords(int.Parse(nodo.name), nodo.transform.position.x, nodo.transform.position.z);
        }

        //inicializamos
        inicializado = true;

        //colocamos al personaje
        personaje.transform.position = new Vector3(nodosCoords[inicio].x, 0.5f, nodosCoords[inicio].z);

        //calculamos la ruta
        CalculaRuta();
    }

    private void OnDisable()
    {
        inicializado = false;
    }

    /// <summary>
    /// Metodo para adicionar el arista al grafo
    /// </summary>
    /// <param name="pNodoInicio">El primer nodo de la arista</param>
    /// <param name="pNodoFinal">El segundo nodo de la arista</param>
    public void AdicionaArista(int pNodoInicio, int pNodoFinal)
    {
        mAdyacencia[pNodoInicio, pNodoFinal] = 1;

        // Solo colocar esta linea si la arista se muve en ambas direcciones
        mAdyacencia[pNodoFinal, pNodoInicio] = 1;

    }

    /// <summary>
    /// Metodo para adicionar las coordenadas de los nodos
    /// </summary>
    /// <param name="pNodo">El numero del nodo</param>
    /// <param name="pX">Ubicacion en x</param>
    /// <param name="pZ">Ubicacion en Z</param>
    public void AdicionaCoords(int pNodo, float pX, float pZ)
    {
        nodosCoords[pNodo] = new NodoGrafo(pX, pZ);
    }

    /// <summary>
    /// Obtener Adyacencia
    /// </summary>
    /// <param name="pFila"></param>
    /// <param name="pColumna"></param>
    /// <returns>la adyacencia</returns>
    public int ObtenAdyacencia(int pFila, int pColumna)
    {
        return mAdyacencia[pFila, pColumna];
    }

    /// <summary>
    /// Metodo para calcular el grado del nodo
    /// </summary>
    public void CalcularIndegree()
    {
        int n = 0;
        int m = 0;

        for (n = 0; n < cantNodos; n++)
        {
            for (m = 0; m < cantNodos; m++)
            {
                if (mAdyacencia[m, n] == 1)
                    indegree[n]++;
            }
        }
    }

    /// <summary>
    /// Metodo para encontrar el IO
    /// </summary>
    /// <returns></returns>
    public int EncuentraI0()
    {
        bool encontrado = false;
        int n = 0;

        for (n = 0; n < cantNodos; n++)
        {
            if (indegree[n] == 0)
            {
                encontrado = true;
                break;
            }
        }

        if (encontrado)
            return n;
        else
            return -1; // Codigo que indica que no se ha encontrado

    }

    /// <summary>
    /// Metodo para decrementar el grado
    /// </summary>
    /// <param name="pNodo"></param>
    public void DecrementaIndigree(int pNodo)
    {
        indegree[pNodo] = -1;

        int n = 0;

        for (n = 0; n < cantNodos; n++)
        {
            if (mAdyacencia[pNodo, n] == 1)
                indegree[n]--;
        }
    }

    /// <summary>
    /// Metodo para calcular la ruta
    /// </summary>
    public void CalculaRuta()
    {

        // Creamos la tabla
        // 0 - Visitado
        // 1 - Distancia
        // 2 - Previo
        int[,] tabla = new int[cantNodos, 3];

        int n = 0;
        int distancia = 0;
        int m = 0;

        // Inicializamos la tabla
        for (n = 0; n < cantNodos; n++)
        {
            tabla[n, 0] = 0;
            tabla[n, 1] = int.MaxValue;
            tabla[n, 2] = 0;
        }
        tabla[inicio, 1] = 0;

        for (distancia = 0; distancia < cantNodos; distancia++)
        {
            for (n = 0; n < cantNodos; n++)
            {
                if ((tabla[n, 0] == 0) && (tabla[n, 1] == distancia))
                {
                    tabla[n, 0] = 1;
                    for (m = 0; m < cantNodos; m++)
                    {
                        // Verificamos que el nodo sea adyacente
                        if (ObtenAdyacencia(n, m) == 1)
                        {
                            if (tabla[m, 1] == int.MaxValue)
                            {
                                tabla[m, 1] = distancia + 1;
                                tabla[m, 2] = n;
                            }
                        }
                    }
                }
            }
        }

        ruta.Clear();

        int nodo = final;

        while (nodo != inicio)
        {
            ruta.Add(nodo);
            nodo = tabla[nodo, 2];
        }
        ruta.Add(inicio);

        ruta.Reverse();

    }
}
