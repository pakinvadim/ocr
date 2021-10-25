using System;
using Accusoft.FormFixSdk;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Recognition {
	public abstract class OcrZoneField {
		public event EventHandler<OcrZoneRecognizeErrorArgs> RecognizeError;

		protected OcrZoneField(OcrTemplateField field) {
			Field = field;
		}

		public OcrTemplateField Field { get; }

		protected abstract FieldResult Perform(FormImage image);

		public FieldResult Recognize(FormImage image) {
			try {
				return Perform(image);
			} catch (Exception exception) {
				InvokeHandler(exception);
				return null;
			}
		}

		private void InvokeHandler(Exception exception) {
			EventHandler<OcrZoneRecognizeErrorArgs> handler = RecognizeError;
			handler?.Invoke(this, new OcrZoneRecognizeErrorArgs(exception));
		}
	}
}