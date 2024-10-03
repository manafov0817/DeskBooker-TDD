using DeskBooker.Core.Abstract;
using DeskBooker.Core.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly DeskBookingRequest _request;
        private readonly List<Desk> _availableDesks;
        private readonly Mock<IDeskBookingRepostiory> _deskBookingRepositoryMock;
        private readonly Mock<IDeskRepositiory> _deskRepositoryMock;
        private readonly DeskBookingRequestProcessor _processor;

        public DeskBookingRequestProcessorTests()
        {
            _request = new DeskBookingRequest
            {
                FirstName = "Mahammad",
                LastName = "Manafov",
                Email = "manafov0817@gmail.com",
                Date = new DateTime(2020, 1, 28)
            };

            _availableDesks = new List<Desk>() { new Desk() { Id = 3 } };

            _deskBookingRepositoryMock = new Mock<IDeskBookingRepostiory>();

            _deskRepositoryMock = new Mock<IDeskRepositiory>();
            _deskRepositoryMock.Setup(x => x.GetAvailableDesks(_request.Date)).Returns(_availableDesks);

            _processor = new DeskBookingRequestProcessor(_deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequestValues()
        {
            // Act
            DeskBookingResult result = _processor.BookDesk(_request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_request.FirstName, result.FirstName);
            Assert.Equal(_request.LastName, result.LastName);
            Assert.Equal(_request.Email, result.Email);
            Assert.Equal(_request.Date, result.Date);
        }

        [Fact]
        public void ShouldThrowExceptionIfRequestIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));

            Assert.Equal("request", exception.ParamName);
        }

        [Fact]
        public void ShouldSaveDeskBooking()
        {

            DeskBooking savedDeskBooking = null;

            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                                      .Callback<DeskBooking>(deskBooking => savedDeskBooking = deskBooking);

            _processor.BookDesk(_request);

            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Once);

            Assert.NotNull(savedDeskBooking);
            Assert.Equal(_request.FirstName, savedDeskBooking.FirstName);
            Assert.Equal(_request.LastName, savedDeskBooking.LastName);
            Assert.Equal(_request.Email, savedDeskBooking.Email);
            Assert.Equal(_request.Date, savedDeskBooking.Date);
            Assert.Equal(_availableDesks.First().Id, savedDeskBooking.DeskId);
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
        {
            _availableDesks.Clear();

            _processor.BookDesk(_request);

            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
        }

        [Theory]
        [InlineData(DeskBookingResultCode.Success, true)]
        [InlineData(DeskBookingResultCode.NotAvailableDesk, false)]
        public void ShouldReturnExpectedResultCode(DeskBookingResultCode deskBookingResultCode, bool isDeskAvailable)
        {
            if (!isDeskAvailable)
                _availableDesks.Clear();

            var expectedResult = _processor.BookDesk(_request);

            Assert.Equal(expectedResult.Code, deskBookingResultCode);
        }


        [Theory]
        [InlineData(5, true)]
        [InlineData(null, false)]
        public void ShouldReturnExpectedDeskBookingId(int? deskBookingId, bool isDeskAvailable)
        {
            if (!isDeskAvailable)
                _availableDesks.Clear();
            else _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                                              .Callback<DeskBooking>(deskBooking => deskBooking.Id = deskBookingId);

            var expectedResult = _processor.BookDesk(_request);

            Assert.Equal(expectedResult.DeskBookingId, deskBookingId);
        }

    }
}
