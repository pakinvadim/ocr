using System;
using System.Runtime.Serialization;
using Accusoft.FormDirectorSdk;

namespace Appulate.Ocr.Forms {
	[DataContract(Name = "OcrField")]
	public class OcrField : OcrTemplateField {
		public OcrField(Guid fieldId) : base(fieldId) { }

		public OcrField(Field field) : base(field) {
			foreach (DataItem item in field.Operations) {
				if (item.Type == FormConst.OcrOp) {
					Recognition = item.Content;
					continue;
				}
				throw new NotSupportedException(item.Type);
			}
		}

		public override FieldTypes Type => FieldTypes.Ocr;
	}
}