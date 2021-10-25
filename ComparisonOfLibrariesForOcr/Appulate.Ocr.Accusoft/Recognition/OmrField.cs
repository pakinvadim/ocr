using Accusoft.FormFixSdk;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Recognition {
	public class OmrField : OcrZoneField {
		private readonly FormModel _model;

		public OmrField(OcrTemplateField field, FormModel model) : base(field) {
			_model = model;
		}

		protected override FieldResult Perform(FormImage image) {
			using (var omrProcessor = new OmrProcessor(Workspace.FormFix)) {
				omrProcessor.ReadFromStream(Field.Recognition);
				omrProcessor.ClipArea = Field.Location;
				omrProcessor.FormModel = _model;
				return new OmrResult(Field, omrProcessor.AnalyzeField(image));
			}
		}
	}
}