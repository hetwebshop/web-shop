using API.DTOs;
using System.Threading.Tasks;

namespace API.Services.Payment
{
    public interface IPaymentService
    {
        Task<PaymentSessionResponseDto> CreatePaymentSession(CreatePaymentSessionDto req);
    }
}
