namespace Appulate.Ocr.Forms {
	public interface IFormHeadSet : IFormSet {
		bool RequiresSavingHashFile { get; }
		void SaveHashToFile();
	}
}