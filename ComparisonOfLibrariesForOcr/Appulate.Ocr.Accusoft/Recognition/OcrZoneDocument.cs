using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Accusoft.ImagXpressSdk;
using Appulate.Ocr.Accusoft.Identification;

namespace Appulate.Ocr.Accusoft.Recognition {
	public sealed class OcrZoneDocument : IDisposable {
		private bool _disposed;
		private int _recognizedPageCount;

		public event EventHandler<ProgressEventArgs> PageRecognized;
		public event EventHandler<OcrZoneRecognizeErrorArgs> RecognizeError;

		public Guid TemplateId { get; }
		public OcrZonePage[] Pages { get; }

		public OcrZoneDocument(Guid templateId,
		                       FormIdentificationResult[] results) {
			TemplateId = templateId;
			Pages = results.Select(result => new OcrZonePage(result)).ToArray();
		}

		public void Recognize() {
			if (_recognizedPageCount != 0) {
				return;
			}
			Task.WaitAll(CreateRecognizeTasks());
		}

		private Task[] CreateRecognizeTasks() {
			return Pages.Select(page => Task.Run(() => {
				try {
					page.RecognizeError += (_, args) => InvokeError(args.Exception, page.PageId, args.Message);
					page.Recognize();
				} catch (Exception ex) {
					InvokeError(ex, page.PageId);
				} finally {
					Interlocked.Increment(ref _recognizedPageCount);
				}
			})).ToArray();
		}

		private void InvokeError(Exception exception, Guid pageId, string message = null) {
			RecognizeError?.Invoke(this, new OcrZoneRecognizeErrorArgs(exception, $"Document: {TemplateId}{Environment.NewLine}" +
			                                                                      $"Page: {pageId}{Environment.NewLine}" +
			                                                                      $"{message}"));
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					foreach (OcrZonePage ocrZonePage in Pages) {
						ocrZonePage.Dispose();
					}
				}
				_disposed = true;
			}
		}

		~OcrZoneDocument() {
			Dispose(false);
		}
	}
}