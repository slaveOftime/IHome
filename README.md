# IHome

This is a hobby project. Below is the network flow of how I access IHome after I leave home. But for play locally you can just use VSCode and develop in raspbery pi directly.

```text
Phone browser
   |
   |
Cloud machine VM (public IP)
frp server (tcp tunnel, https://github.com/fatedier/frp)
   |
   |
Home raspberry pi (frp client setted up)
IHome.Server (aspnet core + blazor server)
```

The main stuff is in IHome.Server, it is using [Fun.Blazor](https://github.com/slaveOftime/Fun.Blazor), aspnet core server side mode, dotnet iot.

- Auth with cookie (put default user name and password under appsettings.Production.json)
- MJPEG streamming for video
- Wheel control
- Servo control for camera angle
- CPU temperature monitor


# How to build

1. Make sure you have .NET SDK 6 installed
2. Make sure you have nodejs and pnpm installed (just for tailwindcss)
3. Under IHome root folder run: pnpm install && pnpm run watch-css
4. Under IHome.Server run: dotnet run
5. With VSCode you can use port forward to access https://localhost:5001 through your conputer. 

# Deploy

This is just some helper script to help me deploy the code to a specific location in raspberry pi after build successful in it.

1. Add below service for the first time

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
