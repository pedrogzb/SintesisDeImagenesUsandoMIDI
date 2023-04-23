using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ConexionManagerFilterSO", menuName = "ScriptableObjects/ConexionManagerFilterSO")]

public class ConexionManagerFilterSO : ScriptableObject
{
    [SerializeField]
    //public List <IConexionManagerFilter> _conexiones;
    private List<GameObject> _referencias = new List<GameObject>();
    private List<IConexionManagerFilter> _conexiones;
    public void Awake()
    {
        for (int i = 0; i < _referencias.Count; i++)
            _conexiones[i] = _referencias[i].GetComponent<IConexionManagerFilter>();
    }
    public void EnviarEnventoNoteOn(Vector3Int EventoNoteOn) 
    {
        foreach (IConexionManagerFilter _conexion in _conexiones)
            _conexion.EventoMidiNoteOn(EventoNoteOn);
    }
    public void EnviarEnventoNoteOff(Vector3Int EventoNoteOff)
    {
        foreach (IConexionManagerFilter _conexion in _conexiones)
            _conexion.EventoMidiNoteOff(EventoNoteOff);
    }
}
