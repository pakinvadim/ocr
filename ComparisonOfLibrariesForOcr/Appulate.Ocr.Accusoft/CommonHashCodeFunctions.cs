using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Appulate.Ocr.Accusoft {
	public static class CommonHashCodeFunctions {
		public static string GetMd5Hash(string str) {
			using (var md5Provider = new MD5CryptoServiceProvider()) {
				byte[] stateBytes = md5Provider.ComputeHash(Encoding.UTF8.GetBytes(str));
				return stateBytes.ToHexString();
			}
		}

		public static string GetMd5Hash(byte[] content) {
			using (var md5Provider = new MD5CryptoServiceProvider()) {
				byte[] stateBytes = md5Provider.ComputeHash(content);
				return stateBytes.ToHexString();
			}
		}

		public static string GetMd5Hash(Stream stream) {
			using (var md5Provider = new MD5CryptoServiceProvider()) {
				byte[] stateBytes = md5Provider.ComputeHash(stream);
				return stateBytes.ToHexString();
			}
		}

		public static string GetSha1Hash(string str) {
			using (var sha1Provider = new SHA1CryptoServiceProvider()) {
				byte[] stateBytes = sha1Provider.ComputeHash(Encoding.UTF8.GetBytes(str));
				return stateBytes.ToHexString();
			}
		}

		public static string ToHexString(this byte[] bytes) {
			return string.Concat(bytes.Select(b => b.ToString("x2")));
		}
	}
}