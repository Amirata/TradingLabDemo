# مرحله 1: استفاده از Node.js به عنوان تصویر پایه برای بیلد
FROM node:20-alpine AS build

# تنظیم دایرکتوری کاری
WORKDIR /app

# کپی فایل‌های package.json و package-lock.json (یا yarn.lock)
COPY client-app/package*.json .

# نصب وابستگی‌ها
RUN npm install --omit-dev

# کپی بقیه فایل‌های پروژه
COPY client-app/ .

ARG VITE_APP_API_URL
ARG VITE_APP_IMG_URL
ENV VITE_APP_API_URL=$VITE_APP_API_URL
ENV VITE_APP_IMG_URL=$VITE_APP_IMG_URL

# بیلد پروژه با Vite
RUN npm run build

# مرحله 2: استفاده از یک سرور سبک برای سرو کردن فایل‌های استاتیک
FROM nginx:1.25.3-alpine

# کپی فایل‌های بیلد شده از مرحله قبل به دایرکتوری nginx
COPY --from=build /app/dist /usr/share/nginx/html

# کپی فایل تنظیمات سفارشی Nginx
COPY client-app/nginx.conf /etc/nginx/conf.d/default.conf

# پورت پیش‌فرض nginx
EXPOSE 80

# اجرای nginx
CMD ["nginx", "-g", "daemon off;"]