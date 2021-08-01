using System;
using System.Text.Json.Serialization;

namespace Card_issuer_country.Models
{
    public class ConfirmPaymentRequest
    {
        [JsonPropertyName("payment_method_id")]
        public string PaymentMethodId { get; set; }

        [JsonPropertyName("payment_intent_id")]
        public string PaymentIntentId { get; set; }
    }
}