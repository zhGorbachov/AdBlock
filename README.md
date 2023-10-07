# 🚫 Ad-blocking Proxy Server


## ⚙ Discription:
A lightweight tool to block ad-related requests utilizing the Titanium.Web.Proxy library.


## 🌟 Features:
 - Blocks ad-related requests at the proxy level.
 - Easily expandable with a custom list of domains or paths to block.
 - Console-based feedback for blocked requests.


## 📃 Tasks on the project:
 - Creating a proxy server - (completed);
 - Filtering regular advertising, banners, scripts, etc - (completed);
 - Filtering ads in YouTube videos - (uncompleted); 
 - Logging about errors and blocked advertisements - (completed);
 - User-interface with starting and stopping the proxy server - (uncompleted);


## 🚀 Installation:
1. Clone the repository:
```
git clone [repository_link]
```
2. Open the project
3. Restore NuGet packages:
```
dotnet restore
```
4. Build and run the application.
```
dotnet build
dotnet run
```


## 🛠️ How to use it:
Firstly, when you start it at the first time, you will need to agree this cetrificate to work with it adblocker:

![image](https://github.com/zhGorbachov/AdBlock/assets/115892673/c788a95a-3cd1-4d4b-aa6c-155dccb14c40)

I work with browser LibreWolf with built-in connection to a proxy server, as it is more convenient and efficient to test and work with it on small project, than start it from you pc system. If you choise this solution with LibreWolf, you will need to open Settings and configure proxy-server connection:<br>
![img.png](img/img.png)
<br>After it you need to configure the confiration, and indicate the ip-adress and port on which you run the proxy server, in my example it is "127.0.0.1" and "8888":<br>
![img_1.png](img/img_1.png)


## 📋 Extension:
To add additional domains or paths for blocking, modify the list in the OnBeforeRequest and OnBeforeResponse method

## 💾 Presenation of project:
https://docs.google.com/presentation/d/14MqVbUBDD6N3YTe1NZDfvhvGp_g218iS8KobB0WDGf0/edit?usp=sharing

## 🎈 And after it confirms, you can start to work with Adblocker, enjoy...
