# Dockerfile
# Use the official Node.js image with the desired version
FROM node:18-alpine

# Set the working directory
WORKDIR /app

# Copy only the package.json and package-lock.json (or yarn.lock) first
# to leverage Docker cache for dependencies if they haven't changed
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the rest of the application code
COPY . .

# Expose the port Next.js will run on
EXPOSE 5500

# Start the Next.js application
CMD ["npm", "run", "dev"]
