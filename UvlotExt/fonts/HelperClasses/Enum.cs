namespace UvlotApp.HelperClasses
{
    public enum BootstrapAlertType
    {
        Plain = 0,
        Success = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Primary = 5
    }

    public enum NotificationPosition
    {
        TopRight = 0,
        TopLeft = 1,
        BottomRight = 2,
        BottomLeft = 3
    }

    public enum NotificationTheme
    {
        Blue = 0,
        Azure = 1,
        Orange = 2,
        Gray = 3,
        Green = 4,
        Red = 5
    }
    public enum Status
    {
        Successful = 0,
        Failed = 1
    }

    public enum Processor
    {
        Flutterwave = 0,
        Moneywave = 1,
        FlutterwaveRave = 2,
    }

    public enum TransactionType
    {
        Collections = 0,
        FundsTransfer = 1,
        PayOut = 2,
        AccountToAccount = 3
    }


    public enum TrxStatus
    {
        Successful = 0,
        Initiated = 1,
        Processing = 2,
        Cancelled = 3,
        AwaitingValidation = 4,
        Failed = 5,
    }

    public enum ChargeWith
    {
        Card = 0,
        Account = 1,
        Token = 2
    }

    public enum Mode
    {
        AlphaNumeric = 1,
        AlphaNumericUpper = 2,
        AlphaNumericLower = 3,
        AlphaUpper = 4,
        AlphaLower = 5,
        Numeric = 6
    }
    public enum HashName
    {
        SHA1 = 1,
        MD5 = 2,
        SHA256 = 4,
        SHA384 = 8,
        SHA512 = 16
    }

    public enum Relationship
    {
        Self = 1,
        Family = 2,
        Friend = 3
    }

    public enum TrxResponse
    {
        Successful = 00,
        PendingOtpvalidation = 02,
        OtpValidationFailed = 03,
        TransactionError = 050,
        InvalidMerchant = 100,
        InvalidCustomerId = 101,
        MissingTransactionAmount = 102,
        InvalidTransactionAmount = 103,
        DuplicateTransactionReference = 104,
        InvalidTransactionReference = 105,
        InvalidReturnUrl = 106,
        MissingTransactionHash = 107,
        DataIntegrityError = 108,
        InvalidGateway = 109,
        MissingTransactionReference = 110,
        MissingReturnUrl = 111,
        InvalidTransactionCurrency = 112,
        MissingTransactionCurrency = 113,
        UserCancelled = 114,
        NoSuchRequest = 115,
        TransactionFailed = 116,
        GatewayAuthenticationError = 117,
        TransactionAmountMismatch = 118,
        InvalidProductReference = 119,
        TransactionProcessing = 120,
        DataNotAvailable = 121,
        InvalidRequest = 122,
        TransactionRecordNotFound = 123,
        IssuerOrSwitchInoperative = 124,
        AuthorizationFailed = 125,
        InvalidPublikeyORSecretKey = 121
    }
}
