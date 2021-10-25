using System;
using System.Drawing;
using System.Runtime.Serialization;
using Accusoft.FormDirectorSdk;
using Accusoft.ScanFixXpressSdk;

namespace Appulate.Ocr.Forms {
	[DataContract(Name = "Field")]
	[KnownType(typeof(OmrField))]
	[KnownType(typeof(OcrField))]
	public abstract class OcrTemplateField : IOcrTemplateField {
		private string _recognition;
		[DataMember(Name = "Id")]
		public Guid Id { get; set; }
		[DataMember(Name = "Location")]
		public Rectangle Location { get; set; }
		[DataMember(Name = "Construction")]
		public string Construction { get; set; }

		[DataMember(Name = "Recognition")]
		public string Recognition {
			get {
				if (string.IsNullOrWhiteSpace(_recognition)) {
					throw new InvalidOperationException("Recognition rules are not defined");
				}
				return _recognition;
			}
			set => _recognition = value;
		}

		[DataMember(Name = "Enhancements")]
		public string Enhancements { get; set; }

		public abstract FieldTypes Type { get; }

		public Enhancements GetEnhancements(ScanFix scanFix) {
			if (!string.IsNullOrEmpty(Enhancements)) {
				var enhancements = new Enhancements();
				enhancements.ReadFromStream(Enhancements, scanFix);
				return enhancements;
			}
			return null;
		}

		protected OcrTemplateField(Guid fieldId) {
			Id = fieldId;
		}

		protected OcrTemplateField(Field field) {
			Id = Guid.Parse(field.Name);
			Location = field.Location;
			Construction = field.Construction.Content;

			foreach (DataItem item in field.OtherDataItems) {
				if (item.Type == FormConst.Type) {
					continue;
				}
				throw new NotImplementedException(item.Type);
			}

			int id = field.Operations.GetIndexOfType(FormConst.EnhancementOp);
			if (id > -1) {
				Enhancements = field.Operations[id].Content;
				field.Operations.Remove(field.Operations[id]);
			}
		}
	}
}