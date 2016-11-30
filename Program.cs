using System;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types;
using System.Threading;

class Program
{

    //Создаем клиент бота
    private static readonly TelegramBotClient Bot = new TelegramBotClient("327156098:AAGFdzD_D6Uh1zw5ueZnPTETSuM57Np70po");
    //Создаем ответ таймера
    public static TimerCallback timerCallback = new TimerCallback(setTaimer);
    //обявляем таймер
    public static Timer timer;

    static void Main(string[] args)
    {
        //добавляем обработчик сообщений
        Bot.OnMessage+=Message;

        var me = Bot.GetMeAsync().Result;

        Console.WriteLine(me.Username);
        //включяем бота
        Bot.StartReceiving();
        while(true){}
    }
    //Сообщение которое надо напомнить 
    public static string RemindeMessage;
    //ID чата которому надо напомнить 
    public static long ChatID;

    //обработчик сообщений 
    private static void Message(object sender, MessageEventArgs messageEventArgs){
        var message = messageEventArgs.Message;
        //проверяем пришло ли на сообщение 
        if(message==null) return;

        if(message.Text.StartsWith("/ping")){
            Bot.SendTextMessageAsync(message.Chat.Id, "/pong");
        } 
        
        // по команде /hepl создаем клавиатуру 
        if(message.Text.StartsWith("/help")){
                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new [] // first row
                    {
                        new KeyboardButton("Установить Таймер")
                    }
                });

                Bot.SendTextMessageAsync(message.Chat.Id, "/settaimer  чч:мм:сс Текс напоминания \r\n /ping \r\n /help", replyMarkup:keyboard);
        }
        
        if(message.Text.StartsWith("Установить Таймер")){

            Bot.SendTextMessageAsync(message.Chat.Id, "Через сколько должен сработать таймер? \r\n /settaimer  чч:мм:сс Текс напоминания");
        }
        //установка таймера
         if(message.Text.StartsWith("/settaimer")){

             //разбавем строку на массив слов
             string[] timeArray = message.Text.Split(' ');
             //сохраняем время первоого срабатывания
             string time = timeArray[1];
             //очишаем сообщение которое надо повторить 
             RemindeMessage="";
             //присваиваем ID чата 
             ChatID = message.Chat.Id;

             //собираем текс сообщения
             for (int i = 2; i<timeArray.Length;i++ ){
                 RemindeMessage+=timeArray[i]+" ";
             }
             //парсим время из строки в формат TimeSpan
             TimeSpan timeSpan = TimeSpan.Parse(time);

             //устонавливаем таймер
             timer = new Timer(timerCallback,0,timeSpan.Minutes*60*1000+timeSpan.Seconds*1000,10000);

         }

         
         if(message.Text.StartsWith("/stop")){
             //удаляем тайамер
             timer.Dispose();

             Bot.SendTextMessageAsync(ChatID, "Таймер остановлен ");
         }


    }

    //функция выполняемая при каждом тике таймера 
    private static void setTaimer(object obj){
        Bot.SendTextMessage(ChatID, RemindeMessage);
        
    }
}
