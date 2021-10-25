using System;
using Accusoft.ImagXpressSdk;

namespace Appulate.Ocr.Accusoft.Recognition {
	public class OcrDocumentCollection : IDisposable {
		private bool _disposed;
		private int _processedDocumentCount;

		public event EventHandler<ProgressEventArgs> ResponseProgress;

		public OcrZoneDocument[] Items { get; }

		public OcrDocumentCollection(OcrZoneDocument[] documents) {
			Items = documents;
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					foreach (OcrZoneDocument document in Items) {
						document?.Dispose();
					}
				}
				_disposed = true;
			}
		}

		~OcrDocumentCollection() {
			Dispose(false);
		}
	}
}