using System;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;
using UnityEngine;

public class InputMidiFile : MonoBehaviour
{
    [SerializeField]
    private SelectorRecursoEntradaSO SelectorInputFile;
    [SerializeField][Header("Posición del recurso selecionado en la lista de nombres \n(comenzando en 1)")][Min(1)]
    private int PosicionDeSeleccion;
    [SerializeField][Header("Habilitar reprodución de salida del archivo MIDI")]
    private bool HabilitarSalidaMIDI;
    [SerializeField]
    private GameObject objetoSalidaMIDI;
    private Playback _playback;
    private string MidiFileName;
    [SerializeField][Header("Habilitar reprodución en bucle del archivo MIDI")]
    private bool ReproducirEnBucle;
    private IConexionManagerFilter[] _referenciasConexion;
    private IConexionInputOutputDevice _referenciaOutputDevice;

    private void Start()
    {
        InicializarMidiFile();
        _referenciasConexion = GetComponentsInChildren<IConexionManagerFilter>();
        _referenciaOutputDevice = (objetoSalidaMIDI == null) ? null : objetoSalidaMIDI.GetComponent<IConexionInputOutputDevice>();
    }
    private void OnMidiEventPlayed(object sender, MidiEventPlayedEventArgs e)
    {
        if (e.Event.EventType == MidiEventType.NoteOn)
        {
            NoteEvent noteEvent = (NoteEvent)e.Event;
            //Debug.Log($"Event received from '{MidiFileName}' at {DateTime.Now}: {e.Event.EventType}");
            //Debug.Log($"Canal: {noteEvent.Channel}. Nota: {noteEvent.NoteNumber}. Velocity: {noteEvent.Velocity}");
            EnviarEnventoNoteOn(new Vector3Int(noteEvent.Channel, noteEvent.NoteNumber, noteEvent.Velocity));
            if (HabilitarSalidaMIDI) _referenciaOutputDevice?.SendEventoMidiNoteOn(e.Event);
        }
        if (e.Event.EventType == MidiEventType.NoteOff)
        {
            NoteEvent noteEvent = (NoteEvent)e.Event;
            //Debug.Log($"Event received from '{MidiFileName}' at {DateTime.Now}: {e.Event.EventType}");
            //Debug.Log($"Canal: {noteEvent.Channel}. Nota: {noteEvent.NoteNumber}. Velocity: {noteEvent.Velocity}");
            EnviarEnventoNoteOff(new Vector3Int(noteEvent.Channel, noteEvent.NoteNumber, noteEvent.Velocity));
            if (HabilitarSalidaMIDI) _referenciaOutputDevice?.SendEventoMidiNoteOff(e.Event);
        }
    }
    public void EnviarEnventoNoteOn(Vector3Int EventoNoteOn)
    {
        foreach (IConexionManagerFilter _conexion in _referenciasConexion)
            _conexion.EventoMidiNoteOn(EventoNoteOn);
    }
    public void EnviarEnventoNoteOff(Vector3Int EventoNoteOff)
    {
        foreach (IConexionManagerFilter _conexion in _referenciasConexion)
            _conexion.EventoMidiNoteOff(EventoNoteOff);
    }
    private void InicializarMidiFile() 
    {

        try 
        {
            MidiFileName = SelectorInputFile.obtenerNombre(PosicionDeSeleccion);
            Debug.Log($"Initializing MidiFile [{MidiFileName}]...");
            MidiFile midiFile = MidiFile.Read(string.Concat("Assets/SintesisDeImagenesUsandoMidi/Resources/", MidiFileName, ".mid"));
            _playback = midiFile.GetPlayback();
            _playback.Loop = ReproducirEnBucle;
            _playback.EventPlayed += OnMidiEventPlayed;
            _playback.Start();
            Debug.Log($"MidiFile [{MidiFileName}] initialized");
        }
        catch
        {
            Debug.Log($"No existe el archivo [{MidiFileName}].Recuerda que se tiene que encontrar en Assets/SintesisDeImagenesUsandoMidi/Resources/");
        }
        
        
    }
    private void OnApplicationQuit()
    {
        if (_playback != null)
        {
            _playback.EventPlayed -= OnMidiEventPlayed;
            _playback.Dispose();
        }
    }
}
