using MechiraSinit.Dto;
using MechiraSinit.Models;

namespace MechiraSinit.Services
{
    public interface IGiftService
    {
        List<GiftDto>GetAllGifts(string? search, string? sort);
        int AddGift(GiftDto giftDto);
        User? RunRaffle(int giftId);
        bool UpdateGift(int id, GiftDto giftDto);
        bool DeleteGift(int id);
        Gift GetGiftById(int id);
        List<ReportWinnerDto> GetWinnersReport();
    }
}