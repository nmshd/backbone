FROM node:20

WORKDIR /app

COPY AdminUi/src/AdminUi/ClientApp/package*.json ./
COPY AdminUi/src/AdminUi/ClientApp/tsconfig.json ./
COPY AdminUi/src/AdminUi/ClientApp/tsconfig.app.json ./

RUN npm install

EXPOSE 4200

CMD ["npm", "start"]