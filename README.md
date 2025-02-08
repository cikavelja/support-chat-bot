# 📌 Support Chat Bot

## 📖 Overview
This repository provides a **custom AI-powered support chat bot**, built using:
- 🖥 **C# API** utilizing the **Ollama AI server** with the `all-minilm` model
- 📊 **Qdrant Vector Database** for storing and retrieving support topics
- 🌐 **Angular and React** as UI options

This guide will walk you through setting up the chat bot, installing dependencies, and running the project.

---

## 🎥 Video Explanation
For a detailed video explanation, watch this video:
[![Watch the video](https://img.youtube.com/vi/P9sKwAVuDTw/0.jpg)](https://youtu.be/P9sKwAVuDTw)

---

## ⚙️ **Prerequisites**
Ensure you have the following installed:
- 🐳 **Docker** (for running Qdrant)
- 🏗 **.NET Core 9+** (for running the API)
- 📦 **Node.js & npm/yarn** (for running Angular or React UI)
- 🔗 **Git** (for cloning the repository)

---

## 🏗 **1. Install Ollama AI Server**
Ollama is required to generate embeddings using the `all-minilm` model.

### 🐧 **Linux/macOS**
```sh
curl -fsSL https://ollama.ai/install.sh | sh
```

### 🖥 **Windows**
1. Download the **Windows installer** from [Ollama AI](https://ollama.ai/download)
2. Run the installer and follow the instructions

Verify the installation:
```sh
ollama --version
```

---

## 🔄 **2. Pull the All-MiniLM Model**
Once Ollama is installed, pull the `all-minilm` model:
```sh
ollama pull all-minilm
```
Confirm the model is available:
```sh
ollama list
```

---

## 🗄 **3. Install and Run Qdrant Vector Database**
Qdrant is used as a **vector database** to store and retrieve support topics efficiently.

### 🐳 **Using Docker**
Pull Qdrant using the following command:
```sh
docker pull qdrant/qdrant
```
Run Qdrant using the following command:
```sh
docker run -p 6333:6333 -p 6334:6334  -v qdrant_storage:/qdrant/storage qdrant/qdrant
```

Check if Qdrant is running:
🌐 **http://localhost:6333/**

Qdrant dashboard:
🌐 **http://localhost:6333/dashboard**

---

## 🔽 **4. Clone the Repository**
Clone the chat bot repository from GitHub:
```sh
git clone https://github.com/cikavelja/support-chat-bot.git
cd support-chat-bot
```

---

## 🏗 **5. Set Up the C# API**
Navigate to the backend folder:
```sh
cd backend
```

### 📦 **Install Dependencies**
```sh
dotnet restore
```

### 🚀 **Run the API**
```sh
dotnet run
```

The API should now be running at 🌐 `http://localhost:5140/`.

---

## 🌐 **6. Run the Angular UI**
Navigate to the Angular frontend directory:
```sh
cd frontend-angular
```

### 📦 **Install Dependencies**
```sh
npm install
```

### 🚀 **Run the Angular App**
```sh
npm start
```

The Angular UI should now be accessible at 🌐 `http://localhost:4200/`.

---

## ⚛️ **7. Run the React UI**
Navigate to the React frontend directory:
```sh
cd frontend-react
```

### 📦 **Install Dependencies**
```sh
npm install
```

### 🚀 **Run the React App**
```sh
npm start
```

The React UI should now be accessible at 🌐 `http://localhost:3000/`.

---

## 🛠 **8. Usage**
1. Open the **Angular or React UI**.
2. Enter a support query or topic.
3. The query is converted to an **embedding vector** using Ollama AI.
4. The **Qdrant vector database** searches for the most relevant support topics.
5. The result is displayed in the UI.

---

## 🔗 **9. API Endpoints**

### 📌 **POST `/api/Support/query`** (User Query)
- **Request Body:**
```json
{
  "userPrompt": "How do I update my payment details?",
  "connectionId": "12345"
}
```
- **Response:**
```json
{
  "response": "To update your payment method, go to the 'Billing' section and update your payment details."
}
```

### 📌 **POST `/api/SupportAdmin/add`** (Admin Adds New Support Topic)
- **Request Body:**
```json
{
  "title": "Payment Update",
  "description": "To update payment details, navigate to 'Billing' and select 'Payment Methods'."
}
```
- **Response:**
```json
{
  "message": "Support topic added successfully!",
  "id": 123456789
}
```

---

## 🤝 **10. Contributing**
1. Fork the repository
2. Create a new branch
3. Make your changes
4. Submit a pull request

---

## 🛠 **11. Troubleshooting**

### ❓ **Qdrant Not Running?**
Check if the container is running:
```sh
docker ps
```
If not, restart it:
```sh
docker start qdrant
```

### ❓ **API Not Responding?**
Check if the API is running at 🌐 `http://localhost:5140/`:
```sh
curl http://localhost:5140/api/Support/query
```

### ❓ **Frontend Not Loading?**
Ensure that the React/Angular app is running and check the console for errors.

---

## 📜 **12. License**
This project is licensed under the **MIT License**.

---

## 📧 **13. Contact**
For support, contact: ✉️ `veljkovic.nenad@gmail.com`

🚀 Happy Coding!
