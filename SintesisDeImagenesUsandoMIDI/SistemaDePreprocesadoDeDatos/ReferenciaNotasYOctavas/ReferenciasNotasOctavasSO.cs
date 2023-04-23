using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ReferenciasNotasOctavas", menuName = "ScriptableObjects/ReferenciasNotasOctavasSO", order = 1)]
public class ReferenciasNotasOctavasSO : ScriptableObject
{
    [SerializeField]private List<string> _referenciasNotas   = new List<string>();
    [SerializeField]private List<string> _referenciasOctavas = new List<string>();
    public void Awake()
    {
        if (_referenciasNotas.Count != 12 || _referenciasOctavas.Count != 11)
            Debug.LogError("No se han referenciado los suficientes parametros");
    }

    public string getReferenciaNota(int Nota) => _referenciasNotas[Nota]; 
    public string getReferenciaOctava(int Octava) => _referenciasOctavas[Octava + 1]; 
}
