namespace Appulate.Ocr.Forms {
	public interface IFormSet {
		string ImageHashCode { get; }
		string IdentificationHashCode { get; set; }
		bool MultipleMatchesEnabled { get; }
		OcrForm[] Forms { get; }
	}
}