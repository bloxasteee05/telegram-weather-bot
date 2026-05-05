// подключение библиотек для работы с Telegram
using System.Linq.Expressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

// подключение токена
string botToken = "8685325423:AAHlOF-j_0Az0OfsRNHXqjipQiaRoGfttRQ";
// подключение ключа погоды
string weatherApiKey = "0a0a5af3ad978bf2ca6b8014f4da2235";
    
// создание бота    
var bot = new TelegramBotClient(botToken);

// записная книжка — храним город каждого пользователя
// ключ — id пользователя, значение — его город
var userCities = new Dictionary<long, string>();

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
        // создаем кнопки
        var keyboard = new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(
            new[]
            {
                // первая строка кнопок
                new[]
                {
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Москва"),
                    new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Сибай")
                },
                // втроая строка кнопкок
                new [] { new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Минск"),
                new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("Уфа") }
                
                
            })
        
        {
            // кнопки подстраиваются под размер экрана
           ResizeKeyboard = true
        };
            
        
        // ответ на команду /start
        await bot.SendMessage(chatId,
            "👋 Привет! Я бот погоды!\n\n" +
            "🌍 Напиши мне любой город, а я дам ответ!\n\n" +
            "Например: Москва, Вашингтон и т.д!\n\n" +
            "❓ Понадобилась помощь? Пиши - /help",
        replyMarkup: keyboard,
        cancellationToken: ct);
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
    
    // проверяем какую командку написал пользователь
    if (messageText == "/forecast")
    {
        // ответ на команду /forecast
        await bot.SendMessage(chatId,
            "📅 Напиши название города для прогноза на 5 дней!\n\n" +
            "Например: Москва или London",
            cancellationToken: ct);
        return;
    }
    // проверяем какую командку написал пользователь
    if (messageText == "/info")
        {
            // ответ на команду /info
            await bot.SendMessage(chatId,
                "⛅ Информация о боте\n\n" +
                "Привет! Я — твой погодный помощник 🌍\nПомогу быстро узнать актуальную погоду в любом городе ☁️\n\n" +
                "📌 Что я умею:\n• " +
                "Показывать текущую температуру 🌡️\n• " +
                "Отправлять прогноз на день и неделю 🛠️⚠️ (в разработке)\n• " +
                "Показывать влажность, ветер и осадки 🌧️⚠️ (в разработке)\n• " +
                "Быстро находить погоду по названию города 🔎\n\n⚡ " +
                "Команды бота:\n`" +
                "/forecast — прогноз 🛠️ (в разработке)\n`" +
                "/help — помощь по командам ❓\n`" +
                "/author — информация о разработчике 👨‍💻\n`" +
                "/info — информация о боте ℹ️\n\n" +
                "🌈 Спасибо, что пользуешься ботом!\n" +
                "Желаю тебе только хорошей погоды ☀️\n\n\n");
            CancellationToken:
            return;
        }
    // проверяем какую командку написал пользователь
    if (messageText == "/ping")
    {
        // ответ на команду /ping
        await bot.SendMessage(chatId,
            "🟢 Бот работает!\n\n" +
            "⚡️ Задержка: в норме\n" +
            "🤖 Статус: онлайн",
            cancellationToken: ct);
        return;
    }
    
    // проверяем какую командку написал пользователь
if (messageText == "/setcity")
{
    // ответ на команду /stcity
    await bot.SendMessage(chatId,
        "📍 Напиши свой город после команды!\n\n" +
        "Например: /setcity Сибай",
        cancellationToken: ct);
    return;
}

// если пользователь написал /setcity Город — сохраняем город
if (messageText.StartsWith("/setcity "))
{
    // достаём название города из сообщения
    var savedCity = messageText.Replace("/setcity ", "");
    
    // сохраняем город в записную книжку
    userCities[chatId] = savedCity;
    
    await bot.SendMessage(chatId,
        $"✅ Твой город сохранён: {savedCity}\n\n" +
        $"Теперь напиши /myweather чтобы узнать погоду!",
        cancellationToken: ct);
    return;
}

// показываем погоду для сохранённого города
if (messageText == "/myweather")
{
    // проверяем есть ли сохранённый город
    if (!userCities.ContainsKey(chatId))
    {
        await bot.SendMessage(chatId,
            "❌ Ты ещё не сохранил свой город!\n\n" +
            "Напиши: /setcity Москва",
            cancellationToken: ct);
        return;
    }
    
    // берём сохранённый город из записной книжки
    var myCity = userCities[chatId];
    await bot.SendMessage(chatId, 
        $"🔍 Ищу погоду для: {myCity}...", 
        cancellationToken: ct);
    
    // ищем погоду для сохранённого города
    var url = $"https://api.openweathermap.org/data/2.5/weather?q={myCity}&appid={weatherApiKey}&units=metric&lang=ru";
    var response = await httpClient.GetStringAsync(url);
    var json = Newtonsoft.Json.Linq.JObject.Parse(response);
    
    var cityName = json["name"]?.ToString();
    var temp = Math.Round(double.Parse(json["main"]?["temp"]?.ToString()), 0).ToString();
    var feelsLike = Math.Round(double.Parse(json["main"]?["feels_like"]?.ToString()), 0).ToString();
    var description = json["weather"]?[0]?["description"]?.ToString();
    var humidity = json["main"]?["humidity"]?.ToString();
    
    var answer = $"📍 Погода в твоём городе {cityName}:\n" +
                 $"🌡 Температура: {temp}°C\n" +
                 $"🤔 Ощущается как: {feelsLike}°C\n" +
                 $"💧 Влажность: {humidity}%\n" +
                 $"☁️ {description}";
    
    await bot.SendMessage(chatId, answer, cancellationToken: ct);
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
        var temp = Math.Round(double.Parse(json["main"]?["temp"]?.ToString()), 0).ToString();
        var feelsLike = Math.Round(double.Parse(json["main"]?["feels_like"]?.ToString()), 0).ToString();
        var description = json["weather"]?[0]?["description"]?.ToString();
        var humidity = json["main"]?["humidity"]?.ToString();
        // получаем числовое значение температуры для совета
        var tempValue =  double.Parse(temp);
        

        // даем умный совет в зависимости от погоды
        string advice;
        if (tempValue >= 25)
            advice = "😎 На улице жарко — возьми воду и солнцезащитный крем!";
        else if (tempValue >= 15)
            advice = "👕 Отличная погода — можно идти гулять!";
        else if (tempValue >= 5)
            advice = "🧥 Прохладно — возьми куртку!";
        else if (tempValue >= -10)
            advice = "🥶 Холодно — одевайся теплее!";
        else
            advice = "🧊 Очень сильный мороз — лучше остаться дома!";
        
        // красивый ответ пользователю
        var answer = $"🌤 Погода в {cityName}:\n" +
                     $"🌡 Температура: {temp}°C\n" +
                     $"🤔 Ощущается как: {feelsLike}°C\n" +
                     $"💧 Влажность: {humidity}%\n" +
                     $"☁️ {description}\n\n" +
                     $"💡 Совет: {advice}";

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


