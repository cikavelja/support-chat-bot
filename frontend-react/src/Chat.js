import React, { useState, useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';

const Chat = () => {
  const [messages, setMessages] = useState([]);
  const [userInput, setUserInput] = useState('');
  const chatContainerRef = useRef(null);
  let connection = useRef(null);

  useEffect(() => {
    connection.current = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5140/supportHub') // Update URL if needed
      .withAutomaticReconnect()
      .build();

    connection.current.start()
      .then(() => console.log('Connected to SignalR'))
      .catch(err => console.error('Error while starting connection:', err));

    connection.current.on('ReceiveMessage', (message) => {
      addMessage(message, false); // Add bot's response
    });

    return () => {
      connection.current.stop();
    };
  }, []);

  const sendMessage = () => {
    const trimmedInput = userInput.trim();
    if (trimmedInput !== '') {
      addMessage(trimmedInput, true);
      sendMessageToServer(trimmedInput);
      setUserInput('');
    }
  };

  const addMessage = (text, isUser) => {
    setMessages(prevMessages => [...prevMessages, { text, isUser }]);
    setTimeout(() => {
      if (chatContainerRef.current) {
        chatContainerRef.current.scrollTop = chatContainerRef.current.scrollHeight;
      }
    }, 0);
  };

  const sendMessageToServer = async (message) => {
    try {
      await fetch('http://localhost:5140/api/support/query', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(message)
      });
    } catch (error) {
      console.error('Error sending message:', error);
    }
  };

  return (
    <div className="container">
      <h2 className="text-center mb-4">Technical Support Assistant</h2>
      <div className="chat-container" ref={chatContainerRef} style={{ maxHeight: '400px', overflowY: 'auto' }}>
        {messages.map((message, index) => (
          <div key={index} className={`message ${message.isUser ? 'user-message' : ''}`}>
            {message.text}
          </div>
        ))}
      </div>
      <div className="input-group mt-3">
        <input
          type="text"
          className="form-control"
          placeholder="Type your question here..."
          value={userInput}
          onChange={(e) => setUserInput(e.target.value)}
        />
        <button onClick={sendMessage} className="btn btn-primary">Send</button>
      </div>
    </div>
  );
};

export default Chat;
