using System.Collections.Generic;
using System.IO;
using Vintasoft.Imaging;
using Vintasoft.Imaging.FormsProcessing;
using Vintasoft.Imaging.FormsProcessing.FormRecognition;
using Vintasoft.Imaging.FormsProcessing.FormRecognition.Ocr;
using Vintasoft.Imaging.FormsProcessing.TemplateMatching;
using Vintasoft.Imaging.ImageProcessing;
using Vintasoft.Imaging.Ocr;
using Vintasoft.Imaging.Ocr.Tesseract;

namespace Appulate.Ocr.VintaSoft.Core {
	public class VintaSoftIndetificator {
		private readonly FormRecognitionManager _formRecognitionManager;
		private readonly FormTemplateManager _formTemplateManager;
		private readonly KeyZoneRecognizerCommand[] _selectedKeyZoneRecognizerCommands;
		private static readonly bool AutomaticallyImageBackgroundCompensation = false;

		public VintaSoftIndetificator(string templatePath) {
			var keyZoneRecognizerCommands = new KeyZoneRecognizerCommand[] {
				new KeyLineRecognizerCommand(),
				new KeyMarkRecognizerCommand()
			};
			_selectedKeyZoneRecognizerCommands = new[] {
				keyZoneRecognizerCommands[0]
			};

			var ocrEngine = new TesseractOcr(".");
			OcrFieldTemplate.OcrEngineManager = new OcrEngineManager(ocrEngine);

			_formRecognitionManager = new FormRecognitionManager();
			_formRecognitionManager.TemplateMatching.ImageImprintGenerator = CreateImageImprintGenerator(imagePreprocessing: null);
			_formRecognitionManager.ImageRecognitionStarted += FormRecognitionTask_RecognitionStarted;
			_formRecognitionManager.ImageRecognitionFinished += FormRecognitionTask_RecognitionFinished;
			_formRecognitionManager.ImageRecognitionError += FormRecognitionTask_RecognitionError;
			_formRecognitionManager.RecognitionProgress += FormRecognitionTask_RecognitionProgress;
			_formRecognitionManager.MaxThreads = 16;
			_formRecognitionManager.TemplateMatching.MinConfidence = 0.85f;
			_formTemplateManager = _formRecognitionManager.FormTemplates;

			FormDocumentTemplate template = FormDocumentTemplate.Deserialize(templatePath);
			_formTemplateManager.LoadFromDocument(template);
		}

		public FormRecognitionResult Run(string filePath) {
			var image = new VintasoftImage(filePath);
			if (AutomaticallyImageBackgroundCompensation) {
				CompensateTemplateImagesBackground();
			}
			FormRecognitionResult result = _formRecognitionManager.Recognize(image);
			//AffineMatrix matrix = result.TemplateMatchingResult.ImageCompareResult.TransformMatrix;
			return result;
		}

		private ImageImprintGeneratorCommand CreateImageImprintGenerator(ProcessingCommandBase imagePreprocessing) {
			foreach (KeyZoneRecognizerCommand recognizerCommand in _selectedKeyZoneRecognizerCommands) {
				recognizerCommand.ImagePreprocessing = imagePreprocessing;
			}
			return new ImageImprintGeneratorCommand(_selectedKeyZoneRecognizerCommands);
		}

		private void FormRecognitionTask_RecognitionStarted(object sender, ImageEventArgs e) { }

		private void FormRecognitionTask_RecognitionFinished(object sender, FormRecognitionFinishedEventArgs e) { }

		private void FormRecognitionTask_RecognitionError(object sender, FormRecognitionErrorEventArgs e) { }

		private void FormRecognitionTask_RecognitionProgress(object sender, ProgressEventArgs e) { }

		private void CompensateTemplateImagesBackground() {
			int imageCount = _formRecognitionManager.TemplateImages.Count;
			for (var i = 0; i < imageCount; i++) {
				VintasoftImage templateImage = _formRecognitionManager.TemplateImages[i];
				FormPageTemplate pageTemplate = _formRecognitionManager.FormTemplates.GetPageTemplate(templateImage);
				_formRecognitionManager.FormTemplates.CompensateTemplateImageBackground(templateImage, pageTemplate);
			}
		}
	}
}