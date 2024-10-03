using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeskBooker.Web.Pages
{
    public class BookDeskModelTests
    {
        private Mock<IDeskBookingRequestProcessor> _processorMock;
        private DeskBookingResult _deskBookingResult;
        private BookDeskModel _bookDeskModel;

        public BookDeskModelTests()
        {
            _processorMock = new Mock<IDeskBookingRequestProcessor>();

            _bookDeskModel = new BookDeskModel(_processorMock.Object)
            {
                DeskBookingRequest = new DeskBookingRequest()
            };
            _deskBookingResult = new DeskBookingResult()
            {
                Code = DeskBookingResultCode.Success
            };

            _processorMock.Setup(x => x.BookDesk(_bookDeskModel.DeskBookingRequest)).Returns(_deskBookingResult);
        }

        [Fact]
        public void ShouldCallBookDeskMethodOfProcessor()
        {

            _bookDeskModel.OnPost();

            _processorMock.Verify(x => x.BookDesk(_bookDeskModel.DeskBookingRequest), Times.Once);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public void ShouldCallBookDeskMethodOfProcessorIfModelIsValid(int expectedBookDeskCalls, bool isValid)
        {
            if (!isValid)
                _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");


            _bookDeskModel.OnPost();

            _processorMock.Verify(x => x.BookDesk(_bookDeskModel.DeskBookingRequest), Times.Exactly(expectedBookDeskCalls));
        }

        [Fact]
        public void ShouldAddModelErrorIfNoDeskIsAvailable()
        {

            _deskBookingResult.Code = DeskBookingResultCode.NotAvailableDesk;

            _bookDeskModel.OnPost();

            var modelState = Assert.Contains("DeskBookingRequest.Date", _bookDeskModel.ModelState);

            var modelError = Assert.Single(modelState.Errors);

            Assert.Equal("No desk available for selected date", modelError.ErrorMessage);
        }

        [Fact]
        public void ShouldNotAddModelErrorIfNoDeskIsAvailable()
        {
            _bookDeskModel.OnPost();

            Assert.DoesNotContain("DeskBookingRequest.Date", _bookDeskModel.ModelState);
        }

        [Theory]
        [InlineData(typeof(PageResult), false, null)]
        [InlineData(typeof(PageResult), false, DeskBookingResultCode.NotAvailableDesk)]
        [InlineData(typeof(RedirectToPageResult), true, DeskBookingResultCode.Success)]
        public void ShouldReturnExpectedActionResult(Type expectedActionResultType, bool isValid, DeskBookingResultCode? deskBookingResultCode)
        {
            if (!isValid)
                _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");

            if (deskBookingResultCode.HasValue)
                _deskBookingResult.Code = deskBookingResultCode.Value;


            IActionResult actionResult = _bookDeskModel.OnPost();

            Assert.IsType(expectedActionResultType, actionResult);
        }

        [Fact]
        public void ShouldRedirectToBookDeskConfirmationPage()
        {
            _deskBookingResult.Code = DeskBookingResultCode.Success;
            _deskBookingResult.DeskBookingId = 7;
            _deskBookingResult.FirstName = "Mahammad";
            _deskBookingResult.Date = new DateTime(2020, 1, 28);

            IActionResult actionResult = _bookDeskModel.OnPost();

            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(actionResult);
            IDictionary<string, object> routeValues = redirectToPageResult.RouteValues;
            var deskBookingId = Assert.Contains("DeskBookingId", routeValues);
            var firstName = Assert.Contains("FirstName", routeValues);
            var date = Assert.Contains("Date", routeValues);
            Assert.Equal("BookDeskConfirmation", redirectToPageResult.PageName);
            Assert.Equal(3, routeValues.Count);
            Assert.Equal(_deskBookingResult.DeskBookingId, deskBookingId);
            Assert.Equal(_deskBookingResult.FirstName, firstName);
            Assert.Equal(_deskBookingResult.Date, date);
        }
    }
}
