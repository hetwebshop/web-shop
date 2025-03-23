using System;

namespace API.Entities.Payment
{
    public class UserTransaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public long? MonriTransactionId { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Amount { get; set; }
        public string ChFullName { get; set; }
        public string ResponseMessage { get; set; }
        public string ResponseCode { get; set; }
        public TransactionType TransactionType { get; set; }
        public bool IsProcessed { get; set; }
        public string OrderInfo { get; set; }
        public bool IsAddingCredits { get; set; } //adding credits after paying credits, and after refund ai analyis, otherwise it is removing credits
    }

    public enum TransactionType
    {
        CreditsPurchase,//triggered when paying for credits
        PostingAd, // triggered when user post ad
        RunningAi,
        RefundAiCredits, //when AI analysis fails
    }

    public static class OrderInfoMessages
    {
        public const string CreditsPurchaseMessage = "Uplata kredita putem kartice";
        public const string PostingAdMessage = "Kupovina oglasa";
        public const string RunningAiMessage = "Aktivacija AI analize";
        public const string RefundAiCreditsMessage = "Vraćanje kredita zbog neuspjele AI analize";
    }
}
