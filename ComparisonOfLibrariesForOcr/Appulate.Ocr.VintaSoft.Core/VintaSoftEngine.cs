using System.Collections.Generic;
using Vintasoft.Imaging.FormsProcessing;
using Vintasoft.Imaging.FormsProcessing.FormRecognition;

namespace Appulate.Ocr.VintaSoft.Core {
	public class VintaSoftEngine {
		private readonly VintaSoftIndetificator _vintaSoftIndetificator;

		public VintaSoftEngine(string templatePath) {
			_vintaSoftIndetificator = new VintaSoftIndetificator(templatePath);
		}

		public List<(int Page, string Field, string Text)> Run(string filePath) {
			FormRecognitionResult result = _vintaSoftIndetificator.Run(filePath);
			return CreateResults(result);
		}

		private List<(int Page, string Field, string Text)> CreateResults(FormRecognitionResult result) {
			if (result == null) {
				return null;
			}
			var data = new List<(int Page, string Field, string Text)>();
			foreach (FormField resultFormPage in result.RecognizedPage.Items) {
				data.Add((1,
				          resultFormPage.Name,
				          resultFormPage.Value));
			}
			return data;
		}
	}
}