# build quasar-app
FROM node:20-alpine as build-quasar-app
ARG build_type=build
WORKDIR /app
COPY quasar-web-app/package*.json ./
RUN npm install
COPY quasar-web-app/ .
RUN npm run $build_type

# production stage
FROM nginx:stable-alpine as production-stage
ARG nginx_config=prod-nginx.conf
COPY --from=build-quasar-app /app/dist/spa /usr/share/nginx/html
COPY $nginx_config /etc/nginx/conf.d/default.conf
COPY $nginx_config /etc/nginx/sites-available/aghanimsfantasy.com
COPY $nginx_config /etc/nginx/sites-enabled/aghanimsfantasy.com

# install certbot
RUN apk add certbot certbot-nginx

EXPOSE 80
EXPOSE 443
CMD ["nginx", "-g", "daemon off;"]