using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Appulate.Ocr.Accusoft.Identification;

namespace Appulate.Ocr.Forms {
	[DataContract(Name = "FormSet")]
	public class OcrFormSet : FormSet {
		private const string FileName = "form-ocr.xml";

		[DataMember(Name = "Forms")]
		private readonly List<OcrForm> _forms = new ();
		[DataMember(Name = "Identification")]
		public string Identification { get; set; }
		[DataMember(Name = "Enhancement")]
		public string Enhancement { get; set; }
		[DataMember(Name = "MinOffsetY")]
		public int? MinOffsetY { get; set; }
		[DataMember(Name = "MaxOffsetY")]
		public int? MaxOffsetY { get; set; }
		[DataMember(Name = "ComparableAcords")]
		public List<OcrAcordType> ComparableAcords { get; private set; } = new ();
		[DataMember(Name = "SyncId")]
		public Guid TemplateSyncId { get; private set; }
		public string FilePath { get; set; }
		public override bool MultipleMatchesEnabled => false;
		public override OcrForm[] Forms => _forms.ToArray();

		public static OcrFormSet Load(string templatePath, string imagesFolder) {
			if (!templatePath.EndsWith(".xml")) {
				templatePath = Path.Combine(templatePath, FileName);
			}
			using (FileStream stream = File.OpenRead(templatePath)) {
				var serializer = new DataContractSerializer(typeof(OcrFormSet));
				var formSet = (OcrFormSet)serializer.ReadObject(stream);
				formSet.FilePath = templatePath;
				foreach (OcrForm form in formSet.Forms) {
					form.TemplatePath = Path.GetDirectoryName(templatePath);
					form.ImagesFolder = imagesFolder;
					form.ComparableAcords = formSet.ComparableAcords;
					form.MaxOffsetY = formSet.MaxOffsetY;
					form.MinOffsetY = formSet.MinOffsetY;
				}
				return formSet;
			}
		}
	}
}