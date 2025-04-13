// See https://aka.ms/new-console-template for more information
using System.Text.Json;

public class CovidConfig
{
    public string satuan_suhu { get; set; }
    public int batas_hari_deman { get; set; }
    public string pesan_ditolak { get; set; }
    public string pesan_diterima { get; set; }

    private static string configFilePath = "covid_config.json";

    public static CovidConfig Load()
    {
        if (!File.Exists(configFilePath))
        {
            // Set default config
            var defaultConfig = new CovidConfig
            {
                satuan_suhu = "celcius",
                batas_hari_deman = 14,
                pesan_ditolak = "Anda tidak diperbolehkan masuk ke dalam gedung ini",
                pesan_diterima = "Anda dipersilahkan untuk masuk ke dalam gedung ini"
            };
            File.WriteAllText(configFilePath, JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true }));
            return defaultConfig;
        }

        // File sudah ada → baca dan cek placeholder
        string json = File.ReadAllText(configFilePath);
        var config = JsonSerializer.Deserialize<CovidConfig>(json);

        // Ganti nilai placeholder jika ditemukan
        if (config.pesan_ditolak == "CONFIG3")
            config.pesan_ditolak = "Anda tidak diperbolehkan masuk ke dalam gedung ini";

        if (config.pesan_diterima == "CONFIG4")
            config.pesan_diterima = "Anda dipersilahkan untuk masuk ke dalam gedung ini";

        if (config.satuan_suhu == "CONFIG1")
            config.satuan_suhu = "celcius";

        if (config.batas_hari_deman.ToString() == "CONFIG2") // jaga-jaga kalau CONFIG2 masuk literal string
            config.batas_hari_deman = 14;

        // Simpan ulang config yang sudah dibersihkan
        config.Simpan();

        return config;
    }

    public void Simpan()
    {
        File.WriteAllText(configFilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void UbahSatuan()
    {
        satuan_suhu = satuan_suhu == "celcius" ? "fahrenheit" : "celcius";
        Simpan();
    }
}

class Program
{
    static void Main()
    {
        CovidConfig config = CovidConfig.Load();

        // Panggil pergantian satuan
        config.UbahSatuan();
        Console.WriteLine($"Satuan suhu telah diubah menjadi: {config.satuan_suhu}");

        Console.WriteLine($"Berapa suhu badan anda saat ini? Dalam nilai {config.satuan_suhu}");
        double suhu = Convert.ToDouble(Console.ReadLine());

        Console.WriteLine("Berapa hari yang lalu (perkiraan) anda terakhir memiliki gejala demam?");
        int hariDemam = Convert.ToInt32(Console.ReadLine());

        bool suhuValid = false;

        if (config.satuan_suhu == "celcius")
        {
            suhuValid = suhu >= 36.5 && suhu <= 37.5;
        }
        else if (config.satuan_suhu == "fahrenheit")
        {
            suhuValid = suhu >= 97.7 && suhu <= 99.5;
        }

        if (suhuValid || hariDemam < config.batas_hari_deman)
        {
            Console.WriteLine(config.pesan_diterima);
        }
        else
        {
            Console.WriteLine(config.pesan_ditolak);
        }
    }
}