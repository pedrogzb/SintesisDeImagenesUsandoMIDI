using UnityEngine;

public class SyncMidiFiles : MonoBehaviour
{
    private InputMidiFile[] inputMidiFiles;
    private void Awake()
    {
        inputMidiFiles = GetComponentsInChildren<InputMidiFile>();
    }
    void Start()
    {
        foreach (InputMidiFile i in inputMidiFiles) i.EmpezarReproduccion();
    }
}
