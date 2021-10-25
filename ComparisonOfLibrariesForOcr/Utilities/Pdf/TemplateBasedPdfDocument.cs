namespace Utilities.Pdf {
	public class TemplateBasedPdfDocument {
		public TemplateBasedPdfDocument(string name, byte[] content) {
			Name = name;
			Content = content;
		}

		public string Name { get; }
		public byte[] Content { get; }
	}
}