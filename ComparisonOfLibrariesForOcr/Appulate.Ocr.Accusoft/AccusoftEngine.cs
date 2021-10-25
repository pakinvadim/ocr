using System;
using System.Collections.Generic;
using System.Linq;
using Appulate.Ocr.Accusoft.Identification;
using Appulate.Ocr.Accusoft.Recognition;

namespace Appulate.Ocr.Accusoft {
	public class AccusoftEngine {
		private readonly FormIdentificationProcessor _identificator;

		public AccusoftEngine(string templatePath) {
			_identificator = new FormIdentificationProcessor(templatePath);
		}

		public List<(int Page, string Field, string Text)> Run(string filePath) {
			FormIdentificationResult[] result = _identificator.Run(filePath);

			OcrZoneDocument[] documents = result.GroupBy(r => r.Model.FormDefinition.TemplateSyncId)
			                                    .Select(g => new OcrZoneDocument(g.Key, g.ToArray()))
			                                    .ToArray();
			var collection = new OcrDocumentCollection(documents);
			Recognition(collection);

			return CreateResults(collection);
		}

		private void Recognition(OcrDocumentCollection documents) {
			foreach (OcrZoneDocument ocrZoneDocument in documents.Items) {
				ocrZoneDocument.Recognize();
			}
		}

		private List<(int Page, string Field, string Text)> CreateResults(OcrDocumentCollection collection) {
			var data = new List<(int Page, string Field, string Text)>();
			foreach (OcrZonePage resultFormPage in collection.Items.SelectMany(d => d.Pages)) {
				foreach (KeyValuePair<Guid, FieldResult> field in resultFormPage.Results) {
					data.Add((resultFormPage.PageNum,
					          field.Key.ToString(),
					          field.Value?.Text));
				}
			}
			return data;
		}
	}
}