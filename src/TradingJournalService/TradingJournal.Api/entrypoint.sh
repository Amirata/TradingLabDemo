#!/bin/sh
echo "Starting entrypoint script as $(id -u):$(id -g)"
chown -R 1654:1654 /app/wwwroot/TechnicImages || echo "Failed to chown"
chmod -R 775 /app/wwwroot/TechnicImages || echo "Failed to chmod"
ls -ld /app/wwwroot/TechnicImages
echo "Gosu path: $(which gosu)"
echo "Testing gosu with id command"
/usr/sbin/gosu 1654:1654 id || echo "Gosu failed to switch user"
echo "Switching to user 1654 and running app"
/usr/sbin/gosu 1654:1654 dotnet TradingJournal.Api.dll || echo "Failed to run app with gosu"
echo "This line should not appear if gosu worked"