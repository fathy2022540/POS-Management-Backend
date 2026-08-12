namespace POS.Infrastructure.Services.DocumentAnalysis
{
    public interface IDocumentAnalysisService
    {
        /// <summary>
        /// Analyzes a base64-encoded document and returns true if the expectedText is found within it.
        /// Used to validate that a Tax Number or Commercial Registration Number exists in the uploaded document.
        /// </summary>
        /// <param name="base64FileContent">The base64-encoded document content (PDF or image).</param>
        /// <param name="expectedText">The text to look for (e.g. Tax Number or CR Number).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<bool> ValidateDocumentContainsTextAsync(string base64FileContent, string expectedText, CancellationToken cancellationToken = default);
    }
}
