using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectorRecursoEntrada", menuName = "ScriptableObjects/SelectorRecursoEntradaSO")]

public class SelectorRecursoEntradaSO : ScriptableObject
{
    [SerializeField] 
    private List<string> NombresDeRecursos = new List<string>();
    public string obtenerNombre(int PosicionDeSeleccion) => 
        (PosicionDeSeleccion<= NombresDeRecursos.Count)? NombresDeRecursos[PosicionDeSeleccion - 1]:"Nombre no Registrado";
}
