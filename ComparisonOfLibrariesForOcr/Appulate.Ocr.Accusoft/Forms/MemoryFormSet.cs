using System.Linq;

namespace Appulate.Ocr.Forms {
	public class MemoryFormSet : FormSet {
		private readonly OcrForm[] _forms;

		public override bool MultipleMatchesEnabled { get; }

		public override OcrForm[] Forms => _forms;

		public MemoryFormSet(OcrForm[] forms) {
			_forms = forms;
			ImageHashCode = CalculateImageHashCode();
			MultipleMatchesEnabled = GetMultipleMatchesEnabled();
		}

		public MemoryFormSet(FormSet[] formSets) {
			_forms = formSets.SelectMany(set => set.Forms).ToArray();
			ImageHashCode = CalculateImageHashCode();
			MultipleMatchesEnabled = GetMultipleMatchesEnabled();
		}

		private bool GetMultipleMatchesEnabled() {
			return _forms.Select(f => f.TemplateSyncId).Distinct().Count() > 1;
		}
	}
}