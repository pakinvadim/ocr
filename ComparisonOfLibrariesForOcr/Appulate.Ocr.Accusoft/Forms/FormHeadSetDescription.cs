using System.IO;
using System.Runtime.Serialization;

namespace Appulate.Ocr.Forms {
	[DataContract(Name = "HeadSet")]
	public class FormHeadSetDescription {
		[DataMember]
		public string ImageHashCode { get; set; }
		[DataMember]
		public string IdentificationHashCode { get; set; }

		public static FormHeadSetDescription Load(string fileName) {
			using (var stream = new MemoryStream(File.ReadAllBytes(fileName))) {
				var serializer = new DataContractSerializer(typeof(FormHeadSetDescription));
				return (FormHeadSetDescription)serializer.ReadObject(stream);
			}
		}

		public void Save(string fileName) {
			var serializer = new DataContractSerializer(GetType());
			using (var stream = new MemoryStream()) {
				serializer.WriteObject(stream, this);
				File.WriteAllBytes(fileName, stream.ToArray());
			}
		}
	}
}