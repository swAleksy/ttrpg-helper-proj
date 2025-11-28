# --- DEVELOPMENT STAGE ---
# Use a Node base image
FROM node:20-alpine AS development

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and package-lock.json (or yarn.lock)
# to install dependencies
COPY package*.json ./

# Install dependencies (this will be fast since node_modules is a separate volume)
RUN npm install

# Copy all source code
# (This is mostly redundant when using the 'volumes' mount, but good for base image setup)
COPY . .

# Expose the development server port (Vite default is 5173, but 3000 is common)
# Change this if your `package.json` script uses a different port.
EXPOSE 5173

# Start the Vue development server
# The --host 0.0.0.0 is crucial for Docker to expose the port correctly.
CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]