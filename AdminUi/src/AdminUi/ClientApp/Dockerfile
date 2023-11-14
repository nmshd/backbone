FROM node:20

RUN echo "RUN echo"

WORKDIR /app

COPY AdminUi/src/AdminUi/ClientApp/package*.json ./
COPY AdminUi/src/AdminUi/ClientApp/tsconfig.json ./
COPY AdminUi/src/AdminUi/ClientApp/tsconfig.app.json ./

RUN npm install

RUN npm install -g @angular/cli

EXPOSE 4200

CMD ["npm", "start"]