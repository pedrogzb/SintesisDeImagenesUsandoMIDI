using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectorRecursoSalida", menuName = "ScriptableObjects/SelectorRecursoSalidaSO")]

public class SelectorRecursoSalidaSO : ScriptableObject
{
    [SerializeField] 
    public string NombreDeRecursoSalida;
    [SerializeField][Header("Guarda el número de programa seleccionado \n" +
                            "para cada canal (Valor en el intervalo [0,127])\n" +
                            "si no se indica en este rango se supone el valor 0")] 
    public List<int> ConfiguracionDePrograma  = new List<int>();
 
}
