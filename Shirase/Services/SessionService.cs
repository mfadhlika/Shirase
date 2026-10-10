using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Shirase.Services
{
    public class SessionService
    {
        private byte[]? _encryptedSecret;

        private byte[]? _encryptedDeviceId;

        private readonly string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "session.json");

        public string Secret
        {
            get
            {
                if (_encryptedSecret == null)
                {
                    return "";
                }

                return Decrypt(_encryptedSecret);
            }
            set
            {
                _encryptedSecret = Encrypt(value);

                Save();
            }
        }

        public string DeviceId
        {
            get
            {
                if (_encryptedDeviceId == null)
                {
                    return "";
                }

                return Decrypt(_encryptedDeviceId);
            }
            set
            {
                _encryptedDeviceId = Encrypt(value);

                Save();
            }
        }

        public SessionService()
        {
            if (!File.Exists(path))
            {
                return;
            }

            using var file = File.OpenRead(path);
            var session = JsonSerializer.Deserialize<Session>(file);

            if (session == null)
            {
                return;
            }

            _encryptedSecret = Convert.FromBase64String(session.EncryptedSecret);
            _encryptedDeviceId = Convert.FromBase64String(session.EncryptedDeviceId);
        }

        private void Save()
        {
            using var stream = File.CreateText(path);
            var session = new Session()
            {
                EncryptedSecret = _encryptedSecret != null ? Convert.ToBase64String(_encryptedSecret) : "",
                EncryptedDeviceId = _encryptedDeviceId != null ? Convert.ToBase64String(_encryptedDeviceId) : "",
            };

            var content = JsonSerializer.Serialize(session);

            stream.Write(content);
        }

        private void Clear()
        {
            _encryptedSecret = null;
            _encryptedDeviceId = null;

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private static byte[] Encrypt(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            var encrypted = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);

            return encrypted;
        }

        private static string Decrypt(byte[] encrypted)
        {
            var decrypted = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);

            return Encoding.UTF8.GetString(decrypted);
        }

        private class Session
        {
            public string EncryptedSecret { get; set; }
            public string EncryptedDeviceId { get; set; }
        }
    }
}
