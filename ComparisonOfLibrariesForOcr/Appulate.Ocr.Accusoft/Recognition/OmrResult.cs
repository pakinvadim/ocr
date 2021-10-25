using System.Drawing;
using Accusoft.FormFixSdk;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Recognition {
	public sealed class OmrResult : FieldResult {
		public OmrFieldResult Result { get; }
		public override Rectangle Area { get; }
		public override string Text => Result.Text;
		public override Rectangle Location { get; }

		public OmrResult(OcrTemplateField field,
		                 OmrFieldResult result)
			: base(field) {
			Result = result;
			Rectangle bubbleRect = Result.Segments[0].Bubbles[0].Area;
			Area = bubbleRect;
			Location = new Rectangle(field.Location.X + bubbleRect.X / 2,
			                         field.Location.Y + bubbleRect.Y / 2,
			                         bubbleRect.Width,
			                         bubbleRect.Height);
		}
	}
}