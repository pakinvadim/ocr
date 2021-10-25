using System;
using Accusoft.FormFixSdk;
using Appulate.Ocr.Accusoft;

namespace Appulate.Ocr.Forms {
	public class OcrFormModel : FormModel {
		public OcrForm FormDefinition { get; }

		private bool _disposed;

		public OcrFormModel(OcrForm formDefinition) : base(Workspace.FormFix) {
			try {
				FormDefinition = formDefinition;
				ReadDataItem += ReadDataItemHandler;
				ReadFormImage += ReadFormImageHandler;
				ReadChecksum += ReadChecksumHandler;
				WriteDataItem += WriteDataItemHandler;
			} catch (Exception exception) {
				throw Exception(exception);
			}
		}

		private void ReadDataItemHandler(object sender, DataItemEventArgs e) {
			try {
				switch (e.Type) {
					case FormConst.InternalIdentificationOp:
						e.Content = FormDefinition.IdentificationHashCode;
						break;
					default:
						throw new NotSupportedException(e.Type);
				}
			} catch (Exception exception) {
				throw Exception(exception);
			}
		}

		private void WriteDataItemHandler(object sender, DataItemEventArgs e) {
			try {
				switch (e.Type) {
					case FormConst.InternalIdentificationOp:
						FormDefinition.IdentificationHashCode = e.Content;
						break;
					default:
						throw new NotSupportedException(e.Type);
				}
			} catch (Exception exception) {
				throw Exception(exception);
			}
		}

		private void ReadChecksumHandler(object sender, ReadChecksumEventArgs args) {
			try {
				args.Checksum = FormDefinition.PartialHashCode;
			} catch (Exception exception) {
				throw Exception(exception);
			}
		}

		private void ReadFormImageHandler(object sender, ReadFormImageEventArgs e) {
			try {
				e.FormImage = FormDefinition.ReadImage();
			} catch (Exception exception) {
				throw Exception(exception);
			}
		}

		private Exception Exception(Exception exception) {
			return new Exception($"Failed to create form model {FormDefinition.PageId}", exception);
		}

		protected override void Dispose(bool disposing) {
			if (!_disposed) {
				base.Dispose(disposing);
				_disposed = true;
			}
		}
	}
}