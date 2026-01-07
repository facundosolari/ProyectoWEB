using Application.Models.Request;
using Application.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReglaDescuentoService
    {
        ReglaDescuentoResponse? GetById(int id);
        List<ReglaDescuentoResponse> GetAll();
        (bool, string) Create(ReglaDescuentoRequest request);
        (bool, string) Update(int id, ReglaDescuentoRequest request);
        (bool, string) SoftDelete(int id);
        (bool, string) HardDelete(int id);
    }
}
