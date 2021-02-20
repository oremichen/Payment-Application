using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAppService.Dto;
using PaymentAppService.PaymentService;
using PaymentAppService.Validations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Payment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentApplicationService _applicationService;

        public PaymentController(IPaymentApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("The request is invalid");

                //validation method
                var isValid = ValidationExtension.ValidateRequest(model);

                if (isValid == false) { return BadRequest("The request should be validated before processed"); }

                var result = await _applicationService.ExecuteProcessPayment(model);
                if (result == false)
                {
                    throw new Exception();
                }
                return Ok("Payment is processed");
              
            }
            catch (Exception)
            {
                return BadRequest(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
