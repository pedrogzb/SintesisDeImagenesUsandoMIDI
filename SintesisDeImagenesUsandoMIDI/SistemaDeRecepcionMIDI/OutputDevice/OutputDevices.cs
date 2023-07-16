using System;
using System.Linq;
using UnityEngine;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Common;


public class OutputDevices : MonoBehaviour,IConexionInputOutputDevice
{
    private OutputDevice _outputDevice;
    [SerializeField]
    private SelectorRecursoSalidaSO SelectorOutput;
    private string OutputDeviceName;
    [SerializeField] 
    private bool UsarValorDeVelocidad;
    [SerializeField][Header("Manda los mensajes de programa a cada canal \npara que se actualicen")]
    private bool ActualizarNumerosDePrograma = false;
    BytesToMidiEventConverter bytesToMidi;
    private void Start()
    {
        bytesToMidi = new BytesToMidiEventConverter();
        InitializeOutputDevice();
    }
    private void InitializeOutputDevice()
    {
        OutputDeviceName = SelectorOutput.NombreDeRecursoSalida;
        Debug.Log($"Initializing output device [{OutputDeviceName}]...");
        var allOutputDevices = OutputDevice.GetAll();
        if (!allOutputDevices.Any(d => d.Name == OutputDeviceName))
        {
            var allDevicesList = string.Join(Environment.NewLine, allOutputDevices.Select(d => $"  {d.Name}"));
            Debug.Log($"There is no [{OutputDeviceName}] device presented in the system. Here the list of all device:{Environment.NewLine}{allDevicesList}");
            return;
        }

        _outputDevice = OutputDevice.GetByName(OutputDeviceName);
        _outputDevice.PrepareForEventsSending();
        ActualizarNumeroDePrograma();
        Debug.Log($"Output device [{OutputDeviceName}] initialized.");
    }
    public void SendEventoMidiNoteOn(Vector3Int mensaje) 
    {
        byte[] bits = { SevenBitNumber.Values[mensaje.y], (UsarValorDeVelocidad)? SevenBitNumber.Values[mensaje.z] : SevenBitNumber.MaxValue };
        _outputDevice.SendEvent(bytesToMidi.Convert((byte)(0b_1001_0000 + FourBitNumber.Values[mensaje.x]), bits));
    }
    public void SendEventoMidiNoteOff(Vector3Int mensaje)
    {
        byte [] bits = { SevenBitNumber.Values[mensaje.y], (UsarValorDeVelocidad) ? SevenBitNumber.Values[mensaje.z] : SevenBitNumber.MaxValue };
        _outputDevice.SendEvent(bytesToMidi.Convert((byte)(0b_1000_0000+ FourBitNumber.Values[mensaje.x]), bits));
    }
    public void SendEventoMidiNoteOn(MidiEvent e) => _outputDevice.SendEvent(e);
    
    public void SendEventoMidiNoteOff(MidiEvent e)=> _outputDevice.SendEvent(e);
    
    public void SendEventoCambioDePrograma(int numeroDeCanal, int numeroDePrograma) 
    {
        byte[] mensajeCambioDeTono = { (byte)(0b_1100_0000 + FourBitNumber.Values[numeroDeCanal]), SevenBitNumber.Values[numeroDePrograma] };
        _outputDevice.SendEvent(bytesToMidi.Convert(mensajeCambioDeTono));
        //Debug.Log("Se ha mandado el mensaje de Cambio de Canal");
    }
    public void ActualizarNumeroDePrograma() 
    {
        for(int i = 0; i <= 15; i++) 
        {
            SendEventoCambioDePrograma(i, SelectorOutput.ConfiguracionDePrograma[i]) ;
        }
    }
    private void OnApplicationQuit()
    {
        if (_outputDevice != null)
            _outputDevice.Dispose();
    }
    private void Update()
    {

        if (ActualizarNumerosDePrograma)
        {
            ActualizarNumeroDePrograma();
            ActualizarNumerosDePrograma = false;
        }

    }
}
