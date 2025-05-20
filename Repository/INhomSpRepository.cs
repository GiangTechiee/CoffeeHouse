

using CoffeeHouse.Models;

namespace CoffeeHouse.Repository
{
    public interface INhomSpRepository
    {
        TbNhomSanPham Add(TbNhomSanPham nhomSp);
        TbNhomSanPham Update(TbNhomSanPham nhomSp);
        TbNhomSanPham Delete(String maNhomSp);
        TbNhomSanPham GetAllNhomSp(String maNhomSp);
        IEnumerable<TbNhomSanPham> GetAllNhomSp();
    }
}
