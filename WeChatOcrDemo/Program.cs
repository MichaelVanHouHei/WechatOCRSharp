// See https://aka.ms/new-console-template for more information

using WeChatOcrDemo;

var mongo = new MMMojoCall();
mongo.WithUserDir("D:\\wechat\\[3.9.10.19]").WithOcrExe("C:\\Users\\zxcni\\AppData\\Roaming\\Tencent\\WeChat\\XPlugin\\Plugins\\WeChatOCR\\7071\\extracted\\WeChatOCR.exe").DoOcr();
Console.WriteLine("Hello, World!");
Console.Read();