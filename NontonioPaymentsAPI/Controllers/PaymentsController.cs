using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
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

    [HttpPost("create-checkout-session")]
    public IActionResult CreateCheckoutSession(
        [FromBody] PaymentIntentRequest request)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment",

            SuccessUrl =
                "https://nontoniopaymentsapi.onrender.com/api/payments/success",

            CancelUrl =
                "https://nontoniopaymentsapi.onrender.com/api/payments/cancel",

            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Quantity = 1,

                    PriceData =
                        new SessionLineItemPriceDataOptions
                        {
                            Currency = "mxn",

                            UnitAmount =
                                (long)(request.Amount * 100),

                            ProductData =
                                new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Pedido NONTONIO"
                                }
                        }
                }
            }
        };

        var service = new SessionService();

        var session = service.Create(options);

        return Ok(new
        {
            url = session.Url
        });
    }

    [HttpGet("success")]
    public IActionResult Success()
    {
        return Content(
            "Pago realizado correctamente.");
    }

    [HttpGet("cancel")]
    public IActionResult Cancel()
    {
        return Content(
            "Pago cancelado.");
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
