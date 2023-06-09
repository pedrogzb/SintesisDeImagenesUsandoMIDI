using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class InputFilter : MonoBehaviour,IConexionManagerFilter
{
    [Header("Filtrar seg�n el rango de canal. Rango de 0:15")]
    [SerializeField] private bool FiltrarPorCanal;
    [SerializeField] private Vector2Int _RangoCanalAFiltrar;//Canal a filtrar en el rango 0 a 15
    [Header("Filtrar seg�n el rango de octavas. Rango de -1:9")]
    [SerializeField] private bool FiltrarPorOctava;
    [SerializeField] private Vector2Int _RangoOctavaAFiltrar;//Canal a filtrar en el rango -1 a 9
    private IConexionFilterPreprocesado _referencia;
    private Queue<Vector3Int> _NotasOn;
    private Queue<Vector3Int> _NotasOff;

    public void Start()
    {
        _referencia = GetComponent<IConexionFilterPreprocesado>();
        _NotasOn = new Queue<Vector3Int>();
        _NotasOff = new Queue<Vector3Int>();
        StartManejoDeColas();
    }
    public void EventoMidiNoteOn(Vector3Int NoteOn) 
    {
        int _canal = NoteOn.x;
        if (FiltrarPorCanal && (_canal < _RangoCanalAFiltrar.x || _canal > _RangoCanalAFiltrar.y)) return;

        int _octava = (NoteOn.y / 12) - 1;
        if (FiltrarPorOctava && (_octava < _RangoOctavaAFiltrar.x || _octava > _RangoOctavaAFiltrar.y)) return;
        int _nota = NoteOn.y % 12;
        int _velocity = NoteOn.z;
        _NotasOn.Enqueue(new Vector3Int(_nota, _octava, _velocity));
        //_NotaOnChaged = true;
        //_referencia.NotaPulsada(new Vector3Int(_nota, _octava,_velocity));


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
        _NotasOff.Enqueue(new Vector3Int(_nota, _octava, _velocity));
        //_NotaOffChaged = true;
        //_referencia.NotaDesPulsada(new Vector3Int(_nota, _octava, _velocity));
    }
    private void StartManejoDeColas() 
    {
        StartCoroutine(DesEncoladoDeNotaOff());
        StartCoroutine(DesEncoladoDeNotaOn ());
    }
    private IEnumerator DesEncoladoDeNotaOn() 
    {
        while (true) 
        {
            while(_NotasOn.Count > 0) 
            {
                _referencia.NotaPulsada(_NotasOn.Dequeue());
            }
            yield return null;
        }
    }
    private IEnumerator DesEncoladoDeNotaOff() 
    {
        while (true)
        {
            while (_NotasOff.Count > 0)
            {
                _referencia.NotaDesPulsada(_NotasOff.Dequeue());
            }
            yield return null;
        }
    }
    public void OnApplicationQuit()=>StopAllCoroutines();
    
}

