using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        // Ввод хэш-значений из консоли
        List<string> hashes = InputHashes();

        // Выбор режима работы
        Console.WriteLine("Выберите режим: (1) Однопоточный, (2) Многопоточный");
        string mode = Console.ReadLine();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        if (mode == "1")
        {
            BruteForceSingleThread(hashes);
        }
        else if (mode == "2")
        {
            Console.WriteLine("Введите количество потоков: ");
            int threads = int.Parse(Console.ReadLine());
            BruteForceMultiThread(hashes, threads);
        }
        else
        {
            Console.WriteLine("Выбран некорректный режим.");
            return;
        }

        stopwatch.Stop();
        Console.WriteLine($"Затраченное время: {stopwatch.Elapsed.TotalSeconds} секунд.");
    }

    // Ввод хэш-значений из консоли
    static List<string> InputHashes()
    {
        List<string> hashes = new List<string>();
        Console.WriteLine("Введите хэш-значения по одному. Нажмите Enter на пустой строке для завершения.");

        while (true)
        {
            Console.Write("Введите хэш-значение (или нажмите Enter для завершения): ");
            string hash = Console.ReadLine();
            if (string.IsNullOrEmpty(hash)) break;
            hashes.Add(hash);
        }
        return hashes;
    }

    // Вычисление MD5 и SHA-256 хэшей
    static (string, string) CalculateHashes(string password)
    {
        using (MD5 md5 = MD5.Create())
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] md5Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            byte[] sha256Bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

            string md5Hash = BitConverter.ToString(md5Bytes).Replace("-", "").ToLower();
            string sha256Hash = BitConverter.ToString(sha256Bytes).Replace("-", "").ToLower();

            return (md5Hash, sha256Hash);
        }
    }

    // Однопоточный перебор
    static void BruteForceSingleThread(List<string> hashes)
    {
        string alphabet = "abcdefghijklmnopqrstuvwxyz";

        foreach (var combination in GenerateCombinations(alphabet, 5))
        {
            string password = new string(combination);
            var (md5Hash, sha256Hash) = CalculateHashes(password);
            if (hashes.Contains(md5Hash) || hashes.Contains(sha256Hash))
            {
                Console.WriteLine($"Пароль: {password}, Хэш: {(hashes.Contains(md5Hash) ? md5Hash : sha256Hash)}");
            }
        }
    }

    // Многопоточный перебор
    static void BruteForceMultiThread(List<string> hashes, int threads)
    {
        string alphabet = "abcdefghijklmnopqrstuvwxyz";
        var combinations = GenerateCombinations(alphabet, 5);

        Parallel.ForEach(combinations, new ParallelOptions { MaxDegreeOfParallelism = threads }, combination =>
        {
            string password = new string(combination);
            var (md5Hash, sha256Hash) = CalculateHashes(password);
            if (hashes.Contains(md5Hash) || hashes.Contains(sha256Hash))
            {
                Console.WriteLine($"Пароль: {password}, Хэш: {(hashes.Contains(md5Hash) ? md5Hash : sha256Hash)}");
            }
        });
    }

    // Генерация комбинаций для всех возможных пятибуквенных паролей
    static IEnumerable<char[]> GenerateCombinations(string alphabet, int length)
    {
        int count = (int)Math.Pow(alphabet.Length, length);
        for (int i = 0; i < count; i++)
        {
            char[] result = new char[length];
            int temp = i;
            for (int j = 0; j < length; j++)
            {
                result[j] = alphabet[temp % alphabet.Length];
                temp /= alphabet.Length;
            }
            yield return result;
        }
    }
}
