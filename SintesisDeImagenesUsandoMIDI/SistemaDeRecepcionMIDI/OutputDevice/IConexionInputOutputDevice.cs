using UnityEngine;
using Melanchall.DryWetMidi.Core;
public interface IConexionInputOutputDevice
{
    public void SendEventoMidiNoteOn(MidiEvent e);
    public void SendEventoMidiNoteOff(MidiEvent e);

    public void SendEventoMidiNoteOn(Vector3Int mensaje);
    public void SendEventoMidiNoteOff(Vector3Int mensaje);
    public void SendEventoCambioDePrograma(int numeroDeCanal, int numeroDePrograma);
}
