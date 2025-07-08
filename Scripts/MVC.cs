namespace Scripts;

    interface IPassportService
    {
        string GetPassportStatus(Passport passport);
    }

    class PassportView
    {
        private readonly PassportPresenter _presenter;

        public PassportView(PassportPresenter presenter)
        {
            if (presenter == null)
                throw new ArgumentNullException(nameof(presenter),"Net presentera");

            _presenter = presenter;
        }

        public void Start()
        {
            Console.WriteLine("Введите серию и номер паспорта:");
            string input = Console.ReadLine();

            try
            {
                string result = _presenter.CheckPassport(input);
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
    }

    class PassportPresenter
    {
        private readonly IPassportService _service;

        public PassportPresenter(IPassportService service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service),"Net servica");

            _service = service;
        }

        public string CheckPassport(string rawInput)
        {
            var passport = new Passport(rawInput);
            return _service.GetPassportStatus(passport);
        }
    }

class Passport
{
    public Passport(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
            throw new ArgumentException("Введите серию и номер паспорта");

        rawData = rawData.Replace(" ", "").Trim();

        if (rawData.Length < 10)
            throw new ArgumentException("Неверный формат серии или номера паспорта");

        RawNumber = rawData;
        Hash = Hasher.ComputeSha256Hash(rawData);
    }

    public string RawNumber { get; }
    public string Hash { get; }
    }

    static class Hasher
    {
        public static string ComputeSha256Hash(string rawData)
        {
            if (rawData == null)
                throw new ArgumentNullException(nameof(rawData),"Net rawData");

                SHA256 sha256Hash = SHA256.Create()
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
        }
    }

    class SqlitePassportService : IPassportService
    {
        public string GetPassportStatus(Passport passport)
        {
            if (passport == null)
                throw new ArgumentNullException(nameof(passport),"Net pasporta");

            string commandText = $"SELECT * FROM passports WHERE num='{passport.Hash}' LIMIT 1;";
            string dbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "db.sqlite");

            if (!File.Exists(dbPath))
                throw new FileNotFoundException("Файл db.sqlite не найден. Положите файл в папку вместе с exe.");

            using (var connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var adapter = new SQLiteDataAdapter(new SQLiteCommand(commandText, connection));
                var table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count == 0)
                    return $"Паспорт «{passport.RawNumber}» в списке участников дистанционного голосования НЕ НАЙДЕН";

                bool status = Convert.ToBoolean(table.Rows[0][1]);

                return status
                    ? $"По паспорту «{passport.RawNumber}» доступ ПРЕДОСТАВЛЕН"
                    : $"По паспорту «{passport.RawNumber}» доступ НЕ ПРЕДОСТАВЛЯЛСЯ";
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var service = new SqlitePassportService();
            var presenter = new PassportPresenter(service);
            var view = new PassportView(presenter);

            view.Start();
        }
    }
