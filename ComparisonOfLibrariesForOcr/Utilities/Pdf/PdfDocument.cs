using System;
using System.Text;
using WebSupergoo.ABCpdf11;
using WebSupergoo.ABCpdf11.Objects;

namespace Utilities.Pdf {
	public sealed class PdfDocument : IDisposable {
		private bool _disposed;

		public Doc Doc { get; private set; }

		public int PageCount => Doc.PageCount;

		internal PdfDocument() {
			Doc = AbcPdfWrapper.CreateDocument();
		}

		public void Detach() {
			if (Doc != null && !_disposed) {
				Doc = null;
				_disposed = true;
			} else {
				throw new InvalidOperationException("There's no attached document.");
			}
		}

		/// <remarks>
		///     The PdfDocument object calls Dispose() on the provided Doc object when PdfDocument.Dispose is called
		/// </remarks>
		/// <param name="document">The document to be wrapped.</param>
		internal PdfDocument(Doc document) {
			Doc = document;
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool isDisposing) {
			if (_disposed) {
				return;
			}
			_disposed = true;
			if (isDisposing) {
				Doc doc = Doc;
				doc?.Dispose();
			}
			Doc = null;
		}

		~PdfDocument() {
			Dispose(false);
		}

		public byte[] GetData() {
			return Doc.GetData();
		}

		public string GetPageText(int pageNumber, bool includeAnnotations) {
			int previousPage = Doc.PageNumber;
			try {
				Doc.PageNumber = pageNumber;
				return Doc.GetText(Page.TextType.Text, includeAnnotations);
			} finally {
				Doc.PageNumber = previousPage;
			}
		}

		public string GetText(bool includeAnnotations) {
			var sb = new StringBuilder();
			for (int i = 1; i <= Doc.PageCount; i++) {
				Doc.PageNumber = i;
				sb.Append(Doc.GetText(Page.TextType.Text, includeAnnotations));
			}
			return sb.ToString();
		}
	}
}