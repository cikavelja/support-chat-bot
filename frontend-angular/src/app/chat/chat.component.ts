import { Component } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { CommonModule } from '@angular/common'; // Import CommonModule for *ngFor
import { FormsModule } from '@angular/forms'; // Import FormsModule for ngModel

@Component({
  selector: 'app-chat',
  standalone: true, // Ensure standalone is set to true
  imports: [CommonModule, FormsModule], // Add CommonModule and FormsModule here
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent {
  connection!: signalR.HubConnection;
  messages: { text: string; isUser: boolean }[] = [];
  userInput = '';

  ngOnInit(): void {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5140/supportHub') // Update the URL if needed
      .withAutomaticReconnect() // Optional: Reconnect on failure
      .build();

    this.startConnection();
  }

  startConnection(): void {
    this.connection.start()
      .then(() => console.log('Connected to SignalR'))
      .catch(err => console.error('Error while starting connection:', err));

    this.connection.on('ReceiveMessage', (message: string) => {
      this.addMessage(message, false); // Add bot's response
    });
  }

  sendMessage(): void {
    const userInput = this.userInput.trim();
    if (userInput !== '') {
      this.addMessage(userInput, true); // Add user's message
      this.sendMessageToServer(userInput);
      this.userInput = ''; // Clear input field
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
    try {
      await fetch('http://localhost:5140/api/support/query', { // Update the URL if needed
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(message)
      });
    } catch (error) {
      console.error('Error sending message:', error);
    }
  }
}