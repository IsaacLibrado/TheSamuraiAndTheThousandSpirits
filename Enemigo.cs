using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase padre para los enemigos del jugador
/// </summary>
/// Version 1.0
/// Creador Isaac Librado
public class Enemigo : MonoBehaviour {

	//Atributos del enemigo
	public int escudo;
	public int salud;

	// Use this for initialization
	void Start () {
		salud = 100;
		escudo = Random.Range(1, 3);
	}
	
	// Update is called once per frame
	void Update () {
		if (salud <= 0)
			Destroy(gameObject);
	}
}
