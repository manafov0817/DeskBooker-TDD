using DeskBooker.Core.Abstract;
using DeskBooker.Core.Domain;
using System;
using System.Linq;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessor : IDeskBookingRequestProcessor
    {
        private readonly IDeskBookingRepository _deskBookingRepostiory;
        private readonly IDeskRepository _deskRepostiory;

        public DeskBookingRequestProcessor(IDeskBookingRepository deskBookingRepostiory, IDeskRepository deskRepositiory)
        {
            _deskBookingRepostiory = deskBookingRepostiory;
            _deskRepostiory = deskRepositiory;
        }

        public DeskBookingResult BookDesk(DeskBookingRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var deskBookingResult = Create<DeskBookingResult>(request);

            var availableDesks = _deskRepostiory.GetAvailableDesks(request.Date);

            if (availableDesks.FirstOrDefault() is Desk availableDesk)
            {
                DeskBooking deskBooking = Create<DeskBooking>(request);
                deskBooking.DeskId = availableDesk.Id;
                _deskBookingRepostiory.Save(deskBooking);
                deskBookingResult.Code = DeskBookingResultCode.Success;
                deskBookingResult.DeskBookingId = deskBooking.Id;
            }
            else deskBookingResult.Code = DeskBookingResultCode.NotAvailableDesk;

            return deskBookingResult;
        }

        private static T Create<T>(DeskBookingRequest request) where T : DeskBookingBase, new()
        {
            return new T()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Date = request.Date,
            };
        }
    }
}