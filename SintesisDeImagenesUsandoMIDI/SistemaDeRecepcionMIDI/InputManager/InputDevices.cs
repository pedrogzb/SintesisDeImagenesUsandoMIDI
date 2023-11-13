using System;
using System.Linq;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;
using UnityEngine;

public class InputDevices : MonoBehaviour
{   
    [SerializeField]
    private SelectorRecursoEntradaSO SelectorInput;
    [SerializeField][Header("Posición del recurso selecionado en la lista de nombres \n(comenzando en 1)")][Min(1)] 
    private int PosicionDeSeleccion;
    [SerializeField][Header("Habilitar reprodución de salida de los mensajes MIDI")]
    private bool HabilitarSalidaMIDI;
    [SerializeField]
    private GameObject objetoSalidaMIDI;
    private InputDevice _inputDevice;
    private IConexionManagerFilter[] _referenciasConexion;
    private IConexionInputOutputDevice _referenciaOutputDevice;

    void Start()
    {
        InicializarInputDevice();
        _referenciasConexion = GetComponentsInChildren<IConexionManagerFilter>();
        _referenciaOutputDevice = (objetoSalidaMIDI == null) ? null : objetoSalidaMIDI.GetComponent<IConexionInputOutputDevice>();
    }
    private void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        if (e.Event.EventType == MidiEventType.NoteOn) 
        {
            NoteEvent noteEvent = (NoteEvent)e.Event;
            //Debug.Log($"Evento recibido de '{midiDevice.Name}' a {DateTime.Now}: {e.Event.EventType}");
            //Debug.Log($"Canal: {noteEvent.Channel}. Nota: {noteEvent.NoteNumber}. Velocidad: {noteEvent.Velocity}");
            EnviarEventoNoteOn(new Vector3Int(noteEvent.Channel, noteEvent.NoteNumber, noteEvent.Velocity));
            if(HabilitarSalidaMIDI)_referenciaOutputDevice?.SendEventoMidiNoteOn(e.Event);
        }
        if (e.Event.EventType == MidiEventType.NoteOff)
        {
            NoteEvent noteEvent = (NoteEvent)e.Event;
            //Debug.Log($"Evento recibido de '{midiDevice.Name}' a {DateTime.Now}: {e.Event.EventType}");
            //Debug.Log($"Canal: {noteEvent.Channel}. Nota: {noteEvent.NoteNumber}. Velocidad: {noteEvent.Velocity}");
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
    private void InicializarInputDevice()
    {
        string InputDeviceName = SelectorInput.obtenerNombre(PosicionDeSeleccion);

        Debug.Log($"Inicializando el dispositivo de entrada [{InputDeviceName}]...");
        var allOutputDevices = InputDevice.GetAll();
        if (!allOutputDevices.Any(d => d.Name == InputDeviceName))
        {
            var allDevicesList = string.Join(Environment.NewLine, allOutputDevices.Select(d => $"  {d.Name}"));
            Debug.Log($"No se encuentra el dipositivo [{InputDeviceName}] en el sistema. Aquí puedes encontrar una lista de los dispositivos disponibles:{Environment.NewLine}{allDevicesList}");
            return;
        }

        _inputDevice = InputDevice.GetByName(InputDeviceName);
        _inputDevice.EventReceived += OnEventReceived;
        _inputDevice.StartEventsListening();

        Debug.Log($"Dispositivo de entrada [{InputDeviceName}] inicializado.");
    }
    private void OnApplicationQuit()
    {
        if (_inputDevice != null)
        {
            _inputDevice.StopEventsListening();
            _inputDevice.EventReceived -= OnEventReceived;
            (_inputDevice as IDisposable)?.Dispose();
        }
    }
}
