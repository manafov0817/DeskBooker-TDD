using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Abstract
{
    public interface IDeskBookingRepostiory
    {
        void Save(DeskBooking deskBooking);
    }
}
