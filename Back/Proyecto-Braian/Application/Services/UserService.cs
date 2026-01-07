using Application.Interfaces;
using Application.Mappings;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserResponse? GetUserById(int id)
        {
            var user = _userRepository.GetUserById(id);
            if (user != null)
            {
                return UserDTO.ToUserResponse(user);
            }
            return null;

        }
        public List<UserResponse>? GetAllUsers()
        {
            var users = _userRepository.GetAllUsers();
            if (users != null)
            {
                return UserDTO.ToUserResponse(users);
            }
            return null;
        }

        public (bool, string) CreateUser(UserRequest request)
        {
            if (_userRepository.ExistsByUsername(request.Usuario) ||
            _userRepository.ExistsByEmail(request.Email))
            {
                return (false, "Credenciales en uso");
            }
            var user = UserDTO.ToUserEntity(request);
            if (user != null)
            {
                _userRepository.AddUser(user);
                return (true, "Usuario creado con éxito");
            }
            return (false, "No se pudo crear el usuario");
        }

        public (bool, string) UpdateUserAdmin(UserRequest request, int id)
        {
            if (_userRepository.ExistsByUsername(request.Usuario))
                return (false, "Nombre ya en uso");

            if (_userRepository.ExistsByEmail(request.Email))
                return (false, "Email ya en uso");

            var entity = _userRepository.GetUserById(id);
            if (entity == null) return (false, "Usuario no encontrado");

            UserDTO.ToUserUpdateAdmin(request, entity);
            _userRepository.UpdateUser(entity);
            return (true, "Usuario actualizado");
        }

        public (bool, string) UpdateUser(UserPatchRequest request, int id, int tokenUserId)
        {
            var entity = _userRepository.GetUserById(id);
            if (entity == null) return (false, "Usuario no encontrado");

            if (tokenUserId != entity.Id)
                return (false, "No es el usuario en cuestion");

            if (!string.IsNullOrEmpty(request.Email) &&
                request.Email != entity.Email &&
                _userRepository.ExistsByEmail(request.Email))
                return (false, "Email no valido");

            UserDTO.ToUserUpdate(request, entity);
            _userRepository.UpdateUser(entity);
            return (true, "Usuario actualizado");
        }

        public (bool, string) SoftDeleteUser(int id)
        {
            var entity = _userRepository.GetUserById(id);
            if (entity != null)
            {
                _userRepository.SoftDeleteUser(entity);
                return (true, "Habilitacion cambiada con exito");
            }
            return (false, "No se pudo cambiar la habilitacion");
        }
        public (bool, string) HardDeleteUser(int id)
        {
            var entity = _userRepository.GetUserById(id);
            if (entity != null)
            {
                _userRepository.HardDeleteUser(entity);
                return (true, "Usuario borrado con éxito");
            }
            return (false, "No se pudo borrar al usuario");
        }

    }
}
