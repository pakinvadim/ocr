using WebSupergoo.ABCpdf11;
using WebSupergoo.ABCpdf11.Operations;

namespace Utilities.Pdf {
	public static class PdfDocTextExtensions {
		public static bool ContainsText(this Doc document, string text) {
			var op = new TextOperation(document);
			op.PageContents.AddPages();
			return op.GetText().Contains(text);
		}
	}
}