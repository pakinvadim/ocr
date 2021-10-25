using System;
using System.Collections.Generic;
using Accusoft.FormFixSdk;
using Accusoft.ScanFixXpressSdk;
using Appulate.Ocr.Accusoft.Identification;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Recognition {
	public sealed class OcrZonePage : IDisposable {
		private readonly FormIdentificationResult _result;
		private bool _disposed;
		private readonly Enhancements _defaultPageEnhancements = new () {
			Options = {
				new LineRemovalOptions(10, 10, 5, 1, 50),
				new DespeckleOptions(2, 2)
			}
		};

		public event EventHandler<OcrZoneRecognizeErrorArgs> RecognizeError;

		public int PageNum => _result.PageNum;
		public Guid PageId => _result.Model.FormDefinition.PageId;
		public Dictionary<Guid, FieldResult> Results { get; } = new ();
		public FormImage AlignedImage => _result.AlignedImage;
		public OcrFormModel Model => _result.Model;
		public OcrForm FormDefinition => _result.Model.FormDefinition;

		public OcrZonePage(FormIdentificationResult result) {
			_result = result;
		}

		public void Recognize() {
			OcrForm formDefinition = _result.Model.FormDefinition;
			using (ScanFix scanFix = Workspace.CreateScanFix()) {
				AlignedImage.CopyTo(scanFix);
				scanFix.ExecuteEnhancements(_defaultPageEnhancements);

				using (var cleanedImage = new FormImage(Workspace.FormFix)) {
					scanFix.TransferTo(cleanedImage);
					using (var factory = new FieldFactory(_result.Model)) {
						foreach (OcrTemplateField field in formDefinition.Fields) {
							DropOutProcessor dropOutProcessor = null;
							try {
								OcrTemplateField formField = field;
								OcrZoneField zoneField = factory.Create(formField);
								zoneField.RecognizeError += (_, args) => InvokeError(formField, args.Exception);
								using (dropOutProcessor = new DropOutProcessor(Workspace.FormFix)) {
									using (FormImage dropOutImage = DropOutField(cleanedImage, dropOutProcessor, formField)) {
										using (FormImage enhancedImage = EnhanceField(formField, dropOutImage)) {
											FieldResult result = zoneField.Recognize(enhancedImage);
											if (result != null) {
												if (Results.ContainsKey(result.Id)) {
													throw new Exception($"ResultMapping already contains key {result.Id}");
												}
												Results.Add(result.Id, result);
											}
										}
									}
								}
							} catch (Exception ex) {
								dropOutProcessor?.Dispose();
								InvokeError(field, ex);
							}
						}
					}
				}
			}
		}

		private void InvokeError(OcrTemplateField field, Exception exception) {
			RecognizeError?.Invoke(this, new OcrZoneRecognizeErrorArgs(exception, string.Format("Field: {1}{0}", Environment.NewLine, field.Id)));
		}

		private FormImage DropOutField(FormImage imageToProcess, DropOutProcessor dropOutProcessor, OcrTemplateField field) {
			dropOutProcessor.ReadFromStream(field.Construction);
			dropOutProcessor.Area = field.Location;
			DropOutResult dropOutResult = dropOutProcessor.CreateImageOfField(imageToProcess, _result.RegistrationResult);
			if (dropOutResult.Confidence <= 0) {
				throw new Exception("Dropout failed: confidence is zero");
			}
			return dropOutResult.Image;
		}

		private static FormImage EnhanceField(OcrTemplateField field, FormImage image) {
			try {
				using (ScanFix scanFix = Workspace.CreateScanFix()) {
					Enhancements enhancements = field.GetEnhancements(scanFix);
					if (enhancements != null) {
						image.TransferTo(scanFix);
						scanFix.ExecuteEnhancements(enhancements);
						scanFix.TransferTo(image);
					}
					return image;
				}
			} catch (Exception ex) {
				throw new Exception("Field Enhancement Exception: " + ex.Message, ex);
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					_result?.Dispose();
				}
				_disposed = true;
			}
		}

		~OcrZonePage() {
			Dispose(false);
		}
	}
}