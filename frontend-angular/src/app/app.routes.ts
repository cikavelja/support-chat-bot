import { Route } from '@angular/router';
import { ChatComponent } from './chat/chat.component'; // Adjust the path if necessary
import { SupportAdminComponent } from './support-admin/support-admin.component';

export const routes: Route[] = [
  { path: 'chat', component: ChatComponent }, // Route for the chat page
  { path: 'app-support-admin', component: SupportAdminComponent }, // Route for the app-support-admin page
  { path: '', redirectTo: '/chat', pathMatch: 'full' }, // Redirect to /chat by default
];