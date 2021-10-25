using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Appulate.Ocr.Forms {
	[DataContract(Name = "HeadSet")]
	public class FormHeadSet : FormSet, IFormHeadSet {
		private static readonly object Lock = new ();
		private readonly string _headSetHashFilePath;

		public bool RequiresSavingHashFile { get; private set; } = true;

		public override bool MultipleMatchesEnabled => true;

		public override OcrForm[] Forms { get; }

		public FormHeadSet(string templatesFolder, string resourcesFolder, string webSiteUrl) {
			_headSetHashFilePath = Path.Combine(templatesFolder, $"ocrhash-{webSiteUrl}.xml");
			var templatesDirectory = new DirectoryInfo(templatesFolder);

			Forms = templatesDirectory.GetFiles("form-ocr.xml", SearchOption.AllDirectories)
			                          .Select(f => OcrFormSet.Load(f.FullName, $@"{resourcesFolder}\{f.Directory.Name}"))
			                          .SelectMany(form => form.Forms)
			                          .ToArray();
			ImageHashCode = CalculateImageHashCode();
			LoadHashFile();
		}

		private void LoadHashFile() {
			lock (Lock) {
				if (File.Exists(_headSetHashFilePath)) {
					FormHeadSetDescription description = FormHeadSetDescription.Load(_headSetHashFilePath);

					if (description.IdentificationHashCode != null && description.ImageHashCode == ImageHashCode) {
						IdentificationHashCode = description.IdentificationHashCode;
						RequiresSavingHashFile = false;
					}
				}
			}
		}

		public void SaveHashToFile() {
			lock (Lock) {
				if (IdentificationHashCode == null) {
					return;
				}
				var description = new FormHeadSetDescription {
					IdentificationHashCode = IdentificationHashCode,
					ImageHashCode = ImageHashCode
				};
				description.Save(_headSetHashFilePath);
				RequiresSavingHashFile = false;
			}
		}
	}
}