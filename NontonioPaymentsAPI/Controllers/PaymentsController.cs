using Microsoft.AspNetCore.Mvc;
using Stripe;
using NontonioPaymentsAPI.Models;

namespace NontonioPaymentsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    [HttpPost("create-intent")]
    public IActionResult CreateIntent(
        [FromBody] PaymentIntentRequest request)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(request.Amount * 100),
            Currency = "mxn",
            AutomaticPaymentMethods =
                new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
        };

        var service = new PaymentIntentService();

        var intent = service.Create(options);

        return Ok(new
        {
            clientSecret = intent.ClientSecret,
            paymentIntentId = intent.Id
        });
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = 10000,
                Currency = "mxn",
                AutomaticPaymentMethods =
                    new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true
                    }
            };

            var service = new PaymentIntentService();

            var intent = service.Create(options);

            return Ok(new
            {
                intent.Id,
                intent.ClientSecret,
                Status = "Stripe funcionando correctamente"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message
            });
        }
    }
}