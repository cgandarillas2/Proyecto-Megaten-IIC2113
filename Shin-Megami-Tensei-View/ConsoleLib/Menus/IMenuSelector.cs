namespace Shin_Megami_Tensei_View.ConsoleLib;

public interface IMenuSelector<T>
{
    /// <summary>
    /// Muestra el menú y permite al usuario seleccionar una opción.
    /// </summary>
    /// <param name="options">Lista de opciones disponibles</param>
    /// <param name="context">Contexto adicional para el display</param>
    /// <returns>Opción seleccionada, o null si se cancela</returns>
    
    T SelectFrom(List<T> options, object context = null);
}