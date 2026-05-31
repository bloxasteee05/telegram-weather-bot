// подключение библиотек для работы с Telegram
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

// подключение токена
string botToken = "8685325423:AAHlOF-j_0Az0OfsRNHXqjipQiaRoGfttRQ";
    
// создание бота    
var bot = new TelegramBotClient(botToken);     

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

    // Отвечаем пользователю
    await bot.SendMessage(chatId, $"Ты написал: {messageText}", cancellationToken: ct);
}

// Функция которая обрабатывает ошибки
Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
{
    Console.WriteLine($"Ошибка: {exception.Message}");
    return Task.CompletedTask;
}
