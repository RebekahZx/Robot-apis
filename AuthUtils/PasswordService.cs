using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Isopoh.Cryptography.SecureArray;
using Microsoft.Extensions.Options;


namespace robot_controller_api.AuthUtils
{
    // public class PasswordService
    // {
    //     private readonly string _pepper;

    //     public PasswordService(IOptions<AppSettings> config)
    //     {
    //         _pepper = config.Value.Pepper;
    //     }

    //     public string HashPassword(string plainPassword)
    //     {
    //         var combined = plainPassword + _pepper;
    //         var salt = GenerateSalt();

    //         var config = new Argon2Config
    //         {
    //             Type = Argon2Type.DataIndependentAddressing,
    //             Version = Argon2Version.Nineteen,
    //             TimeCost = 4,
    //             MemoryCost = 65536,
    //             Lanes = 4,
    //             Threads = 4,
    //             Password = Encoding.UTF8.GetBytes(combined),
    //             Salt = salt,
    //             HashLength = 32
    //         };

    //         var hasher = new Argon2(config);
    //         using var hash = hasher.Hash(); // returns SecureArray<byte>

    //         byte[] hashBytes = new byte[hash.Buffer.Length];
    //         Buffer.BlockCopy(hash.Buffer, 0, hashBytes, 0, hashBytes.Length);

    //         return config.EncodeString(hashBytes); // encodes config + salt + hash
    //     }

    //     public bool VerifyPassword(string plainPassword, string encodedHash)
    //     {
    //         var combined = plainPassword + _pepper;
    //         return Argon2.Verify(encodedHash, combined);
    //     }

    //     public static byte[] GenerateSalt(int length = 16)
    //     {
    //         var salt = new byte[length];
    //         using var rng = RandomNumberGenerator.Create();
    //         rng.GetBytes(salt);
    //         return salt;
    //     }
    // }
    public class PasswordService
{
    private readonly string _pepper;
    private readonly string _salt;

    public PasswordService(IConfiguration config)
    {
        _pepper = config["AppSettings:Pepper"];
        _salt = config["AppSettings:Salt"];
    }

    public string HashPassword(string plainPassword)
    {
        var combined = plainPassword + _pepper + _salt;
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(combined);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public bool VerifyPassword(string plainPassword, string storedHash)
    {
        var hash = HashPassword(plainPassword);
        return hash == storedHash;
    }
}

}
