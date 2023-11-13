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
    [SerializeField][Header("Habilitar reprodución en bucle del archivo MIDI")]
    private bool ReproducirEnBucle;
    private IConexionManagerFilter[] _referenciasConexion;
    private IConexionInputOutputDevice _referenciaOutputDevice;
    private void Awake()=>InicializarMidiFile();
    
    private void Start()
    {
        _referenciasConexion = GetComponentsInChildren<IConexionManagerFilter>();
        _referenciaOutputDevice = (objetoSalidaMIDI == null) ? null : objetoSalidaMIDI.GetComponent<IConexionInputOutputDevice>();
        if (GetComponentsInParent<SyncMidiFiles>().Length == 0) EmpezarReproduccion();
    }
    private void OnMidiEventPlayed(object sender, MidiEventPlayedEventArgs e)
    {
        if (e.Event.EventType == MidiEventType.NoteOn)
        {
            NoteEvent noteEvent = (NoteEvent)e.Event;
            //Debug.Log($"Event received from '{MidiFileName}' at {DateTime.Now}: {e.Event.EventType}");
            //Debug.Log($"Canal: {noteEvent.Channel}. Nota: {noteEvent.NoteNumber}. Velocity: {noteEvent.Velocity}");
            EnviarEventoNoteOn(new Vector3Int(noteEvent.Channel, noteEvent.NoteNumber, noteEvent.Velocity));
            if (HabilitarSalidaMIDI) _referenciaOutputDevice?.SendEventoMidiNoteOn(e.Event);
        }
        if (e.Event.EventType == MidiEventType.NoteOff)
        {
            NoteEvent noteEvent = (NoteEvent)e.Event;
            //Debug.Log($"Event received from '{MidiFileName}' at {DateTime.Now}: {e.Event.EventType}");
            //Debug.Log($"Canal: {noteEvent.Channel}. Nota: {noteEvent.NoteNumber}. Velocity: {noteEvent.Velocity}");
            EnviarEventoNoteOff(new Vector3Int(noteEvent.Channel, noteEvent.NoteNumber, noteEvent.Velocity));
            if (HabilitarSalidaMIDI) _referenciaOutputDevice?.SendEventoMidiNoteOff(e.Event);
        }
    }
    public void EnviarEventoNoteOn(Vector3Int EventoNoteOn)
    {
        foreach (IConexionManagerFilter _conexion in _referenciasConexion)
            _conexion.EventoMidiNoteOn(EventoNoteOn);
    }
    public void EnviarEventoNoteOff(Vector3Int EventoNoteOff)
    {
        foreach (IConexionManagerFilter _conexion in _referenciasConexion)
            _conexion.EventoMidiNoteOff(EventoNoteOff);
    }
    private void InicializarMidiFile() 
    {
        try 
        {
            string MidiFileName = SelectorInputFile.obtenerNombre(PosicionDeSeleccion);
            Debug.Log($"Inicializando el dispositivo de entrada [{MidiFileName}]...");
            MidiFile midiFile = MidiFile.Read(string.Concat("Assets/SintesisDeImagenesUsandoMidi/Resources/", MidiFileName, ".mid"));
            _playback = midiFile.GetPlayback();
            _playback.Loop = ReproducirEnBucle;
            _playback.EventPlayed += OnMidiEventPlayed;
            Debug.Log($"MidiFile [{MidiFileName}] inicializado");
        }
        catch
        {
            string MidiFileName = SelectorInputFile.obtenerNombre(PosicionDeSeleccion);
            Debug.Log($"No existe el archivo [{MidiFileName}].Recuerda que se tiene que encontrar en un subdirectorio de Assets/SintesisDeImagenesUsandoMidi/Resources/");
        }       
    }
    public void EmpezarReproduccion() => _playback.Start();
    private void OnApplicationQuit()
    {
        if (_playback != null)
        {
            _playback.EventPlayed -= OnMidiEventPlayed;
            _playback.Dispose();
        }
    }
}
