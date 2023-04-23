using System;
using System.Collections.Generic;
using UnityEngine;

public class InputFilter : MonoBehaviour,IConexionManagerFilter
{
    [Header("Filtrar según el rango de canal. Rango de 0:15")]
    [SerializeField] private bool FiltrarPorCanal;
    [SerializeField] private Vector2Int _RangoCanalAFiltrar;//Canal a filtrar en el rango 0 a 15
    [Header("Filtrar según el rango de octavas. Rango de -1:9")]
    [SerializeField] private bool FiltrarPorOctava;
    [SerializeField] private Vector2Int _RangoOctavaAFiltrar;//Canal a filtrar en el rango -1 a 9
    [SerializeField] private Bypass _referencia;
    IConexionFilterPreprocesado _ref;
    public void Start() => _ref = GetComponent<IConexionFilterPreprocesado>();
    public void EventoMidiNoteOn(Vector3Int NoteOn) 
    {
        int _canal = NoteOn.x;
        if (FiltrarPorCanal && (_canal < _RangoCanalAFiltrar.x || _canal > _RangoCanalAFiltrar.y)) return;

        int _octava = (NoteOn.y / 12) - 1;
        if (FiltrarPorOctava && (_octava < _RangoOctavaAFiltrar.x || _octava > _RangoOctavaAFiltrar.y)) return;
        int _nota = NoteOn.y % 12;
        int _velocity = NoteOn.z;
        //_referencia.NotaPulsada(new Vector3Int(_nota, _octava,_velocity));
        try { _ref.NotaPulsada(new Vector3Int(_nota, _octava, _velocity)); } catch (Exception e) { Debug.Log(e.Message);};
        
    }
    public void EventoMidiNoteOff(Vector3Int NoteOff)
    {
        //Debug.Log($"Nota despulsada desde el filter");
        int _canal = NoteOff.x;
        if (FiltrarPorCanal && (_canal < _RangoCanalAFiltrar.x || _canal > _RangoCanalAFiltrar.y)) return;

        int _octava = (NoteOff.y / 12) - 1;
        if (FiltrarPorOctava && (_octava < _RangoOctavaAFiltrar.x || _octava > _RangoOctavaAFiltrar.y)) return;
        int _nota = NoteOff.y % 12;
        int _velocity = NoteOff.z;
        _referencia?.NotaDesPulsada(new Vector3Int(_nota, _octava, _velocity));
    }
}
