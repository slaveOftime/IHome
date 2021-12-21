# IHome

This is a hobby project. Below is the network flow.

```text
Phone browser
   |
   |
Cloud machine VM (public IP)
frp server (tcp tunnel, https://github.com/fatedier/frp)
   |
   |
Home raspberry pi (frp client setted up)
IHome.Server
```

The main stuff in IHome.Server, it is using [Fun.Blazor](https://github.com/slaveOftime/Fun.Blazor), aspnet core server side mode, dotnet iot.

- Auth with cookie (put default user name and password under appsettings.Production.json)
- MJPEG streamming for video
- Wheel control
- CPU temperature monitor


# How to build

1. Make sure you have .NET SDK 5 and 6 installed (6 is used for build, 5 is used as target, will update to 6 soon)
2. Make sure you have nodejs and pnpm installed (just for tailwindcss)
3. Currently because there is a bug in Iot.Device.Binding in 1.5.0 and 2.0.0 prerelease, so I have to pull down dotnet/iot project from github and modify and build it by me self. You can check file /IHome.Server/IHome.Server.fsproj
4. Under IHome root folder run: pnpm run watch-css
5. Under IHome.Server run: dotnet run

# Deploy

1. Add below service

```text
[Unit]
Description=IHome Service
After=network.target

[Service]
WorkingDirectory=/home/pi/IHome
Type=simple
Restart=on-failure
RestartSec=5s
ExecStart=/home/pi/dotnet/dotnet IHome.Server.dll
ExecReload=/home/pi/dotnet/dotnet IHome.Server.dll
LimitNOFILE=1048576
```

2. Run bash deploy.sh
