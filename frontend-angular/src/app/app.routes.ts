import { Route } from '@angular/router';
import { ChatComponent } from './chat/chat.component'; // Adjust the path if necessary

export const routes: Route[] = [
  { path: 'chat', component: ChatComponent }, // Route for the chat page
  { path: '', redirectTo: '/chat', pathMatch: 'full' }, // Redirect to /chat by default
];