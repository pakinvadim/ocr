using System.Drawing;
using System.Linq;
using Accusoft.SmartZoneOCRSdk;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Recognition {
	public sealed class OcrResult : FieldResult {
		public override Rectangle Area { get; }

		public TextBlockResult Result { get; }

		public override string Text { get; }

		public override Rectangle Location { get; }

		public OcrResult(OcrTemplateField field, TextBlockResult result) : base(field) {
			Result = result;
			Text = CleanOcrResults(result.Text);
			Location = new Rectangle(field.Location.X + result.Area.X, field.Location.Y + result.Area.Y, field.Location.Width, field.Location.Height);
			Area = result.Area;
		}

		private static string CleanOcrResults(string result) {
			result = result.Replace("~", "").Trim();
			char[] trimCharAtBegin = { ':', ',', '.', '|', 'i', 'I' };

			int pos = 0;
			for (int i = 0; i < result.Length - 1; i++) {
				if (trimCharAtBegin.Contains(result[i]) && result[i + 1] == ' ') {
					for (int j = i + 1; j < result.Length; j++) {
						if (result[j] != ' ') {
							pos = j;
							i = j - 1;
							break;
						}
					}
				} else {
					break;
				}
			}

			if (pos != 0) {
				result = result.Substring(pos);
			}

			return result;
		}
	}
}