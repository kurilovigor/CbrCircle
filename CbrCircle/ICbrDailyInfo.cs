using System;
using System.Threading.Tasks;

namespace CbrCircle
{
    public interface ICbrDailyInfo
    {
        Task<decimal?> GetCurs(DateTime dt);
    }
}