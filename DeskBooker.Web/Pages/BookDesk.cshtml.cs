using DeskBooker.Core.Domain;
using DeskBooker.Core.Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeskBooker.Web.Pages
{
    public class BookDeskModel : PageModel
    {
        private IDeskBookingRequestProcessor _deskBookingRequestProcessor;

        public BookDeskModel(IDeskBookingRequestProcessor deskBookingRequestProcessor)
        {
            _deskBookingRequestProcessor = deskBookingRequestProcessor;
        }

        [BindProperty]
        public DeskBookingRequest DeskBookingRequest { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var deskBookingResult = _deskBookingRequestProcessor.BookDesk(DeskBookingRequest);

            if (deskBookingResult.Code == DeskBookingResultCode.NotAvailableDesk)
            {
                ModelState.AddModelError("DeskBookingRequest.Date", "No desk available for selected date");
                return Page();
            }

            return RedirectToPage("BookDeskConfirmation", new { deskBookingResult.DeskBookingId, deskBookingResult.FirstName, deskBookingResult.Date });
        }
    }
}