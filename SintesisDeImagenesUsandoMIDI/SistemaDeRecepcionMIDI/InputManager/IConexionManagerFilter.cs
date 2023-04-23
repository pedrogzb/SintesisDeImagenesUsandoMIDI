using UnityEngine;
public interface IConexionManagerFilter
{
    public void EventoMidiNoteOn(Vector3Int NoteOn);
    public void EventoMidiNoteOff(Vector3Int NoteOff);
}
