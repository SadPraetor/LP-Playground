

using System.Text.Json.Serialization;

public class StoreMessage
    {
        [JsonPropertyName("variables")]
        public IDictionary<string, object> Variables { get; set; }

        public IDictionary<string, object> MapModelProviders(IDictionary<string, object> additionalProviders)
        {
            additionalProviders.Add("Message", this);
            return additionalProviders;
        }
    }

    // TODO using also for templating check if some properties are "JsonIgnore"

    public class EmailMessage : StoreMessage
    {
        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("culture")]
        public string Culture { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("cc")]
        public string Cc { get; set; }

        [JsonPropertyName("bcc")]
        public string Bcc { get; set; }

        [JsonPropertyName("replyTo")]
        public string ReplyTo { get; set; }

        [JsonPropertyName("body")]
        public required StoreDocument Body { get; set; } = new();

        [JsonPropertyName("noSendBefore")]
        public string NoSendBeforeTemplate { get; set; }

        [JsonIgnore]
        public DateTime NoSendBefore { get; set; }

        [JsonPropertyName("attachments")]
        public IEnumerable<EmailAttachment>? Attachments { get; set; }

        [JsonPropertyName("attachment_ids")]
        public List<int>? AttachmentIds { get; set; }
    }

   
    public class DocumentMessage : StoreMessage
    {
        [JsonPropertyName("document")]
        public StoreDocument Document { get; set; } = new();
    }

    public class StoreDocument
    {
        [JsonPropertyName("html")]
        public string? Html { get; set; }

        [JsonPropertyName("format")]
        public string? Format { get; set; }

        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        [JsonPropertyName("def")]
        public IEnumerable<StorePage>? Def { get; set; }
    }

    public class StorePage
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("template")]
        public string? Template { get; set; }
    }

    public class EmailAttachment
    {
        [JsonPropertyName("application_type")]
        public ApplicationType? ApplicationType { get; set; }

        

    }

