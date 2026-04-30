// подключение библиотек для работы с Telegram
using System.Linq.Expressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

// подключение токена
string botToken = "8685325423:AAHlOF-j_0Az0OfsRNHXqjipQiaRoGfttRQ";
// подключение ключа погоды
string weatherApiKey = "0a0a5af3ad978bf2ca6b8014f4da2235";
    
// создание бота    
var bot = new TelegramBotClient(botToken);

// Создаём клиент для запросов в интернет
var httpClient = new HttpClient();

// бот слушает команды
var cts = new CancellationTokenSource();

bot.StartReceiving(
    // Эта функция вызывается когда приходит новое сообщение
    HandleUpdateAsync,
    // Эта функция вызывается если произошла ошибка
    HandleErrorAsync,
    cancellationToken: cts.Token
    );

// сообщение в консоли что бот запустился
Console.WriteLine("Бот успешно запустился!");
Console.ReadLine();

// остонавливаем бота
cts.Cancel();

// Функция которая обрабатывает входящие сообщения
async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
{
    // Проверяем что это именно текстовое сообщение
    if (update.Message?.Text == null) return;

    // Сохраняем текст сообщения и id чата
    var messageText = update.Message.Text;
    var chatId = update.Message.Chat.Id;

    // проверяем какую командку написал пользователь
    if (messageText == "/start")
    {
        // ответ на команду /start
        await bot.SendMessage(chatId,
            "👋 Привет! Я бот погоды!\n\n" +
            "🌍 Напиши мне любой город, а я дам ответ!\n\n" +
            "Например: Москва, Вашингтон и т.д!\n\n" +
            "❓ Понадобилась помощь? Пиши - /help\n\n");
        CancellationToken:
        return;
    }

    // проверяем какую командку написал пользователь
    if (messageText == "/help")
    {
        // ответ на команду /help
        await bot.SendMessage(chatId,
            "Не помогу❗❗❗❗\n\n");
        CancellationToken:
        return;
    }

    // проверяем какую командку написал пользователь
    if (messageText == "/author")
    {
        // ответ на команду /author
        await bot.SendMessage(chatId,
            "@bloxasteee\n\n");
        CancellationToken:
        return;
    }


    // если пользователь написал не команду ищем погоду
    var city = messageText;

    // Сообщаем пользователю что ищем погоду
    await bot.SendMessage(chatId, $"🔍 Ищу погоду для: {city}...", cancellationToken: ct);

    // Запрашиваем погоду — оборачиваем в try/catch чтобы поймать ошибку
    try
    {
        var url =
            $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={weatherApiKey}&units=metric&lang=ru";
        var response = await httpClient.GetStringAsync(url);

        // Достаём нужные данные из ответа
        var json = Newtonsoft.Json.Linq.JObject.Parse(response);
        var cityName = json["name"]?.ToString();
        var temp = json["main"]?["temp"]?.ToString();
        var description = json["weather"]?[0]?["description"]?.ToString();
        var humidity = json["main"]?["humidity"]?.ToString();
        var feelsLike = json["main"]?["feels_like"]?.ToString();

        var answer = $"🌤 Погода в {cityName}:\n" +
                     $"🌡 Температура: {temp}°C\n" +
                     $"🤔 Ощущается как: {feelsLike}°C\n" +
                     $"💧 Влажность: {humidity}%\n" +
                     $"☁️ {description}";

        await bot.SendMessage(chatId, answer, cancellationToken: ct);
    }
    catch
    {
        // Если город не найден — отвечаем красиво
        await bot.SendMessage(chatId,
            $"❌ Город \"{city}\" не найден!\n\n" +
            $"Попробуй написать по другому\n" +
            $"Например: Москва, London, Tokyo",
            cancellationToken: ct);
    }
}

// Функция которая обрабатывает ошибки
    Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }


