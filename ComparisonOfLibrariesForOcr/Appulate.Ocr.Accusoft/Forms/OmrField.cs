using System;
using System.Runtime.Serialization;
using Accusoft.FormDirectorSdk;

namespace Appulate.Ocr.Forms {
	[DataContract(Name = "OmrField")]
	public class OmrField : OcrTemplateField {
		public OmrField(Guid fieldId) : base(fieldId) { }

		public OmrField(Field field) : base(field) {
			foreach (DataItem item in field.Operations) {
				if (item.Type == FormConst.OmrOp) {
					Recognition = item.Content;
					continue;
				}
				throw new NotImplementedException(item.Type);
			}
		}

		public override FieldTypes Type => FieldTypes.Omr;
	}
}