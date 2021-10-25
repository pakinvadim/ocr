using System;

namespace Appulate.Ocr.Accusoft.Recognition {
	public class OcrZoneRecognizeErrorArgs : EventArgs {
		public string Message { get; } = string.Empty;

		public Exception Exception { get; }

		public OcrZoneRecognizeErrorArgs(Exception exception, string message = null) {
			if (!string.IsNullOrEmpty(message)) {
				Message = message;
			}
			Exception = exception;
		}
	}
}