# Build stage
FROM node:18-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./
RUN npm ci --only=production

# Copy source code
COPY . .

# Build the app
RUN npm run build

# Production stage
FROM nginx:alpine AS production
COPY --from=build /app/build /usr/share/nginx/html

# Copy custom nginx config (optional)
# COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]