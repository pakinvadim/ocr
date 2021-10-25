using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using Accusoft.FormFixSdk;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Identification {
	public class FormIdentificationProcessor : IDisposable {
		private readonly IdentificationProcessor _processor;
		private readonly IFormSet _formSet;
		private bool _disposed;

		public FormIdentificationProcessor(string templatePath) {
			IFormSet formSet = OcrFormSet.Load(templatePath, templatePath);
			try {
				_processor = new IdentificationProcessor(Workspace.FormFix);
				_processor.IdentificationQuality = 100;
				_processor.IdentificationCertainty = 100;
				_processor.MinimumIdentificationConfidence = 70;
				_processor.IdentifyRotated90 = false;
				_processor.IdentifyRotated180 = false;
				_processor.IdentifyRotated270 = false;
				_processor.MaximumIdentificationBestMatches = formSet.MultipleMatchesEnabled ? 5 : 1;
				_processor.IncludeBestMatchesBelowConfidence = true;

				_processor.ReadChecksum += ReadChecksumHandler;
				_processor.ReadDataItem += ReadDataItemHandler;
				_processor.WriteDataItem += WriteDataItemHandler;

				_formSet = formSet;

				foreach (OcrForm formDefinition in formSet.Forms) {
					_processor.FormModels.Add(new OcrFormModel(formDefinition));
				}
			} catch {
				_processor?.Dispose();
				throw;
			}
		}

		public FormIdentificationResult[] Run(string filePath) {
			var bitmap = (Bitmap)Bitmap.FromFile(filePath);
			FormImage image = FormImage.FromBitmap(bitmap, Workspace.FormFix);
			using (IdentificationResult result = IdentifyInternal(image)) {
				return result.BestMatches.Cast<IdentificationMatch>()
				             .OrderBy(m => m.FormModelIndex)
				             .Select(m => new FormIdentificationResult(((OcrFormModel)_processor.FormModels[m.FormModelIndex]).FormDefinition,
				                                                       result.GetOtherRegistrationResult(m.FormModelIndex, m.Orientation).AlignImage(image),
				                                                       1, result.GetConfidence(m.FormModelIndex, IdentificationOrientation.NoRotation))).ToArray();
			}
		}

		private IdentificationResult IdentifyInternal(FormImage image) {
			if (_formSet.IdentificationHashCode == null) {
				LockFormSet();
			}
			try {
				return _processor.Identify(image);
			} finally {
				ReleaseFormSet();
			}
		}

		private void ReadChecksumHandler(object sender, ReadChecksumEventArgs e) {
			e.Checksum = _formSet.ImageHashCode.GetHashCode();
		}

		private void ReadDataItemHandler(object sender, DataItemEventArgs e) {
			e.Content = e.Type == FormConst.InternalIdentificationOp
				            ? _formSet.IdentificationHashCode
				            : throw new NotSupportedException(e.Type);
		}

		private void WriteDataItemHandler(object sender, DataItemEventArgs e) {
			_formSet.IdentificationHashCode = e.Type == FormConst.InternalIdentificationOp
				                                  ? e.Content
				                                  : throw new NotSupportedException(e.Type);
			ReleaseFormSet();
		}

		private void LockFormSet() {
			Monitor.Enter(_formSet);
		}

		private void ReleaseFormSet() {
			if (Monitor.IsEntered(_formSet)) {
				Monitor.Exit(_formSet);
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					foreach (FormModel form in _processor.FormModels) {
						form?.Dispose();
					}
					_processor.Dispose();
				}
				_disposed = true;
			}
		}

		~FormIdentificationProcessor() {
			Dispose(false);
		}
	}
}