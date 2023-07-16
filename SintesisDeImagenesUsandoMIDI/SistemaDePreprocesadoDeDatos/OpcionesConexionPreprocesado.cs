namespace OpcionesConexionPreprocesado
{
    public enum OpcionesDeCalculo 
    {
        Media,
        Max,
        Min,
        UltimaPulsada,
        UltimaDesPulsada
    }
    public enum OpcionesDeRemapAnimator
    {
        ModuloReescalado,
        ClampReescalado,
        ReescaladoClamp
    }
    public enum OpcionesDeRemapShader
    {
        ModuloReescalado,
        ClampReescalado,
        ReescaladoClamp,
        Vector2Reescalado,
        Vector3Reescalado,
        Vector4Reescalado
    }
    public enum TipoSalidaAnimator 
    {
        Int,
        Float,
        Bool,
        Trigger
    }
    public enum TipoSalidaShader
    {
        Int,
        Float,
        Vector,
        Color
    }
    public enum ZonaEjecucion 
    {
        EnNotaOn,
        EnNotaOff,
        EnAmbas
    }
}
