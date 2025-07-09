using System;
using System.Runtime.InteropServices;

public interface IHashProvider
    {
        string ComputeHash(string input);
    }

        public class Sha256HashProvider : IHashProvider
        {
            public string ComputeHash(string input)
            {
                if (input == null)
                    throw new ArgumentNullException(nameof(input),"Нет входного параметра");

                    SHA256 sha256 = SHA256.Create()
                    byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                    StringBuilder sb = new StringBuilder();
                    foreach (byte b in bytes)
                        sb.Append(b.ToString("x2"));
                    return sb.ToString();
                
            }
        }

    public class PassportInput
    {
        public string CleanedNumber { get; }

        public PassportInput(string rawInput)
        {
            if (string.IsNullOrWhiteSpace(rawInput))
                throw new ArgumentException("Введите серию и номер паспорта");

            var cleaned = rawInput.Replace(" ", string.Empty).Trim();
            
            if (cleaned.Length < 10)
            throw new ArgumentException("Неверный формат серии или номера паспорта");

            CleanedNumber = cleaned;
        }
    }
    
    public interface IPassportService
{
    bool? GetAccessStatusByHash(string passportHash);
}

public interface IPassportRepository
{
    bool? GetByHash(string hash);
}

namespace PassportChecker
{
    public class SqlitePassportRepository : IPassportRepository
    {
        public bool? GetByHash(string hash)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash), "Нет Хэша");

            string dbPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "db.sqlite");

            if (!File.Exists(dbPath))
                throw new FileNotFoundException("Файл db.sqlite не найден. Положите файл в папку вместе с exe.");

            using (var connection = new SQLiteConnection($"Data Source={dbPath}"))
            {
                connection.Open();
                var command = new SQLiteCommand($"SELECT * FROM passports WHERE num='{hash}' LIMIT 1;", connection);
                var adapter = new SQLiteDataAdapter(command);
                var table = new DataTable();
                adapter.Fill(table);

                if (table.Rows.Count == 0)
                    return null;

                return Convert.ToBoolean(table.Rows[0][1]);
            }
        }
    }
}

// PassportService.cs
namespace PassportChecker
{
    public class PassportService : IPassportService
    {
        private readonly IPassportRepository _repository;

        public PassportService(IPassportRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository),"Нет репозитория");

            _repository = repository;
        }

        public bool? GetAccessStatusByHash(string passportHash)
        {
            if (passportHash == null)
                throw new ArgumentNullException(nameof(passportHash), "Нет Хэша");

            return _repository.GetByHash(passportHash);
        }
    }
}

    public class PassportPresenter
    {
        private readonly IPassportService _service;
        private readonly IHashProvider _hashProvider;
        private readonly IPassportView _view;

        public PassportPresenter(IPassportService service, IHashProvider hashProvider, IPassportView view)
        {
        if (service == null)
            throw new ArgumentNullException(nameof(service), "Нет сервиса");

        if (hashProvider == null)
            throw new ArgumentNullException(nameof(hashProvider), "Нет хэша");

        if (view == null)
            throw new ArgumentNullException(nameof(view), "Нет вьюва");

            _service = service;
            _hashProvider = hashProvider;
            _view = view;
        }

        public void OnPassportEntered(string rawInput)
        {
        if (rawInput == null)
            throw new ArgumentNullException(nameof(rawInput), "Нет информации");

            try
            {
                var passportInput = new PassportInput(rawInput);
                string hash = _hashProvider.ComputeHash(passportInput.CleanedNumber);
                bool? result = _service.GetAccessStatusByHash(hash);

                if (result == null)
                {
                    _view.ShowMessage($"Паспорт «{passportInput.CleanedNumber}» в списке участников дистанционного голосования НЕ НАЙДЕН");
                }
                else if (result == true)
                {
                    _view.ShowMessage($"По паспорту «{passportInput.CleanedNumber}» доступ ПРЕДОСТАВЛЕН");
                }
                else
                {
                    _view.ShowMessage($"По паспорту «{passportInput.CleanedNumber}» доступ НЕ ПРЕДОСТАВЛЯЛСЯ");
                }
            }
            catch (Exception ex)
            {
                _view.ShowError(ex.Message);
            }
        }
    }

    public interface IPassportView
    {
        void ShowMessage(string message);
        void ShowError(string error);
    }

    public class PassportView : IPassportView
    {
        private readonly PassportPresenter _presenter;

        public PassportView(PassportPresenter presenter)
        {
        if (presenter == null)
            throw new ArgumentNullException(nameof(presenter), "Нет презентера");

            _presenter = presenter;
        }

        public void Start()
        {
            Console.WriteLine("Введите серию и номер паспорта:");
            string input = Console.ReadLine();
            _presenter.OnPassportEntered(input);
        }

        public void ShowMessage(string message)
        {
        if (message == null)
            throw new ArgumentNullException(nameof(message), "Нет сообщения");

            Console.WriteLine(message);
        }

        public void ShowError(string error)
        {
        if (error == null)
            throw new ArgumentNullException(nameof(error), "Нет текста ошибки");

            Console.WriteLine("Ошибка: " + error);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var repository = new SqlitePassportRepository();
            var service = new PassportService(repository);
            var hashProvider = new Sha256HashProvider();
            PassportView view = null;
            var presenter = new PassportPresenter(service, hashProvider, view);
            view = new PassportView(presenter);

            view.Start();
        }
    }
