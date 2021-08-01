using System;
using System.Text.Json.Serialization;

namespace CardIssuerCountry
{
    public class ConfirmPaymentRequest
    {
        [JsonPropertyName("payment_method_id")]
        public string PaymentMethodId { get; set; } = null!;

        [JsonPropertyName("payment_intent_id")]
        public string PaymentIntentId { get; set; } = null!;
    }
}