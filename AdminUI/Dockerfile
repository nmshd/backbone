# Build stage
FROM node:lts-alpine as build-stage
WORKDIR /app
COPY package*.json /app/
RUN npm install
COPY ./ /app/
ARG configuration=production
RUN npm run build -- --output-path=./dist/out --configuration $configuration

# Deploy stage
FROM nginx:1.24
COPY --from=build-stage /app/dist/out/ /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf