using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Contracts;

namespace Backend.Services;

public interface IUsuariosService
{
    Task<IReadOnlyList<UsuarioDto>> ObtenerTodosAsync();

    Task<UsuarioDto?> ObtenerPorIdAsync(int id);

    Task<UsuarioDto> CrearAsync(CrearActualizarUsuarioRequest usuario);

    Task<UsuarioDto?> ActualizarAsync(int id, CrearActualizarUsuarioRequest usuarioActualizado);

    Task<bool> EliminarAsync(int id);
}
