namespace CrpDbCalls
{
    public class ChecklistItem
    {
        public int Id { get; set; }
        public bool Checked { get; set; }

        public int VehicleId { get; set; }

        public ChecklistItemCode DefinitionId { get; set; }

        public bool Active { get; set; }

        public DateTime Activated { get; set; }
        
    }

    public enum ChecklistItemCode
    {
        None = 0,
        DateConfirmed,
        TransferProtocol,
        InspectionOrder,
        InspectionReportRequest, // Not used, but don't delete!
        InspectionReportDelivered,
        Res24,
        ServiceSale,
        B2BRefPriceEntered,
        CRISale,
        AuctionEvaluation,
        ForComments,
        NOLSSale,
        ProformaIssued,
        PublishedOnWeb,
        Inquiry,
        Preparation2Order,
        Preparation2Delivered,
        DocumentsSent,
        DocumentsSupplied,
        TCToClient,
        PriceApprovalRequested,
        PriceApprovalAccepted,
        ClientRejected,
        ProformaPaid,
        RIOrdered,
        RIDone,
        B2BRefPriceApproved,
        CalculationOrdered,
        CalculationDelivered,
        Preparation1Order,
        Preparation1Delivered,
        B2DPriceSet,
        B2DPriceApproved,
        SalePriceAccepted,
        ContractIssued,
        POASupplied,
        TransferOrder,
        ContractSigned,
        ClientPOAIssued,
        ClientPOASigned,
        SentToCRI,
        AuctionRequested,
        EnteredIntoAuction,
        POAIssued,
        TransferRequest,
        Transferred,
        POAAndCodesSent,
        Exported,
        TransferProtocolSigned,
        HandedOver,
        CMRConfirmed,
        Transport,
        ClientsComplaints,
        TCSent,
        InvoiceIssued,
        AutionBidEmailSent,
        OwnerFeedbackPositive,
        OwnerFeedbackNegative,
        SaleConfirmation,
        MoneyReturned,
        TransferredBack,
        TakenOver,
        AuctionResultEntered,
        AuctionResultOK,
        SystemPriceSet,
        DecisionMatrixOK,
        DecisionRiskOK,
        DecisionLPCOK,
        OfferSent,
        OfferAccepted,
        DiscountNegotiating,
        OfferRejected,
        B2CFeeEntered,
        EmployeeReservation,
        VatRefundRequest,
        ContractSignedByClient,
        ContractSignedBySeller
    }
}