cd IHome.Server
/home/pi/dotnet/dotnet publish -c Release -o /home/pi/IHome
sudo systemctl stop ihome
sudo systemctl start ihome
sudo systemctl status ihome
