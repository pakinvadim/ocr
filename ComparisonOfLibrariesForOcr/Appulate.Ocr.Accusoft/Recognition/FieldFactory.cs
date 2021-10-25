using System;
using Accusoft.FormFixSdk;
using Accusoft.SmartZoneOCRSdk;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Recognition {
	public class FieldFactory : IDisposable {
		private readonly FormModel _model;
		private readonly SmartZoneOCR _smartZoneOcr;
		private bool _disposed;

		public FieldFactory(FormModel model) {
			_model = model;
			_smartZoneOcr = Workspace.CreateSmartZoneOcr();
		}

		public OcrZoneField Create(OcrTemplateField field) {
			if (field.Type == FieldTypes.Ocr) {
				return new OcrField(field, _smartZoneOcr);
			}

			if (field.Type == FieldTypes.Omr) {
				return new OmrField(field, _model);
			}

			throw new NotSupportedException("Field is not supported");
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					_smartZoneOcr.Dispose();
				}
				_disposed = true;
			}
		}

		~FieldFactory() {
			Dispose(false);
		}
	}
}