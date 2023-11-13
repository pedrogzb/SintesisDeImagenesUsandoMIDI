using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectorRecursoEntrada", menuName = "ScriptableObjects/SelectorRecursoEntradaSO")]

public class SelectorRecursoEntradaSO : ScriptableObject
{
    [SerializeField] 
    private List<string> NombresDeRecursos = new List<string>();
    public string obtenerNombre(int PosicionDeSeleccion) => 
        (PosicionDeSeleccion<= NombresDeRecursos.Count && PosicionDeSeleccion > 0) ? NombresDeRecursos[PosicionDeSeleccion - 1]:"Nombre no Registrado";
}
