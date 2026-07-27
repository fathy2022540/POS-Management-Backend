using Microsoft.Extensions.Configuration;
using Mscc.GenerativeAI;

namespace JRM.Infrastructure.Services.DocumentAnalysis
{
    public class DocumentAnalysisService : IDocumentAnalysisService
    {
        private readonly GenerativeModel _model;

        public DocumentAnalysisService(IConfiguration configuration)
        {
            var apiKey = configuration["GeminiSettings:ApiKey"]
                ?? throw new InvalidOperationException("GeminiSettings:ApiKey is not configured.");

            var modelName = configuration["GeminiSettings:Model"] ?? "gemini-2.0-flash";

            var googleAI = new GoogleAI(apiKey);
            _model = googleAI.GenerativeModel(model: modelName);
        }

        /// <summary>
        /// Sends a base64-encoded document to Gemini Pro as an inline multimodal part,
        /// and asks it whether the expectedText (Tax Number or CR Number) appears in the document.
        /// Returns true only when Gemini answers "yes".
        /// </summary>
        public async Task<bool> ValidateDocumentContainsTextAsync(
            string base64FileContent,
            string expectedText,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(base64FileContent) || string.IsNullOrWhiteSpace(expectedText))
                return false;

            // Detect whether the content is a PDF or an image from its base64 magic bytes
            var mimeType = DetectMimeType(base64FileContent);

            var prompt = $"Does this document contain the number or text \"{expectedText}\"? " +
                         $"Answer with only YES or NO.";

            var parts = new List<IPart>
            {
                new InlineData { MimeType = mimeType, Data = base64FileContent },
                new TextData { Text = prompt }
            };

            var response = await _model.GenerateContent(parts);

            var answer = response?.Text?.Trim() ?? string.Empty;

            return answer.StartsWith("YES", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Detects the MIME type from the first bytes of the base64-encoded content.
        /// Supports PDF and the most common image formats.
        /// </summary>
        private static string DetectMimeType(string base64Content)
        {
            try
            {
                // Decode only the first 8 bytes — enough to read the magic number
                var headerBytes = Convert.FromBase64String(
                    base64Content[..Math.Min(base64Content.Length, 12)] + "==");

                // PDF: %PDF  (25 50 44 46)
                if (headerBytes.Length >= 4 &&
                    headerBytes[0] == 0x25 && headerBytes[1] == 0x50 &&
                    headerBytes[2] == 0x44 && headerBytes[3] == 0x46)
                    return "application/pdf";

                // PNG: 89 50 4E 47
                if (headerBytes.Length >= 4 &&
                    headerBytes[0] == 0x89 && headerBytes[1] == 0x50 &&
                    headerBytes[2] == 0x4E && headerBytes[3] == 0x47)
                    return "image/png";

                // JPEG: FF D8
                if (headerBytes.Length >= 2 &&
                    headerBytes[0] == 0xFF && headerBytes[1] == 0xD8)
                    return "image/jpeg";
            }
            catch
            {
                // Fall through to default
            }

            // Default to PDF for unknown types
            return "application/pdf";
        }
    }
}
