using MechiraSinit.Dto;
using MechiraSinit.Models;

namespace MechiraSinit.Services
{
    public interface IDonorService
    {
        List<DonorDto> GetAllDonors(string? search);
        int AddDonor(DonorDto donorDto);
        void AddDonor(Donor donor);
        bool DeleteDonor(int id);

         bool UpdateDonor(DonorDto donorDto);

    }
}
