using System.IO;
using Leadtools;
using Leadtools.Barcode;
using Leadtools.Caching;
using Leadtools.Codecs;
using Leadtools.Document;
using Leadtools.Forms.Auto;
using Leadtools.Forms.Common;
using Leadtools.ImageProcessing;
using Leadtools.ImageProcessing.Core;
using Leadtools.Ocr;

namespace Appulate.Ocr.LeadTools.Core {
	public class LeadToolsIdentificator {
		private readonly IOcrEngine _ocrEngine;
		private readonly IOcrEngine _cleanUpOcrEngine;
		private readonly BarcodeEngine _barcodeEngine;
		private readonly AutoFormsEngine _autoEngine;
		private readonly LoadDocumentOptions _loadDocumentOptions;
		private const string CachePath = @"C:\LEADTOOLS22\Resources\Images\Forms\Forms to be Recognized\OCR\cache";

		public LeadToolsIdentificator(string templatePath) {
			_ocrEngine = OcrEngineManager.CreateEngine(OcrEngineType.LEAD);
			_ocrEngine.Startup(rasterCodecs: null,
			                   documentWriter: null,
			                   workDirectory: null,
			                   startupParameters: null);
			_ocrEngine.SettingManager.SetEnumValue("Recognition.Fonts.DetectFontStyles", 0);
			_ocrEngine.SettingManager.SetBooleanValue("Recognition.Fonts.RecognizeFontAttributes", false);
			if (_ocrEngine.SettingManager.IsSettingNameSupported("Recognition.RecognitionModuleTradeoff")) {
				_ocrEngine.SettingManager.SetEnumValue("Recognition.RecognitionModuleTradeoff", "Accurate");
			}
			// if (_ocrEngine.SettingManager.IsSettingNameSupported("Recognition.Zoning.EnableDoubleZoning")) {
			// 	_ocrEngine.SettingManager.SetBooleanValue("Recognition.Zoning.EnableDoubleZoning", false);
			// }

			_cleanUpOcrEngine = OcrEngineManager.CreateEngine(OcrEngineType.LEAD);
			_cleanUpOcrEngine.Startup(null, null, null, null);

			_barcodeEngine = new BarcodeEngine();

			_loadDocumentOptions = LoadDocumentOptionsWithCache();

			var rasterCodecs = new RasterCodecs();
			//To turn off the dithering method when converting colored images to 1-bit black and white image during the load
			//so the text in the image is not damaged.
			RasterDefaults.DitheringMethod = RasterDitheringMethod.None;
			//To ensure better results from OCR engine, set the loading resolution to 300 DPI
			rasterCodecs.Options.Load.Resolution = 300;
			rasterCodecs.Options.RasterizeDocument.Load.Resolution = 300;

			var workingRepository = new DiskMasterFormsRepository(rasterCodecs, templatePath);
			var managers = AutoFormsRecognitionManager.Ocr;
			//managers |= AutoFormsRecognitionManager.Barcode;
			//managers |= AutoFormsRecognitionManager.Default;
			//managers |= AutoFormsRecognitionManager.None;
			_autoEngine = new AutoFormsEngine(workingRepository,
			                                  _ocrEngine, _barcodeEngine, managers,
			                                  minimumConfidenceKnownForm: 30,
			                                  minimumConfidenceRecognized: 80,
			                                  recognizeFirstPageOnly: true);
			_autoEngine.UseThreadPool = true;
			//_autoEngine.MinimumConfidenceKnownForm = 40;
			_autoEngine.FilledFormType = FormsPageType.Normal;
		}

		public AutoFormsRunResult Run(string filePath) {
			LEADDocument leadDocument = DocumentFactory.LoadFromFile(filePath, _loadDocumentOptions);
			leadDocument.Text.ImagesRecognitionMode = DocumentTextImagesRecognitionMode.Always;
			leadDocument.Text.OcrEngine = _ocrEngine;

			Cleanup(leadDocument);

			AutoFormsRunResult result = _autoEngine.Run(leadDocument, categories: null);
			// AutoFormsRecognizeFormResult result2 = _autoEngine.RecognizeForm(leadDocument, null);
			// AutoFormsRecognizeFormResult result3 = _autoEngine.RecognizeForm(leadDocument, null, null, null);

			return result;
		}

		private void Cleanup(LEADDocument leadDocument) {
			if (!leadDocument.Images.IsSvgViewingPreferred) {
				leadDocument.IsReadOnly = false;
				foreach (DocumentPage page in leadDocument.Pages) {
					RasterImage image = page.GetImage();
					image.ChangeViewPerspective(RasterViewPerspective.TopLeft);
					CleanupImage(image, "", 1, image.PageCount);
					page.SetImage(image);
				}
			}
		}

		private void CleanupImage(RasterImage imageToClean, string fileName, int startIndex, int count) {
			if (_cleanUpOcrEngine.IsStarted) {
				using (IOcrDocument document = _cleanUpOcrEngine.DocumentManager.CreateDocument()) {
					for (int i = startIndex; i < startIndex + count; i++) {
						imageToClean.Page = i;
						document.Pages.AddPage(imageToClean, null);
						int angle = -document.Pages[0].GetDeskewAngle();
						var cmd = new RotateCommand(angle * 10, RotateCommandFlags.Bicubic, RasterColor.FromKnownColor(RasterKnownColor.White));
						cmd.Run(imageToClean);
						document.Pages.Clear();
					}
				}
			} else {
				for (int i = startIndex; i < startIndex + count; i++) {
					imageToClean.Page = i;
					var deskewCommand = new DeskewCommand();
					if (imageToClean.Height > 3500) {
						deskewCommand.Flags = DeskewCommandFlags.DocumentAndPictures | DeskewCommandFlags.DoNotPerformPreProcessing |
						                      DeskewCommandFlags.UseNormalDetection | DeskewCommandFlags.DoNotFillExposedArea;
					} else {
						deskewCommand.Flags = DeskewCommandFlags.DeskewImage | DeskewCommandFlags.DoNotFillExposedArea;
					}
					deskewCommand.Run(imageToClean);
				}
			}
		}

		private LoadDocumentOptions LoadDocumentOptionsWithCache() {
			if (!Directory.Exists(CachePath)) {
				Directory.CreateDirectory(CachePath);
			}
			return new LoadDocumentOptions {
				Cache = new FileCache { CacheDirectory = CachePath }
			};
		}
	}
}