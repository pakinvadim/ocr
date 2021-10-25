using System.Linq;
using System.Runtime.Serialization;
using Appulate.Ocr.Accusoft;

namespace Appulate.Ocr.Forms {
	[DataContract]
	[KnownType(typeof(OcrFormSet))]
	[KnownType(typeof(FormHeadSet))]
	public abstract class FormSet : IFormSet {
		[DataMember(Name = "ImageHashCode")]
		public string ImageHashCode { get; protected set; }

		[DataMember(Name = "IdentificationHashCode")]
		public string IdentificationHashCode { get; set; }

		public abstract bool MultipleMatchesEnabled { get; }

		public abstract OcrForm[] Forms { get; }

		protected string CalculateImageHashCode() {
			return CommonHashCodeFunctions.GetMd5Hash(string.Join(string.Empty, Forms.Select(form => form.ImageHashCode)));
		}
	}
}