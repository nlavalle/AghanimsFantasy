# build frontend
FROM node:20-alpine AS build-frontend
ARG build_type=build
WORKDIR /app
COPY vue3-web-app/package*.json ./
RUN npm install
COPY vue3-web-app/ .
RUN npm run $build_type

# production stage
FROM nginx:stable-alpine AS production-stage
ARG nginx_config=prod-nginx.conf
COPY --from=build-frontend /app/dist /usr/share/nginx/html
COPY $nginx_config /etc/nginx/conf.d/default.conf
COPY $nginx_config /etc/nginx/sites-available/aghanimsfantasy.com
COPY $nginx_config /etc/nginx/sites-enabled/aghanimsfantasy.com

# install certbot
RUN apk add certbot certbot-nginx

EXPOSE 80
EXPOSE 443
CMD ["nginx", "-g", "daemon off;"]