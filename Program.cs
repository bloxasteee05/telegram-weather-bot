// подключение библиотек для работы с Telegram
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
    var city = update.Message.Text;
    var chatId = update.Message.Chat.Id;
    
    // Сообщаем пользователю что ищем погоду
    await bot.SendMessage(chatId, $"🔍 Ищу погоду для: {city}...", cancellationToken: ct);

    // Запрашиваем погоду
    var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={weatherApiKey}&units=metric&lang=ru";
    var response = await httpClient.GetStringAsync(url);

    // Достаём нужные данные из ответа сервиса погоды
    // Parse это переводчик он превращает этот непонятный текст в структуру из которой можно доставать нужные кусочки.
    var json = Newtonsoft.Json.Linq.JObject.Parse(response);
    
    // Достаем название города
    var cityName = json["name"]?.ToString();
    // Достаем температуру
    var temp = json["main"]?["temp"]?.ToString();
    // Достаем описание погоды
    var description = json["weather"]?[0]?["description"]?.ToString();
    // Достаем влажность
    var humidity = json["main"]?["humidity"]?.ToString();
    // Достаем ощущаемую температуру
    var feelsLike = json["main"]?["feels_Like"]?.ToString();
    // Делаем красивое оформление
    var answer = $"🌤 Погода в {cityName}:\n" +
                 $"🌡️ Температура: {temp}:\n" +
                 $"☁️ Описание погоды: {description}:\n" +
                 $"💧 Влажность: {humidity}:\n" +
                 $"🤔 Ощущается как: {feelsLike}:\n";
    // отправляет красивый ответ пользователю
    await bot.SendMessage(chatId, answer, cancellationToken: ct);




}

// Функция которая обрабатывает ошибки
Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
{
    Console.WriteLine($"Ошибка: {exception.Message}");
    return Task.CompletedTask;
}
