using Application.Models.Request;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDescuentoService
    {
        DescuentoResponse? GetDescuentoById(int id);
        List<DescuentoResponse> GetAllDescuentos();
        (bool, string) CreateDescuento(DescuentoRequest request);
        (bool, string) UpdateDescuento(int id, DescuentoRequest request);
        (bool, string) SoftDeleteDescuento(int id);
        (bool, string) HardDeleteDescuento(int id);
    }
}
