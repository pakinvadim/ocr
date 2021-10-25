using Accusoft.FormFixSdk;
using Accusoft.SmartZoneOCRSdk;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Recognition {
	public class OcrField : OcrZoneField {
		private readonly SmartZoneOCR _smartZoneOcr;

		public OcrField(OcrTemplateField field, SmartZoneOCR smartZoneOcr) : base(field) {
			_smartZoneOcr = smartZoneOcr;
		}

		protected override FieldResult Perform(FormImage image) {
			_smartZoneOcr.ReadFromStream(Field.Recognition);
			_smartZoneOcr.Reader.MinimumCharacterConfidence = 1;
			return new OcrResult(Field, _smartZoneOcr.Reader.AnalyzeField(image));
		}
	}
}