using OpcionesConexionPreprocesado;
using System.Collections.Generic;
using UnityEngine;



public class AnimatorPreprocesado : MonoBehaviour, IConexionFilterPreprocesado
{
    [SerializeField] private Animator _animator;
    [SerializeField] AnimatorConexionSO _animatorConexion;

    private void Start()
    {
        if(!_animatorConexion.Inicializar())Debug.LogWarning("No se ha inicializado correctamente la conexión");
    }
    public void NotaPulsada(Vector3Int NoteOn)
    {
        _animatorConexion.ActualizarEnNotaOn(NoteOn);

        for (int i = 0; i < _animatorConexion.Count; i++)
        {
            if (_animatorConexion.zonaEjecucion[i] != ZonaEjecucion.EnNotaOff)
                selector(i);
        }
    }
    public void NotaDesPulsada(Vector3Int NoteOff)
    {
        _animatorConexion.ActualizarEnNotaOff(NoteOff);

        for (int i = 0; i < _animatorConexion.Count; i++)
        {
            if (_animatorConexion.zonaEjecucion[i] != ZonaEjecucion.EnNotaOn)
                selector(i);
        }
    }
    private void selector(int num) 
    {
        switch (_animatorConexion.tipoSalida[num])
        {
            case TipoSalidaAnimator.Bool:
                _animator.SetBool(_animatorConexion.getNombre(num), _animatorConexion.getValor(num) == 1);
                break;
            case TipoSalidaAnimator.Float:
                _animator.SetFloat(_animatorConexion.getNombre(num), _animatorConexion.getValor(num));
                break;
            case TipoSalidaAnimator.Int:
                _animator.SetInteger(_animatorConexion.getNombre(num), (int)_animatorConexion.getValor(num));
                break;
            case TipoSalidaAnimator.Trigger:
                _animator.SetTrigger(_animatorConexion.getNombre(num));
                break;
        }
    }


}
