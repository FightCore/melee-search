# Build stage
FROM node:22-alpine AS build
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY . .

# Build the Angular application
RUN npm run build

# Production stage
FROM nginx:alpine AS production

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy built static files from build stage
COPY --from=build /app/dist/meleesearch/browser /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
