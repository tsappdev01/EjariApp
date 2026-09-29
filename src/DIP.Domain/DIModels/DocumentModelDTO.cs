using System.Collections.Generic;

namespace DIP.DIModels
{
    public class DocumentModelDTO
    {
        public string status { get; set; }
        public string createdDateTime { get; set; }
        public string lastUpdatedDateTime { get; set; }
        public DIanalyzeResultDTO analyzeResult { get; set; }
    }

    public class DIanalyzeResultDTO
    {
        public string apiVersion { get; set; }
        public string modelId { get; set; }
        public List<DIdocumentsDTO> documents { get; set; }
        public string contentFormat { get; set; }
    }

    public class DIdocumentsDTO
    {
        public string docType { get; set; }
        public List<BoundingRegion> boundingRegions { get; set; }
        public Dictionary<string, DIField> fields { get; set; }
        public double confidence { get; set; }
        public List<Span> spans { get; set; }
    }

    public class DIField
    {
        public string type { get; set; }
        public string valueString { get; set; }
        public string valueDate { get; set; }
        public int valueNumber { get; set; }
        public object valueArray { get; set; }
        public string content { get; set; }
        public List<BoundingRegion> boundingRegions { get; set; }
        public decimal confidence { get; set; }
        public List<Span> spans { get; set; }
        //public List<ValueArrayItem> valueArray { get; set; }
    }

    public class BoundingRegion
    {
        public int pageNumber { get; set; }
        public List<double> polygon { get; set; }
    }

    public class Span
    {
        public int offset { get; set; }
        public int length { get; set; }
    }

    public class ValueArrayItem
    {
        public string type { get; set; }
        public ValueObject valueObject { get; set; }
    }

    public class ValueObject
    {
        public DIField name { get; set; }
        public DIField nationality { get; set; }
        public DIField number { get; set; }
        public DIField share { get; set; }
    }

    public class DocumentMatchDTO
    {
        public string ModelId { get; set; }
        public string DocumentCode { get; set; }
        public string DocumentName { get; set; }
        public decimal ConfidenceScore { get; set; }
    }

    public class InvalidDocumentDTO
    {
        public string DocumnetCode { get; set; }
        public int DocuemntId { get; set; }
        public int Attempt { get; set; }
        public List<InvalidDocumentFieldDTO> fieldsError { get; set; } = [];
        public string DocumentError { get; set; } = string.Empty;
    }

    public class InvalidDocumentFieldDTO
    {
        public string ModelId { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldType { get; set; }
        public decimal ConfidenceScore { get; set; }
        public string FieldError { get; set; } = string.Empty;
    }

    public class TenancyContractValidation
    {
        public bool TenantEmail { get; set; } = false;
        public int TenantEmailStatus { get; set; } = 0;
        public bool TenantPhone { get; set; } = false;
        public int TenantPhoneStatus { get; set; } = 0;
    }
}
