# TradingLab Project

This repository contains the `TradingLab` microservices application, developed as a portfolio project for my resume.

You can run this application on an Ubuntu server by following these instructions:

## Install Docker
1. Log in to your server and update the package index:
   ```bash
   sudo apt update
   sudo apt upgrade -y
   ```
2. Install the necessary packages to use Docker’s repository:
   ```bash
   sudo apt install -y apt-transport-https ca-certificates curl software-properties-common
   ```
3. Add Docker’s official GPG key to verify packages:
   ```bash
   curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
   ```
4. Add the Docker repository to your system:
   ```bash
   echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
   ```
5. Refresh the package index:
   ```bash
   sudo apt update
   ```
6. Install Docker and its related tools:
   ```bash
   sudo apt install -y docker-ce docker-ce-cli containerd.io
   ```
7. Verify that the Docker service is running:
   ```bash
   sudo systemctl status docker
   ```
8. If the Docker service is not active, enable and start it:
   ```bash
   sudo systemctl enable docker
   sudo systemctl start docker
   ```
9. (Optional) Allow non-root user access: To run Docker commands without `sudo`, add your user to the `docker` group. Log out and back in to apply the changes:
   ```bash
   sudo usermod -aG docker $USER
   ```
10. Test the Docker installation by running a test container:
    ```bash
    docker run hello-world
    ```
    If successful, you’ll see a welcome message from Docker.

## Set Up DNS Records for `yourdomain.com`
1. Log in to your DNS provider’s dashboard (e.g., Cloudflare).
2. Add the following DNS records:

   ```plaintext
   Type  | Name           | Content            | Proxy Status | TTL
   ------|----------------|--------------------|--------------|------
   A     | yourdomain.com | <Your Server IP>   | Proxied      | Auto
   CNAME | api            | yourdomain.com     | Proxied      | Auto
   CNAME | www            | yourdomain.com     | Proxied      | Auto
   ```
    - Use **Proxied** for the root domain to enable CDN, DDoS protection, and performance improvements.
    - Use **DNS Only** for subdomains if they are managed elsewhere or require direct access to the server.
    - **TTL Auto** is recommended for dynamic or standard configurations.

## Clone and Install
1. Clone the repository to your server:
   ```bash
   git clone https://github.com/Amirata/TradingLabDemo.git
   ```
2. Change to the `TradingLabDemo` directory:
   ```bash
   cd TradingLabDemo
   ```
3. Edit the `compose.yaml` file to configure your domain:
   ```bash
   sudo nano compose.yaml
   ```
    - Save changes by pressing `Ctrl + X`, then `Y`, then `Enter`.
4. Edit the `appsettings.json` file to configure your domain:
   ```bash
   sudo nano src/TradingJournalService/TradingJournal.Api/appsettings.json
   ```
    - Save changes by pressing `Ctrl + X`, then `Y`, then `Enter`.
5. Build the services on your server (this may take several minutes):
   ```bash
   docker compose build
   ```
6. Run the services in detached mode:
   ```bash
   docker compose up -d
   ```

## Add SSL Certificate
1. Install Certbot and the Nginx plugin:
   ```bash
   sudo apt install certbot python3-certbot-nginx
   ```
2. Generate an SSL certificate for your domain and subdomains:
   ```bash
   sudo certbot certonly --standalone -d yourdomain.com -d api.yourdomain.com -d www.yourdomain.com --email your-email@example.com --agree-tos --no-eff-email
   ```
3. Verify that the certificate was installed correctly:
   ```bash
   sudo certbot certificates
   ```
4. Create a `devcerts` folder in the `TradingLabDemo` directory if it doesn’t exist:
   ```bash
   mkdir devcerts
   ```
5. Copy the SSL certificates to the `devcerts` folder:
   ```bash
   sudo cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem ./devcerts/yourdomain.com.crt
   sudo cp /etc/letsencrypt/live/yourdomain.com/privkey.pem ./devcerts/yourdomain.com.key
   sudo cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem ./devcerts/api.yourdomain.com.crt
   sudo cp /etc/letsencrypt/live/yourdomain.com/privkey.pem ./devcerts/api.yourdomain.com.key
   sudo cp /etc/letsencrypt/live/yourdomain.com/fullchain.pem ./devcerts/www.yourdomain.com.crt
   sudo cp /etc/letsencrypt/live/yourdomain.com/privkey.pem ./devcerts/www.yourdomain.com.key
   ```
    - **Note**: If you’re using a single certificate for all domains (e.g., a wildcard or multi-domain certificate), the paths for `api.yourdomain.com` and `www.yourdomain.com` may be the same as `yourdomain.com`. Adjust the commands accordingly.

## Enable Firewall
1. Check the firewall status to ensure ports 80 (HTTP) and 443 (HTTPS) are open:
   ```bash
   sudo ufw status
   ```
2. If the firewall is not enabled, enable it:
   ```bash
   sudo ufw enable
   ```
3. Allow ports 80 and 443 if they are not already permitted:
   ```bash
   sudo ufw allow 80
   sudo ufw allow 443
   ```

## TradingLab Is Ready
1. You should now be able to access the application at `https://yourdomain.com`.