import { Component } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent {
  connection!: signalR.HubConnection;
  messages: { text: string; isUser: boolean }[] = [];
  userInput = '';
  connectionId: string | null = null; // Store user's SignalR connection ID

  ngOnInit(): void {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5140/supportHub') // Ensure this URL matches your backend
      .withAutomaticReconnect()
      .build();

    this.startConnection();
  }

  async startConnection(): Promise<void> {
    try {
      await this.connection.start();
      console.log('Connected to SignalR');

      // Get the Connection ID
      this.connectionId = this.connection.connectionId;
      console.log('Connection ID:', this.connectionId);

      this.connection.on('ReceiveMessage', (message: string) => {
        this.addMessage(message, false); // Add bot's response
      });
    } catch (err) {
      console.error('Error while starting connection:', err);
    }
  }

  sendMessage(): void {
    const userInput = this.userInput.trim();
    if (userInput !== '') {
      this.addMessage(userInput, true);
      this.sendMessageToServer(userInput);
      this.userInput = '';
    }
  }

  addMessage(text: string, isUser: boolean): void {
    this.messages.push({ text, isUser });
    this.scrollChatToBottom();
  }

  scrollChatToBottom(): void {
    setTimeout(() => {
      const chatContainer = document.querySelector('.chat-container');
      if (chatContainer) {
        chatContainer.scrollTop = chatContainer.scrollHeight;
      }
    }, 0);
  }

  async sendMessageToServer(message: string): Promise<void> {
    if (!this.connectionId) {
      console.error('No connection ID available. Cannot send message.');
      return;
    }

    try {
      const response = await fetch('http://localhost:5140/api/support/query', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          userPrompt: message,
          connectionId: this.connectionId // Pass connection ID to identify user
        })
      });

      if (!response.ok) {
        console.error('Error sending message:', response.statusText);
      }
    } catch (error) {
      console.error('Error sending message:', error);
    }
  }
}
