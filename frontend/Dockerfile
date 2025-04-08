
# Build stage
FROM node:18 AS build
WORKDIR /app

# Copiem doar fișierele legate de dependințe
COPY package*.json ./
RUN npm install

# Injectăm variabila API_URL pentru build React
ARG REACT_APP_API_URL
ENV REACT_APP_API_URL=$REACT_APP_API_URL
ENV NODE_OPTIONS=--openssl-legacy-provider

# Copiem restul fișierelor
COPY . .

# Build aplicația React
RUN npm run build

# Stage 2: Serve cu nginx
FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

