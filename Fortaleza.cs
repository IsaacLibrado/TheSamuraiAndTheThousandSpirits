using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase para el control de las fortalezas
/// </summary>
public class Fortaleza : Enemigo {

	//Plantilla de los aliados que se deben crear 
	public GameObject plantilla;

	//Game object de textos
	public TextMesh vida;

	void Start()
    {
		//Inicializamos valores
		escudo = Random.Range(1, 3);

		vida = gameObject.GetComponentsInChildren<TextMesh>()[0];
	}


	// Update is called once per frame
	void Update () {
		//Validamos la salud para poner texto y colores

		//Ponemos el texto de vida
		vida.text = salud.ToString();

		if (salud < 50)
			vida.color = Color.yellow;

		if (salud < 10)
			vida.color = Color.red;

		if (salud<=0)
        {
			//Obtenemos un rango al azar
			int valor = Random.Range(2, 5);

			//Creamos la cantidad random de aliados
			for(int n=0;n<valor;n++)
            {
				plantilla.GetComponent<Personaje>().ataque.gameObject.SetActive(true);
				GameObject objToSpawn = Object.Instantiate(plantilla);
				objToSpawn.transform.position = new Vector3(transform.position.x,0.5f,transform.position.z);
				objToSpawn.SetActive(true);
			}

			//Nos destruimos
			gameObject.SetActive(false);
        }			
	}
}
