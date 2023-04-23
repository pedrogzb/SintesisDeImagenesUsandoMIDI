using System;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;
using UnityEngine;

public class InputDevices : MonoBehaviour
{
    private IInputDevice _inputDevice;
    private IConexionManagerFilter[] _referenciasConexion;
    void Start()
    {
        _inputDevice = InputDevice.GetByIndex(0);
        _inputDevice.EventReceived += OnEventReceived;
        _inputDevice.StartEventsListening();
        _referenciasConexion = GetComponentsInChildren<IConexionManagerFilter>();
        Debug.Log(_referenciasConexion.Length);
    }
    private void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
    {
        var midiDevice = (MidiDevice)sender;
        if (e.Event.EventType == MidiEventType.NoteOn) 
        {
            NoteEvent noteEvent = (NoteEvent)e.Event;
            Debug.Log($"Event received from '{midiDevice.Name}' at {DateTime.Now}: {e.Event.EventType}");
            Debug.Log($"Canal: {noteEvent.Channel}. Nota: {noteEvent.NoteNumber}. Velocity: {noteEvent.Velocity}");
            EnviarEnventoNoteOn(new Vector3Int(noteEvent.Channel, noteEvent.NoteNumber, noteEvent.Velocity));
        }
        if (e.Event.EventType == MidiEventType.NoteOff)
        {
            NoteEvent noteEvent = (NoteEvent)e.Event;
            Debug.Log($"Event received from '{midiDevice.Name}' at {DateTime.Now}: {e.Event.EventType}");
            Debug.Log($"Canal: {noteEvent.Channel}. Nota: {noteEvent.NoteNumber}. Velocity: {noteEvent.Velocity}");
            EnviarEnventoNoteOff(new Vector3Int(noteEvent.Channel, noteEvent.NoteNumber, noteEvent.Velocity));
        }
    }
    private void OnApplicationQuit()
    {
        if (_inputDevice != null)
        {
            _inputDevice.EventReceived -= OnEventReceived;
            (_inputDevice as IDisposable)?.Dispose();
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
}
